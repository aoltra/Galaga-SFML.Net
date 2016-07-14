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
using System.Linq;
using System.Text;

using edu.CiclosFormativos.Games.DIDAM.Resources;
using edu.CiclosFormativos.Games.DIDAM.Entities;
using edu.CiclosFormativos.Games.DIDAM.Scenes;
using edu.CiclosFormativos.Games.DIDAM.Commands;
using edu.CiclosFormativos.Games.DIDAM.Paths;

using SFML.Graphics;
using SFML.System;

using edu.CiclosFormativos.DAM.DI.Galaga.Background;

using NLog.Config;
using NLog.Targets;
using NLog;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula el mundo en el que transcurre el juego
    /// </summary>
    class World
    {
        // variables miembro
        private RenderWindow _window;                   // ventana donde se dibujará
        private SceneNode _sceneGraph;                  // nodo raíz del grafo de escena
        private ResourcesManager _resManager; // Gestor de recursos del mundo 
        private Dictionary<String,CurvePath> _curveMap; // Diccionario donde se alamncena las posibles trayectorias curvas
        private List<SceneNode> _sceneLayers;           // lista ordenada por orden de dibujo de las capas
        private View _worldView;                        // vista (camara) que visualizará nuestro trozo de mundo
        private FloatRect _worldBounds;                 // dimensiones (límites) del mundo
    
        private CommandQueue _commandQueue;             // cola de comandos

        private Entities.PlayerShip _playerShip;        // entidad nave jugador
        private List<Entities.EnemyShip> _dockShip;     // lista de naves enemigas antes de que salgan a jugar
        private Dictionary<String, Entities.TransformEntity> _leaders;      // mapa de líderes de escuadrones o pelotón
        private float _stageTime;                       // tiempo actual de la fase en juego (en segundos)

        private ShootPlayerPool _shootPlayerPool;       // piscina de misiles disponibles para el jugador

        private const int BORDER = 40;                  // borde del mundo en el que no se juega

        private int _level;                             // fase del juego

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // enum con las diferentes capas en orden de dibujo
        private enum Layer
        {
            BACKGROUND,         // 0
            AIR,                // 1
            LAYERCOUNT          // 2 (número de capas)    
        }

        // constantes
        public const float ENEMYSHIP_SEP_X = 15f;
        public static float [] ENEMYSHIP_ROW_Y = {50,100,150,200,250};
        private static float _leaderBoundRight;
        private static float _leaderBoundLeft;
        
        /// <summary>
        /// Devuelve la cola de comandos 
        /// </summary>
        public CommandQueue CommandQueue { get { return _commandQueue;  } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto en el que se ejecuta</param>
        public World(Scene.Context context) {

            try
            {
                _window = context.Window;
                _resManager = context.ResourcesManager;

                _sceneGraph = new SceneNode();
                _sceneLayers = new List<SceneNode>();
                _dockShip = new List<Entities.EnemyShip>();
                _leaders = new Dictionary<String, Entities.TransformEntity>();
                
                _curveMap = new Dictionary<String,CurvePath>();
     
                _commandQueue = new CommandQueue();
            
                // asigno la vista por defecto (viewport completo y tamaño igual en pixeles al de al ventana
                // hago una copia de la vista por defecto, para mantener guardada la original por si quiero volver a ella 
                _worldView = new View(_window.DefaultView);

                // asignamos al mundo la vista definida (por ahora un clon de la de por defecto)
                _window.SetView(_worldView);

                // pongo dimensiones al mundo
                _worldBounds = new FloatRect(0, 0, _worldView.Size.X, _worldView.Size.Y);
                _leaderBoundLeft = _worldBounds.Width * .4f;
                _leaderBoundRight = _worldBounds.Width * .6f;

                // incializo la fase
                _level = 1;

                // inicializo las piscinas de objetos
                Entities.EnemyShip.InitializeEnemiesTypeConfiguration(_resManager);
                Entities.Shoot.InitializeShootTypeConfiguration(_resManager);

                _shootPlayerPool = new ShootPlayerPool(15);     // 15 misiles de partida. Si hacen falta más se incrementará de uno en uno

                // construyo el mundo
                BuildWorld();

                // Prepare the view
                _worldView.Center = new Vector2f(_worldView.Size.X / 2, _worldBounds.Height - (_worldView.Size.Y / 2));

                // asignamos al mundo la vista definida (con el centro cambiado)
                _window.SetView(_worldView);

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex.Message);
            }

        }

        /// <summary>
        /// Construye todos los elementos que forman parte del mundo
        /// </summary>
        private void BuildWorld() {

            SceneNode layer;

            try 
            {
         
                // Creo las capas como SceneNode's hijos del árbol raíz.
                for (int contLayer = 0; contLayer < (int)Layer.LAYERCOUNT; contLayer++) {
                    layer = new SceneNode();
                    _sceneGraph.AddChild(layer);
                    _sceneLayers.Add(layer);
                }

                ///// BACKGROUND
                // se crea un background estrellado parallax con scroll
                Star.Size = new FloatRect(0, 0, _window.Size.X, _window.Size.Y);

                List<SceneNode> deepSpace1 = StarBackgroundGenerator.StarGenerator(55,
                    StarBackgroundGenerator.StarType.SMALL, new Vector2f(0, 60f), _window).Cast<SceneNode>().ToList();
                _sceneLayers[(int)Layer.BACKGROUND].AddChilds(deepSpace1);

                List<SceneNode> deepSpace2 = StarBackgroundGenerator.StarGenerator(15,
                    StarBackgroundGenerator.StarType.MEDIUM, new Vector2f(0, 90f), _window).Cast<SceneNode>().ToList();
                _sceneLayers[(int)Layer.BACKGROUND].AddChilds(deepSpace2);

                List<SceneNode> deepSpace3 = StarBackgroundGenerator.StarGenerator(3,
                    StarBackgroundGenerator.StarType.BIG, new Vector2f(0, 100f), _window).Cast<SceneNode>().ToList();
                _sceneLayers[(int)Layer.BACKGROUND].AddChilds(deepSpace3);

                ////// NAVE JUGADOR
                // Añado la nave del jugador
                _playerShip = new Entities.PlayerShip(_resManager, 
                    new FloatRect(_worldBounds.Left + BORDER, 0, _worldBounds.Width - BORDER, _worldBounds.Height),
                    _shootPlayerPool);
                _playerShip.Position = new Vector2f(_worldView.Size.X / 2, _worldView.Size.Y - 60);
                _sceneLayers[(int)Layer.AIR].AddChild(_playerShip);




                // Nave enemiga prueba
              
                Entities.EnemyShip.EnemiesShipData data = new Entities.EnemyShip.EnemiesShipData();
                data._xOrigin = .55f * _worldBounds.Width;
                data._yOrigin = -40;
                data._xFormation = 1;
                data._yFormation = 4;
                data._rotationOrigin = 180;
                data._spawnTime = 3;                 // s

                ///// CURVAS
                _curveMap.Add("Sacacorchos1", new Paths.Corkscrew(data._xOrigin, data._yOrigin,
                data._xFormation, data._yFormation, _worldBounds));
                _curveMap.Add("Sacacorchos1_sim", new Paths.Symmetric(_curveMap["Sacacorchos1"],
                    new Vector2f [] { new Vector2f(_worldBounds.Width/2,0), 
                                      new Vector2f(_worldBounds.Width/2,10) }));
                data._path = _curveMap["Sacacorchos1"];

                // platoon leader
                Entities.PlatoonLeader platoonLeader = new Entities.PlatoonLeader(-60, _leaderBoundLeft, _leaderBoundRight,_resManager);
#if DEBUG
                platoonLeader.Visible = true;
#endif
                _sceneLayers[(int)Layer.AIR].AddChild(platoonLeader);
                platoonLeader.Position = new Vector2f(_worldView.Size.X / 2, ENEMYSHIP_ROW_Y[0]);
                _leaders.Add("Platoon", platoonLeader);

                Entities.EnemyShip enemy;
                // creo las entidades y las añado al muelle de naves
                data._xFormation = 1;
                data._yFormation = 4;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BUTTERFLY, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._spawnTime = 3.30f;
                data._xFormation = -1;
                data._yFormation = 4;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BUTTERFLY, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._spawnTime = 3.60f;
                data._xFormation = 1;
                data._yFormation = 5;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BUTTERFLY, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._spawnTime = 3.90f;
                data._xFormation = -1;
                data._yFormation = 5;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BUTTERFLY, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._path = _curveMap["Sacacorchos1_sim"];
                data._spawnTime = 3.0f;
                data._xFormation = 1;
                data._yFormation = 2;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BEE, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._spawnTime = 3.30f;
                data._xFormation = -1;
                data._yFormation = 2;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BEE, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._spawnTime = 3.60f;
                data._xFormation = 1;
                data._yFormation = 3;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BEE, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);

                data._spawnTime = 3.90f;
                data._xFormation = -1;
                data._yFormation = 3;
                enemy = new Entities.EnemyShip(Entities.EnemyShip.Type.BEE, data);
                enemy.StateChangeEvent += new Entities.EnemyShip.StateChange(SyncEnemyWithLeader);
                _dockShip.Add(enemy);
  
                // Podría ser aconsejable ordenarla lista una vez insertadas las naves. 
                // siendo por tiempo es posible que sea más fácil simplemente introducirlas ordenadas
                // pero si, por ejemplo, se buscara que fuera la mínima distancia a un punto sería o si 
                // las naves se generan de manera aleatoria, sería más recomendable reordenar la lista
                // esa reordenación se puede realizar con 
                // _dockShip.Sort((x,y) => x.propiedad > y.propiedad );
                // o como en esta caso, con la reescritura del a función CompareTo. Esta función deber 
                // ser implementada ya que se fuerza a que EnemyShip implemente IComparable
                _dockShip.Sort();

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex.Message);
            }
        }


        private void LoadLevel() 
        {
        
        
        }

        /// <summary>
        /// Dibuja el nodo
        /// </summary>
        public void Draw() 
        {
            // reasignamos la vista
            _window.SetView(_worldView);

            // dibujamos el grafo de escena
            _window.Draw(_sceneGraph);
        }

        /// <summary>
        /// Actualizo el estado de los elementos del mundo
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        public void Update(SFML.System.Time dt)
        {
            _stageTime += dt.AsSeconds();

            LinkEnemiesToSceneGraph();

            // Introduzco los commandos de la cola en el grafo
            while (!_commandQueue.IsEmpty)
                _sceneGraph.OnCommand(_commandQueue.Pop(), dt);

            // actualizamos el grafo de escena
            _sceneGraph.Update(dt);
        }

        /// <summary>
        /// Pone una nave enemiga en el grafo de escena
        /// </summary>
        private void LinkEnemiesToSceneGraph() 
        {
            // repaso todos los enemigos en espera y recojo aquellos cuyo tiempo de salida
            // sea mayor que el que llevamos de fase
            for (int enm = 0; enm < _dockShip.Count; enm++)
            { 
                Entities.EnemyShip enemy = (Entities.EnemyShip)_dockShip[enm];
                if (enemy.EnemyData._spawnTime > _stageTime) break;
                
                _sceneLayers[(int)Layer.AIR].AddChild(enemy);
                _dockShip.RemoveAt(enm);
                enm--;
            }
        }

        /// <summary>
        /// Sincroniza la nave enemiga con el líder del pelotón
        /// Es la función que se subscribe al delegado ChangeStateDelgate para escuchar el evento ChangeSate
        /// </summary>
        /// <param name="sender">Nave a sincronizar</param>
        public void SyncEnemyWithLeader(object sender) 
        {
            Entities.EnemyShip enemy = (Entities.EnemyShip)sender;

            Entities.TransformEntity platoonLeader;
            _leaders.TryGetValue("Platoon", out platoonLeader);

            if (enemy.State == Entities.EnemyShip.StateType.ENTRY)
            {
                float t0 = (enemy.Position.Y - enemy.EnemyData._yFormation) / enemy.MaxSpeed;
                  ///  Math.Sign(enemy.EnemyData._xFormation) * (enemy.EnemyData._spawnTime - (float)Math.Floor(enemy.EnemyData._spawnTime));

                float xTarget = platoonLeader.Position.X + platoonLeader.Velocity.X * t0;

                // control del cambio de dirección de líder
                if (xTarget < _leaderBoundLeft)
                    xTarget = 2 * _leaderBoundLeft - xTarget + enemy.EnemyData._xFormation;

                if (xTarget > _leaderBoundRight)
                    xTarget = 2 * _leaderBoundRight - xTarget + enemy.EnemyData._xFormation;

                enemy.RenewPath(new Paths.User(new float[,] { {enemy.Position.X,enemy.Position.Y,0,-1},
                                            {xTarget,enemy.EnemyData._yFormation, 0,-1}}));
            }

            if (enemy.State == Entities.EnemyShip.StateType.SYNC)
            {
                enemy.Rotation = 180;
                enemy.Position = new Vector2f(enemy.EnemyData._xFormation, enemy.EnemyData._yFormation - ENEMYSHIP_ROW_Y[0]);
                enemy.Parent.RemoveChild(enemy);
                platoonLeader.AddChild(enemy);
                enemy.SyncAnimation(((Entities.PlatoonLeader)platoonLeader).CurrentTile,((Entities.PlatoonLeader)platoonLeader).CurrentTime);

            }
        }
    }
}
