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
   
    /// <summary>
    /// Encapsula la escena del menu principal de la aplicación
    /// </summary>
    class MenuScene : Scene
    {
        // variables miembro
        private List<SFML.Graphics.Text> _options;              // textos de las opcines del menu
        private int _optionIndex;                               // opciones seleccionada

        private const float OPTION_SEPARATION = 70f;            // separación entre opciones del menu

        // Opciones del menu
        private enum MenuOptions
        {
            PLAY,
            EXIT
        }

        // logger
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public MenuScene(Scene.Context context, SceneManager scnManager) 
            : base (context, scnManager)
        {
            _logger.Log(LogLevel.Info, " >>> Configurando escena del Menú");

            _optionIndex = (int)MenuOptions.PLAY; ;
            _options = new List<SFML.Graphics.Text>();

            // opcion jugar
            SFML.Graphics.Text textPlay = new SFML.Graphics.Text();
            textPlay.Font = (SFML.Graphics.Font)context.ResourcesManager["Fuentes:Titulo"];
            textPlay.DisplayedString = "Jugar";
            textPlay.Position = new SFML.System.Vector2f((context.Window.Size.X - textPlay.GetLocalBounds().Width) * 0.5f,
                (context.Window.Size.Y - 2 * textPlay.CharacterSize - OPTION_SEPARATION) *0.5f);
            _options.Add(textPlay);

            // opción salir
            SFML.Graphics.Text textExit = new SFML.Graphics.Text();
            textExit.Font = (SFML.Graphics.Font)context.ResourcesManager["Fuentes:Titulo"];
            textExit.DisplayedString = "Salir";
            textExit.Position = new SFML.System.Vector2f((context.Window.Size.X - textExit.GetLocalBounds().Width) * 0.5f,
                textPlay.Position.Y + textPlay.CharacterSize + OPTION_SEPARATION);
            _options.Add(textExit);

            UpdateOptionText();
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
            return true;
        }

        /// <summary>
        /// Dibuja el mundo
        /// </summary>
        public override void Draw()
        {
            // Dibuja los elementos contenidos en la escena
            SFML.Graphics.RenderWindow window = SceneContext.Window;
           
            foreach (var option in _options)
            {
                window.Draw(option);
            }
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

            if (key == SFML.Window.Keyboard.Key.Return)              // flecha del cursor hacia abajo
            {
                _logger.Log(LogLevel.Info, " >>>>> Opción aceptada: " + _optionIndex);
                if (_optionIndex == (int)MenuOptions.PLAY)
                {
                    // quito la escena actual de la pila
                    _logger.Log(LogLevel.Info, " >>>> Pop");
                    RequestManagerScenePop();

                    // pongo en la pila el menu
                    _logger.Log(LogLevel.Info, " >>>> Push Juego");
                    RequestManagerScenePush((int)Application.SceneID.GAME);
                }
                else if (_optionIndex == (int)MenuOptions.EXIT)
                {
                    //si la opción es salir quito todas las escenas de la pila
                    RequestManagerSceneClear();
                }
            }
            else if (key == SFML.Window.Keyboard.Key.Up)             // flecha del cursor hacia arriba
            {
                if (_optionIndex > 0)
                    _optionIndex--;
                else
                    _optionIndex = _options.Count - 1;
               
                UpdateOptionText();
            }
            else if (key == SFML.Window.Keyboard.Key.Down)          // flecha del cursor hacia abajo
            {
                if (_optionIndex < _options.Count - 1) 
                    _optionIndex++;
                else               
                    _optionIndex = 0;

                UpdateOptionText();
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
            if (!isPressed) return false;

            if (button == 2) 
            {
                _logger.Log(LogLevel.Info, " >>>>> Opción aceptada: " + _optionIndex);
                if (_optionIndex == (int)MenuOptions.PLAY)
                {
                    // quito la escena actual de la pila
                    _logger.Log(LogLevel.Info, " >>>> Pop");
                    RequestManagerScenePop();

                    // pongo en la pila el menu
                    _logger.Log(LogLevel.Info, " >>>> Push Juego");
                    RequestManagerScenePush((int)Application.SceneID.GAME);
                }
                else if (_optionIndex == (int)MenuOptions.EXIT)
                {
                    //si la opción es salir quito todas las escenas de la pila
                    RequestManagerSceneClear();
                }
            }

            return false;
        }

        /// <summary>
        /// Cambia el color del texto seleccionado
        /// </summary>
        private void UpdateOptionText()
        {
            if (_options.Count == 0) return;

            // White all texts
            foreach (var option in _options)
            {
                option.Color = SFML.Graphics.Color.White;
            }

            // Pongo en rojo el seleccionado
            _options[_optionIndex].Color = SFML.Graphics.Color.Red;
        }

    }
}

