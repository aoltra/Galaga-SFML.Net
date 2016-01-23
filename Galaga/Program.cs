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

using NLog.Config;
using NLog.Targets;
using NLog;

// funciona con .Net 4.0
namespace edu.CiclosFormativos.DAM.DI.Galaga
{
	class Program
	{
        /// <summary>
        /// Nivel de error a mostrar en el Log
        /// </summary>
        public static int ErrorLevel { private get; set; }

        /// <summary>
        /// Mostrar Log en fichero
        /// </summary>
        public static bool LogToFile { private get; set; }

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // configuración por defecto
            ErrorLevel = 0;
            LogToFile = true;

            #region Lectura de parametros
            try
            {
                // sistema básico de control de parametros. Se pueden 
                // encontrar muchos paquetes que realizan el control de una 
                // manera más optimizada y flexible
                if (args.Length > 0)
                {
                    String[] fields;
                    String parameter;
                    foreach (String arg in args)
                    {
                        if (arg[0] == '-')
                        {
                            fields = arg.Split(':');
                            parameter = fields[0].Substring(1);

                            if (parameter.ToLower() == "errorlevel")
                            {
                                ErrorLevel = Int16.Parse(fields[1]);
                            }

                            if (parameter.ToLower() == "logtofile")
                            {
                                LogToFile = Boolean.Parse(fields[1]);
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

            ConfigLogger();                // configuracion del logger

            Game game = new Game();             // instanciación del juego
            game.run();                         // llamada a la función de arranque
        }

        #region Logger
        private static void ConfigLogger()
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
