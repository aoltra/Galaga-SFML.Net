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
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula un comando. 
    /// </summary>
    /// <remarks>
    /// Abstracta. No se puede instanciar
    /// </remarks>
    abstract class Command
    {
        /// <summary>
        /// Asigna o devuelve la categoria del sceneNode para la que está dirigido el comando
        /// </summary>
        /// <remarks>
        /// Admite 16 categorias posibles y la combinaciones posibles entre ellas.
        /// A efectos de un mejor manejo de las mismas es interesante crearse un tipo Enumerado categoria
        /// </remarks>
        public UInt16 Category { get; set; }

        /// <summary>
        /// Función a ejecutar en el comando
        /// </summary>
        /// <remarks>
        /// La función debe implementarse de manera obligatoria en la clases hijas
        /// </remarks>
        /// <param name="scNode">Nodo en el que se ejecuta el comando</param>
        /// <param name="dt">Incremento de tiemp desde la última actualización</param>
        public abstract void Execute(SceneNode scNode, SFML.System.Time dt);
    }
}
