using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Categorias de SceneNode disponibles
    /// </summary>
    public enum Category
    {
        // la 0 y la 1 la proporcionanel motor
        /// <summary>
        /// Ninguna categoria
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Nodo de grafo de escena genérico
        /// </summary>
        SCENE = 1,

        /// <summary>
        /// Nave del jugador
        /// </summary>
        PLAYERSHIP = 2,

        DUMMY = 4           // simplemente para hacer notar que se numeran como potencias de dos
    }
}
