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

using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using SFML.Graphics;

namespace edu.CiclosFormativos.Games.DIDAM.Resources
{
    /// <summary>
    /// Gestiona la carga de recursos de SFML. Clase estática: no permite su instancianción.
    /// </summary>
    public static class SFMLResourcesManager
    {
        /// <summary>
        /// Carga una Texture desde el disco
        /// </summary>
        /// <param name="element">XElement con la información para la carga</param>
        /// <returns>La Texture leida o null si ha habido problemas</returns>
        public static Texture LoadTexture(XElement element)
        {
            Stream stream;
            Texture txt;
            String path = (String)element.Attribute("res");

            try
            {
                // si no es externo (res), busco el interno (src)
                if (path == null)
                {
                    path = (String)element.Attribute("src");
                    if (path == null) return null;
                    else stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                else  // utilizo esta técnica y no el GetType() ya que posiblemente lo exportaré a un DLL
                {
                    stream = Assembly.GetEntryAssembly().GetManifestResourceStream(path);
                    Debug.Assert(stream != null, "No se puede cargar la textura embedida: " + path);
                }
                // Tamaño
                String rect = (String)element.Attribute("rectangle");
                IntRect area = new IntRect();
                if (rect != null)
                {
                    String[] rectCoord;
                    rectCoord = rect.Split(',');
                    area.Left = Int16.Parse(rectCoord[0]);
                    area.Top = Int16.Parse(rectCoord[1]);
                    area.Width = Int16.Parse(rectCoord[2]);
                    area.Height = Int16.Parse(rectCoord[3]);
                }

                txt = new SFML.Graphics.Texture(stream, area);

                // Propiedades
                txt.Repeated = Boolean.Parse((String)element.Attribute("repeated"));
                txt.Smooth = Boolean.Parse((String)element.Attribute("smooth"));
            }
            catch (Exception ex) 
            {
                throw ex;
            }

            return txt;
        }

         /// <summary>
        /// Carga una Font desde el disco
        /// </summary>
        /// <param name="element">XElement con la información para la carga</param>
        /// <returns>La Font leida o null si ha habido problemas</returns>
        public static Font LoadFont(XElement element)
        {
            Stream stream;
            Font fnt;
            String path = (String)element.Attribute("res");

            try
            {
                // si no es externo (res), busco el interno (src)
                if (path == null)
                {
                    path = (String)element.Attribute("src");
                    if (path == null) return null;
                    else stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                else  // utilizo esta técnica y no el GetType() ya que posiblemente lo exportaré a un DLL
                {
                    stream = Assembly.GetEntryAssembly().GetManifestResourceStream(path);
                    Debug.Assert(stream != null, "No se puede cargar la fuente embedida: " + path);
                }

                fnt = new SFML.Graphics.Font(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fnt;

        }
    }
    
}
