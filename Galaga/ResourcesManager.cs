using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using System.Diagnostics;

//using SFML.Graphics;
//using SFML;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Resources
{
    /// <summary>
    /// Gestiona la carga de todo tipo de recursos desde ficheros XML
    /// </summary>
    public class ResourcesManager
    {
        /// <summary>
        /// Separador de secciones. Se necesita para definir el acceso a un recurso dentro de una sección
        /// </summary>
        public const String SECTION_SEPARATOR = ":";

        /// <summary>
        /// Delegado que define como tiene que ser implementadas las funciones de carga de recusos concretos
        /// </summary>
        /// <param name="e">XElement que define la información para la lectura de un tipo de recurso</param>
        /// <returns>Recurso indicado</returns>
        public delegate object LoadResourceTypeDelegate(XElement e);   

        // los recursos serán almacenados para su gestión en un diccionario
        private Dictionary<String, Resource> _resourcesMap = new Dictionary<String, Resource>();
        // diccionario que almacena la posibles funciones de carga
        private Dictionary<String, LoadResourceTypeDelegate> _loadFuncMap = new Dictionary<String, LoadResourceTypeDelegate>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input">URI del fichero XML</param>
        public ResourcesManager(String input = null)
        {
            if (input == null)
                return;

            XDocument doc = XDocument.Load(input);
            Load(doc.Element("resx"));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input">Secuencia de bits del fichero XML</param>
        public ResourcesManager(System.IO.Stream input)
        {
            if (input == null)
                return;

            XDocument doc = XDocument.Load(input);
            Load(doc.Element("resx"));
        }

        /// <summary>
        /// Índice que permite acceder a un id determinado
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception">No se ha registrado una función para la carga de ese tipo de recurso</exception>
        /// <returns>objeto recurso seleccionado</returns>
        public object this[String id]
        {
            get
            {
                Resource res;                               // recurso del mapa de recursos devuelto
                LoadResourceTypeDelegate loadFunc;          // funcion de carga elegida para la carga de un tipo concreto de recurso

                try
                {
                    // compruebo si existe el id, en caso contrario devolverá null
                    // el uso de TryGetValue resulta mas eficiente que la comprobación de la excepción KeyNotFoundException
                    if (_resourcesMap.TryGetValue(id, out res))
                    {
                        object objectResource = res.Weakref.Target;

                        if (objectResource == null) // si es null es que no aun no está cargado o se ha descargado
                        {
                            // hay que cargarlo
                            // obtengo el tipo del recurso a partir del nombre del elemento XElement
                            String type = res.Element.Name.ToString();

                            if (_loadFuncMap.TryGetValue(type, out loadFunc))
                                objectResource = loadFunc(res.Element);
                            else
                                throw new ResourcesManagerException("No se ha definido una función para la carga del tipo de recurso " +
                                     type + ". No se ha cargado el recurso '" + res.Element + "'");
                        }

                        return objectResource;
                    }

                    return null;
                }
                catch (Exception ex) 
                {
                    throw new ResourcesManagerException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Registra funciones de carga de tipos de recurso par poder ser utilizadas cuando sea necesario cargar un recurso
        /// </summary>
        /// <param name="resourceType">Nombre del tipo de recurso al que se le asocia la función de carga. Hace las funciones de key</param>
        /// <param name="f">Función de carga</param>
        public void RegisterLoadFunction(String resourceType, LoadResourceTypeDelegate f)
        {
            try
            {
                _loadFuncMap.Add(resourceType, f);
            }
            catch (Exception ex)
            {
                throw new ResourcesManagerException(ex.Message);
            }
        }

        /// <summary>
        /// Carga recursos desde una nodo raíz del XML.
        /// </summary>
        /// <param name="el">Elemento XML a leer</param>
        /// <param name="section">Seccion en la que se encuentra</param>
        private void Load(XElement el, string section = null)
        {
            if (section != null)
                section += SECTION_SEPARATOR;
            else
                section = "";

            try
            {
                // repaso todos los elementos del nodo
                foreach (XElement element in el.Elements())
                {
                    // obtengo el valor del atributo id
                    XAttribute id = element.Attribute("id");
                    if (id != null)
                    {
                        // si es una sección sigo leyendo el árbol.. hasta encontrar un recurso
                        if (element.Name == "section")
                            Load(element, id.Value);
                        else
                        {
                            // la key ya existe
                            Debug.Assert(!_resourcesMap.ContainsKey(section + id.Value), "Key en mapa de recursos repetida");
                            _resourcesMap.Add(section + id.Value, new Resource(element));
                        }
                        
                    }
                }
            }
            catch (Exception ex) 
            {
                throw new ResourcesManagerException(ex.Message);
            }
        }
    
        /// <summary>
        /// Encapsula un recurso en el mapa de recursos
        /// </summary>
        private class Resource
        {
            public WeakReference Weakref { get; private set; }
            public XElement Element { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="el">elemento xml</param>
            public Resource(XElement el)
            {
                Element = el;
                Weakref = new WeakReference(null, false);
            }
        }
    }

    /// <summary>
    /// Encapsula una excepción generada por el Gestor de Recursos
    /// </summary>
    public class ResourcesManagerException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourcesManagerException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Mensaje de la excepcion</param>
        public ResourcesManagerException(string message)
            : base("[Gestor de Recursos] >> " + message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Mensaje de la excepcion</param>
        /// <param name="inner">Excepcion en la que se bas</param>
        public ResourcesManagerException(string message, Exception inner)
            : base("[Gestor de Recursos] >> " + message, inner)
        {
        }
    }
}
