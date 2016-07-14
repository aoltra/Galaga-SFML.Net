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

namespace edu.CiclosFormativos.Games.DIDAM.Utilities
{
    /// <summary>
    /// Encapsula la creación de curvas polinómicas
    /// </summary>
    /// <remarks>
    /// Soporta los métodos:
    /// - Hermitte
    /// - TODO: Bezier grado 2 y 3
    /// </remarks>
    public class PolynomicUtilities
    {
        /// <summary>
        /// Grado de precisión para el cálculo de la longitud
        /// </summary>
        public static Byte Grade { get { return grade;  } set { grade = value;} }

        private static Byte grade = 100;

        /// <summary>
        /// Calculo de la curva polinómica cúbica que interpola los waypoints manteniendo una determinada
        /// pendiente en cada uno de ellos. Método de Hermitte
        /// </summary>
        /// <param name="points">Matriz de Nx4 (por cada fila x,y,pendX,pendY en el waypoint)</param>
        /// <returns>
        /// Una matriz de 2*nx5, donde n es el número de segmentos. 
        /// Las 4 primeras columnas son los coeficientes de la función de interpolación 
        /// (para x las filas impares y para y las pares)
        /// El 5 es la longitud del segmento</returns>
        public static float[,] HermiteInterpolation(float [,] points)
        {
            // la segunda dimensión de la matriz tiene que ser de 4 valores (x,y,pendX,pendY)
            Debug.Assert(points.GetLongLength(1) == 4);
            Debug.Assert(points.GetLongLength(0) > 1);

            int numSegments = (int)points.GetLongLength(0) - 1, segX;
            int numWaypoints = (int)points.GetLongLength(0);

            float [,] polynomicSegments = new float[2*numSegments,5];

            double[][] mat = MatrixUtilities.MatrixCreate(4 * numSegments, 4 * numSegments);
            double[][] b = MatrixUtilities.MatrixCreate(2, 4 * numSegments);

		    mat[0][0] = 1;
		    mat[1][1] = 1;

            mat[4 * numSegments - 2][4 * numSegments - 4] = 1;
            mat[4 * numSegments - 2][4 * numSegments - 3] = 1;
            mat[4 * numSegments - 2][4 * numSegments - 2] = 1;
            mat[4 * numSegments - 2][4 * numSegments - 1] = 1;

            mat[4 * numSegments - 1][4 * numSegments - 3] = 1;
            mat[4 * numSegments - 1][4 * numSegments - 2] = 2.0;
            mat[4 * numSegments - 1][4 * numSegments - 1] = 3.0;

            // posición inicial
		    b[0][0] = points[0,0]; 
		    b[1][0] = points[0,1]; 

		    // pendiente inicial
		    b[0][1] = points[0,2];
		    b[1][1] = points[0,3]; 

		    // Posición final
            b[0][4 * numSegments - 2] = points[numWaypoints - 1, 0];
            b[1][4 * numSegments - 2] = points[numWaypoints - 1, 1];

		    // Pendiente final
            b[0][4 * numSegments - 1] = points[numWaypoints - 1, 2];
            b[1][4 * numSegments - 1] = points[numWaypoints - 1, 3];
		
		    for (int i = 1; i < numWaypoints - 1; i++) 
            {
			    int k = 4 * (i - 1) + 2;
			    int l = 4 * (i - 1);

			    mat[k][l] = 1;
			    mat[k][l + 1] = 1;
			    mat[k][l + 2] = 1;
			    mat[k][l + 3] = 1;

			    mat[k + 3][l + 4] = 1;

			    mat[k + 1][l + 1] = 1.0;
			    mat[k + 1][l + 2] = 2.0;
			    mat[k + 1][l + 3] = 3.0;
			    mat[k + 1][l + 5] = -1.0;

			    mat[k + 2][l + 2] = 2.0;
			    mat[k + 2][l + 3] = 6.0;
			    mat[k + 2][l + 6] = -2.0;

			    // Location
			    b[0][k] = points[i,0];
			    b[1][k] = points[i,1];

			    // Pendiente
			    b[0][k + 1] = points[i,2];
                b[1][k + 1] = points[i,3];

			    // pendiente
                b[0][k + 2] = points[i, 2];
                b[1][k + 2] = points[i, 3];

			    // punto intemedio
                b[0][k + 3] = points[i, 0];
                b[1][k + 3] = points[i, 1];
		    }

#if DEBUG
		    // Impresión del sistema de ecuaciones
            //int i2b = 0;
            //for (int i = 0; i < 4 * numSegments; i++)
            //{
            //    for (int i2 = 0; i2 < 4 * numSegments; i2++) 
            //        Debug.Write(mat[i][i2].ToString("F3").PadLeft(8) +"  ");
               
            //    Debug.Write("   |   ");
                
            //    Debug.WriteLine(b[0][i2b]  +  "  " + b[1][i2b]);
            //    i2b++;
            //}
#endif

            // resolución
            double[] resX = MatrixUtilities.SystemSolve(mat, b[0]);
            double[] resY = MatrixUtilities.SystemSolve(mat, b[1]);

            // almaceno los datos en los elementos de salida
            for (int i = 0; i < numSegments; i++)
            {
                for (int j = 0; j < 4; j++) 
                {
                    polynomicSegments[2*i, j] = (float)resX[j + 4 * i];
                    polynomicSegments[2*i + 1, j] = (float)resY[j + 4 * i];
                }
            }

            // calculo de longitudes de cada segmento
            for (int seg = 0; seg < numSegments; seg++)
            {
                segX = 2 * seg;
                  
                float incT = (1f / (float)grade), t, t2, t3;
                float segLength = 0, xTempI, yTempI, xTempE, yTempE;
                for (int grd = 0; grd < grade; grd++)
                {
                    t = grd * incT; t2 = t * t; t3 = t2 * t;
                    xTempI = polynomicSegments[segX, 3] * t3 + polynomicSegments[segX, 2] * t2
                        + polynomicSegments[segX, 1] * t + polynomicSegments[segX, 0];
                    yTempI = polynomicSegments[segX + 1, 3] * t3 + polynomicSegments[segX + 1, 2] * t2
                        + polynomicSegments[segX + 1, 1] * t + polynomicSegments[segX + 1, 0];

                    t = (grd + 1) * incT; t2 = t * t; t3 = t2 * t;
                    xTempE = polynomicSegments[segX, 3] * t3 + polynomicSegments[segX, 2] * t2
                        + polynomicSegments[segX, 1] * t + polynomicSegments[segX, 0];
                    yTempE = polynomicSegments[segX + 1, 3] * t3 + polynomicSegments[segX + 1, 2] * t2
                        + polynomicSegments[segX + 1, 1] * t + polynomicSegments[segX + 1, 0];

                    segLength += Utilities.VectorUtilities.VectorLength(xTempI - xTempE, yTempI - yTempE);

                }

                polynomicSegments[segX, 4] = polynomicSegments[segX + 1, 4] = segLength;
            }

            return polynomicSegments;
        }
    }
}
