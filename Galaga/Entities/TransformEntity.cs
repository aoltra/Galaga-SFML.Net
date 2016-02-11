using System;
using System.Collections.Generic;
using System.Text;

using SFML.Graphics;
using SFML.System;

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
        /// Dibuja (sí <paramref name="visible"/> es true) la forma 
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        protected sealed override void DrawCurrent(RenderTarget rt, RenderStates rs)
        {
            if (Visible) rt.Draw(_shape,rs);
        }

    }
}
