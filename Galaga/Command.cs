
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula un comando. 
    /// </summary>
    /// <remarks>
    /// Abstracta. No se puede instanciar
    /// </remarks>
    abstract class Command
    {
        /// <summary>
        /// Asigna o devuelve la categoria del sceneNode para la que está dirigido el comando
        /// </summary>
        /// <remarks>
        /// Admite 16 categorias posibles y la combinaciones posibles entre ellas.
        /// A efectos de un mejor manejo de las mismas es interesante crearse un tipo Enumerado categoria
        /// </remarks>
        public UInt16 Category { get; set; }

        /// <summary>
        /// Función a ejecutar en el comando
        /// </summary>
        /// <remarks>
        /// La función debe implementarse de manera obligatoria en la clases hijas
        /// </remarks>
        /// <param name="scNode">Nodo en el que se ejecuta el comando</param>
        /// <param name="dt">Incremento de tiemp desde la última actualización</param>
        public abstract void Execute(SceneNode scNode, SFML.System.Time dt);
    }
}
