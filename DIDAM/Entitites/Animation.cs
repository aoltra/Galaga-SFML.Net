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
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace edu.CiclosFormativos.Games.DIDAM.Entities
{
    /// <summary>
    /// Encapsula la funcionalidad necesaria para animar una entidad de manera independiente al game loop principal
    /// </summary>
    public class Animation : SFML.Graphics.Transformable, SFML.Graphics.Drawable
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
        private IntRect [] _rectTiles;                             // rectangulo que ocupa cada uno de los tiles

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
        /// <remarks>
        /// No la llamo Scale para no colisionar con la definida en Transformable
        /// </remarks>
        public Vector2f ScaleSprite
        { 
            get { return _sprite.Scale; }

            set { _sprite.Scale = value; } 
        }

        /// <summary>
        /// Devuelve los límites del tile actual
        /// </summary>
        public FloatRect LocalBounds { get { return _localBounds; } }

        /// <summary>
        /// Devuelve los límites del tile (sprite) con respecto al origen
        /// </summary>
        public FloatRect GlobalBounds { get { return _sprite.GetGlobalBounds(); } }

        /// <summary>
        /// Devuelve el tile actual
        /// </summary>
        public uint CurrentTile  { get { return _currentTile; } }

        /// <summary>
        /// Devuelve el tiempo transcurrido en el tile actual
        /// </summary>
        public SFML.System.Time CurrentTime { get { return _elapsedTime;  } } 

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

            _rectTiles = new IntRect[_totalTiles];
            
            // los rects se numeran empezando en la esquina superior izquierda y hacia la derecha y luego nueva fila
            for (int row=0;row<_numTilesY;row++)
                for (int col=0;col<_numTilesX;col++)
                    _rectTiles[col + row * _numTilesX] = 
                        new IntRect((int)(col * TileSize.X), (int)(row * TileSize.Y), (int)TileSize.X, (int)TileSize.Y);

            _sprite = new Sprite(texture);
            _sprite.TextureRect = _rectTiles[0];

            _localBounds = (FloatRect)_sprite.TextureRect;
            _sprite.Origin = new SFML.System.Vector2f(_localBounds.Width / 2f, _localBounds.Height / 2f);

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

        /// <summary>
        /// Actualiza el estado de la animación en función del tiempo transcurrido desde la última actualización
        /// </summary>
        /// <param name="dt">tiempo transcurrido desde la última actualización</param>
        public void Update(SFML.System.Time dt) 
        {
            if (!IsRunning) return;
            
            _elapsedTime += dt;

            if (_tileDuration[_currentTile] > _elapsedTime) return;

            uint row = (uint)(_currentTile / _numTilesX);
            uint column = _currentTile - row*_numTilesX;

            _sprite.TextureRect = _rectTiles[column + row*_numTilesX];
        
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

        /// <summary>
        /// Sincroniza la animación con unos parámetros dados
        /// </summary>
        /// <param name="tile">Tile a sincronizar</param>
        /// <param name="dt">Tiempo que lleva ese tile activo</param>
        public void SyncAnimation(uint tile, SFML.System.Time dt) 
        {
            if (tile >= _totalTiles)
                _currentTile = _totalTiles - 1;
            else if (tile < 0)
                _currentTile = 0;
            else
                _currentTile = tile;
            
            _currentTile = tile;
            _elapsedTime = dt;
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
