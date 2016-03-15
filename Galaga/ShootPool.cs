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

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula la piscina de misiles del jugador
    /// </summary>
    class ShootPlayerPool : ObjectPool<Entities.Shoot>
    {
        /// <summary>
        /// Constructor. Crea la piscina con n objetos de partida. Cada vez que se quiera incrementar su tamaño
        /// se hará de uno en uno
        /// </summary>
        /// <param name="initSize">Tamaño inicial de la piscina</param>
	    public ShootPlayerPool(int initSize) : 
            base(initSize) { }

        /// <summary>
        /// Implementa la creación de misiles del jugador
        /// </summary>
        /// <returns>Misíl del jugador</returns>
        override protected Entities.Shoot AllocObject() 
        {
            return new Entities.Shoot(Entities.Shoot.Type.PLAYER);
        }
    }

    /// <summary>
    /// Encapsula la piscina de misiles del jugador
    /// </summary>
    class ShootEnemiesPool : ObjectPool<Entities.Shoot>
    {
         /// <summary>
        /// Constructor. Crea la piscina con n objetos de partida. Cada vez que se quiera incrementar su tamaño
        /// se hará de uno en uno
        /// </summary>
        /// <param name="initSize">Tamaño inicial de la piscina</param>
        public ShootEnemiesPool(int initSize) : 
            base(initSize) { }


        /// <summary>
        /// Implementa la creación de misilea de los enemigos
        /// </summary>
        /// <returns>Misíl del enemigo</returns>
        override protected Entities.Shoot AllocObject()
        {
            return new Entities.Shoot(Entities.Shoot.Type.ENEMIES);
        }
    }
}
