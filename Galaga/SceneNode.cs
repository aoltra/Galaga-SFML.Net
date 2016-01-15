using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace edu.CiclosFormativos.DAM.DI.Galaga
{
    /// <summary>
    /// Encapsula un nodo del grafo de escena
    /// </summary>
    /// <remarks>
    /// Se utiliza un List y no una LinkedList ya que se va a utilizar
    /// un acceso indexado
    /// </remarks>
    class SceneNode : SFML.Graphics.Transformable
    {
        /// <summary>
        /// Asigna o devuelve el nodo padre
        /// </summary>
        public SceneNode Parent { get; set; }

        /// <summary>
        /// Devuelve si el nodo es el nodo raíz
        /// </summary>
        public Boolean IsRoot { get { return Parent == null; } }

        /// <summary>
        /// Devuelve si el nodo es una hoja
        /// </summary>
        public Boolean IsLeaf { get { return children.Count == 0; } }

        /// <summary>
        /// Devuelve el nivel del nodo
        /// </summary>
        public int Level
        {
            get
            {
                if (this.IsRoot) return 0;
                return Parent.Level + 1;
            }
        }

        private List<SceneNode> children = null;     // lista de hijos

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneNode() {
            children = new List<SceneNode>();
        }

        /// <summary>
        /// Añade un nodo al árbol
        /// </summary>
        /// <param name="scNode"></param>
        public void AddChild(SceneNode scNode) {
            scNode.Parent = this;
            children.Add(scNode);
        }

        /// <summary>
        /// Quita el nodo de la lista de nodos
        /// </summary>
        /// <param name="scNode">Nodo a eliminar</param>
        public void RemoveChild(SceneNode scNode) {
            bool encontrado = children.Remove(scNode);
            Debug.Assert(encontrado);

            scNode.Parent = null;
        }
        
    }
}
