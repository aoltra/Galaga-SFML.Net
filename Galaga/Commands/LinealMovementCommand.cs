using System;
using System.Collections.Generic;
using System.Text;

using SFML.System;
using edu.CiclosFormativos.DAM.DI.Galaga.Entities;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Commands
{
    /// <summary>
    /// Encapsula un comando que mueve de manera lineal una entidad
    /// </summary>
    class LinealMovementCommand : Command
    {
        // Variables miembrp
        private Vector2f velocity;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">Velocidad en la dirección x</param>
        /// <param name="y">Velocidad en la dirección y</param>
        public LinealMovementCommand(float x, float y)
            : this (new Vector2f(x, y))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vel">Vector velocidad</param>
        public LinealMovementCommand(Vector2f vel)
        {
            velocity = vel;
        }

        public override void Execute(SceneNode subject, SFML.System.Time dt)
        {
             //SFML.System.Vector2f speed = new Vector2f(0f, 0f);
            // speed.Y += _playerSpeed;
            ((Entity)subject).Position += velocity * dt.AsSeconds();
        }
    }
}
