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

using System;
using System.Collections.Generic;
using System.Text;

using edu.CiclosFormativos.Games.DIDAM.Commands;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    class Player
    {
        // variables miembro
        private float _playerSpeed;                             // velocidad del jugador

        private Dictionary<Action, Command> _actionBinding;     // diccionario que relaciona cada posible accion del jugador con su comando

        // Define las acciones que puede realizar el jugador 
        private enum Action 
        { 
            MOVERIGHT,
            MOVELEFT,
            FIRE
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Player() 
        {
            _playerSpeed = 150;                         // 150 px/s
            _actionBinding = new Dictionary<Action, Command>();

            // inicializo las posibles acciones que puede que realizar el usuario
            _actionBinding.Add(Action.MOVELEFT, new Commands.LinealMovementCommand(-_playerSpeed, 0));
            _actionBinding.Add(Action.MOVERIGHT, new Commands.LinealMovementCommand(_playerSpeed, 0));
            _actionBinding.Add(Action.FIRE, new Commands.FireCommand());

            // indico que todas las acciones afectan a la nave de usuario
            foreach (KeyValuePair<Action,Command> cmd in _actionBinding)
                cmd.Value.Category = (UInt16)Category.PLAYERSHIP;
        }


        /// <summary>
        /// Maneja los eventos del tipo "en tiempo real" creando un comando e introduciendolo en la cola
        /// </summary>
        /// <param name="commands">Cola de comandos en la que se introducura el comando</param>
        /// <remarks> 
        /// Comandos por los que se pregunta si están activos en cada frame
        /// </remarks>
        public void HandleRealtimeInput(CommandQueue commands)
        {
            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.A))
            {
                Commands.LinealMovementCommand lmC = (Commands.LinealMovementCommand)_actionBinding[Action.MOVELEFT];
                commands.Push(lmC);
            }

            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.D))
            {
                Commands.LinealMovementCommand lmC = (Commands.LinealMovementCommand)_actionBinding[Action.MOVERIGHT];
                commands.Push(lmC);
            }

            //if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.Space))
            //{
            //    Commands.FireCommand lmC = (Commands.FireCommand)_actionBinding[Action.FIRE];
            //    commands.Push(lmC);
            //}
        }

        /// <summary>
        /// Maneja los eventos de teclado creando un comando e introduciendolo en la cola
        /// </summary>
        /// <param name="key">tecla pulsada</param>
        /// <param name="isPressed">Si está pulsada</param>
        /// <param name="commands">Cola de comandos en la que se introducura el comando</param>
        /// <remarks> 
        /// En C# la gestion de los eventos se realiza con eventos y delegados, por lo que aqui ya llega el evento procesado.
        /// En Java el evento llega sin ser procesado por el delegado por lo que la función debe
        /// recibir el evento y filtrarlo según lo que quiera gestionar: HandleInputEvent(SFML.Window.Event evnt, CommandQueue commands) 
        /// </remarks>
        public void HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed, CommandQueue commands)
        {
            if ( key == SFML.Window.Keyboard.Key.Space && !isPressed)
            {
                Commands.FireCommand lmC = (Commands.FireCommand)_actionBinding[Action.FIRE];
                commands.Push(lmC);
            }
        }



    }
}
