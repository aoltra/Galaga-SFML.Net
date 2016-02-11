using System;
using System.Collections.Generic;
using System.Text;

using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    class PlattonLeader : TransformEntity
    {
        // limite inferior y superior del movimiento del lider
        private float _lowL, _highL;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad del lider (dirección X)</param>
        public PlattonLeader(float velocity, float lowL, float highL)
            : this(velocity, lowL, highL, new SFML.Graphics.Color(100,80,250))
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad del líder</param>
        /// <param name="color">Color con que se representa</param>
        public PlattonLeader(float velocity, float lowL, float highL, SFML.Graphics.Color color)
            : base(new Vector2f(velocity,0))
        {
            _shape.FillColor = color;
            _lowL = lowL;
            _highL = highL;
        }

        /// <summary>
        /// Actualiza el líder del pelotón
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <remarks>
        /// Ya nadie puede heredarla
        /// </remarks>
        override sealed protected void UpdateCurrent(SFML.System.Time dt)
        {
            base.UpdateCurrent(dt);

            if (Position.X <= _lowL || Position.X >= _highL)
                VelocityX  = -Velocity.X;

        }

    }
}
