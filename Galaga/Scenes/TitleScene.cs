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

using NLog;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Scenes
{
    /// <summary>
    /// Encapsula la escena del titulo de la aplicación
    /// </summary>
    public class TitleScene : Scene
    {
        // variables miembro
        private SFML.Graphics.Sprite _backgroundSprite;             // sprite de fondo
        private SFML.Graphics.Text _text;                           // texot

        private bool _showText;                                     // indica si se muestra el texto o no 
        private SFML.System.Time textEffectTime;                    // tiempo transcurrido desde que le texto ha aparecido o desaparecido

        private const float BLINK_TIME = 0.5f;                      // tiempo que dura el parpadeo

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

            _showText = true;
            textEffectTime = SFML.System.Time.Zero;

            // configuramos el texto
            _text.Font = (SFML.Graphics.Font)context.ResourcesManager["Fuentes:Titulo"];
            _text.DisplayedString = "Pulsa una tecla para empezar";
            _text.Position = new SFML.System.Vector2f((context.Window.Size.X - _text.GetLocalBounds().Width) * 0.5f,
                context.Window.Size.Y - _text.CharacterSize - 50);
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
            // en función del tiempo que ha pasado le indico si se ha de mostrar o no el texto
            textEffectTime += time;

            if (textEffectTime >= SFML.System.Time.FromSeconds(BLINK_TIME))
            {
                _showText = !_showText;
                textEffectTime = SFML.System.Time.Zero;
            }

            return true;
        }

        /// <summary>
        /// Dibuja el mundo
        /// </summary>
        public override void Draw()
        {
            // Dibuja los elementos contenidos en la escena
            SFML.Graphics.RenderWindow window = SceneContext.Window;
            window.Draw(_backgroundSprite);

            if (_showText) window.Draw(_text);
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
