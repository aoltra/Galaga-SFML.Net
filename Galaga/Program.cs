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

// funciona con .Net 4.0
namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    class Program
    {
        static void Main(string[] args)
        {
            Application application = new Application();       // instanciación de la aplicación

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
                                application.ErrorLevel = Int16.Parse(fields[1]);
                            }

                            if (parameter.ToLower() == "logtofile")
                            {
                                application.LogToFile = Boolean.Parse(fields[1]);
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

            application.ConfigLogger();                 // configuracion del logger
            application.Run();                          // ejecuto el bucle principal de la app
        }
    }
}
