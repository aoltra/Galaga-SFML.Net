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

using SFML.System;

using edu.CiclosFormativos.Games.DIDAM.Paths;
using edu.CiclosFormativos.Games.DIDAM.Paths;


namespace edu.CiclosFormativos.DAM.DI.Galaga.Paths
{
    class User : CurvePath
    {
        public enum DataType { WAYPOINTS, COEFFICENTS };

        public User(float[,] input, DataType data = DataType.WAYPOINTS)
        {
            if (data == DataType.WAYPOINTS)
            {
                _waypoints = input;

                // calculo de coeficientes
                _coef = PolynomicUtilities.HermiteInterpolation(_waypoints);
            }
            else 
            {
                _waypoints = null;
                _coef = input;
            }

            CalculateCurveLength();
        }
    }

    /// <summary>
    /// Crea un curva simétrica a otra con respecto a un eje
    /// </summary>
    /// <remarks>
    /// Sólo ejes ortogonales
    /// </remarks>
    class Symmetric : CurvePath
    {
        public Symmetric(CurvePath curve, Vector2f [] axis)
        {
            Debug.Assert((axis[0].X == axis[1].X || axis[0].Y == axis[1].Y) &&
                         (axis[0].X != axis[1].X || axis[0].Y != axis[1].Y), 
                         "Simetría con respecto a ejes no ortogonales no soportada");

            Debug.Assert(axis.GetLength(0) == 2, "Eje mal expresado");

            _coef = new float [2*curve.NumSegments,5];
            for (int seg = 0; seg < curve.NumSegments; seg++)
            {
                // simetría vertical
                if (axis[0].X == axis[1].X)
                {    
                    _coef[2 * seg, 0] = 2*axis[0].X - curve.Coefficients[2 * seg, 0];
                    _coef[2 * seg + 1, 0] = curve.Coefficients[2 * seg + 1, 0];
                } 
                else 
                {
                    _coef[2 * seg, 0] = curve.Coefficients[2 * seg, 0];
                    _coef[2 * seg + 1, 0] = 2 * axis[0].Y - curve.Coefficients[2 * seg + 1, 0]; ;
                }

                // copio la longitud
                _coef[2 * seg + 1, 4] = _coef[2 * seg, 4] = curve.Coefficients[2 * seg, 4];

                for (int grade=1;grade<4;grade++)
                {
                    // simetría vertical
                    if (axis[0].X == axis[1].X)
                    {
                        _coef[2 * seg, grade] = -1 * curve.Coefficients[2 * seg, grade];
                        _coef[2 * seg + 1, grade] = curve.Coefficients[2 * seg + 1, grade];
                    }
                    else // simetría horizontal
                    {
                        _coef[2 * seg, grade] = curve.Coefficients[2 * seg, grade];
                        _coef[2 * seg + 1, grade] = -1 * curve.Coefficients[2 * seg + 1, grade];
                    }
                }
            }

            _totalLength = curve.Length;
        }
    }
}
