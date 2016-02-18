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

using SFML.Graphics;
using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{

    class Animation : SFML.Graphics.Transformable, SFML.Graphics.Drawable
    {
        // variables miembro
        private int _numLoops;                                  // número de veces que se repite 0 = infinito
        private uint _currentLoop;                              // ciclo actual
       
        private SFML.System.Time _elapsedTime;                  // tiempo transcurrido desde el principio de la animación
        private SFML.System.Time[] _tileDuration;               // duración de cada uno de los tiles. En esta versión sólo soporta duraciones iguales
       
        private uint _currentTile;                              // frame que se está mostrando
        private uint _totalTiles;                               // número total de tiles
        private uint _numTilesX, _numTilesY;                    // número de tiles en la textura en X e Y
        private FloatRect _localBounds;                         // limites del tile actual

        private SFML.Graphics.Sprite _sprite;                   // sprite donde dibujaremos

        /// <summary>
        /// Devuelve si la animación esta ejecutándose
        /// </summary>
        public Boolean IsRunning { get; private set; }

        /// <summary>
        /// Devuelve si la animación está en bucle infinito
        /// </summary>
        public Boolean IsLoop { get { return _numLoops == 0 ? true : false; } }

        /// <summary>
        /// Devuelve o asigna la escala del sprite
        /// </summary>
        public Vector2f Escale 
        { 
            get { return _sprite.Scale; }

            set { _sprite.Scale = value; } 
        }

        public FloatRect LocalBounds { get { return _localBounds; } }

        /// <summary>
        /// Devuelve el tamaaño de cada tile
        /// </summary>
        public Vector2u TileSize { get; private set; } 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Textura a utilizar</param>
        public Animation(Texture texture) : this(texture, new Vector2u(0,0)) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Textura a utilizar</param>
        /// <param name="tileSize">Número de frames en los que se divide la textura</param>
        public Animation(Texture texture, Vector2u tileSize) : this (texture, tileSize, new SFML.System.Time()) { }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="texture">Textura a utilizar</param>
        /// <param name="tileSize">Número de frames en los que se divide la textura</param>
        /// <param name="totalDuration">Duración total de la animación</param>
        public Animation(Texture texture, Vector2u tileSize, SFML.System.Time totalDuration) : this(texture, tileSize, totalDuration, 0) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Textura a utilizar</param>
        /// <param name="tileSize">Número de frames en los que se divide la textura</param>
        /// <param name="totalDuration">Duración total de la animación</param>
        /// <param name="numLoops">Número de veces que se repite la animación (0 infinito)</param> 
        public Animation(Texture texture, Vector2u tileSize, SFML.System.Time totalDuration, int numLoops) 
        {
            _currentLoop = 1;
            _currentTile = 0;

            _elapsedTime = SFML.System.Time.Zero;
            
            _numTilesX = (texture.Size.X / tileSize.X);
            _numTilesY = (texture.Size.Y / tileSize.Y);

            _totalTiles = _numTilesX * _numTilesY;
            TileSize = tileSize;
            _numLoops = numLoops;

            _sprite = new Sprite(texture);
            _sprite.TextureRect = new IntRect(0, 0, (int)tileSize.X, (int)tileSize.Y);

            _localBounds = (FloatRect)_sprite.TextureRect;

            _tileDuration = new SFML.System.Time[_totalTiles];

            // inicializo la duración de todos los tiles de manera homogenea
            for (int time=0;time<_totalTiles;time++)
            {
                _tileDuration[time] = totalDuration / _totalTiles;
            }

            IsRunning = false;

            if (texture.Size.X % tileSize.X != 0 || texture.Size.Y % tileSize.Y != 0)
                throw new Exception("No se pueden generar los tiles a partir de la textura. Valores no enteros");
        }

        #region Funciones de control de la animación
        /// <summary>
        /// Arranca desde cero la animación
        /// </summary>
        public void Run() 
        {
            _currentTile = 0;
            _currentLoop = 1;

            IsRunning = true;
        }

        /// <summary>
        /// Para la animación
        /// </summary>
        public void Stop() { IsRunning = false; }

        /// <summary>
        /// Continua con la reproducción de la animación. Si estaba parada la empieza de cero
        /// </summary>
        public void Resume() { IsRunning = true;  }


        public void Update(SFML.System.Time dt) 
        {
            if (!IsRunning) return;
            
            _elapsedTime += dt;

            if (_tileDuration[_currentTile] > _elapsedTime) return;

            uint row = (uint)(_currentTile / _numTilesX);
            uint column = _currentTile - row*_numTilesX;

            _sprite.TextureRect =  new IntRect((int)(column * TileSize.X), (int)(row * TileSize.Y), (int)TileSize.X, (int)TileSize.Y);
            _localBounds = (FloatRect)_sprite.TextureRect;

            // se ha acabado un ciclo
            if ((_currentTile + 1) >= _totalTiles)
            {
                _currentTile = 0;

                if (!IsLoop && _currentLoop>_numLoops) Stop();
                else _currentLoop++; 
            }
            else 
                _currentTile++;

            _elapsedTime = SFML.System.Time.Zero;
        }
        #endregion 

        #region Funciones de dibujo
        /// <summary>
        /// Dibuja la animación
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        public void Draw(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {
            // almaceno la combinación de la transformación que viene aplicada desde fuera (rs.Transform) y de la animación (Transform). Lo que obtengo es la transformación global del nodo
            // esto funciona bien ya que renderStates y Transform son struct y no una clase!! se hace una copia, 
            // no se crea otra referencia
            rs.Transform.Combine(Transform);

            rt.Draw(_sprite, rs);                           // Dibujo el nodo actual
        }
        #endregion

    }
}
