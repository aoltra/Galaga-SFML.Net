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
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula la funcionalidad de una piscina de objetos
    /// Permite la creación de varios objetos del mismo tipo y su posterior reciclaje 
    /// una vez han sido utilizados.
    /// No puede ser instanciada, sólo hace funciones de clase base
    /// </summary>
    /// <typeparam name="T">Tipo del objeto del que se crea la piscina</typeparam>
    abstract class ObjectPool<T>
    {
        // variables miembro
        private List<T> _items;                     // lista de objetos

        /// <summary>
        /// Obtiene el tamaño del bloque de crecimiento. 
        /// </summary>
	    public int Growth { get; private set; }

        /// <summary>
        /// Devuelve el tamaño máximo de objeto que puede contener la piscina
        /// </summary>
        public int PoolSize { get; private set;  }

        /// <summary>
        /// Devuelve el número de objeto sin reciclar.
        /// </summary>
        /// <remarks>
        /// O lo que es lo mismo el número de huecos que tenemos en nuestra piscina, o también
        /// el número de objetos de la piscina que están en uso ahora mismo
        /// </remarks>
	    public int UnrecycledObjectCount { get; private set; }

        /// <summary>
        /// Constructor básico. Inicializa el tamaño inicial de la piscina a cero
        /// </summary>
        public ObjectPool(): this(0) { }

        /// <summary>
        /// Constructor. Crea la piscina con n objetos de partida. Cada vez que se quiera incrementar su tamaño
        /// se hará de uno en uno
        /// </summary>
        /// <param name="initSize">Tamaño inicial de la piscina</param>
	    public ObjectPool(int initSize) : 
            this(initSize, 1) { }

        /// <summary>
        /// Constructor. Crea la piscina con n objetos de partida, incrementando su tamaño cada vez en growth
        /// El tamaño máximo de la piscina es el valor máximo qu epuede tomar int
        /// </summary>
        /// <param name="initSize">Tamaño inicial de la piscina</param>
        /// <param name="growth">Tamaño del bloque de crecimiento</param>
	    public ObjectPool(int initSize, int growth) :
            this(initSize, growth, int.MaxValue)
        {
	    }

        /// <summary>
        /// Constructor. Crea la piscina con n objetos de partida, un tamaño de crecimiento growth y un tamaño máximo 
        /// de maxSize
        /// </summary>
        /// <param name="initSize">Tamaño inicial de la piscina</param>
        /// <param name="growth">Tamaño del bloque de crecimiento</param>
        /// <param name="maxSize">Tamaño máximo de la piscina</param>
	    public ObjectPool(int initSize, int growth, int maxSize)
        {
            Debug.Assert(growth > 0, "El tamaño de la piscina tiene que ser mayor de cero");
		
            Debug.Assert(maxSize >= 0, "El tamaña máximo tiene que ser mayor que cero");
		   
		    Growth = growth;
            PoolSize = maxSize;
		    _items = new List<T>(initSize);

		    if(initSize > 0) {
                this.batchAllocatePoolObjects(initSize);
		    }
	    }

        /// <summary>
        /// Instancia un conjunto de objetos
        /// </summary>
        /// <param name="count">Número de objetos a instanciar</param>
        /// <remarks>
        /// En C# no es necesario sincronizala ya que lo es por defecto. En Java sí
        /// Al sincronizarla bloqueamos el método para que ningún otro hilo le pueda 
        /// interrumpir hasta que acabe.
        /// </remarks>
        private void batchAllocatePoolObjects(int count) 
        {
		    List<T> availableItems = _items;

		    int tmpCount = PoolSize - _items.Count;

		    if(count < tmpCount) tmpCount = count;
		    
		    for(int i = tmpCount - 1; i >= 0; i--) {
                _items.Add(AllocObject());
		    }
	    }

        /// <summary>
        /// Crea un objeto del tipo T. Ha de ser creado en cada una de las piscinas especificas
        /// </summary>
        /// <returns>El objeto de tipo T reservado</returns>
        protected abstract T AllocObject();

        /// <summary>
        /// Devuelve un objeto de la piscina
        /// </summary>
        /// <returns></returns>
        public T GetObject() {
		    
            T item;

            // si quedan objetos en la piscina por usar
		    if(_items.Count > 0) 
            {
                // obtengo el ultimo elemento del list y lo elimino
                item = _items[_items.Count - 1];
			    _items.RemoveAt(_items.Count - 1);
		    } 
            else 
            {
                // reserva 1
			    if(Growth == 1 || PoolSize == 0) 
				    item = AllocObject();
                else // reserva más de uno
                {
                    batchAllocatePoolObjects(Growth);
                    item = _items[_items.Count - 1];
                    _items.RemoveAt(_items.Count - 1);
			    }
			    
				Debug.WriteLine("No hay más objetos <" + item.GetType().Name +"> en la piscina. Se crean " + Growth + " mas.");	    
		    }

            HandleGetObject(item);

		    UnrecycledObjectCount++;
		    return item;
	    }

        /// <summary>
        /// Recicla un item
        /// </summary>
        /// <param name="item">item a reciclar</param>
	    public void RecycleObject(T item) 
        {
		    if(item == null) {
                throw new ObjectoPoolException("El Objeto null no puede ser reciclado");
		    }

		    HandleRecycleObject(item);

		    if(_items.Count < PoolSize) {
			    _items.Add(item);
		    }

            // un hueco menos
		    UnrecycledObjectCount--;

		    if(UnrecycledObjectCount < 0) {
			    throw new ObjectoPoolException("Se están reciclando más objetos de los que se crearon");
		    }
	    }

        /// <summary>
        /// Permite personalizar el reciclado del objeto. 
        /// </summary>
        /// <param name="item">objeto reciclado</param>
        protected void HandleRecycleObject(T item) 
        {  
            // Está vacía en principio ya que en la clase base no hace nada
            // pero en las hijas permite que se postprocesar el objeto reciclado
        }

        /// <summary>
        /// Permite personalizar la obtención del objeto. 
        /// </summary>
        /// <param name="item">objeto obtenido</param>
        protected void HandleGetObject(T item)
        {
            // Está vacía en principio ya que en la clase base no hace nada
            // pero en las hijas permite que se postprocesar el objeto obtenido
        }

    }

    /// <summary>
    /// Encapsula una excepción generada por la piscina de objetos
    /// </summary>
    public class ObjectoPoolException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectoPoolException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Mensaje de la excepción</param>
        public ObjectoPoolException(string message)
            : base("[ObjectPool] >> " + message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Mensaje de la excepcion</param>
        /// <param name="inner">Excepcion en la que se basa</param>
        public ObjectoPoolException(string message, Exception inner)
            : base("[ObjectPool] >> " + message, inner)
        {
        }
    }
}
