using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

using NLog;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    class Game
    {
        // Variables miembro
        private RenderWindow _window;                   // ventana principal

        private World _world;                           // mundo del juego
        private Player _player;					        // jugador

        private bool _isPaused;                         // juego pausado o no
        private SFML.System.Time _timePerFrame;         // en este caso indica el mínimo requerido

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Constructor
        public Game()
        {
            _logger.Log(LogLevel.Info, " >> Configurando juego.");

            // buffer 32 bits de colors
            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            // Creamos la ventana principal
            _logger.Log(LogLevel.Info, " >> Creando ventana principal.");
            // ventana no redimensionable
            _window = new RenderWindow(new VideoMode(800, 600), "Galaga ", Styles.Close, contextSettings);

            _player = new Player();

            _timePerFrame = SFML.System.Time.FromSeconds(1f / 40f);           // como mínimo 40 frames por segundo

            _isPaused = false;
            RegisterDelegates();

            _world = new World(_window);
        }

        ////////////////////////
        // Métodos
        ////////////////////////
        /// <summary>
        /// Ejecuta el bulce principal del juego
        /// </summary>
        public void run()
        {

            Clock clock = new Clock();
            SFML.System.Time timeSinceLastUpdate = SFML.System.Time.Zero;

            // Game Loop
            while (_window.IsOpen)
            {
                // Procesamos eventos. Este procesamiento de evento se podría quitar ya que sólo
                // tendría importancia para aquellos eventos que no afectasen al mundo
                // en este caso el Close. Si lo quitaramos sólo se retrasaría un poco (hasta el paso del tiempo
                // del frame) al ejecución del evento 
                _window.DispatchEvents();

                // procesamos los eventos en el propio jugador
                _player.HandleRealtimeInput(_world.CommandQueue);

                // para cada uno de los ciclos reinicio el reloj a cero y devuelvo
                // el tiempo que ha transcurrido
                timeSinceLastUpdate = clock.Restart();

                // si el tiempo transcurrido es mayor que el que queremos por cada frame
                while (timeSinceLastUpdate > _timePerFrame)
                {
                    timeSinceLastUpdate -= _timePerFrame;   // le quito un frame

                    // Procesamos eventos
                    _window.DispatchEvents();

                    // procesamos los eventos en el propio jugador
                    _player.HandleRealtimeInput(_world.CommandQueue);

                    if (!_isPaused)
                        update(_timePerFrame);

                    // si después de este ciclo el tiempo que ha transcurrido sigue siendo mayor al de un frame
                    // repito el ciclo y voy actualizando el mundo, aunque no lo renderice
                }

                // en cada ciclo actualizo y renderizo
                if (!_isPaused)
                    update(timeSinceLastUpdate);

                render();
            }
        }

        /// <summary>
        ///  Registra los delegados
        /// </summary>
        private void RegisterDelegates()
        {
            _window.Closed += new EventHandler(OnClose);
            // se comentan para facilitar las labores de depuración
            //_window.GainedFocus += new EventHandler(OnGainedFocus);
            //_window.LostFocus += new EventHandler(OnLostFocus);
            _window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
            _window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyReleased);
        }

        // actualiza el estado del mundo en función del tiempo transcurrido desde la última actualización
        private void update(SFML.System.Time time)
        {
            // calculamos las nuevas posiciones de los elementos del mundo
            _world.Update(time);
        }

        /// <summary>
        /// Dibuja el mundo
        /// </summary>
        private void render()
        {
            // limpia la pantalla (por defecto en negro, pero podemos asignarle un color)
            _window.Clear();
            // Dibuja los elementos contenidos en el mundo
            _world.Draw();
            // muestra la pantalla. Hace el cambio de un buffer a otro (doble buffer)
            _window.Display();
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
            _player.HandleKeyboardEvent(e.Code, true, _world.CommandQueue);
        }

        /// <summary>
        /// Se ha soltado en una tecla
        /// </summary>
        /// <param name="sender">Objeto que genera el evento</param>
        /// <param name="e">Información sobre la tecla soltada</param>
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            _player.HandleKeyboardEvent(e.Code, false, _world.CommandQueue);
        }
        #endregion

       
    }
}
