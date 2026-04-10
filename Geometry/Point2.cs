using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    public readonly struct Point2 : IFormattable
    {
        public Point2(Vector2 vector, float scalar) : this()
        {
            Vector = vector;
            Scalar = scalar;
            Position = vector / scalar;
        }

        public Vector2 Vector { get; }
        public float Scalar { get; }
        public Vector2 Position { get; }

        public float DistanceTo(Point2 other) => Vector2.Distance(Position, other.Position);

        public static implicit operator Vector2(Point2 point) => point.Position;
        public static implicit operator Point2(Vector2 vector) => new Point2(vector, 1);

        public static Vector2 operator +(Point2 A, Point2 B) => A.Position + B.Position;
        public static Vector2 operator -(Point2 A, Point2 B) => A.Position - B.Position;
        public static Point2 operator +(Point2 A, Vector2 v) => new Point2( A.Vector + A.Scalar * v, A.Scalar);
        public static Point2 operator -(Point2 A, Vector2 v) => new Point2( A.Vector - A.Scalar * v, A.Scalar);

        public string ToString(string format, IFormatProvider formatProvider)
           => $"Point({Vector.ToString(format, formatProvider)}, {Scalar.ToString(format, formatProvider)})";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g");

    }
}
