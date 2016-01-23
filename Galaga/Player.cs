using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    class Player
    {
        // variables miembro
        private float _playerSpeed;                     // velocidad del jugador

        /// <summary>
        /// Constructor
        /// </summary>
        public Player() 
        {
            _playerSpeed = 150;                         // 150 px/s
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
           

            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.W))
            {
               
            }

            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.A))
            {
              
            }

            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.S))
            {
               
            }

            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.D))
            {
                Commands.LinealMovementCommand lmC = new Commands.LinealMovementCommand(_playerSpeed,0);
                lmC.Category = (UInt16)Category.PlayerShip;
                commands.Push(lmC);
            }
        }

        /// <summary>
        /// Maneja los eventos de teclado creando un comando e introduciendolo en la cola
        /// </summary>
        /// <param name="key">tecla pulsada</param>
        /// <param name="isPressed">Si estla pulsada</param>
        /// <param name="commands">Cola de comandos en la que se introducura el comando</param>
        /// <remarks> 
        /// En C# la gestion de los eventos se realiza con eventos y delegados, por lo que aqui ya llega el evento procesado.
        /// En Java el evento llega sin ser procesado por el delegado por lo que la función debe
        /// recibir el evento y filtrarlo según lo que quiera gestionar: HandleInputEvent(SFML.Window.Event evnt, CommandQueue commands) 
        /// </remarks>
        public void HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed, CommandQueue commands)
        {
            
        }



    }
}
