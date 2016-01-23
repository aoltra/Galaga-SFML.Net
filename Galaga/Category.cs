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
        None = 0,

        /// <summary>
        /// Nodo de grafo de escena genérico
        /// </summary>
        Scene = 1,

        /// <summary>
        /// Nave del jugador
        /// </summary>
        PlayerShip = 2,

        Dummy = 4           // simplemente para hacer notar que se numeran como potencias de dos
    }
}
