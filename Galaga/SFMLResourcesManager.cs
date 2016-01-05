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
            return new SFML.Graphics.Texture(path);    
        }

    }

    
}
