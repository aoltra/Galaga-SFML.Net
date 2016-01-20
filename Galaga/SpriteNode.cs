using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
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
