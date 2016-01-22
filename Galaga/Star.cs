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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using SFML.Graphics;
using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula una estrella del background
    /// </summary>
    class Star : Entity
    {
        // Variables privadas
        private CircleShape _shape;                 // circulo que define la estrella

        /// <summary>
        /// Asiga no devuelve el valor del tamaño del campo de estrellas. Es común para todas las estrellas generadas
        /// </summary>
        public static FloatRect Size { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Posición inicial</param>
        /// <param name="radius">Radio</param>
        /// <param name="color">Color de la estrella</param>
        /// <param name="velocity">Velocidad de la estrella</param>
        public Star(Vector2f pos, float radius, Color color, Vector2f velocity) 
            : base(velocity) {

            _shape = new CircleShape(radius);
            Position = pos;
            _shape.FillColor = color;
        }

        /// <summary>
        /// Dibuja la estrella
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        override protected void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {
            // en el destino (rt) dibujamos el sprite con un estado determinado (rs)
            rt.Draw(_shape, rs);
        }


        /// <summary>
        /// Actualiza la estrella
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <remarks>
        /// Ya nadie puede heredarla
        /// </remarks>
        override sealed protected void UpdateCurrent(SFML.System.Time dt)
        {
            // se llama a la funciín del padre
            base.UpdateCurrent(dt);

            Debug.Assert(Size.Height > 0);
            Debug.Assert(Size.Width > 0);

            if (Position.Y > Size.Height)
            {
                Random rnd = new Random();
                Position = new Vector2f(rnd.Next((int)Size.Left, (int)Size.Width), -10);
            }
        }
    }
}
