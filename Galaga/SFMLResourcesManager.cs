using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.IO;
using System.Reflection;
using SFML.Graphics;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Resources
{
    /// <summary>
    /// Gestiona la carga de recursos de SFML. Clase estática: no permite su instancianción.
    /// </summary>
    static class SFMLResourcesManager
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
                    stream = Assembly.GetEntryAssembly().GetManifestResourceStream(path);

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
                throw new ResourcesManagerException(ex.Message);
            }

            return txt;
        }
    }
    
}
