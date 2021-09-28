using System;
using System.ComponentModel;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Triangle : IFormattable
    {
        public Triangle(Vector2 a, Vector2 b, Vector2 c) : this()
        {
            A = a;
            B = b;
            C = c;

            Area = LinearAlgebra.Area(a, b, c);
            Centroid = (A + B + C) / 3;
            AreaMoment = LinearAlgebra.AreaMoment(a, b, c);
        }

        public Vector2 A { get; }
        public Vector2 B { get; }
        public Vector2 C { get; }

        [Browsable(false)] public Side AB { get => new Side(A, B); }
        [Browsable(false)] public Side BC { get => new Side(B, C); }
        [Browsable(false)] public Side CA { get => new Side(C, A); }

        [Browsable(false)] public float Area { get; }
        [Browsable(false)] public Vector2 Centroid { get; }
        [Browsable(false)] public float AreaMoment { get; }

        /// <summary>
        /// Gets the barycentric coordinates of a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The triplet of weights such that <code>P=w_A*A + w_B*B + w_C*C</code></returns>
        public (float w_A, float w_B, float w_C) GetBaryCoords(Vector2 point)
        {
            float d = LinearAlgebra.Area(A, B, C);

            return (LinearAlgebra.Area(point, B, C) / d,
                    LinearAlgebra.Area(A, point, C) / d,
                    LinearAlgebra.Area(A, B, point) / d);
        }

        /// <summary>
        /// Determines a point is inside this triangle.
        /// </summary>
        /// <param name="point">The point.</param>
        public bool Contains(Vector2 point)
        {
            (float w_A, float w_B, float w_C) = GetBaryCoords(point);

            return w_A >= 0 && w_A <= 1
                && w_B >= 0 && w_B <= 1
                && w_C >= 0 && w_C <= 1;
        }

        public Triangle Offset(Vector2 delta)
            => new Triangle(A + delta, B + delta, C + delta);
        public Triangle Rotate(Vector2 pivot, float angle) 
            => new Triangle(
                LinearAlgebra.RotateAbout(A, pivot, angle),
                LinearAlgebra.RotateAbout(B, pivot, angle),
                LinearAlgebra.RotateAbout(C, pivot, angle));

        public static Triangle operator +(Triangle triangle, Vector2 delta)
            => triangle.Offset(delta);
        public static Triangle operator -(Triangle triangle, Vector2 delta)
            => triangle.Offset(-delta);

        public string ToString(string format, IFormatProvider formatProvider)
            => $"{{{A.ToString(format, formatProvider)},{B.ToString(format, formatProvider)},{C.ToString(format, formatProvider)}}}";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g");

    }
}
