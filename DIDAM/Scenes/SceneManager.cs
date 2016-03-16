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

namespace edu.CiclosFormativos.Games.DIDAM.Scenes
{
    /// <summary>
    /// Gestiona la creación y uso de las escenas
    /// </summary>
    public class SceneManager
    {
        // variables miembro
        private List<Scene> _stack;                                 // modelizo la pila con una lista
        private List<StackRequest> _requestList;                    // lista de peticiones de operaciones a realizar en la pila

        // diccionario que almacena la posibles funciones de creacion de escenas
        private Dictionary<int, CreateSceneTypeDelegate> _createFuncMap = 
            new Dictionary<int, CreateSceneTypeDelegate>();

        /// <summary>
        /// Delegado que define como tiene que ser creadas las funciones de creación de escenas
        /// </summary>
        /// <returns>Scene creada</returns>
        public delegate Scene CreateSceneTypeDelegate();

        /// <summary>
        /// Devuelve si el gestor está limpio de escenas
        /// </summary>
        /// <returns>True si no hay escenas, False en caso contrario</returns>
        public bool IsEmpty { get { return _stack.Count == 0; } }
            
        /// <summary>
        /// Constructor
        /// </summary>
        public SceneManager() {

            _stack = new List<Scene>();
            _requestList = new List<StackRequest>();

        
        }

        /// <summary>
        /// Registra funciones de creación de tipos de escenas para poder ser utilizadas cuando sea necesario crear una escena
        /// </summary>
        /// <param name="sceneID">Identificador de la escena al que se le asocia la función de creación. Hace las funciones de key</param>
        /// <param name="f">Función de carga</param>
        public void RegisterCreateFunction(int sceneID, CreateSceneTypeDelegate f)
        {
            try
            {
                _createFuncMap.Add(sceneID, f);
            }
            catch (Exception ex)
            {
                throw new SceneManagerException(ex.Message);
            }
        }

        /// <summary>
        /// Introduce una escena en la pila
        /// </summary>
        /// <param name="sceneID">Identificador de la escena</param>
        /// <remarks>
        /// Realmente mete la acción en la lista de peticiones pendientes a la espera de poder ser realizada
        /// </remarks>
        public void Push(int sceneID)
        {
            _requestList.Add(new StackRequest(StackRequest.StackAction.PUSH, sceneID));
        }

        /// <summary>
        /// Saca la escena cima de la pila
        /// </summary>
        /// <remarks>
        /// Realmente mete la acción en la lista de peticiones pendientes a la espera de poder ser realizada
        /// </remarks>
        public void Pop()
        {
            _requestList.Add(new StackRequest(StackRequest.StackAction.POP));
        }

        /// <summary>
        /// Limpia el gestor de escenas
        /// </summary>
        /// <remarks>
        /// Realmente mete la acción en la lista de peticiones pendientes a la espera de poder ser realizada
        /// </remarks>
        public void Clear()
        {
            _requestList.Add(new StackRequest(StackRequest.StackAction.CLEAR));
        }

        /// <summary>
        /// Actualiza todas las escenas
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        public void Update(SFML.System.Time dt)
        {
            // itera desde el final de la lista hacia el principio, o lo que es lo mismo
            // itera la pila desde la cima hacia abajo
            for (int i = _stack.Count - 1; i >= 0; i--)
            {
                if (!_stack[i].Update(dt))
                    break;
            }

            ApplyRequest();
        }

        /// <summary>
        /// Dibuja toda la pila 
        /// </summary>
        public void Draw()
        {
            // la escena que está más arriba (la activa) es la última en dibujarse
            foreach (Scene scene in _stack)
            {
                scene.Draw();
            }
        }

        /// <summary>
        /// Redirige las pulsaciones de teclado en modo eventos "típicos" a cada una de las escenas de la pila
        /// </summary>
        /// <param name="key">Tacla pulsada</param>
        /// <param name="isPressed">True si está pulsada o se libera</param>
        public void HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed)
        {
            // itera desde el final de la lista hacia el principio, o lo que es lo mismo
            // itera la pila desde la cima hacia abajo
            // de manera similar al update si la función de manejo del teclado en cada pila
            // devuelve un false, se corta el proceso
            for (int i = _stack.Count - 1; i >= 0; i--)
            {
                if (!_stack[i].HandleKeyboardEvent(key, isPressed))
                    break;
            }

            ApplyRequest();
        }

        /// <summary>
        /// Aplica las operaciones almacenadas en la lista de acciones pendientes
        /// </summary>
        private void ApplyRequest() 
        { 
            foreach (StackRequest sR in _requestList) 
            {
                switch (sR.Action)
                {
                    case StackRequest.StackAction.PUSH:
                        _stack.Add(CreateScene(sR.SceneID));
                        break;

                    case StackRequest.StackAction.POP:
                        _stack.RemoveAt(_stack.Count - 1);
                        break;

                    case StackRequest.StackAction.CLEAR:
                        _stack.Clear();
                        break;
                }
            }

            // una vez procesada toda la lista se borra su contenido
            _requestList.Clear();
        }

        /// <summary>
        /// Crea la escena en función de su identificador
        /// </summary>
        /// <param name="sceneID">Identificador de la escena</param>
        /// <exception cref="Exception">No se ha registrado una función para la creación de ese tipo de escena</exception>
        /// <returns>La escena creada</returns>
        private Scene CreateScene(int sceneID) 
        {
            CreateSceneTypeDelegate createFunc;          // funcion de creación elegida para la creación de una escena en concreto

            if (_createFuncMap.TryGetValue(sceneID, out createFunc))
                return createFunc();
            else
                throw new SceneManagerException("No se ha definido una función para la creación de ese tipo de escena " +
                     sceneID + ". No se ha creado la escena.");
        }
        
    }

    /// <summary>
    /// Encapsula las diferentes peticiones sobre la pila
    /// </summary>
    class StackRequest
    {
        // Acciones a realizar sobre la pila
        public enum StackAction
        {
            PUSH,
            POP,
            CLEAR
        }

        public StackAction Action  { get; private set; }
        public int SceneID { get; private set; }

        public StackRequest(StackAction action, int sceneID = 0)
        {
            Action = action;
            SceneID = sceneID;
        }
    }

    /// <summary>
    /// Encapsula una excepción generada por el Gestor de Escenas
    /// </summary>
    public class SceneManagerException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SceneManagerException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Mensaje de la excepción</param>
        public SceneManagerException(string message)
            : base("[Gestor de Escenas] >> " + message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Mensaje de la excepcion</param>
        /// <param name="inner">Excepcion en la que se basa</param>
        public SceneManagerException(string message, Exception inner)
            : base("[Gestor de Escenas] >> " + message, inner)
        {
        }        
    }
}
