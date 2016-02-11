using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Paths
{
    class User : CurvePath
    {
        //public User(float [,] waypoints)
        //{
        //    _waypoints = waypoints;
           
        //    // calculo de coeficientes
        //    _coef = Utilities.PolynomicUtilities.HermiteInterpolation(_waypoints);

        //    CalculateCurveLentgh();
        //}

        public User(float[,] coefficients)
        {
            _waypoints = null;
            _coef = coefficients;

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
