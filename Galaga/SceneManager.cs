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

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Gestiona la creación y uso de las escenas
    /// </summary>
    public class SceneManager
    {
        // variables miembro
        private List<Scene> _stack;                                 // modelizo la pila con una lista

        // diccionario que almacena la posibles funciones de creacion de escenas
        private Dictionary<int, CreateSceneTypeDelegate> _createFuncMap = 
            new Dictionary<int, CreateSceneTypeDelegate>();

        /// <summary>
        /// Delegado que define como tiene que ser creadas las funciones de creación de escenas
        /// </summary>
        /// <param name="scnContext">Contexto o datos a intercambiarse entre escenas</param>
        /// <param name="scnManager">Gestor de escenas</param>
        /// <returns>Scene creada</returns>
        public delegate Scene CreateSceneTypeDelegate(Scene.Context scnContext, SceneManager scnManager);   

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneManager() {

            _stack = new List<Scene>();


        
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
