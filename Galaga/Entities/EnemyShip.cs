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

using SFML.System;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    /// <summary>
    /// Encapsula la funcionalidad de las naves enemigas.
    /// </summary>
    /// <remarks>
    /// - Puede disparar
    /// - Dan puntos al usuario según el estado en el que se encuentren
    /// - Pueden colisionar con los disparos del jugador o con la propia nave del jugador
    /// - Tiene movimientos controlados por la IA
    /// - Tiene 3 estados: entrada, formación, ataque
    /// - Se pueden dividir en varios según un evento: les impacta un misil o están en ataque. En formación se vuelven a unir
    /// - Atacan en escuadrones
    /// - Tienen una posición definida en la formación
    /// - Cambia de textura según se mueven
    /// - Puede ser necesario impactar en ellos mas de un misil para destruirlos
    /// </remarks>
    class EnemyShip : Entity
    {
        // variables miembro
        private Type _type;                         // tipo de nave enemiga
        
        private UInt16 _formationPoints;            // puntos que da si está en formación
        private UInt16 _attackPoints;               // puntos que da si está en ataque   

        private UInt16 _hitPoints;                  // número de impactos que puede aguantar antes del destruirse
        private Vector2f _posFormation;             // posición formación 

        /// <summary>
        /// Tipos de naves enemigas
        /// </summary>
        public enum Type
        {
            BEE,
            BUTTERFLY,
            CRAB,
            SCORPION,
            GREENBIRD,
            TYPECOUNT
        }

        /// <summary>
        /// Estado en el que esta en un momento determinado la nave
        /// </summary>
        private enum State 
        { 
            ENTRY,
            FORMATION,
            ATTACK
        }


        public EnemyShip(Type type)
            : base()
        {
            _type = type;
        }

        /// <summary> 
        /// Define los datos de configuración de los tipos de naves enemigas
        /// </summary>
        /// <remarks>
        /// Se utliza una estructura  ya que en realidad lo que hace es definir un tip de dato, aunque se podría utilizar una clase
        /// </remarks>
        private struct EnemiesShipData
        {
            public UInt16 _hitPoints;                  // número de impactos que puede aguantar antes del destruirse
            public String _textureKey;                 // ID de la textura asociada
            public UInt16 _formationPoints;            // puntos que da si está en formación
            public UInt16 _attackPoints;               // puntos que da si está en ataque   

        
        }

       
        //////////////////////////////////////////////////////////
        /// ELEMENTOS ESTATICOS
        //////////////////////////////////////////////////////////
        private static EnemiesShipData [] EnemiesTypeConf;

        public static void InitializeEnemiesTypeConfiguration() 
        {
            EnemiesTypeConf = new EnemiesShipData[(int)Type.TYPECOUNT];

            // tipo Bee
            EnemiesTypeConf[(int)Type.BEE]._hitPoints = 1;
            EnemiesTypeConf[(int)Type.BEE]._textureKey = "Naves:Bee";
            EnemiesTypeConf[(int)Type.BEE]._formationPoints = 80;
            EnemiesTypeConf[(int)Type.BEE]._attackPoints = 160;

            // tipo Butterfly
            EnemiesTypeConf[(int)Type.BUTTERFLY]._hitPoints = 1;
            EnemiesTypeConf[(int)Type.BUTTERFLY]._textureKey = "Naves:Butterfly";
            EnemiesTypeConf[(int)Type.BUTTERFLY]._formationPoints = 100;
            EnemiesTypeConf[(int)Type.BUTTERFLY]._attackPoints = 200;

        }
    }
}
