using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

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
            String path = (String)element.Attribute("src");
            if (path == null) return null;

            // Tamaño
            String rect = (String)element.Attribute("rectangle");
            IntRect area = new IntRect(); 
            if (rect != null)
            {
                String[] rectCoord;
                rectCoord = rect.Split(',');
                area.Left = Int16.Parse(rectCoord[0]);
                
            }
            //new IntRect(
            //    int.Parse(ints[0]),
            //    int.Parse(ints[1]),
            //    int.Parse(ints[2]),
            //    int.Parse(ints[3]));

            Texture txt = new SFML.Graphics.Texture(path, area);

            // Propiedades
            txt.Repeated = Boolean.Parse((String)element.Attribute("repeated"));
            txt.Smooth = Boolean.Parse((String)element.Attribute("smooth"));

            return txt;
        }

    }

    
}
