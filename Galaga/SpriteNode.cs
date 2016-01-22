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

using SFML.Graphics;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula un nodo que únicamente muestra un sprite, sin movimiento.
    /// </summary>
    class SpriteNode : SceneNode
    {
        private Sprite sprite;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">textura a dibujar</param>
        public SpriteNode(Texture texture) 
            : this (texture, new IntRect(0,0,0,0))
        {  }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">textura a dibujar</param>
        /// <param name="rect">Rectangulo de la textura que se desea ver</param>
        public SpriteNode(Texture texture, IntRect rect)
        {
            sprite = new Sprite(texture, rect);
        }

        /// <summary>
        /// Dibuja el sprite
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        protected override void DrawCurrent(RenderTarget rt, RenderStates rs)
        {
            rt.Draw(sprite, rs);
        }
    }
}
