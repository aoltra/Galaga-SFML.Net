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
    /// Encapsula un nodo del grafo de escena
    /// </summary>
    /// <remarks>
    /// Se utiliza un List y no una LinkedList ya que se va a utilizar
    /// un acceso indexado
    /// </remarks>
    class SceneNode : SFML.Graphics.Transformable, SFML.Graphics.Drawable
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
        public Boolean IsLeaf { get { return _children.Count == 0; } }

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

        private List<SceneNode> _children = null;     // lista de hijos

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneNode() 
        {
            Parent = null;
            _children = new List<SceneNode>();
        }

        /// <summary>
        /// Añade un nodo hijo al nodo
        /// </summary>
        /// <param name="scNode">Nodo a aña´dir</param>
        public void AddChild(SceneNode scNode) {
            scNode.Parent = this;
            _children.Add(scNode);
        }

        /// <summary>
        /// Quita el nodo de la lista de nodos hijo
        /// </summary>
        /// <param name="scNode">Nodo a quitar</param>
        /// <remarks>
        /// Se quita de la lista pero no se elimina de memoria
        /// </remarks>
        public void RemoveChild(SceneNode scNode) {
            bool encontrado = _children.Remove(scNode);
            Debug.Assert(encontrado);

            scNode.Parent = null;
        }

        #region Funciones relativas al dibujado
        /// <summary>
        /// Dibuja el nodo
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        /// <remarks>
        /// Esta función ya hace lo que tiene que hace y no necesita ser particularizada en ningún nodo. Para particularizar
        /// el dibujo de un nodo se utiliza DrawCurrent
        /// </remarks>
        public void Draw(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        { 
            // almaceno la combinación de la transformación de el padre y de este nodo (hay que recordar que la 
            // de este nodo es relativa al padre). Lo que obtengo es la transformación global del nodo
            // esto funciona bien ya que rednerStates y Transform son struct y no una clase!! se hace una copia, 
            // no se crea otra referencia
            rs.Transform.Combine(Transform);

            DrawCurrent(rt, rs);                // Dibujo el nodo actual
            DrawChildren(rt,rs);
        }

        /// <summary>
        /// Dibuja el nodo actual.
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        /// <remarks>
        /// Se declara virtual para que pueda ser modificada en las clases hijas sobreescibiendose (override)
        /// </remarks>
        protected virtual void DrawCurrent(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        { 
            // no hace nada. Es genérica, no sabe como dibujar nada en particular
            // si se llama al DrawCurrent de una entidad que no tenga esa función declarada 
            // se usará esta (que no hará nada)
        }

        /// <summary>
        /// Dibuja todos los nodos hijos
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        private void DrawChildren(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs) {

            foreach (SceneNode sc in _children) {
                sc.Draw(rt, rs);
            }
        }
        #endregion

        #region Funciones relativas a la actualización
        /// <summary>
        /// Actualiza el nodo
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        public void Update(SFML.System.Time dt)
        {
            UpdateCurrent(dt);
            UpdateChildren(dt);
        }

        /// <summary>
        /// Actualiza el nodo
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        /// <remarks>
        /// Se declara virtual para que pueda ser modificada en las clases hijas sobreescibiendose (override)
        /// </remarks>
        protected virtual void UpdateCurrent(SFML.System.Time dt)
        {
            // no hace nada. Es genérica, no sabe como actualizar nada en particular
            // si se llama al UpdateCurrent de una entidad que no tenga esa función declarada 
            // se usará esta (que no hará nada)
        }

        /// <summary>
        /// Actualiza todos los nodos hijos
        /// </summary>
        /// <param name="dt">Incremento de tiempo desde la última actualización</param>
        private void UpdateChildren(SFML.System.Time dt)
        {
            foreach (SceneNode sc in _children)
            {
                sc.Update(dt);
            }
        }
        #endregion

        /// <summary>
        /// Devuuelve la Transformacion necesaria para pasar a coordenadas globales (del mundo)
        /// </summary>
        public SFML.Graphics.Transform WorldTrasform 
        {
            get 
            { 
                SFML.Graphics.Transform transform = SFML.Graphics.Transform.Identity;

                for (SceneNode node = this; node != null; node = node.Parent) 
                    transform = node.Transform  * transform;
                
                return transform;
            }
        }

        /// <summary>
        /// Devuuelve la Transformacion necesaria para pasar a coordenadas globales (del mundo)
        /// </summary>
        public SFML.System.Vector2f WorldPosition
        {
            get
            {
                // OJO! la propiedad Position me da la posición del (0,0) del nodo con respecto al sistema
                // de referencia de su nodo padre. Ese (0,0) por defecto es la esquina superior derecha del nodo
                // al hacer el calculo de la transformación global tengo ya en cuenta la transformación del nodo en cuestión
                // luego el punto que traslado es el origen (0,0).. esté donde esté
                return WorldTrasform * new SFML.System.Vector2f();
            }

        }
    }
}
