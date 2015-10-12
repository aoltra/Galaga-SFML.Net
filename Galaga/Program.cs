using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

// funciona con .Net 4.0
namespace edu.CiclosFormativos.DAM.DI.Galaga
{
	class Game
	{
		static void Main(string[] args)
		{
			Game game = new Game();             // instanciación del juego 
			game.run();                         // llamada a la función de arranque
		}
		
		// Variables miembro
		private RenderWindow _window;                    // ventana principal
	
		private CircleShape _player;					 // jugador (un simple círculo cyan)
		private bool _IsMovingUp,_IsMovingDown,_IsMovingLeft,_IsMovingRight;
		
		// Constructor
		public Game() {
			
			// buffer 32 bits de colors
			ContextSettings contextSettings = new ContextSettings();
			contextSettings.DepthBits = 32;
			
			// Creamos la ventana principal
			_window = new RenderWindow(new VideoMode(1280, 1024), "Galaga ", Styles.Default, contextSettings);

			_player = new CircleShape ();
			_player.Radius = 40f;
			_player.Position = new Vector2f(100f, 100f);
			_player.FillColor = Color.Cyan;

			RegisterDelegates();
		}

		////////////////////////
		// Métodos
		////////////////////////
		public void run() {

			// Game Loop
			while (_window.IsOpen)
			{
				// Procesamos eventos
				_window.DispatchEvents();

				update();
				render();
			}
		}

		// Registra los delegados
		private void RegisterDelegates() {
			
			_window.Closed += new EventHandler(OnClose);
			_window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
			_window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyReleased);
		}
		
		// actualiza el estado del mundo
		private void update() {
			SFML.System.Vector2f movement = new Vector2f(0f, 0f);

            // desplaza 1 px en el sentido que haya inidcado la pulsacion del teclado
			if (_IsMovingUp)
				movement.Y -= 1f;
			if (_IsMovingDown)
				movement.Y += 1f;
			if (_IsMovingLeft)
				movement.X -= 1f;
			if (_IsMovingRight)
				movement.X += 1f;
			_player.Position += movement;
		}
		
		// Dibuja el mundo
		private void render() { 
			// limpia la pantalla (por defecto en negro, pero podemos asignarle un color)
			_window.Clear();
			// Dibuja un elemento "dibujable", Drawable. En este caso nuestro "jugador": el círculo
			_window.Draw (_player);
			// muestra la pantalla
			_window.Display ();
		}
		
		/////////////////////////////////////
		// funciones suscritas a delegados
		/////////////////////////////////////
		
		// La ventana se ha cerrado
		private void OnClose(object sender, EventArgs e) {
			
			Window window = (Window)sender;
			window.Close();
		}

		private void OnKeyPressed(object sender, KeyEventArgs e) {
			handlePlayerInput(e.Code, true);
		}

		private void OnKeyReleased(object sender, KeyEventArgs e) {
			handlePlayerInput(e.Code, false);
		}

		private void handlePlayerInput(SFML.Window.Keyboard.Key key, bool pressed) {

			if (key == SFML.Window.Keyboard.Key.W)
				_IsMovingUp = pressed;
			else if (key == SFML.Window.Keyboard.Key.S)
				_IsMovingDown = pressed;
			else if (key == SFML.Window.Keyboard.Key.A)
				_IsMovingLeft = pressed;
			else if (key == SFML.Window.Keyboard.Key.D)
				_IsMovingRight = pressed;
		}
	}
}
