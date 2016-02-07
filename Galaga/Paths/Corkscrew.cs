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
    class Corkscrew
    {
        public float Length { get { return totalLength; } }

        private float totalLength;
        private float[,] _coef;

        public float[,] getWaypoints(float xIni, float yIni, float xEnd, float yEnd,
            FloatRect worldBounds)
        {
            float[,] _waypoints = new float[7, 4];

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

            return _waypoints;
        }

        public float[,] getCoefficients(float xIni, float yIni, float xEnd, float yEnd,
            FloatRect worldBounds)
        {

            _coef = Utilities.PolynomicUtilities.HermiteInterpolation(getWaypoints(xIni, yIni, xEnd, yEnd, worldBounds));

            int numSeg = (int)_coef.GetLongLength(0)/2;

            for (int n = 0; n <numSeg ; n++)
                totalLength += GetArcLength(1, n);

            return _coef;
        }

        public Vector2f GetPoint(float t, int segmentIndex)
        {

            float t2 = t * t, t3 = t2 * t;

            return new Vector2f(_coef[2 * segmentIndex, 3] * t3 + _coef[2 * segmentIndex, 2] * t2
                       + _coef[2 * segmentIndex, 1] * t + _coef[2 * segmentIndex, 0],
                         _coef[2 * segmentIndex + 1, 3] * t3 + _coef[2 * segmentIndex + 1, 2] * t2
                       + _coef[2 * segmentIndex + 1, 1] * t + _coef[2 * segmentIndex + 1, 0]);

        }


        private Vector2f GetDerivate(float t, int segmentIndex)
        {
            float t2 = t * t, t3 = t2 * t;

            return new Vector2f(3 * _coef[2 * segmentIndex, 3] * t2 + 2 * _coef[2 * segmentIndex, 2] * t
                       + _coef[2 * segmentIndex, 1],
                         3 * _coef[2 * segmentIndex + 1, 3] * t3 + 2 * _coef[2 * segmentIndex + 1, 2] * t2
                       + _coef[2 * segmentIndex + 1, 1]);
        }

        private float Speed(float t, int segmentIndex)
        {
            return VectorUtilities.VectorLength(GetDerivate(t, segmentIndex));
        }


        public float GetCurveParameterEuler(float s, int segmentIndex) // 0 <= s <= L , o u t p u t i s t
        {
            float t = 0;
            float h = s / 100;

            for (int i = 1; i <= 100; i++)
            {
                // The d i v i s i o ns here migh t be a p r o blem i f t h e d i v i s o r s a r e
                // n e a r l y z e r o .
                float k1 = h / Speed(t, segmentIndex);
                float k2 = h / Speed(t + k1 / 2, segmentIndex);
                float k3 = h / Speed(t + k2 / 2, segmentIndex);
                float k4 = h / Speed(t + k3, segmentIndex);
                t += (k1 + 2 * (k2 + k3) + k4) / 6;
            }

            return t;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tf">entre 0 y 1</param>
        /// <param name="segmentIndex"></param>
        /// <returns></returns>
        public float GetArcLength(float tf, int segmentIndex)
        {

            if (tf > 1) tf = 1;
            if (tf < 0) tf = 0;

            int grade = (int)(tf * 100);
            int segX = 2 * segmentIndex;

            float incT = (tf / (float)grade), t, t2, t3;
            float segLength = 0, xTempI, yTempI, xTempE, yTempE;
            for (int grd = 0; grd < grade; grd++)
            {
                t = grd * incT; t2 = t * t; t3 = t2 * t;
                xTempI = _coef[segX, 3] * t3 + _coef[segX, 2] * t2
                    + _coef[segX, 1] * t + _coef[segX, 0];
                yTempI = _coef[segX + 1, 3] * t3 + _coef[segX + 1, 2] * t2
                    + _coef[segX + 1, 1] * t + _coef[segX + 1, 0];

                t = (grd + 1) * incT; t2 = t * t; t3 = t2 * t;
                xTempE = _coef[segX, 3] * t3 + _coef[segX, 2] * t2
                    + _coef[segX, 1] * t + _coef[segX, 0];
                yTempE = _coef[segX + 1, 3] * t3 + _coef[segX + 1, 2] * t2
                    + _coef[segX + 1, 1] * t + _coef[segX + 1, 0];

                segLength += Utilities.VectorUtilities.VectorLength(new Vector2f(xTempI - xTempE, yTempI - yTempE));

            }

            return segLength;

        }

        public float GetCurveParameterNewton(float s, int segmentIndex) 
        {
            float t = s /  GetArcLength(1,segmentIndex);
            float epsilon = 0.0001f;

            float low = 0, upper = 1;

            for (int i = 0; i < 40; i++) 
            {
                float F = GetArcLength(t, segmentIndex) - s;
                if (Math.Abs(F) < epsilon)
                {
                    return t;
                }

                float DF = Speed(t, segmentIndex);
                float tCandidate = t - F / DF;
                if (F > 0)
                {
                    upper = t;
                    if (tCandidate <= low)
                    {
                        t = 0.5f * (upper + low);
                    }
                    else
                    {
                        t = tCandidate;
                    }

                }
                else
                {
                    low = t;

                    if (tCandidate <= upper)
                    {
                        t = 0.5f * (upper + low);
                    }
                    else
                    {
                        t = tCandidate;
                    }
                }
            }

            return t;


        }
    }
}
