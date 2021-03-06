﻿#region GPL License
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

using System.Collections.Generic;

namespace edu.CiclosFormativos.Games.DIDAM.Commands
{
    /// <summary>
    /// Encapsula una cola FIFO de comandos
    /// </summary>
    public class CommandQueue
    {
        // variables miembro
        private Queue<Command> queue = new Queue<Command>();

        /// <summary>
        /// Devuelve true si la cola esta vacía o false en caso contrario
        /// </summary>
        public bool IsEmpty { get { return queue.Count == 0; } }

        /// <summary>
        /// Mete un comando en la cola
        /// </summary>
        /// <param name="command">Comando a introducirse en la cola</param>
        /// <remarks>
        /// Lo pone en la última posición
        /// </remarks>
        public void Push(Command command)
        {
            queue.Enqueue(command);
        }
    
        /// <summary>
        /// Saca un comando de la cola
        /// </summary>
        /// <returns>El primer comando de la cola</returns>
        public Command Pop()
        {
            return queue.Dequeue();
        }
    }
}
