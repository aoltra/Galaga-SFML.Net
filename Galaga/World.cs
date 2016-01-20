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

using edu.CiclosFormativos.DAM.DI.Galaga.Resources;

using SFML.Graphics;
using SFML.System;

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
        private List<SceneNode> _sceneLayers;           // lista ordenada por orden de dibujo de las capas
        private View _worldView;                        // vista (camara) que visualizará nuestro trozo de mundo
        private FloatRect _worldBounds;                 // dimensiones (límites) del mundo
        private ResourcesManager _resManager;           // Gestor de recursos del mundo 

        private float _scrollSpeed;                     // velocidad del scroll del mundo (el espacio estrellado)

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // enum con las diferentes capas en orden de dibujo
        private enum Layer
        {
            Background,         // 0
            Air,                // 1
            LayerCount          // 2 (número de capas)    
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Ventana de dibujo</param>
        public World(RenderWindow window) {

            try
            {
                _window = window;
           
                _sceneGraph = new SceneNode();
                _sceneLayers = new List<SceneNode>();

                _scrollSpeed = -60;                                 // 60 px/s
            
                // asigno la vista por defecto (viewport completo y tamaño igual en pixeles al de al ventana
                // hago una copia de la vista por defecto, para mantener guardada la original por si quiero volver a ella 
                _worldView = new View(_window.DefaultView);

                // asignamos al mundo la vista definida (por ahora un clon de la de por defecto)
                _window.SetView(_worldView);

                // Se crea el gestor de recursos y se leen los elementos
                _resManager = new Resources.ResourcesManager(
                    this.GetType().Assembly.GetManifestResourceStream("Galaga.main.resxml"));
                _resManager.RegisterLoadFunction("texture", Resources.SFMLResourcesManager.LoadTexture);
            
                // pongo dimensiones al mundo
                _worldBounds = new FloatRect(0, 0, _worldView.Size.X, 3000);

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

        private void BuildWorld() {

            SceneNode layer;

            try 
            {

                // Creo las capas como SceneNode's hijos del árbol raíz.
                for (int contLayer = 0; contLayer < (int)Layer.LayerCount; contLayer++) {
                    layer = new SceneNode();
                    _sceneGraph.AddChild(layer);
                    _sceneLayers.Add(layer);
                }

                ///// BACKGROUND
                // creo el SpriteNode asignandole la textura 
                SpriteNode background = new SpriteNode(_resManager["Fondos:EspacioEstrellado"] as Texture,
                       new IntRect((int)_worldBounds.Left, (int)_worldBounds.Top, (int)_worldBounds.Width, (int)_worldBounds.Height));

                // lo posiciono en la esquina superior derecha del mundo
                background.Position = new Vector2f(_worldBounds.Left, _worldBounds.Top);
                _sceneLayers[(int)Layer.Background].AddChild(background);

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex.Message);
            }
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
            // movemos la vista, en caso de que lleguemos al principio la recolocamos abajo de nuevo  
            if (_worldView.Center.Y <= _worldBounds.Top + _worldView.Size.Y/2)
                _worldView.Center = new Vector2f(_worldView.Size.X / 2, _worldBounds.Height - (_worldView.Size.Y / 2));
            else
                _worldView.Move(new Vector2f(0, _scrollSpeed * dt.AsSeconds()));

            // actualizamos el grafo de escena
            _sceneGraph.Update(dt);
        }

    }
}
