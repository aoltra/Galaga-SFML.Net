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


namespace edu.CiclosFormativos.Games.DIDAM.Scenes
{
    /// <summary>
    /// Encapsula una escena o estado del juego
    /// </summary>
    /// <remarks>No es posible instanciar objetos de ella</remarks>
    public abstract class Scene
    {
        // Variables miembro
        private SceneManager _scnManager;                   // gestor de escenas
        private Context _context;                           // contexto de trabajo

        /// <summary>
        /// Devuelve el contexto en el que trabaja la escena
        /// </summary>
        protected Context SceneContext { get { return _context; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto o datos a intercambiarse entre escenas</param>
        /// <param name="scnManager">Gestor de escenas</param>
        public Scene(Scene.Context context, SceneManager scnManager)
        {
            _context = context;
            _scnManager = scnManager;
        }

        /// <summary>
        /// Dibuja la escena
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Actualiza el estado de los elementos de la escena
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <returns>true si se deja que las escena inferiores en el gestor también se actualicen, false en caso contrario</returns>
        public abstract bool Update(SFML.System.Time dt);

        /// <summary>
        /// Gestiona los eventos de pulsaciones en teclado. Cada escena tiene que tener uno
        /// </summary>
        /// <param name="key">Tacla pulsada</param>
        /// <param name="isPressed">True si está pulsada o se libera</param>
        /// <returns>true si se deja que las escena inferiores en el gestor también lo controlen, false en caso contrario</returns>
        public abstract bool HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed);

        /// <summary>
        /// Introduce una escena en la pila
        /// </summary>
        /// <param name="sceneID"></param>
        protected void RequestManagerScenePush(int sceneID) { _scnManager.Push(sceneID); }

        /// <summary>
        /// Extrae una escena de la pila
        /// </summary>
        protected void RequestManagerScenePop() { _scnManager.Pop(); }

        /// <summary>
        /// Limpa la pila del gestor de escenas
        /// </summary>
        protected void RequestManagerSceneClear() {  _scnManager.Clear(); }

        /// <summary>
        /// Encapsula los datos que se pasan entre escenas
        /// </summary>
        public class Context 
        {
            /// <summary>
            /// Devuelve la ventana de renderizado
            /// </summary>
            public RenderWindow Window { get; private set; }

            /// <summary>
            /// Devuelve el gestor de recursos
            /// </summary>
            public Resources.ResourcesManager ResourcesManager { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="window">Ventana de renderizado</param>
            public Context(RenderWindow window, Resources.ResourcesManager resMan) 
            {
                Window = window;
                ResourcesManager = resMan;
            }
        }
    
    }


}
