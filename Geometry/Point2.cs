using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    using static LinearAlgebra;

    public readonly struct Point2 : 
        IGeometry, 
        IFormattable
    {
        #region Factory
        public Point2(Vector2 vector, float scalar) : this()
        {
            if (scalar != 0)
            {
                Vector = vector / scalar;
                Scalar = 1;
            }
            else
            {
                Vector = vector;
                Scalar = scalar;
            }
            Position = vector / scalar;
        }

        public static Point2 Zeros { get; } = new Point2(Vector2.Zero, 0);
        public static Point2 Origin { get; } = new Point2(Vector2.Zero, 1);
        public static Point2 InfX { get; } = new Point2(Vector2.UnitX, 0);
        public static Point2 InfY { get; } = new Point2(Vector2.UnitY, 0);
        public static explicit operator Vector2(Point2 point) => point.Position;
        public static implicit operator Point2(Vector2 vector) => new Point2(vector, 1); 
        #endregion

        #region Properties
        public Vector2 Vector { get; }
        public float Scalar { get; }
        public Vector2 Position { get; }
        #endregion

        #region Algebra
        public static Point2 operator *(float factor, Point2 A) => new Point2(factor*A.Vector, factor*A.Scalar);
        public static Point2 operator -(Point2 A) => new Point2(-A.Vector, -A.Scalar);
        public static Point2 operator +(Point2 A, Point2 B) => new Point2(A.Vector + B.Vector, A.Scalar + B.Scalar);
        public static Point2 operator -(Point2 A, Point2 B) => new Point2(A.Vector - B.Vector, A.Scalar - B.Scalar);
        public static Point2 operator +(Point2 A, Vector2 delta) => Offset(A, delta);
        public static Point2 operator -(Point2 A, Vector2 delta) => Offset(A, -delta);

        #endregion

        #region Formatting
        public string ToString(string format, IFormatProvider formatProvider)
           => $"Point({Vector.ToString(format, formatProvider)}, {Scalar.ToString(format, formatProvider)})";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g"); 
        #endregion

        #region Geometry
        public float DistanceTo(Point2 other) => Vector2.Distance(Position, other.Position);
        /// <summary>
        /// Finds the point where two lines meet.
        /// </summary>
        /// <param name="A">The first line.</param>
        /// <param name="B">The second line.</param>
        /// <returns>A point</returns>
        public static Point2 Meet(Line2 A, Line2 B)
        {
            return new Point2(
                Cross(A.Vector, B.Scalar) + Cross(A.Scalar, B.Vector),
                Cross(A.Vector, B.Vector));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Span(Point2 A, Point2 B) => A.Position - B.Position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2 Offset(Point2 A, Vector2 delta) => new Point2(A.Vector + A.Scalar * delta, A.Scalar);
        public bool Contains(Point2 point) => Span(this, point).Length() < Geometry.Tolerance;
        public Contact GetClosestPoint(Point2 other) => new Contact(this, other);
        #endregion

    }
}
