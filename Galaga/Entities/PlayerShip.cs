using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    /// <summary>
    /// Encapsula la funcionalidad de la nave del jugador
    /// </summary>
    /// <remarks>
    /// - Puede disparar
    /// - Es controlada por el usuario
    /// - Puede colisionar con los disparos de otras naves y con las propias naves
    /// </remarks>
    class PlayerShip : Entity
    {
        private Sprite _sprite;         // sprite donde dibujar la textura

        public PlayerShip(Resources.ResourcesManager resManager)
            : base() 
        {
            _sprite = new Sprite((Texture)resManager["Naves:NaveJugador"]);
        }

        /// <summary>
        /// Dibuja el PlayerShip
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="rs"></param>
        override protected void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {
            // en el destino (rt) dibujamos el sprite con un estado determinado (rs)
            rt.Draw(_sprite,rs);    
        }
    }
}
