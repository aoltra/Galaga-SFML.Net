using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

using NLog;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Scenes
{
    /// <summary>
    /// Encapsula la escena principal la aplicación: el juego
    /// </summary>
    class GameScene : Scene
    {
        // Variables miembro
        private World _world;                           // mundo del juego
        private Player _player;					        // jugador

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Constructor
        public GameScene(Scene.Context context, SceneManager scnManager) 
            : base (context, scnManager)
        {
            _logger.Log(LogLevel.Info, " >>> Configurando escena juego.");

            _player = new Player();

            _world = new World(context);

        }

        ////////////////////////
        // Métodos
        ////////////////////////
     
        /// <summary>
        /// Actualiza el estado de la escena en función del tiempo transcurrido desde la última actualización 
        /// </summary>
        /// <param name="time">tiempo transcurrido desde la última actualización </param>
        /// <returns>true: siempre deja que las escenas inferiores se actualicen</returns>
        public override bool Update(SFML.System.Time time)
        {
            // calculamos las nuevas posiciones de los elementos del mundo
            _world.Update(time);

            _player.HandleRealtimeInput(_world.CommandQueue);

            return true;
        }

        /// <summary>
        /// Dibuja el mundo
        /// </summary>
        public override void Draw()
        {
            // Dibuja los elementos contenidos en el mundo
            _world.Draw();
        }

        /// <summary>
        /// Gestiona para esta escena el manejo de los eventos de teclado
        /// </summary>
        /// <param name="key">Tacla pulsada</param>
        /// <param name="isPressed">True si está pulsada o se libera</param>
        /// <returns>true si se deja que las escena inferiores en el gestor también lo controlen, false en caso contrario</returns>
        public sealed override bool HandleKeyboardEvent(Keyboard.Key key, bool isPressed)
        {
            // redirecciona la gestión al player
            _player.HandleKeyboardEvent(key, isPressed, _world.CommandQueue);

            // Se pulsa escape y vamos a la escena de pause
            if (isPressed && key == Keyboard.Key.Escape)
            {
                _logger.Log(LogLevel.Info, " >>>> Push Pause");
                RequestManagerScenePush((int)Application.SceneID.PAUSE);
            }

            return true;
        }

        /// <summary>
        /// Gestiona para esta escena el manejo de los eventos de pulsación de una tecla del joystick
        /// </summary>
        /// <param name="joystick">id del joystick al que hace referencia el evento</param>
        /// <param name="button">Botón pulsado</param>
        /// <param name="isPressed">True si está pulsado o se libera</param>
        /// <returns>true si se deja que las escena inferiores en el gestor también lo controlen, false en caso contrario</returns>
        public sealed override bool HandleJoystickButtonEvent(uint joystick, uint button, bool isPressed)
        {
            // redirecciona la gestión al player
            //_player.HandleKeyboardEvent(key, isPressed, _world.CommandQueue);

            // Se pulsa la X y vamos a la escena de pause
            if (isPressed && button == 0)
            {
                _logger.Log(LogLevel.Info, " >>>> Push Pause");
                RequestManagerScenePush((int)Application.SceneID.PAUSE);
            }

            return true;
        }

       
    }
}
