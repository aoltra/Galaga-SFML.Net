using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

using NLog.Config;
using NLog.Targets;
using NLog;

using System.Diagnostics;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    class Application
    {
        /// <summary>
        /// Nivel de error a mostrar en el Log
        /// </summary>
        public int ErrorLevel { private get; set; }

        /// <summary>
        /// Mostrar Log en fichero
        /// </summary>
        public bool LogToFile { private get; set; }

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Variables miembro
        private RenderWindow _window;                   // ventana principal
        private SceneManager _scnManager;               // gestor de escenas
        private Scene.Context _context;                 // contexto de trabajo

        private SFML.System.Time _timePerFrame;         // en este caso indica el mínimo requerido
        private bool _isPaused;                         // juego pausado o no

        /// <summary>
        /// Identificadores de escena
        /// </summary>
        public enum SceneID
        {
            None,
            Title,
            Menu,
            Game,
            Records,
            Pause
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Application() 
        {
            // configuración por defecto
            ErrorLevel = 0;
            LogToFile = true;
        }

        ////////////////////////
        // Métodos
        ////////////////////////

        public void Init() 
        {
            _logger.Log(LogLevel.Info, " > Configurando aplicación.");

            // buffer 32 bits de colors
            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            // Creamos la ventana principal
            _logger.Log(LogLevel.Info, " >> Creando ventana principal.");
            // ventana no redimensionable
            _window = new RenderWindow(new VideoMode(800, 600), "Galaga ", Styles.Close, contextSettings);

            _scnManager = new SceneManager();
            _context = new Scene.Context(_window);

            _timePerFrame = SFML.System.Time.FromSeconds(1f / 40f);           // como mínimo 40 frames por segundo
            _isPaused = false;

            RegisterDelegates();
            RegisterScenes();

            // pongo la primera escena en la pila
            _logger.Log(LogLevel.Info, " >> Push escena principal.");
            _scnManager.Push((int)SceneID.Game);
        
        }

        /// <summary>
        /// Bucle principal de la aplicación 
        /// </summary>
        public void Run()
        {
            _logger.Log(LogLevel.Info, " >> Bucle principal del juego.");
            SFML.System.Clock clock = new SFML.System.Clock();
            SFML.System.Time timeSinceLastUpdate = SFML.System.Time.Zero;

            // Game Loop
            while (_window.IsOpen)
            {
                // Procesamos eventos. Este procesamiento de evento se podría quitar ya que sólo
                // tendría importancia para aquellos eventos que no afectasen al mundo
                // en este caso el Close. Si lo quitaramos sólo se retrasaría un poco (hasta el paso del tiempo
                // del frame) al ejecución del evento 
                _window.DispatchEvents();

                //// procesamos los eventos en el propio jugador
                //_player.HandleRealtimeInput(_world.CommandQueue);

                // para cada uno de los ciclos reinicio el reloj a cero y devuelvo
                // el tiempo que ha transcurrido
                timeSinceLastUpdate = clock.Restart();

                // si el tiempo transcurrido es mayor que el que queremos por cada frame
                while (timeSinceLastUpdate > _timePerFrame)
                {
                    timeSinceLastUpdate -= _timePerFrame;   // le quito un frame

                    // Procesamos eventos
                    _window.DispatchEvents();

                    //// procesamos los eventos en el propio jugador
                    //_player.HandleRealtimeInput(_world.CommandQueue);

                    if (!_isPaused)
                        Update(_timePerFrame);

                    // si después de este ciclo el tiempo que ha transcurrido sigue siendo mayor al de un frame
                    // repito el ciclo y voy actualizando el mundo, aunque no lo renderice
                    
                    // compruebo si la pila esta vacía (no hay escenas) y en ese caso salgo
                    if (_scnManager.IsEmpty)
                        _window.Close();
                }

                // en cada ciclo actualizo y renderizo
                if (!_isPaused)
                    Update(timeSinceLastUpdate);

                if (_scnManager.IsEmpty)
                    _window.Close();

                Render();
            }
        }

        /// <summary>
        /// Actualiza el estado de la pila de escenas
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        private void Update(SFML.System.Time dt) {  _scnManager.Update(dt); }

        /// <summary>
        /// Redirige las pulsaciones de teclado en modo eventos "típicos" a la pila
        /// </summary>
        /// <param name="key">Tacla pulsada</param>
        /// <param name="isPressed">True si está pulsada o se libera</param>
        private void HandleKeyboardEvent(Keyboard.Key key, bool isPressed)
        {
            _scnManager.HandleKeyboardEvent(key, isPressed);
        }

        /// <summary>
        /// Dibuja las escenas de la pila
        /// </summary>
        private void Render()
        {
            // limpia la pantalla (por defecto en negro, pero podemos asignarle un color)
            _window.Clear();
            // Dibuja los elementos contenidos en la pila
            _scnManager.Draw();
            // muestra la pantalla. Hace el cambio de un buffer a otro (doble buffer)
            _window.Display();
        }

        /// <summary>
        ///  Registra los delegados
        /// </summary>
        private void RegisterDelegates()
        {
            _logger.Log(LogLevel.Info, " >> Registrando delegados...");

            _window.Closed += new EventHandler(OnClose);
            // se comentan para facilitar las labores de depuración
            //_window.GainedFocus += new EventHandler(OnGainedFocus);
            //_window.LostFocus += new EventHandler(OnLostFocus);
            _window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
            _window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyReleased);
        }

        /// <summary>
        /// Registra las funciones de creación de escenas
        /// </summary>
        private void RegisterScenes() 
        {
            _logger.Log(LogLevel.Info, " >> Registrando escenas...");
            
            try
            {
                _scnManager.RegisterCreateFunction((int)SceneID.Game, CreateGameScene);
            }
            catch(SceneManagerException exM)
            {
                _logger.Log(LogLevel.Fatal, exM.Message);
                Debug.Assert(false, exM.Message);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex.Message);
            }
        }

        private Scene CreateGameScene()
        {
            return new GameScene(_context, _scnManager);
        }

        #region Funciones suscritas a delegados
        /// <summary>
        /// La ventana se ha cerrado 
        /// </summary>
        /// <param name="sender">Objeto que genera el evento</param>
        /// <param name="e">Información asociada</param>
        private void OnClose(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
        }

        /// <summary>
        /// Se ha recuperado el foco de la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGainedFocus(object sender, EventArgs e)
        {
            _isPaused = false;
        }

        /// <summary>
        /// Se ha perdido el foco de la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLostFocus(object sender, EventArgs e)
        {
            _isPaused = true;
        }

        /// <summary>
        /// Se ha pulsado en una tecla
        /// </summary>
        /// <param name="sender">Objeto que genera el evento</param>
        /// <param name="e">Información sobre la tecla pulsada</param>
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            HandleKeyboardEvent(e.Code, true);
        }

        /// <summary>
        /// Se ha soltado en una tecla
        /// </summary>
        /// <param name="sender">Objeto que genera el evento</param>
        /// <param name="e">Información sobre la tecla soltada</param>
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            HandleKeyboardEvent(e.Code, false);
        }
        #endregion

        #region Logger
        public void ConfigLogger()
        {
            LogLevel logLevel;

            // admite niveles de error de 0 a 2
            switch (ErrorLevel)
            {
                case 0: // solo los graves que implican salida del programa
                    logLevel = LogLevel.Fatal;
                    break;
                case 1: // avisos
                    logLevel = LogLevel.Warn;
                    break;
                default:  // informacion detallada
                    logLevel = LogLevel.Info;
                    break;
            }

            var config = new LoggingConfiguration();

            if (LogToFile)
            {
                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);

                fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
                fileTarget.FileName = "${basedir}/LogFile.log";

                var rule2 = new LoggingRule("*", logLevel, fileTarget);
                config.LoggingRules.Add(rule2);
            }

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            consoleTarget.Layout = "${message}";

            var rule1 = new LoggingRule("*", logLevel, consoleTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }
        #endregion


    }
}
