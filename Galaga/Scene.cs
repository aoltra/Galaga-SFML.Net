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

namespace edu.CiclosFormativos.DAM.DI.Galaga
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
        public abstract void Update(SFML.System.Time dt);

        /// <summary>
        /// Encapsula los datos que se pasan entre escenas
        /// </summary>
        public class Context 
        {
            public RenderWindow Window { get; private set; }

            public Context(RenderWindow window) 
            {
                Window = window;
            }
        }
    
    }


}
