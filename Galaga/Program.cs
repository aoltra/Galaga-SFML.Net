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
        private float _playerSpeed;                      // velocidad del jugador 
		
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

            _playerSpeed = 25;           // 25 px/s

			RegisterDelegates();
		}

		////////////////////////
		// Métodos
		////////////////////////
		public void run() {
            
            Clock clock = new Clock();
            SFML.System.Time deltaTime;
			
			// Game Loop
			while (_window.IsOpen)
			{
                // para cada uno de los ciclos reinicio el reloj a cero y devuelvo
                // el tiempo que ha pasado desde el inicio
                deltaTime = clock.Restart();

                // Procesamos eventos
                _window.DispatchEvents();

                update(deltaTime);
				render();
			}
		}

		// Registra los delegados
		private void RegisterDelegates() {
			
			_window.Closed += new EventHandler(OnClose);
			_window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
			_window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyReleased);
		}

        // actualiza el estado del mundo en función del tiempo transcurrido desde la última actualización
        private void update(SFML.System.Time time)
        {
			SFML.System.Vector2f speed = new Vector2f(0f, 0f);

            // desplaza 1 px en el sentido que haya inidcado la pulsacion del teclado
			if (_IsMovingUp)
				speed.Y -= _playerSpeed;
			if (_IsMovingDown)
                speed.Y += _playerSpeed;
			if (_IsMovingLeft)
                speed.X -= _playerSpeed;
			if (_IsMovingRight)
                speed.X += _playerSpeed;

            // espacio = velocidad * tiempo. El nuevo espacio se añade a la posición previa
            // del tiempo se obtienen los segundos ya que la velocidad se dam en px/s
            _player.Position += speed * time.AsSeconds(); 
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
