using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula un objeto del juego
    /// </summary>
    abstract class Entity
    {
        public Vector2f Position { get; set; }

        /// <summary>
        /// Asigna o devuelve la velocidad de la entidad
        /// </summary>
        /// <remarks>La velocidad es una magnitud vectorial, no sólo importa el valor absoluto (el módulo), además es necesario
        /// conocer su dirección y sentido</remarks>
        public Vector2f Velocity { get; set; }

        /// <summary>
        /// Constructor básico. Inicializa a cero la velocidad
        /// </summary>
        public Entity() {
            Velocity = new Vector2f(0.0f, 0.0f);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad de la entidad</param>
        public Entity(Vector2f velocity)
        {
            Velocity = velocity;
        }


    }
}
