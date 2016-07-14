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

using SFML.System;
using SFML.Graphics;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Background
{
    /// <summary>
    /// Encapsula un generador de estrellas
    /// </summary>
    class StarBackgroundGenerator
    {
        /// <summary>
        /// Tipos de estrella 
        /// </summary>
        public enum StarType
        {
            SMALL,          // 0
            MEDIUM,         // 1
            BIG             // 2     
        }

        /// <summary>
        /// Genera un lista de estrellas
        /// </summary>
        /// <param name="number">número de estrellas. Tiene que se un valor entre 0 -255)</param>
        /// <param name="type">tipo de las estrellas a generar</param>
        /// <param name="velocity">velocidad de las estrellas a generar</param>
        /// <param name="window">ventana para la que se generar las estrellas</param>
        /// <returns>Lista de estrellas</returns>
        static public List<Star> StarGenerator(Byte number, StarType type, Vector2f velocity, RenderWindow window)
        {
            float _baseRadius = 0.5f;                   // radio base de las estrellas del fondo
      
            Star star;
            int  posX, posY;            // asigno coordenadas enteras
            List<Star> _stars = new List<Star>();
           
            switch (type)
            {
                case StarType.SMALL: _baseRadius = 0.5f; break;
                case StarType.MEDIUM: _baseRadius = 0.8f; break;
                case StarType.BIG: _baseRadius = 1.5f; break;
            }

            Random rnd = new Random();
            
            // Creo las estrellas
            for (int contStar = 0; contStar < number; contStar++)
            {
                byte red = (byte)rnd.Next(150,255);
               
                posX = rnd.Next((int)window.Size.X);
                posY = rnd.Next(-(int)(window.Size.Y * .02), (int)(window.Size.Y * 1.02));

                star = new Star(new Vector2f(posX, posY),_baseRadius,new Color(red,red, 255),velocity);

                _stars.Add(star);
            }

            return _stars;
        }
    }
}
