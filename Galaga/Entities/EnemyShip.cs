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

using System.Diagnostics;

using SFML.System;
using SFML.Graphics;

using NLog;


namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    /// <summary>
    /// Encapsula la funcionalidad de las naves enemigas. 
    /// Implementea el interfaz IComparable, para 
    /// poder realizar la operación de ordenación de las naves por tiempo de salida
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
    class EnemyShip : Entity, IComparable<EnemyShip>
    {
        // variables miembro
        private Type _type;                         // tipo de nave enemiga

        private Sprite _sprite;                     // sprite donde dibujar la textura

        private UInt16 _formationPoints;            // puntos que da si está en formación
        private UInt16 _attackPoints;               // puntos que da si está en ataque   

        private Vector2f _posFormation;             // posición formación 
        private float _rotOrigin;

        private CurvePath _path;                     // recorrido de la nave
       
        private float[] _segmentTimes;              // array que almacena para cada segmento el tiempo que tarda en recorrerse
        private int _segmentIndex;                  // índice del segmento en el que nave se esta moviendo
        private float _segmentTime;                 // tiempo que lleva recorriendo el segmento actual
        private int _segmentCount;                  // número de segmentos del camino

        Paths.Corkscrew ruta;
       
#if DEBUG
        private float[,] _waypoints;                // waypoints
        SFML.Graphics.VertexArray _waypointsLines;  // lineas que muestran la trayectoria lineal entre los waypoints
#endif
      
        /// <summary>
        /// Devuelve el módulo de la velocidad que puede alcanzar la nave
        /// </summary>
        public float MaxSpeed { get { return EnemiesTypeConf[(int)_type]._maxSpeed;  } }

        /// <summary>
        /// Devuelve el tiempo desde el inicio de la fase que hay que esperar para que salga 
        /// </summary>
        public float SpawnTime { get; private set; } 

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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


        public EnemyShip(Type type, EnemiesShipData shipData, FloatRect worldBounds)
            : base()
        {
            _logger.Log(LogLevel.Info, " >>> Creando enemigo. Tipo " + type + " (" + GetHashCode() + ")");
            _type = type;

            _sprite = new Sprite((Texture)EnemiesTypeConf[(int)_type]._resManager[EnemiesTypeConf[(int)_type]._textureKey]);
            _rotOrigin = _sprite.Rotation = shipData._rotationOrigin;
            _sprite.Scale = new Vector2f(0.7f,0.7f);

            // ubico el origen del sprite en el centro en vez de en la esquina superior derecha
            FloatRect bounds = _sprite.GetLocalBounds();
            _sprite.Origin = new SFML.System.Vector2f(bounds.Width / 2f, bounds.Height / 2f);

            SpawnTime = shipData._spawnTime;

            // ruta
            _path = shipData._path;
            _segmentCount = _path.NumSegments;
            _segmentTimes = new float[_path.NumSegments];
            for (int seg = 0; seg < _path.NumSegments; seg++)                   // cálculo del tiempo que se tarda en recorrer cada uno de los segmentos
                _segmentTimes[seg] = _path.Coefficients[2 * seg, 4] / MaxSpeed;


#if DEBUG
            // dibujo de los waypoints
            _waypoints = _path.Waypoints;

            if (_waypoints != null)
            {
                _waypointsLines = new SFML.Graphics.VertexArray(SFML.Graphics.PrimitiveType.LinesStrip, (uint)_waypoints.GetLongLength(0));

                for (uint n = 0; n < _waypoints.GetLongLength(0); n++)
                {
                    Vertex vrtx = new Vertex(new Vector2f(_waypoints[n, 0], _waypoints[n, 1]), SFML.Graphics.Color.Cyan);
                    _waypointsLines[n] = vrtx;
                }
            }
#endif
           
            _segmentIndex = 0;
            _segmentTime = 0;

            Position = new Vector2f(shipData._xOrigin, shipData._yOrigin);
        }

        private void UpdateMovementPattern(SFML.System.Time dt)
        {
            _segmentTime += dt.AsSeconds();

            if (_segmentIndex >= _segmentCount) {
                _sprite.Rotation = 0;
                return;
            } 

            if (_segmentTime > _segmentTimes[_segmentIndex]) // se ha acabado el segmento
            {
                _segmentIndex++;
                _segmentTime = 0;
                if (_segmentIndex >= _segmentCount) return;
            }

            //float t = _segmentTime / _segmentTimes[_segmentIndex];
            //float t2 = t * t, t3 = t2 * t;

            //float xTemp = _path[2 * _segmentIndex, 3] * t3 + _path[2 * _segmentIndex, 2] * t2
            //           + _path[2 * _segmentIndex, 1] * t + _path[2 * _segmentIndex, 0];
            //float yTemp = _path[2 * _segmentIndex + 1, 3] * t3 + _path[2 * _segmentIndex + 1, 2] * t2
            //    + _path[2 * _segmentIndex + 1, 1] * t + _path[2 * _segmentIndex + 1, 0];

            //double radians = Math.Atan2(Position.Y - yTemp, Position.X - xTemp) + Math.PI / 2;


            //Rotation = (float)(radians * (180.0f / Math.PI)) + _rotOrigin;

            //Position = new Vector2f(xTemp, yTemp);

           // t = ruta.GetCurveParameterEuler(_segmentTime * MaxSpeed, _segmentIndex);
            float t = _path.GetCurveParameterNewton(_segmentTime * MaxSpeed, _segmentIndex);
            Vector2f pos = _path.GetPoint(t, _segmentIndex);

            //_logger.Log(LogLevel.Info, " seg: "+ _segmentIndex +  " t: " + t);

            double radians = Math.Atan2(Position.Y - pos.Y, Position.X - pos.X) + Math.PI / 2;

            Rotation = (float)(radians * (180.0f / Math.PI)) + _rotOrigin;

            Position = pos;


	    }

        /// <summary>
        /// Actualiza el estado de la nave
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <remarks>
        /// Ya nadie puede heredarla
        /// </remarks>
        override sealed protected void UpdateCurrent(SFML.System.Time dt)
        {
            UpdateMovementPattern(dt);

            _sprite.Rotation = Rotation;
            _sprite.Position = Position;
        }

        override protected void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {   
            rt.Draw(_sprite);
   
#if DEBUG
            // Dibujo los waypoints interpolados linealmente
            if (_waypointsLines != null)
                rt.Draw(_waypointsLines);
#endif        
        }

        /// <summary>
        /// Compara diferentes naves enemigas.
        /// </summary>
        /// <param name="compareShip">Nave con la que comparar</param>
        /// <returns><0 si la instancia actual es menor que el parámetro, 0 si son iguales, >0 si es menor</returns>
        /// <remarks>
        /// La comparación se basa en el menor tiempo de salida a escena
        /// </remarks>
        public int CompareTo(EnemyShip compareShip)
        {
            return this.SpawnTime.CompareTo(compareShip.SpawnTime);
        } 

        /// <summary> 
        /// Define los datos de configuración de los tipos de naves enemigas
        /// </summary>
        /// <remarks>
        /// Se utliza una estructura  ya que en realidad lo que hace es definir un tipo de dato, aunque se podría utilizar una clase
        /// </remarks>
        private struct EnemiesShipTypeData
        {
            public UInt16 _hitPoints;                           // número de impactos que puede aguantar antes del destruirse
            public String _textureKey;                          // ID de la textura asociada
            public UInt16 _formationPoints;                     // puntos que da si está en formación
            public UInt16 _attackPoints;                        // puntos que da si está en ataque   

            public float _maxSpeed;                             // modulo de la velocidad que alcanza la entidad (se alcanzará en recorrido lineales)

            // comunes a todos los tipos
            public Resources.ResourcesManager _resManager;      // gestor de recursos
        }

        /// <summary> 
        /// Define los datos de configuración de cada nave enemiga en particular
        /// </summary>
        /// <remarks>
        /// Se utliza una estructura  ya que en realidad lo que hace es definir un tipo de dato, aunque se podría utilizar una clase
        /// </remarks>
        public struct EnemiesShipData
        {
            public float _xOrigin;                  // coordenada x inicial (antes de salir)
            public float _yOrigin;                  // coordenada y inicial (antes de salir)
            public float _rotationOrigin;           // rotación inicial del sprite (en grados)
           
            public float _xFormation;               // coordenada x en formación
            public float _yFormation;               // coordenada y en formación   

            public float _spawnTime;                // momento en el que entra en juego (en relación al incio de la fase)

            public CurvePath _path;                 // curva de entrada de la nave    

        }


        //////////////////////////////////////////////////////////
        /// ELEMENTOS ESTATICOS
        //////////////////////////////////////////////////////////
        private static EnemiesShipTypeData[] EnemiesTypeConf;

        public static void InitializeEnemiesTypeConfiguration(Resources.ResourcesManager resManager)
        {
            EnemiesTypeConf = new EnemiesShipTypeData[(int)Type.TYPECOUNT];

            // tipo Bee
            EnemiesTypeConf[(int)Type.BEE]._hitPoints = 1;
            EnemiesTypeConf[(int)Type.BEE]._textureKey = "Naves:BeeC1";
            EnemiesTypeConf[(int)Type.BEE]._formationPoints = 80;           // ptos
            EnemiesTypeConf[(int)Type.BEE]._attackPoints = 160;             // ptos
            EnemiesTypeConf[(int)Type.BEE]._maxSpeed = 300;                 // px/s

            // tipo Butterfly
            EnemiesTypeConf[(int)Type.BUTTERFLY]._hitPoints = 1;
            EnemiesTypeConf[(int)Type.BUTTERFLY]._textureKey = "Naves:Butterfly";
            EnemiesTypeConf[(int)Type.BUTTERFLY]._formationPoints = 100;    // ptos
            EnemiesTypeConf[(int)Type.BUTTERFLY]._attackPoints = 200;       // ptos
            EnemiesTypeConf[(int)Type.BUTTERFLY]._maxSpeed = 250;                 // px/s

            for (int type = 0; type < (int)Type.TYPECOUNT; type++) {
                EnemiesTypeConf[type]._resManager = resManager;
            }

        }


    }
}
