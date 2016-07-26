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
        public abstract bool IsCircle { get; }

        public abstract void Draw(SFML.Graphics.RenderTarget rt, SFML.Graphics.RenderStates rs);
    }


    /// <summary>
    /// Implementa un collider rectangular
    /// </summary>
    public class ColliderRect : Collider
    {
        // rectangulo que define el collider
        public FloatRect Rectangle { get; private set; }

        private SFML.System.Vector2f [] p;
        private SFML.System.Vector2f axis;

        public SFML.System.Vector2f this[int index] 
        {
            get 
            {
                if (index < 0) index = 0;
                if (index > 3) index = 3;
                return p[index]; 
            }
        }

        /// <summary>
        /// Devuelve si el Collider es un circulo (false)
        /// </summary>
        public override bool IsCircle { get { return false; } }

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

            p = new SFML.System.Vector2f[4];
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
        /// <summary>
        /// Devuelve si hay intersección entre dos rectangulos girados
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>true si hay intersección, false en caso contrario</returns>
        public bool Intersects(ColliderRect other)
        {
            if (!AxisIntersection(this,other)) 
                return false;

            if (!AxisIntersection(other, this))
                return false;

            return true;
        }

        /// <summary>
        /// Devuelve si hay intersección entre ejes
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns>true si la hay, false en caso contrario</returns>
        /// <remarks>
        /// Se basa en el teorema de los ejes. Más info en http://www.gamedev.net/page/resources/_/technical/game-programming/2d-rotated-rectangle-collision-r2604
        /// </remarks>
        private bool AxisIntersection(ColliderRect col1,ColliderRect col2) 
        {
            float pend, denominador = 1;
            float max1, min1, max2, min2;
            float proyX, proyY, comp;

            // ejes del rectángulo 1
            for (int numAxis = 0; numAxis < 2; numAxis++)
            {
                max1 = -999999.9f; min1 = 999999.9f; max2 = -999999.9f; min2 = 999999.9f;
                axis = col1[numAxis] - col1[numAxis + 1];
                denominador = (axis.X * axis.X + axis.Y * axis.Y);

                // vertices del rectángulo 1 (sólo dos ya que los otros dos son perpendiculares)
                for (int numVertex = 0; numVertex < 2; numVertex++)
                {
                    pend = (col1[numVertex + numAxis].X * axis.X + col1[numVertex + numAxis].Y * axis.Y) / denominador;
                    proyX = col1[numVertex + numAxis].X * pend;
                    proyY = col1[numVertex + numAxis].Y * pend;

                    comp = proyX * axis.X + proyY * axis.Y;

                    max1 = Math.Max(max1, comp);
                    min1 = Math.Min(min1, comp);
                }

                // vertices del rectangulo 2 
                for (int numVertex = 0; numVertex < 4; numVertex++)
                {
                    pend = (col2[numVertex].X * axis.X + col2[numVertex].Y * axis.Y) / denominador;
                    proyX = col2[numVertex].X * pend;
                    proyY = col2[numVertex].Y * pend;

                    comp = proyX * axis.X + proyY * axis.Y;

                    max2 = Math.Max(max2, comp);
                    min2 = Math.Min(min2, comp);
                }

                if (min2 > max1 || max2 < min1) return false;
            }

            return true;
        }

        /// <summary>
        /// Actualiza el estado de los puntos que conforman el collider
        /// </summary>
        /// <param name="trans">Matriz de trnasformacion</param>
        public void Update(Transform trans)
        {
            p[0] = trans.TransformPoint(Rectangle.Left, Rectangle.Left);
            p[1] = trans.TransformPoint(Rectangle.Left + Rectangle.Width, Rectangle.Top);
            p[2] = trans.TransformPoint(Rectangle.Left + Rectangle.Width, Rectangle.Top + Rectangle.Height);
            p[3] = trans.TransformPoint(Rectangle.Left, Rectangle.Top + Rectangle.Height);
        }
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