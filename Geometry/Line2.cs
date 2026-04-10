using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    public readonly struct Line2 : IFormattable
    {
        public Line2(Vector2 v, float s) : this()
        {
            Vector = v;
            Scalar = s;
            Tangent = Vector2.Normalize(new Vector2(v.Y, -v.X));
            Normal = Vector2.Normalize(v);
        }

        public Vector2 Vector { get; }
        public float Scalar { get; }
        public Vector2 Tangent { get; }
        public Vector2 Normal { get; }

        public Point2 PointAlong(float distance)
        {
            float v_sq = Vector.LengthSquared();
            float t = distance * LinearAlgebra.Sqrt(v_sq);
            return new Point2(-Vector * Scalar + LinearAlgebra.Cross(t, Vector), v_sq);
        }
        /// <summary>
        /// Projects a point onto the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="point">The point.</param>
        public Point2 Project(Point2 point)
        {
            float t = LinearAlgebra.Cross(Vector, point.Vector);
            return new Point2(
                -Vector * Scalar * point.Scalar + LinearAlgebra.Cross(t, Vector),
                Vector.LengthSquared() * point.Scalar);
        }

        public float DistanceTo(Point2 point)
        {
            return LinearAlgebra.Dot(this, point)
                / (point.Scalar * Vector.Length());
        }

        public bool Contains(Point2 point)
        {
            float d_sign = DistanceTo(point);

            return Math.Abs(d_sign) <= 1e-6f;
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => $"Line[{Vector.X.ToString(format, formatProvider)}x+{Vector.Y.ToString(format, formatProvider)}y+{Scalar.ToString(format, formatProvider)}=0]";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g");


    }
}
