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

        public void Init() 
        {
            _logger.Log(LogLevel.Info, " >> Configurando aplicación.");

            // buffer 32 bits de colors
            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            // Creamos la ventana principal
            _logger.Log(LogLevel.Info, " >> Creando ventana principal.");
            // ventana no redimensionable
            _window = new RenderWindow(new VideoMode(800, 600), "Galaga ", Styles.Close, contextSettings);

            _scnManager = new SceneManager();
            _context = new Scene.Context(_window);

            RegisterScenes();
        
        }

        /// <summary>
        /// Bucle principal de la aplicación 
        /// </summary>
        public void Run()
        {
        }

        /// <summary>
        /// Registra las funciones de creación de escenas
        /// </summary>
        private void RegisterScenes() 
        {
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
