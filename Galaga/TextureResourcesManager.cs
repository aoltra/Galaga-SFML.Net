using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Resources
{
    public class TextureResourcesManager
    {
        // los recursos serán almacenados para su gestión en un diccionario
        private Dictionary<int, WeakReference> _textureMap = new Dictionary<int, WeakReference>();
        //private Dictionary<int, SFML.Graphics.Texture> _textureMap = new Dictionary<int, SFML.Graphics.Texture>();
        
        /// <summary>
        /// Carga Texturas desde el disco
        /// </summary>
        /// <param name="id">Indetificador a asignar a la textura cargada</param>
        /// <param name="filename">Nombre del fichero</param>
        public void Load(int id, String filename) {
            
           // Referencia débil. En cuanto la última referencia fuerte al objeto desaparezca, el GC estará
           // en disposición de recolectarlo
            try
            {
                WeakReference wr = new WeakReference(new SFML.Graphics.Texture(filename));
                _textureMap.Add(id, wr);
            }
            catch (LoadingFailedException ex)
            {
                throw new Exception("Excepción al cargar " +  filename + ". " + ex.Message);
            }
        }

        /// <summary>
        /// Índice que permite acceder a un id determinado
        /// </summary>
        public SFML.Graphics.Texture this[int id]
        {
            get
            {
                WeakReference wr;

                // compruebo si existe el id
                if (_textureMap.TryGetValue(id, out wr))
                {
                    // si existe compruebo si no ha sido eliminado por el GC
                    if (wr.IsAlive) return (SFML.Graphics.Texture)wr.Target;
                    // si ha sido eliminado... elimino la key
                    _textureMap.Remove(id);
                }
                
                return null;
            }
        }
    }
}
