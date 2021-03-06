﻿#region GPL License
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
using System.Text;

using edu.CiclosFormativos.Games.DIDAM.Resources;
using edu.CiclosFormativos.Games.DIDAM.Entities;

using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    class PlatoonLeader : TransformEntity
    {
        // limite inferior y superior del movimiento del lider
        private float _lowL, _highL;

        // Animación que marca los tiempos para los elementos del pelotón
        private Animation _animation;

        /// <summary>
        /// Devuelve el tile actual
        /// </summary>
        public uint CurrentTile 
        { 
            get { return _animation.CurrentTile; } 
        }

        /// <summary>
        /// Devuelve el tiempo que lleva el tile actual activo
        /// </summary>
        public SFML.System.Time CurrentTime { get { return _animation.CurrentTime; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad del lider (dirección X)</param>
        /// <param name="lowL">X inferior del movimiento del líder</param>
        /// <param name="highL">X superior del movimiento del líder</param>
        public PlatoonLeader(float velocity, float lowL, float highL, ResourcesManager resManager)
            : this(velocity, lowL, highL, new SFML.Graphics.Color(100,80,250), resManager)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad del líder</param>
        /// <param name="lowL">X inferior del movimiento del líder</param>
        /// <param name="highL">X superior del movimiento del líder</param>
        /// <param name="color">Color con que se representa</param>
        public PlatoonLeader(float velocity, float lowL, float highL, SFML.Graphics.Color color, ResourcesManager resManager)
            : base(new Vector2f(velocity,0))
        {
            _shape.FillColor = color;
            _lowL = lowL;
            _highL = highL;

            _animation = new Animation((SFML.Graphics.Texture)resManager["Naves:DummyLeader"], new Vector2u(1, 1), SFML.System.Time.FromSeconds(1f));

            _animation.Run();
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

            _animation.Update(dt);

            if (Position.X <= _lowL || Position.X >= _highL)
                VelocityX  = -Velocity.X;

        }

    }
}
