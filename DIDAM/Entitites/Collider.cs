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
using System.Runtime.InteropServices;
using System.Diagnostics;

using SFML.Graphics;

namespace edu.CiclosFormativos.Games.DIDAM.Entities
{
    /// <summary>
    /// Interface a implementar por las entidades que pueden colisionar
    /// </summary>
    public interface ICollider
    {
        Collider GetCollider();
    }

    /// <summary>
    /// Implementa un collider
    /// </summary>
    public abstract class Collider 
    {
        /// <summary>
        /// Devuelve si el Collider es un circulo. 
        /// </summary>
        public virtual bool? IsCircle { get { return null; } }

        public abstract void Draw(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs);
    }


    /// <summary>
    /// Implementa un collider rectangular
    /// </summary>
    public class ColliderRect : Collider
    {
        // rectangulo que define el collider
        private FloatRect Rectangle;

        /// <summary>
        /// Devuelve si el Collider es un circulo (false)
        /// </summary>
        new public bool? IsCircle { get { return false; } }

#if DEBUG
        // forma que representa el collider. Para dibujarla en DEBUG
        private SFML.Graphics.RectangleShape shape = new SFML.Graphics.RectangleShape();

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="color"></param>
        public ColliderRect(FloatRect rect, SFML.System.Vector2f origin, SFML.Graphics.Color color)
        {
            shape.FillColor = SFML.Graphics.Color.Transparent;
            shape.OutlineColor = color;
            shape.OutlineThickness = 1f;
            shape.Origin = origin;
            shape.Size = new SFML.System.Vector2f(rect.Width, rect.Height);

            Rectangle = rect;
        }

        /// <summary>
        /// Constructor por defecto. Color verde
        /// </summary>
        public ColliderRect(FloatRect rect,SFML.System.Vector2f origin) : this (rect, origin, SFML.Graphics.Color.Green) { }

        /// <summary>
        /// Dibuja el collider
        /// </summary>
        /// <param name="rt">Donde se va a dibujar. Suele ser casi siempre una renderWindow, aunque podría ser una renderTexture</param>
        /// <param name="rs">Estados usados para dibujar</param>
        /// <remarks>
        /// Se usa en depuraciones
        /// </remarks>
        public override void Draw(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs)
        {
            rt.Draw(shape, rs);
        }
#else
        /// <summary>
        /// Constructor.
        /// </summary>
        public ColliderRect(FloatRect rect) {  Rectangle = rect; }
#endif

    }

    /// <summary>
    /// CircleRect es una utilidad para manipular circulos
    /// pensando en la gestión de colliders
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CircleRect : IEquatable<CircleRect> 
    {
        /// <summary>
        /// Radio del círculo
        /// </summary>
        public float Radius;

        /// <summary>
        /// Coordenada X del centro del círculo
        /// </summary>        
        public float CenterX;

        /// <summary>
        /// Coordenada Y del centro del círculo
        /// </summary>
        public float CenterY;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="centerX">Coordenada X del centro del círculo</param>
        /// <param name="centerY">Coordenada Y del centro del círculo</param>
        /// <param name="radius">Radio del círculo</param>
        public CircleRect(float centerX, float centerY, float radius)
        {
            CenterX = centerX;
            CenterY = centerY;

            Debug.Assert(radius<0, "El radio del CircleRect no puede ser negativo");

            Radius  = radius;
        }

        /// <summary>
        /// Devuelve si un punto está dentro del área del círculo
        /// </summary>
        /// <param name="x">Coordenada X del punto a comprobar</param>
        /// <param name="y">Coordenada Y del punto a comprobar</param>
        /// <returns></returns>
        public bool Contains(float x, float y)
        {
            float dist = Utilities.VectorUtilities.VectorLength(x-CenterX,y-CenterY);

            return (dist <= Radius);
        }

        /// <summary>
        /// Comprueba si dos circulos se tocan entre sí
        /// </summary>
        /// <param name="rect">Circulo con el que comparar</param>
        /// <returns>true si los círculos se solapan, false en caso contrario</returns>
        public bool Intersects(CircleRect rect)
        {
            float distCenter = Utilities.VectorUtilities.VectorLength(rect.CenterX-CenterX,rect.CenterY-CenterY);
                
            return (distCenter<=(rect.Radius+Radius));
        }

        /// <summary>
        /// Comprueba si un círculo y un rectángulo se tocan entre sí
        /// </summary>
        /// <param name="rect">Círculo con el que comparar</param>
        /// <returns>true si los círculos se solapan, false en caso contrario</returns>
        public bool Intersects(FloatRect rect)
        {
            return (Contains(rect.Left, rect.Top) || Contains(rect.Left + rect.Width, rect.Top) 
                || Contains(rect.Left,rect.Top + rect.Height) || Contains(rect.Left + rect.Width,rect.Top + rect.Height));
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Devuelve una cadena describiendo el objeto
        /// </summary>
        /// <returns>Descripción del objeto</returns>
        ////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return "[CircleRect]" +
                    " Radius(" + Radius + ")" +
                    " CenterX(" + CenterX + ")" +
                    " CenterY(" + CenterY + ")";
        }
			
		////////////////////////////////////////////////////////////
        /// <summary>
        /// Compara los dos objetos CircleRect
        /// </summary>
        /// <param name="obj">Objeto a comprobar</param>
        /// <returns>true si los objetos son el mismo</returns>
        ////////////////////////////////////////////////////////////
        public override bool Equals(object obj)
        {
            return (obj is CircleRect) && Equals((CircleRect)obj);
        }
            
        ///////////////////////////////////////////////////////////
        /// <summary>
        /// Compara dos círculos y comprueba si son iguales
        /// </summary>
        /// <param name="other">Círculo a comprobar</param>
        /// <returns>true si los círculos son iguales, false en caso contrario</returns>
        ////////////////////////////////////////////////////////////
        public bool Equals(CircleRect other)
        {
            return (CenterX == other.CenterX) &&
                    (CenterY == other.CenterY) &&
                    (Radius == other.Radius);
        }
            
        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Devuelve un entero que describe el objeto
        /// </summary>
        /// <returns>Descripción mediante un objeto del puntero</returns>
        ////////////////////////////////////////////////////////////
        public override int GetHashCode()
        {
            return unchecked((int)((uint)Radius ^
                    (((uint)CenterX << 13) | ((uint)CenterX >> 19)) ^
                    (((uint)CenterY << 26) | ((uint)CenterY >>  6))));
        }
			
        /// <summary>
        /// Sobrecarga del operador ==. Comprueba si dos CircleRect son iguales
        /// </summary>
        /// <param name="r1">Primer círculo</param>
        /// <param name="r2">Segundo círculo</param>
        /// <returns>true si son iguales, false en caso contrario</returns>
        public static bool operator ==(CircleRect r1, CircleRect r2)
        {
            return r1.Equals(r2);
        }

        /// <summary>
        /// Sobrecarga del operador !=. Comprueba si dos CircleRect son diferentes
        /// </summary>
        /// <param name="r1">Primer círculo</param>
        /// <param name="r2">Segundo círculo</param>
        /// <returns>false si son iguales, true en caso contrario</returns>
        public static bool operator !=(CircleRect r1, CircleRect r2)
        {
            return !r1.Equals(r2);
        }
	    
    }
}