using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Scenes
{
    /// <summary>
    /// Encapsula la escena del titulo de la aplicación
    /// </summary>
    public class TitleScene : Scene
    {
        // variables miembro
        private SFML.Graphics.Sprite _backgroundSprite;
        private SFML.Graphics.Text _text;

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public TitleScene(Scene.Context context, SceneManager scnManager) 
            : base (context, scnManager)
        {
            _logger.Log(LogLevel.Info, " >>> Configurando escena del título.");

            _text = new SFML.Graphics.Text();

            _backgroundSprite = new SFML.Graphics.Sprite((SFML.Graphics.Texture)context.ResourcesManager["Fondos:Titulo"]);

            // centro el sprite horizontalmente
            _backgroundSprite.Position = 
                new SFML.System.Vector2f((context.Window.Size.X - _backgroundSprite.GetLocalBounds().Width) *.5f, 0f);
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
          

            return true;
        }

        /// <summary>
        /// Dibuja el mundo
        /// </summary>
        public override void Draw()
        {
            // Dibuja los elementos contenidos en la escena
            SceneContext.Window.Draw(_backgroundSprite);
        }

        /// <summary>
        /// Gestiona para esta escena el manejo de los eventos de teclado
        /// </summary>
        /// <param name="key">Tacla pulsada</param>
        /// <param name="isPressed">True si está pulsada o se libera</param>
        /// <returns>true si se deja que las escena inferiores en el gestor también lo controlen, false en caso contrario</returns>
        public sealed override bool HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed)
        {
            // redirecciona la gestión al player
          //  _player.HandleKeyboardEvent(key, isPressed, _world.CommandQueue);

            return true;
        }

    }
}
