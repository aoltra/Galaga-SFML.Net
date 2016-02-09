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
using SFML.System;

using edu.CiclosFormativos.DAM.DI.Galaga.Utilities;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Paths
{
    /// <summary>
    /// Encapsula el recorrido sacacorchos
    /// </summary>
    /// <remarks>
    /// Existe la posibilidad de crear multiples path y que cada uno de ellos actue sólo en determinadas fases del juego
    /// por lo que resulta más interesante instanciarla al principio de cada fase bajo demanda
    /// </remarks>
    class Corkscrew : CurvePath
    {
        public Corkscrew(float xIni, float yIni, float xEnd, float yEnd,
            FloatRect worldBounds)
        {
            _waypoints = new float[7, 4];
           
            _waypoints[0, 0] = xIni;
            _waypoints[0, 1] = yIni;
            _waypoints[0, 2] = 0;
            _waypoints[0, 3] = 1;

            _waypoints[1, 0] = worldBounds.Width * 0.45f;
            _waypoints[1, 1] = worldBounds.Height * .2f;
            _waypoints[1, 2] = 1;
            _waypoints[1, 3] = 2;

            _waypoints[2, 0] = worldBounds.Width * 0.125f;
            _waypoints[2, 1] = worldBounds.Height / 2;
            _waypoints[2, 2] = 1;
            _waypoints[2, 3] = 1;

            _waypoints[3, 0] = worldBounds.Width * 0.125f;
            _waypoints[3, 1] = worldBounds.Height * .7f;
            _waypoints[3, 2] = -1;
            _waypoints[3, 3] = 1;

            _waypoints[4, 0] = worldBounds.Width * 0.45f;
            _waypoints[4, 1] = worldBounds.Height * 0.6f;
            _waypoints[4, 2] = 1;
            _waypoints[4, 3] = 10;

            _waypoints[5, 0] = xEnd;
            _waypoints[5, 1] = yEnd + 70;
            _waypoints[5, 2] = 0;
            _waypoints[5, 3] = -1;

            _waypoints[6, 0] = xEnd;
            _waypoints[6, 1] = yEnd;
            _waypoints[6, 2] = -0f;
            _waypoints[6, 3] = -1;

            // calculo de coeficientes
            _coef = Utilities.PolynomicUtilities.HermiteInterpolation(_waypoints);

            // calculo de la longitud total de la curva
            for (int nSeg = 0; nSeg < NumSegments; nSeg++)
                _totalLength += _coef[2*nSeg, 4];
        }

        /// <summary>
        /// Sobreescritura del ToString
        /// </summary>
        /// <returns>Descripción de la cruva</returns>
        public override string ToString()
        {
            return "CorkscrewPath";
        }
    }
}
