#region GPL License
/*  Galaga-SFML.Net: Galaga's Clon for educational purposes made with SFML.Net library
    Copyright (C) 2015-2016  Alfredo Oltra. 
   
    This program comes with ABSOLUTELY NO WARRANTY.
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>
  
    --------------------------------------------------------------------------------
 
    You can contact the author by email: aoltra@uhurulabs.com, aoltra@gmail.com
    or you can follow on Twitter: @aoltra
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

using NLog.Config;
using NLog.Targets;
using NLog;

// funciona con .Net 4.0
namespace edu.CiclosFormativos.DAM.DI.Galaga
{
	class Game
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

		static void Main(string[] args)
		{
            Game game = new Game();             // instanciación del juego 

            #region Lectura de parametros
            try 
            {
                // sistema básico de control de parametros. Se pueden 
                // encontrar muchos paquetes que realizan el control de una 
                // manera más optimizada y flexible
                if (args.Length > 0) 
                {
                    String [] fields;
                    String parameter;
                    foreach (String arg in args)
                    {
                        if (arg[0] == '-') {
                            fields = arg.Split(':');
                            parameter = fields[0].Substring(1);

                            if (parameter.ToLower() == "errorlevel") {
                                game.ErrorLevel = Int16.Parse(fields[1]);                
                            }

                            if (parameter.ToLower() == "logtofile")
                            {
                                game.LogToFile = Boolean.Parse(fields[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("[Galaga] Línea de comandos. Parámetro incorrecto. " + ex.Message);
            }
            #endregion

            game.ConfigLogger();                // configuracion del logger
            game.Init();
            game.run();                         // llamada a la función de arranque
		}
		
		// Variables miembro
		private RenderWindow _window;                   // ventana principal

        private World _world;                           // mundo del juego

		private Sprite _player;					        // jugador
		private bool _IsMovingUp,_IsMovingDown,_IsMovingLeft,_IsMovingRight;
        private float _playerSpeed;                     // velocidad del jugador 

        private SFML.System.Time _timePerFrame;         // en este caso indica el mínimo requerido

		// Constructor
		public Game() {

            // configuración por defecto
            ErrorLevel = 0;
            LogToFile = true;
        }

        private void Init() 
        {
            _logger.Log(LogLevel.Info, " >> Configurando juego.");

			// buffer 32 bits de colors
			ContextSettings contextSettings = new ContextSettings();
			contextSettings.DepthBits = 32;

            // Creamos la ventana principal
            _logger.Log(LogLevel.Info, " >> Creando ventana principal.");
			_window = new RenderWindow(new VideoMode(1280, 800), "Galaga ", Styles.Default, contextSettings);
            
            // el jugador pasa ahora a ser un Sprite
			_player = new Sprite();
			_player.Position = new Vector2f(100f, 100f);

            _playerSpeed = 100;           // 100 px/s

            _timePerFrame = SFML.System.Time.FromSeconds(1f / 40f);           // como mínimo 40 frames por segundo

			RegisterDelegates();

            _world = new World(_window);

            try
            {
                // prueba del correcto funcionamiento
                Resources.ResourcesManager a = new Resources.ResourcesManager(
                    this.GetType().Assembly.GetManifestResourceStream("Galaga.main.resxml"));
                //String [] ad = this.GetType().Assembly.GetManifestResourceNames();
                a.RegisterLoadFunction("texture",Resources.SFMLResourcesManager.LoadTexture);

                // le asigno la textura Naves:NaveJugador
                _player.Texture = (Texture)a["Naves:NaveJugador"];
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn,ex.Message);
            }
		}

		////////////////////////
		// Métodos
		////////////////////////
		public void run() {
            
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

                // para cada uno de los ciclos reinicio el reloj a cero y devuelvo
                // el tiempo que ha transcurrido
                timeSinceLastUpdate = clock.Restart();

                // si el tiempo transcurrido es mayor que el que queremos por cada frame
                while (timeSinceLastUpdate > _timePerFrame)
                {
                    timeSinceLastUpdate -= _timePerFrame;   // le quito un frame
                    
                    // Procesamos eventos
                    _window.DispatchEvents();

                    update(_timePerFrame);  

                    // si después de este ciclo el tiempo que ha transcurrido sigue siendo mayor al de un frame
                    // repito el ciclo y voy actualizando el mundo, aunque no lo renderice
                }

                // en cada ciclo actualizo y renderizo
                update(timeSinceLastUpdate);          
				render();
			}
		}

		// Registra los delegados
		private void RegisterDelegates() {
			
			_window.Closed += new EventHandler(OnClose);
			_window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
			_window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyReleased);
		}

        // actualiza el estado del mundo en función del tiempo transcurrido desde la última actualización
        private void update(SFML.System.Time time)
        {
			SFML.System.Vector2f speed = new Vector2f(0f, 0f);

            // desplaza 1 px en el sentido que haya indicado la pulsacion del teclado
			if (_IsMovingUp)
				speed.Y -= _playerSpeed;
			if (_IsMovingDown)
                speed.Y += _playerSpeed;
			if (_IsMovingLeft)
                speed.X -= _playerSpeed;
			if (_IsMovingRight)
                speed.X += _playerSpeed;

            // espacio = velocidad * tiempo. El nuevo espacio se añade a la posición previa
            // del tiempo se obtienen los segundos ya que la velocidad se da en px/s
            _player.Position += speed * time.AsSeconds(); 
		}
		
		// Dibuja el mundo
		private void render() { 
			// limpia la pantalla (por defecto en negro, pero podemos asignarle un color)
			_window.Clear();
			// Dibuja un elemento "dibujable", Drawable. En este caso nuestro "jugador": el sprite
			_window.Draw (_player);
			// muestra la pantalla. Hace el cambio de un buffer a otro (doble buffer)
			_window.Display ();
		}
		
		/////////////////////////////////////
		// funciones suscritas a delegados
		/////////////////////////////////////
		
		// La ventana se ha cerrado
		private void OnClose(object sender, EventArgs e) {
			
			Window window = (Window)sender;
			window.Close();
		}

		private void OnKeyPressed(object sender, KeyEventArgs e) {
			handlePlayerInput(e.Code, true);
		}

		private void OnKeyReleased(object sender, KeyEventArgs e) {
			handlePlayerInput(e.Code, false);
		}

		private void handlePlayerInput(SFML.Window.Keyboard.Key key, bool pressed) {

			if (key == SFML.Window.Keyboard.Key.W)
				_IsMovingUp = pressed;
			else if (key == SFML.Window.Keyboard.Key.S)
				_IsMovingDown = pressed;
			else if (key == SFML.Window.Keyboard.Key.A)
				_IsMovingLeft = pressed;
			else if (key == SFML.Window.Keyboard.Key.D)
				_IsMovingRight = pressed;
		}
        
        #region Logger
        private void ConfigLogger() 
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

            if (LogToFile) {
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
