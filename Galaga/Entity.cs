#region GPL License
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
using System.Linq;
using System.Text;

using edu.CiclosFormativos.Games.DIDAM.Scenes;

using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula un objeto del juego
    /// </summary>
    abstract class Entity : SceneNode
    {
        // velocidad de la entidad;
        private Vector2f _velocity;
        
        /// <summary>
        /// Devuelve la velocidad de la entidad
        /// </summary>
        /// <remarks>
        /// La velocidad es una magnitud vectorial, no sólo importa el valor absoluto (el módulo), además es necesario
        /// conocer su dirección y sentido
        /// </remarks>
        public Vector2f Velocity {  get { return _velocity; } }

        /// <summary>
        /// Asigna el componente de la velocidad en X
        /// </summary>
        /// <remarks>
        /// El objetivo es reutilizar el mismo vector velocidad
        /// </remarks>
        public float VelocityX { set { _velocity.X = value; } }

        /// <summary>
        /// Asigna el componente de la velocidad en Y
        /// </summary>
        /// <remarks>
        /// El objetivo es reutilizar el mismo vector velocidad
        /// </remarks>
        public float VelocityY { set { _velocity.Y = value; } }


        /// <summary>
        /// Constructor básico. Inicializa a cero la velocidad
        /// </summary>
        public Entity() 
            : base ()
        {
            _velocity = new Vector2f(0.0f, 0.0f);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad de la entidad</param>
        public Entity(Vector2f velocity)
            : base ()
        {
            _velocity = velocity;
        }

        /// <summary>
        /// Actualizo la posición de la entidad
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        override protected void UpdateCurrent(SFML.System.Time dt)
        {
            // uso un operador sobrecargado para multiplicar vectores por escalares
            Position += Velocity * dt.AsSeconds();
        }
    }
}