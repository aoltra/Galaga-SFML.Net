﻿#region GPL License
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

using edu.CiclosFormativos.Games.DIDAM.Entities;
using edu.CiclosFormativos.Games.DIDAM.Resources;

using SFML.Graphics;
using SFML.System;

using System.Diagnostics;

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
        private Sprite _sprite;             // sprite donde dibujar la textura
        private FloatRect _worldBounds;     // espacio del mundo en el que la nave se puede mover
        private ShootPlayerPool _shootPool; // piscina de misiles

        /// <summary>
        /// Devuelve la/s categoria/s del PlayerShip
        /// </summary>
        public override UInt16 Category
        {
            get { return (UInt16)edu.CiclosFormativos.DAM.DI.Galaga.Category.PLAYERSHIP; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resManager">Gestor de recursos</param>
        /// <param name="worldBounds">Dimensiones de area de movimiento del PlayerShip</param>
        /// <param name="shootPool">Piscina que gestiona el reciclaje de las balas</param>
        public PlayerShip(ResourcesManager resManager, FloatRect worldBounds,ShootPlayerPool shootPool)
            : base() 
        {
            _sprite = new Sprite((Texture)resManager["Naves:NaveJugador"]);
            _worldBounds = worldBounds;

            // ubico el origen del sprite en el centro en vez de en la esquina superior derecha
            FloatRect bounds = _sprite.GetLocalBounds();
            _sprite.Origin = new SFML.System.Vector2f(bounds.Width / 2f, bounds.Height / 2f);

            _shootPool = shootPool;
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

        /// <summary>
        /// Actualiza la nave del jugador
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <remarks>
        /// Ya nadie puede heredarla
        /// </remarks>
        override sealed protected void UpdateCurrent(SFML.System.Time dt)
        {
            // No es necesario llamar a la función del padre ya que la actualización la realiza el comando
          //  base.UpdateCurrent(dt);

            float posX;

            posX = Position.X < _worldBounds.Left ? _worldBounds.Left : Position.X;
            posX = posX > _worldBounds.Width ? _worldBounds.Width : posX; 
                
            Position = new SFML.System.Vector2f(posX, Position.Y);
        }

        /// <summary>
        /// Dispara un misil
        /// </summary>
        public void Fire() 
        { 
            // crea un misil
            Entities.Shoot shoot = _shootPool.GetObject();

            Vector2f offset = new Vector2f(0, -0.5f * _sprite.GetGlobalBounds().Height);

	        shoot.Position = WorldPosition + offset;
		    this.Parent.AddChild(shoot);        
        }      
    }
}
