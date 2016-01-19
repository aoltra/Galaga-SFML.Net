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

using SFML.Graphics;

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

        // enum con las diferentes capas
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

            _window = window;
            // asigno la vista por defecto (viewport completo y tamaño igual en pixeles al de al ventana
            // hago una copia de la vista por defecto, para mantener guardada la original por si quiero volver a ella 
            _worldView = new View(_window.DefaultView);

            // asignamos al mundo la vista definida (por ahora un clon de la de por defecto)
            _window.SetView(_worldView);

            BuildWorld();

        }

        private void BuildWorld() {

            SceneNode layer;

            // Creo las capas como SceneNode's hijos del árbol raíz.
            for (int contLayer = 0; contLayer < (int)Layer.LayerCount; contLayer++) {
                layer = new SceneNode();
                _sceneGraph.AddChild(layer);
                _sceneLayers.Add(layer);
            }
        
        }

        /// <summary>
        /// Dibuja el nodo
        /// </summary>
        public void Draw() { 
        
        }

        /// <summary>
        /// Actualizo el estado de los elementos del mundo
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        public void Update(SFML.System.Time dt)
        { 
        
        
        }

    }
}
