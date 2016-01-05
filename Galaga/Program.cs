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
	
		private Sprite _player;					 // jugador (un simple círculo cyan)
		private bool _IsMovingUp,_IsMovingDown,_IsMovingLeft,_IsMovingRight;
        private float _playerSpeed;                      // velocidad del jugador 

        private SFML.System.Time _timePerFrame;          // en este caso indica el mínimo requerido
		
		// Constructor
		public Game() {
			
			// buffer 32 bits de colors
			ContextSettings contextSettings = new ContextSettings();
			contextSettings.DepthBits = 32;
			
			// Creamos la ventana principal
			_window = new RenderWindow(new VideoMode(1280, 800), "Galaga ", Styles.Default, contextSettings);

            // el jugador pasa ahora a ser un Sprite
			_player = new Sprite();
			_player.Position = new Vector2f(100f, 100f);

            _playerSpeed = 100;           // 100 px/s

            _timePerFrame = SFML.System.Time.FromSeconds(1f / 40f);           // como mínimo 40 frames por segundo

			RegisterDelegates();

            try
            {
                // prueba del correcto funcionamiento
                Resources.ResourcesManager a = new Resources.ResourcesManager(
                    this.GetType().Assembly.GetManifestResourceStream("Galaga.main.resxml"));
                //String [] ad = this.GetType().Assembly.GetManifestResourceNames();
                a.RegisterLoadFunction("texture",Resources.SFMLResourcesManager.LoadTexture);

                // le asigno la textura Naves:NaveJugador
                _player.Texture = (Texture)a["Naves:NaveJugador"];
            }
            catch (Exception)
            { 
            
            }
		}

		////////////////////////
		// Métodos
		////////////////////////
		public void run() {
            
            Clock clock = new Clock();
            SFML.System.Time timeSinceLastUpdate = SFML.System.Time.Zero;
          
			// Game Loop
			while (_window.IsOpen)
			{
                // Procesamos eventos. Este procesamiento de evento se podría quitar ya que sólo
                // tendría importancia para aquellos eventos que no afectasen al mundo
                // en este caso el Close. Si lo quitaramos sólo se retrasaría un poco (hasta el paso del tiempo
                // del frame) al ejecución del evento 
                _window.DispatchEvents();

                // para cada uno de los ciclos reinicio el reloj a cero y devuelvo
                // el tiempo que ha transcurrido
                timeSinceLastUpdate = clock.Restart();

                // si el tiempo transcurrido es mayor que el que queremos por cada frame
                while (timeSinceLastUpdate > _timePerFrame)
                {
                    timeSinceLastUpdate -= _timePerFrame;   // le quito un frame
                    
                    // Procesamos eventos
                    _window.DispatchEvents();

                    update(_timePerFrame);  

                    // si después de este ciclo el tiempo que ha transcurrido sigue siendo mayor al de un frame
                    // repito el ciclo y voy actualizando el mundo, aunque no lo renderice
                }

                // en cada ciclo actualizo y renderizo
                update(timeSinceLastUpdate);          
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

            // desplaza 1 px en el sentido que haya indicado la pulsacion del teclado
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
			// muestra la pantalla. Hace el cambio de un buffer a otro (doble buffer)
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
