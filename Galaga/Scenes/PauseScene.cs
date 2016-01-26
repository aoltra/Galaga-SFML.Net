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

using NLog;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Scenes
{
    class PauseScene : Scene
    {
        // variables miembro
        private SFML.Graphics.Text _mainText;                   // textos de las opcines del menu
        private SFML.Graphics.Text _infoText;                   // textos de las opcines del menu

        private const float OPTION_SEPARATION = 70f;            // separación entre opciones del menu

        // Opciones del menu
        private enum MenuOptions
        {
            Play,
            Exit
        }

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public PauseScene(Scene.Context context, SceneManager scnManager) 
            : base (context, scnManager)
        {
            _logger.Log(LogLevel.Info, " >>> Configurando escena de Pausa");

            _mainText = new SFML.Graphics.Text();
            _infoText = new SFML.Graphics.Text();

            // opcion jugar
            _mainText.Font = (SFML.Graphics.Font)context.ResourcesManager["Fuentes:Titulo"];
            _mainText.DisplayedString = "Pausa";
            _mainText.Position = new SFML.System.Vector2f((context.Window.Size.X - _mainText.GetLocalBounds().Width) * 0.5f,
                (context.Window.Size.Y - 2 * _mainText.CharacterSize - OPTION_SEPARATION) * 0.5f);

            // opción salir
            _infoText.Font = (SFML.Graphics.Font)context.ResourcesManager["Fuentes:Titulo"];
            _infoText.DisplayedString = "Pulsa espacio para ir al menú principal";
            _infoText.CharacterSize = (UInt32)(_infoText.CharacterSize * 0.8f);
            _infoText.Position = new SFML.System.Vector2f((context.Window.Size.X - _infoText.GetLocalBounds().Width) * 0.5f,
                _mainText.Position.Y + _mainText.CharacterSize + OPTION_SEPARATION);

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
            return false;
        }

        /// <summary>
        /// Dibuja el mundo
        /// </summary>
        public override void Draw()
        {
            // Dibuja los elementos contenidos en la escena
            SFML.Graphics.RenderWindow window = SceneContext.Window;

            window.Draw(_infoText);
            window.Draw(_mainText);
        }

        /// <summary>
        /// Gestiona para esta escena el manejo de los eventos de teclado
        /// </summary>
        /// <param name="key">Tacla pulsada</param>
        /// <param name="isPressed">True si está pulsada o se libera</param>
        /// <returns>true si se deja que las escena inferiores en el gestor también lo controlen, false en caso contrario</returns>
        public sealed override bool HandleKeyboardEvent(SFML.Window.Keyboard.Key key, bool isPressed)
        {
            if (!isPressed) return false;

            if (key == SFML.Window.Keyboard.Key.Escape)
            {
                // quito la escena actual de la pila
                _logger.Log(LogLevel.Info, " >>>> Pop");
                RequestManagerScenePop();
            }

            if (key == SFML.Window.Keyboard.Key.Space)
            {
                // vamos al menu
                _logger.Log(LogLevel.Info, " >>>> Clear Scenes");
                RequestManagerSceneClear();
                
                _logger.Log(LogLevel.Info, " >>>> Push Menu");
                RequestManagerScenePush((int)Application.SceneID.Menu);
            }

            return false;

        }

    }
}


