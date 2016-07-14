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

namespace edu.CiclosFormativos.Games.DIDAM.Paths
{
    /// <summary>
    /// Encapsula las utilidades necesarias para la resolución de SLE (sistemas de ecuaciones lineales)
    /// </summary>
    /// <remarks>
    /// Basado en las ideas del artículo de James McCaffrey
    /// https://msdn.microsoft.com/de-de/magazine/jj863137.aspx 
    /// </remarks>
    class MatrixUtilities
    {
        /// <summary>
        /// Soluciona un sistem de ecuaciones lineales
        /// </summary>
        /// <param name="A">Matriz de coeficientes <b>A</b> de <i>Ax=b</i></param>
        /// <param name="b">Vector <b>b</b> de <i>Ax=b</i></param>
        /// <returns>Vector de soluciones</returns>
        public static double[] SystemSolve(double[][] A, double[] b)
        {
            int numElem = A.Length;             // número de elementos de la matriz
            int[] perm;                        
            int toggle;                          
            double[][] luMatrix;

            luMatrix = MatrixDecompose(A, out perm, out toggle);
            if (luMatrix == null)
                return null;                        
            
            double[] bp = new double[b.Length];
            for (int i = 0; i < numElem; ++i)
                bp[i] = b[perm[i]];

            double[] x = HelperSolve(luMatrix, bp);
            
            return x;
        }

        /// <summary>
        /// Descomposicion LU
        /// </summary>
        /// <param name="matrix">Matriz a descomponer</param>
        /// <param name="perm"></param>
        /// <param name="toggle"></param>
        /// <returns>Matriz descompuesta</returns>
        static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            int n = matrix.Length;                          
            double[][] result = MatrixDuplicate(matrix);
            perm = new int[n];

            for (int i = 0; i < n; ++i) { perm[i] = i; }
            
            toggle = 1;
            for (int j = 0; j < n - 1; ++j)                     // columnas
            {
                double colMax = Math.Abs(result[j][j]);         // valor más alto en la columna j
                int pRow = j;
                for (int i = j + 1; i < n; ++i)
                {
                    if (result[i][j] > colMax)
                    {
                        colMax = result[i][j];
                        pRow = i;
                    }
                }

                if (pRow != j)                                  // cambia las filas
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;
                    int tmp = perm[pRow];                       
                    perm[pRow] = perm[j];
                    perm[j] = tmp;
                    toggle = -toggle; 
                }
                if (Math.Abs(result[j][j]) < 1.0E-20)       // cero
                    return null; 

                for (int i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1; k < n; ++k)
                        result[i][k] -= result[i][j] * result[j][k];
                }
            }

            return result;
        }

        /// <summary>
        /// Duplica una matriz
        /// </summary>
        /// <param name="matrix">Matriz en formato array de punteros [][]</param>
        /// <returns>Matriz duplicada en formato [][] </returns>
        static double[][] MatrixDuplicate(double[][] matrix)
        {
            double[][] result = MatrixCreate(matrix.Length, matrix[0].Length);

            for (int i = 0; i < matrix.Length; ++i)             // copia los valores
                for (int j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];

            return result;
        }

        /// <summary>
        /// Crea una matriz de NxM, inicializada a ceros, en formato array de punteros [][]
        /// </summary>
        /// <param name="rows">número de filas</param>
        /// <param name="cols">número de columnas</param>
        /// <returns>Matriz creada en formato [][]</returns>
        public static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];

            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];               // se inicializa automaticamente con 0.0
            
            return result;
        }

        /// <summary>
        /// Calcula el determinante de una matriz 
        /// </summary>
        /// <param name="matrix">Matriz en formato de array de punteros [][]</param>
        /// <returns>Determinante de la matriz</returns>
        public static double MatrixDeterminant(double[][] matrix)
        {
            int[] perm;
            int toggle;

            double[][] lum = MatrixDecompose(matrix, out perm, out toggle);
            if (lum == null)
                throw new Exception("No es posible calcular el determinante");
            
            double result = toggle;
            for (int i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            
            return result;
        }

        /// <summary>
        /// Soluciona el sistema de ecuaciones a partir de un amatriz LU
        /// </summary>
        /// <param name="luMatrix">Matriz LU</param>
        /// <param name="b">Coeficientes b</param>
        /// <returns>Vector de soluciones</returns>
        private static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
         
            b.CopyTo(x, 0);
            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }
            
            x[n - 1] /= luMatrix[n - 1][n - 1];
            
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }
            
            return x;
        }
    }
}
