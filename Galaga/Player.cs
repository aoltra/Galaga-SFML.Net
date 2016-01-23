using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    class Player
    {   
        /// <summary>
        /// Maneja los eventos del tipo "en tiempo real" creando un comando e introduciendolo en la cola
        /// </summary>
        /// <param name="commands">Cola de comandos en la que se introducura el comando</param>
        /// <remarks> 
        /// Comandos por los que se pregunta si están activos en cada frame
        /// </remarks>
        public void HandleRealtimeInput(CommandQueue commands)
        {

        }

        /// <summary>
        /// Maneja los eventos de teclado creando un comando e introduciendolo en la cola
        /// </summary>
        /// <param name="key">tecla pulsada</param>
        /// <param name="isPressed">Si estla pulsada</param>
        /// <param name="commands">Cola de comandos en la que se introducura el comando</param>
        /// /// <remarks> 
        /// En C# la gestion de los eventos se realiza con eventos y delegados, por lo que aqui ya llega el evento procesado.
        /// En Java el evento llega sin ser procesado por el delegado por lo que la función debe
        /// recibir el evento y filtrarlo según lo que quiera gestionar: HandleInputEvent(SFML.Window.Event evnt, CommandQueue commands) 
        /// </remarks>
        public void HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed, CommandQueue commands)
        {
            
        }



    }
}
