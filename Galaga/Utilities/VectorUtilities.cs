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

namespace edu.CiclosFormativos.DAM.DI.Galaga.Utilities
{
    /// <summary>
    /// Encapsula utilidades de manejo de vectores
    /// </summary>
    public class VectorUtilities
    {
        /// <summary>
        /// Devuelve la longitud de un vector
        /// </summary>
        /// <param name="vector">vector</param>
        /// <returns>longitud del vector</returns>
        public static float VectorLength(SFML.System.Vector2f vector)
        {
            return VectorLength(vector.X,vector.Y);
        }

        /// <summary>
        /// Devuelve la longitud de un vector
        /// </summary>
        /// <param name="x">coordenada X</param>
        /// <param name="y">coordenada Y</param>
        /// <returns>longitud del vector</returns>
        public static float VectorLength(float x, float y)
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
    }
}
