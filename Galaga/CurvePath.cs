﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.System;

using edu.CiclosFormativos.DAM.DI.Galaga.Utilities;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Paths
{
    abstract class CurvePath
    {
        /// <summary>
        /// Devuelve una matriz de float con los waypoints
        /// </summary>
        public float[,] Waypoints { get { return _waypoints; } }

        /// <summary>
        /// Devuelve una matriz de float con los waypoints
        /// </summary>
        public float[,] Coefficients { get { return _coef; } }

        /// <summary>
        /// Devuelve la longitud total de la curva
        /// </summary>
        public float Length { get { return _totalLength; } }

        /// <summary>
        /// Devuelve el número de segmento de la curva
        /// </summary>
        public int NumSegments { get { return (int)_waypoints.GetLongLength(0) - 1; } }

        protected float _totalLength;             // longitud total de la curva
        protected float[,] _waypoints;            // puntos que definen los segmentos
        protected float[,] _coef;                 // coeficientes


        public Vector2f GetPoint(float t, int segmentIndex)
        {
            float t2 = t * t, t3 = t2 * t;

            return new Vector2f(_coef[2 * segmentIndex, 3] * t3 + _coef[2 * segmentIndex, 2] * t2
                       + _coef[2 * segmentIndex, 1] * t + _coef[2 * segmentIndex, 0],
                         _coef[2 * segmentIndex + 1, 3] * t3 + _coef[2 * segmentIndex + 1, 2] * t2
                       + _coef[2 * segmentIndex + 1, 1] * t + _coef[2 * segmentIndex + 1, 0]);

        }

        /// <summary>
        /// Calcula la longitud de la curva desde 0 a <paramref name="tf"/> de la curva mediante método numéricos <paramref name="segmentIndex"/>
        /// </summary>
        /// <param name="tf">entre 0 y 1</param>
        /// <param name="segmentIndex">Segmento a estudiar</param>
        /// <returns>Longitud de la curva en ese tramo</returns>
        private float GetArcLength(float tf, int segmentIndex)
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

                segLength += Utilities.VectorUtilities.VectorLength(xTempI - xTempE, yTempI - yTempE);
            }

            return segLength;
        }

        /// <summary>
        /// Obtiene el valor de t en función de la distancia desde el principio del segmento <paramref name="segmentIndex"/> mediante 
        /// el método de Newton
        /// </summary>
        /// <param name="s">Distancia</param>
        /// <param name="segmentIndex">Segmento a estudiar (base zero)</param>
        /// <returns>t entre 0 y 1 para el que esa distancia se habrá recorrido</returns>
        public float GetCurveParameterNewton(float s, int segmentIndex)
        {
            float t = s / _coef[2 * segmentIndex, 4];
            float epsilon = 0.001f;

            float low = 0, upper = 1;

            for (int i = 0; i < 40; i++)
            {
                // la s que le paso ya está lo suficientemente cerca del tiempo que estudio
                float F = GetArcLength(t, segmentIndex) - s;
                if (Math.Abs(F) < epsilon)
                    return t;

                // si no ha cumplido busco otro t candidato
                // para ello hago una búsqueda binaria
                float DF = Speed(t, segmentIndex);
                float tCandidate = t - F / DF;
                if (F > 0)
                {
                    upper = t;
                    if (tCandidate <= low)
                        t = 0.5f * (upper + low);
                    else
                        t = tCandidate;
                }
                else
                {
                    low = t;

                    if (tCandidate <= upper)
                        t = 0.5f * (upper + low);
                    else
                        t = tCandidate;
                }
            }

            return t;
        }

        /// <summary>
        /// devuelve el módulo de la velocidad para un <paramref name="t"/> determinado dentro del segmento número <paramref name="segmentIndex"/>
        /// </summary>
        /// <param name="t">t entre 0 y 1</param>
        /// <param name="segmentIndex">Segmento a estudiar (base zero)</param>
        /// <returns></returns>
        private float Speed(float t, int segmentIndex)
        {
            return VectorUtilities.VectorLength(GetDerivate(t, segmentIndex));
        }

        /// <summary>
        /// Devuelve la primera derivada en un punto determinado <paramref name="t"/> de la curva
        /// </summary>
        /// <param name="t">valor del parámetro t (entre 0 y 1)</param>
        /// <param name="segmentIndex">Segmento a estudiar (base zero)</param>
        /// <returns>Vector que marca la dirección de derivada</returns>
        private Vector2f GetDerivate(float t, int segmentIndex)
        {
            float t2 = t * t, t3 = t2 * t;

            return new Vector2f(3 * _coef[2 * segmentIndex, 3] * t2 + 2 * _coef[2 * segmentIndex, 2] * t
                       + _coef[2 * segmentIndex, 1],
                         3 * _coef[2 * segmentIndex + 1, 3] * t3 + 2 * _coef[2 * segmentIndex + 1, 2] * t2
                       + _coef[2 * segmentIndex + 1, 1]);
        }

        /// <summary>
        /// Obtiene el valor de t en función de la distancia desde el principio del segmento <paramref name="segmentIndex"/> mediante 
        /// el método de Newton
        /// </summary>
        /// <param name="s">Distancia</param>
        /// <param name="segmentIndex">Segmento a estudiar (base zero)</param>
        /// <returns>t entre 0 y 1 para el que esa distancia se habrá recorrido</returns>
        public float GetCurveParameterEuler(float s, int segmentIndex)
        {
            float t = 0;
            float h = s / 100;

            for (int i = 1; i <= 100; i++)
            {
                float k1 = h / Speed(t, segmentIndex);
                float k2 = h / Speed(t + k1 / 2, segmentIndex);
                float k3 = h / Speed(t + k2 / 2, segmentIndex);
                float k4 = h / Speed(t + k3, segmentIndex);
                t += (k1 + 2 * (k2 + k3) + k4) / 6;
            }

            return t;
        }

    }
}