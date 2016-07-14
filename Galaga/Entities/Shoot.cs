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
using System.Text;

using edu.CiclosFormativos.Games.DIDAM.Resources;
using edu.CiclosFormativos.Games.DIDAM.Patterns; 

using SFML.Graphics;
using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{   
    /// <summary>
    /// Encapsula la funcionalidad de las balas
    /// </summary>
    /// <remarks>
    /// - Tienen un vida limitada
    /// - Pueden colisionar con las naves enemigas o con la del propio jugador
    /// </remarks>
    class Shoot : Recyclable
    {
        // variables miembro
        private Type _type;                           // tipo de nave enemiga
        private SFML.Graphics.Sprite _sprite;         // sprite donde dibujar la textura

        /// <summary>
        /// Tipos de balas
        /// </summary>
        public enum Type
        {
            PLAYER,
            ENEMIES,
            TYPECOUNT
        }


        public Shoot(Type type)
            : base()
        {
            _type = type;

            _sprite = new Sprite((Texture)ShootTypeConf[(int)_type]._resManager[ShootTypeConf[(int)_type]._textureKey]);
            _sprite.Scale = new Vector2f(0.7f, 0.7f);
            // ubico el origen del sprite en el centro en vez de en la esquina superior derecha
            FloatRect bounds = _sprite.GetLocalBounds();
            _sprite.Origin = new SFML.System.Vector2f(bounds.Width / 2f, bounds.Height / 2f);

            VelocityY = ShootTypeConf[(int)_type]._maxSpeed;
        }

        /// <summary>
        /// Actualiza el estado de un misil
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <remarks>
        /// Ya nadie puede heredarla
        /// </remarks>
        override sealed protected void UpdateCurrent(SFML.System.Time dt)
        {
            // comprobacion de si está fuera del mundo
            if (_type == Type.PLAYER && Position.Y + _sprite.GetGlobalBounds().Height < 0)
            {
                // lo quito del grafo de escena
                Parent.RemoveChild(this);
                OnRecycle(this);
            }

            base.UpdateCurrent(dt);
        }

        /// <summary>
        /// Dibuja el misíl
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        override protected void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {
            // en el destino (rt) dibujamos el sprite con un estado determinado (rs)
            rt.Draw(_sprite, rs);
        }

        /////////////////////////////////////////////////////////
        //// ESTRUCTURAS DE DATOS
        /////////////////////////////////////////////////////////

        /// <summary> 
        /// Define los datos de configuración de los tipos de balas
        /// </summary>
        /// <remarks>
        /// Se utiliza una estructura ya que en realidad lo que hace es definir un tipo de dato, aunque se podría utilizar una clase
        /// </remarks>
        private struct ShootTypeData
        {
            public String _textureKey;                          // ID de la textura asociada
            public float _maxSpeed;                             // velocidad en el eje Y que alcanza la entidad 

            // comunes a todos los tipos
            public ResourcesManager _resManager;      // gestor de recursos
        }

        //////////////////////////////////////////////////////////
        //// ELEMENTOS ESTATICOS
        //////////////////////////////////////////////////////////
        private static ShootTypeData[] ShootTypeConf;

        public static void InitializeShootTypeConfiguration(ResourcesManager resManager)
        {
            ShootTypeConf = new ShootTypeData[(int)Type.TYPECOUNT];

            // tipo Player
            ShootTypeConf[(int)Type.PLAYER]._textureKey = "Misiles:Jugador";
            ShootTypeConf[(int)Type.PLAYER]._maxSpeed = -400;                 // px/s

            // tipo Enemies
            ShootTypeConf[(int)Type.ENEMIES]._textureKey = "Misiles:Enemigos";
            ShootTypeConf[(int)Type.ENEMIES]._maxSpeed = 350;                 // px/s

            for (int type = 0; type < (int)Type.TYPECOUNT; type++)
            {
                ShootTypeConf[type]._resManager = resManager;
            }

        }
    }
}
