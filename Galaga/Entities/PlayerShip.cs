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

        /// <summary>
        /// Devuelve la/s categoria/s del PlayerShip
        /// </summary>
        public override UInt16 Category
        {
            get { return (UInt16)edu.CiclosFormativos.DAM.DI.Galaga.Category.PlayerShip; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resManager">Gestor de recursos</param>
        public PlayerShip(Resources.ResourcesManager resManager)
            : base() 
        {
            _sprite = new Sprite((Texture)resManager["Naves:NaveJugador"]);
        }

        /// <summary>
        /// Dibuja el PlayerShip
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        override protected void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {
            // en el destino (rt) dibujamos el sprite con un estado determinado (rs)
            rt.Draw(_sprite,rs);    
        }

      
    }
}
