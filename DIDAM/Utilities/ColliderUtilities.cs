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

        /// <summary>
        /// Devuelve un par ordenado, es decir, el nodo con HashID más bajo delente
        /// </summary>
        /// <param name="lhs">Primer Collider</param>
        /// <param name="rhs">Segundo Collider</param>
        /// <returns>un objeto tipo pair de SceneNode ordenados</returns>
        public static Tuple<Scenes.SceneNode,Scenes.SceneNode> GetSortedPair(ICollider lhs, ICollider rhs) 
        {
            return (lhs.GetHashCode() > rhs.GetHashCode()) ? 
                new Tuple<Scenes.SceneNode, Scenes.SceneNode>((Scenes.SceneNode)rhs, (Scenes.SceneNode)lhs) : 
                new Tuple<Scenes.SceneNode, Scenes.SceneNode>((Scenes.SceneNode)rhs, (Scenes.SceneNode)lhs);
        }

        public static bool MatchesCategories(ref Tuple<Scenes.SceneNode, Scenes.SceneNode> colliders, uint type1, uint type2)
        {
            uint category1 = ((Scenes.SceneNode)colliders.Item1).Category;
            uint category2 = ((Scenes.SceneNode)colliders.Item2).Category;

            // hay que recordar que los SceneNode podrían ser de varias categorías al mismo tiempo
            if ((type1 & category1) !=0  && (type2 & category2) !=0)
            {
                return true;
            }
            else if ((type1 & category2) !=0  && ((type2 & category1)!=0))
            {
                colliders = new Tuple<Scenes.SceneNode, Scenes.SceneNode>((Scenes.SceneNode)colliders.Item2, (Scenes.SceneNode)colliders.Item1);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
