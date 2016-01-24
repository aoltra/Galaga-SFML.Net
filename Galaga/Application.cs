using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog.Config;
using NLog.Targets;
using NLog;

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

        public Application() 
        {
            // configuración por defecto
            ErrorLevel = 0;
            LogToFile = true;
        }

        /// <summary>
        /// Bucle principal de la aplicación 
        /// </summary>
        public void Run()
        {
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
