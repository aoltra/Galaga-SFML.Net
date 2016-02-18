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
    /// - Tiene 4 estados: entrada, sincronización, formación, ataque
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
        private Animation _animation;               // animación cuanto está en pelotón

        private UInt16 _formationPoints;            // puntos que da si está en formación
        private UInt16 _attackPoints;               // puntos que da si está en ataque   

        private float _rotOrigin;

        private CurvePath _path;                    // recorrido de la nave
       
        private float[] _segmentTimes;              // array que almacena para cada segmento el tiempo que tarda en recorrerse
        private int _segmentIndex;                  // índice del segmento en el que nave se esta moviendo
        private float _segmentTime;                 // tiempo que lleva recorriendo el segmento actual
        private int _segmentCount;                  // número de segmentos del camino
       
#if DEBUG
        private float[,] _waypoints;                // waypoints
        SFML.Graphics.VertexArray _waypointsLines;  // lineas que muestran la trayectoria lineal entre los waypoints
#endif
      
        /// <summary>
        /// Devuelve el módulo de la velocidad que puede alcanzar la nave
        /// </summary>
        public float MaxSpeed { get { return EnemiesTypeConf[(int)_type]._maxSpeed;  } }

        /// <summary>
        /// Devuelve los datos que definen esta nave
        /// </summary>
        public EnemiesShipData EnemyData { get; private set; }

        /// <summary>
        /// Devuelve el estado actual de la nave
        /// </summary>
        public StateType State { get; private set; }

        /// <summary>
        /// delegado ue define la función que recibe el evento de cambio de estado
        /// </summary>
        /// <param name="sender">Objeto que envia el mensaje (la nave enemiga)</param>
        public delegate void StateChange(object sender);

        /// <summary>
        /// Evento que se lanza cuando la nave enemiga cambia de estado
        /// </summary>
        public event StateChange StateChangeEvent;

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
        public enum StateType
        {
            ENTRY,
            SYNC,
            FORMATION,
            ATTACK
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">tipo de nave enemiga</param>
        /// <param name="shipData">Datos que definen característica concretas de la nave</param>
        public EnemyShip(Type type, EnemiesShipData shipData)
            : base()
        {
            float width;

            _logger.Log(LogLevel.Info, " >>> Creando enemigo. Tipo " + type + " (" + GetHashCode() + ")");
            _type = type;

            //_sprite = new Sprite((Texture)EnemiesTypeConf[(int)_type]._resManager[EnemiesTypeConf[(int)_type]._textureKey]);
            //_rotOrigin = _sprite.Rotation = shipData._rotationOrigin;
            //_sprite.Scale = new Vector2f(0.7f, 0.7f);
            //// ubico el origen del sprite en el centro en vez de en la esquina superior derecha
            //FloatRect bounds = _sprite.GetLocalBounds();
            // _sprite.Origin = new SFML.System.Vector2f(bounds.Width / 2f, bounds.Height / 2f);
            //width = _sprite.Texture.Size.X * _sprite.Scale.X;

            _animation = new Animation((Texture)EnemiesTypeConf[(int)_type]._resManager[EnemiesTypeConf[(int)_type]._textureKey],
                new Vector2u(55, 55), SFML.System.Time.FromSeconds(1f));
            _rotOrigin = _animation.Rotation = shipData._rotationOrigin;
            _animation.Scale = new Vector2f(0.7f, 0.7f);

            // ubico el origen del sprite en el centro en vez de en la esquina superior derecha
            FloatRect bounds = _animation.LocalBounds;
            _animation.Origin = new SFML.System.Vector2f(bounds.Width / 2f, bounds.Height / 2f);

            width = _animation.TileSize.X * _animation.Scale.X;

            //transformo las coordenadas de posición en pixeles
            shipData._xFormation = Math.Sign(shipData._xFormation) * (Math.Abs(shipData._xFormation) - 0.5f) * (width + World.ENEMYSHIP_SEP_X);
            shipData._yFormation = World.ENEMYSHIP_ROW_Y[(int)shipData._yFormation - 1];

            EnemyData = shipData;

            State = StateType.ENTRY;

            // ruta
            RenewPath(shipData._path);

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

           // _animation.Run();
        }

        /// <summary>
        /// Asigna un nuevo path que ha de seguir la nave
        /// </summary>
        /// <param name="newPath">CurvePath que va seguir la nave</param>
        public void RenewPath(CurvePath newPath) 
        {
            _path = null;
            _path = newPath;

            _segmentCount = _path.NumSegments;
            _segmentIndex = 0;
            _segmentTimes = new float[_path.NumSegments];
            for (int seg = 0; seg < _path.NumSegments; seg++)                   // cálculo del tiempo que se tarda en recorrer cada uno de los segmentos
                _segmentTimes[seg] = _path.Coefficients[2 * seg, 4] / MaxSpeed;

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
            if (State != StateType.FORMATION)
                UpdateMovementPattern(dt);
            else
                _animation.Update(dt);
        }

        /// <summary>
        /// Dibuja la nave
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        override protected void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {   
            //rt.Draw(_sprite,rs);
            rt.Draw(_animation, rs);
   
#if DEBUG
            // Dibujo los waypoints interpolados linealmente
            if (_waypointsLines != null)
                rt.Draw(_waypointsLines);
#endif        
        }

        /// <summary>
        /// sincronia la animación
        /// </summary>
        /// <param name="tile">Tile a sincronizar</param>
        /// <param name="dt">Tiempo que lleva ese tile activo</param>
        public void SyncAnimation(uint tile, SFML.System.Time dt)
        {
            _animation.SyncAnimation(tile,dt);
        }

        /// <summary>
        /// Compara diferentes naves enemigas.
        /// </summary>
        /// <param name="compareShip">Nave con la que comparar</param>
        /// <returns>Menor que 0 si la instancia actual es menor que el parámetro, 0 si son iguales, mayor que 0 si es menor</returns>
        /// <remarks>
        /// La comparación se basa en el menor tiempo de salida a escena
        /// </remarks>
        public int CompareTo(EnemyShip compareShip)
        {
            return this.EnemyData._spawnTime.CompareTo(compareShip.EnemyData._spawnTime);
        }

        /////////////////////////////////////////////////////////
        //// MÉTODOS PRIVADOS
        /////////////////////////////////////////////////////////

        /// <summary>
        /// Avanza un paso en la máquina de estados
        /// </summary>
        private void ForwardState()
        {
            switch (State)
            {
                case StateType.ENTRY:
                    State = StateType.SYNC;
                    break;
                case StateType.SYNC:
                    State = StateType.FORMATION;
                    _animation.Resume();
                    break;
            }
        }

        /// <summary>
        /// Actualiza la posición en función del path que se está siguiendo
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        private void UpdateMovementPattern(SFML.System.Time dt)
        {
            _segmentTime += dt.AsSeconds();

            if (_segmentIndex >= _segmentCount)
            {
                //_sprite.Rotation = 0;
                return;
            }

            // se ha acabado el segmento
            if (_segmentTime > _segmentTimes[_segmentIndex]) 
            {
                _segmentIndex++;
                _segmentTime = 0;
                if (_segmentIndex >= _segmentCount)
                {
                    if (StateChangeEvent != null)
                    {
                        StateChangeEvent(this);
                        ForwardState();
                    }

                    return;
                }
            }

            float t = _path.GetCurveParameterNewton(_segmentTime * MaxSpeed, _segmentIndex);
            Vector2f pos = _path.GetPoint(t, _segmentIndex);

            double radians = Math.Atan2(Position.Y - pos.Y, Position.X - pos.X) + Math.PI / 2;

            Rotation = (float)(radians * (180.0f / Math.PI));

            Position = pos;
        }

        /////////////////////////////////////////////////////////
        //// ESTRUCTURAS DE DATOS
        /////////////////////////////////////////////////////////

        /// <summary> 
        /// Define los datos de configuración de los tipos de naves enemigas
        /// </summary>
        /// <remarks>
        /// Se utiliza una estructura  ya que en realidad lo que hace es definir un tipo de dato, aunque se podría utilizar una clase
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
        /// Se utiliza una estructura  ya que en realidad lo que hace es definir un tipo de dato, aunque se podría utilizar una clase
        /// </remarks>
        public struct EnemiesShipData
        {
            public float _xOrigin;                  // coordenada x inicial (antes de salir)
            public float _yOrigin;                  // coordenada y inicial (antes de salir)
            public float _rotationOrigin;           // rotación inicial del sprite (en grados)
           
            // coordenadas en formación. Se inicializan con el valor de la posición fila/columna en la que se encuentran
            // las filas se cuentan de arriba a abajo empezando en 1
            // las columnas se cuentan desde el centro hacia la derecha en positivo y del centro hacia la izquierda en negativo
            // en el constructor de la nave ser realiza la conversión de fila-columna a coordenadas del mundo, pero relativas al líder
            public float _xFormation;               // coordenada x en formación. 
            public float _yFormation;               // coordenada y en formación   

            public float _spawnTime;                // momento en el que entra en juego (en relación al incio de la fase)

            public CurvePath _path;                 // curva de entrada de la nave    

        }

        //////////////////////////////////////////////////////////
        //// ELEMENTOS ESTATICOS
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
            EnemiesTypeConf[(int)Type.BEE]._maxSpeed = 350;                 // px/s

            // tipo Butterfly
            EnemiesTypeConf[(int)Type.BUTTERFLY]._hitPoints = 1;
            EnemiesTypeConf[(int)Type.BUTTERFLY]._textureKey = "Naves:ButterflyC1";
            EnemiesTypeConf[(int)Type.BUTTERFLY]._formationPoints = 100;    // ptos
            EnemiesTypeConf[(int)Type.BUTTERFLY]._attackPoints = 200;       // ptos
            EnemiesTypeConf[(int)Type.BUTTERFLY]._maxSpeed = 350;           // px/s

            for (int type = 0; type < (int)Type.TYPECOUNT; type++) {
                EnemiesTypeConf[type]._resManager = resManager;
            }

        }


    }

}
