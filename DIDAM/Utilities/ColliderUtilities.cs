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

using SFML.System;
using SFML.Graphics;

using edu.CiclosFormativos.Games.DIDAM.Entities;

namespace edu.CiclosFormativos.Games.DIDAM.Utilities
{
    /// <summary>
    /// Encapsula utilidades de manejo de colisiones
    /// </summary>
    public class ColliderUtilities
    {
        /// <summary>
        /// Devuelve si hay o no colisión entre dos nodos del grafo de escena 
        /// que tengan un collider asociado
        /// </summary>
        /// <param name="vector">vector</param>
        /// <returns>longitud del vector</returns>
        public static bool Collision(ICollider lhs, ICollider rhs)
        {
            Collider colliderL = lhs.GetCollider();
            Collider colliderR = rhs.GetCollider();

            // los dos collider son rectangulos
            if (!colliderL.IsCircle && !colliderR.IsCircle)
                return colliderL.Rectangle.Intersects(colliderR.Rectangle);
            else if (colliderL.IsCircle && colliderR.IsCircle)    // los dos son círculos
                return colliderL.Circle.Intersects(colliderL.Circle);
            else if (!colliderL.IsCircle && colliderR.IsCircle)
                return colliderR.Circle.Intersects(colliderL.Rectangle);
            else
                return colliderL.Circle.Intersects(colliderR.Rectangle);

        }
    }
}
