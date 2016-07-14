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

using SFML.Graphics;
using SFML.System;

using edu.CiclosFormativos.Games.DIDAM.Scenes;

namespace edu.CiclosFormativos.DAM.DI.Galaga.Entities
{
    /// <summary>
    /// Encapsula un node de escena que gestiona el movimiento de sus hijos
    /// </summary>
    /// <remarks>
    /// No interfiere con el resto de entidades 
    /// </remarks>
    class TransformEntity : Entity
    {
        /// <summary>
        /// Asigna o devuelve si la entidad se ve en pantalla
        /// </summary>
        public Boolean Visible { get; set; }

        protected CircleShape _shape;	         // círculo que muestra la entidad

        /// <summary>
        /// Constructor básico. Inicializa a cero la velocidad
        /// </summary>
        public TransformEntity() 
            : this(new Vector2f(0,0))
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="velocity">Velocidad de la entidad</param>
        public TransformEntity(Vector2f velocity)
            : base(velocity)
        {
            Visible = false;            // por defecto no se ve
            _shape = new CircleShape(4.0f);
            _shape.FillColor = Color.Red;

            FloatRect bounds = _shape.GetLocalBounds();
            _shape.Origin = new SFML.System.Vector2f(bounds.Width / 2f, bounds.Height / 2f);
        }

        /// <summary>
        /// Dibuja (sí <see cref="Visible"/> es true) la forma 
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        protected sealed override void DrawCurrent(RenderTarget rt, RenderStates rs)
        {
            if (Visible) rt.Draw(_shape, rs);
        }

    }
}
