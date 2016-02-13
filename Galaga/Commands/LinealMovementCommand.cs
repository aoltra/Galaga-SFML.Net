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
            ((Entity)subject).Position += velocity * dt.AsSeconds();
        }
    }
}
