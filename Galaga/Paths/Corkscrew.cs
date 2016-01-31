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
using System.Diagnostics;

using SFML.Graphics;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Paths
{
    /// <summary>
    /// Encapsula el recorrido sacacorchos
    /// </summary>
    class Corkscrew
    {
        public static float[,] getWaypoints(float xIni, float yIni, float xEnd, float yEnd,
            FloatRect worldBounds) 
        {
            float[,] _waypoints = new float[5, 4];

            _waypoints[0, 0] = xIni;
            _waypoints[0, 1] = yIni;
            _waypoints[0, 2] = 1;
            _waypoints[0, 3] = 0;

            _waypoints[1, 0] = worldBounds.Width / 2;
            _waypoints[1, 1] = worldBounds.Height / 2;
            _waypoints[1, 2] = -1;
            _waypoints[1, 3] = 1;

            _waypoints[2, 0] = worldBounds.Width / 2 - (xIni - worldBounds.Width / 2);
            _waypoints[2, 1] = worldBounds.Height / 3;
            _waypoints[2, 2] = 1;
            _waypoints[2, 3] = 1;

            _waypoints[3, 0] = worldBounds.Width / 2 - (xIni - worldBounds.Width / 2);
            _waypoints[3, 1] = worldBounds.Height / 2;
            _waypoints[3, 2] = 1;
            _waypoints[3, 3] = 1;

            _waypoints[4, 0] = xEnd;
            _waypoints[4, 1] = yEnd;
            _waypoints[4, 2] = -1;
            _waypoints[4, 3] = 0;

            return _waypoints;
        }
      
        public static float[,] getCoefficients(float xIni, float yIni, float xEnd, float yEnd, 
            FloatRect worldBounds) {
            
            return Utilities.PolynomicUtilities.HermiteInterpolation(getWaypoints(xIni, yIni, xEnd, yEnd, worldBounds));
        }
    }
}
