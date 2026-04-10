using System;
using System.ComponentModel;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Triangle : IFormattable
    {
        public Triangle(Point2 a, Point2 b, Point2 c) : this()
        {
            A = a;
            B = b;
            C = c;

            Area = LinearAlgebra.Area(a, b, c);
            Centroid = LinearAlgebra.Centroid(a, b, c);
            AreaMoment = LinearAlgebra.AreaMoment(a, b, c);
        }

        public Point2 A { get; }
        public Point2 B { get; }
        public Point2 C { get; }

        [Browsable(false)] public Side AB { get => new Side(A, B); }
        [Browsable(false)] public Side BC { get => new Side(B, C); }
        [Browsable(false)] public Side CA { get => new Side(C, A); }

        [Browsable(false)] public float Area { get; }
        [Browsable(false)] public Point2 Centroid { get; }
        [Browsable(false)] public float AreaMoment { get; }

        /// <summary>
        /// Gets the barycentric coordinates of a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The triplet of weights such that <code>P=w_A*A + w_B*B + w_C*C</code></returns>
        public (float w_A, float w_B, float w_C) GetBaryCoords(Point2 point)
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
        public bool Contains(Point2 point)
        {
            (float w_A, float w_B, float w_C) = GetBaryCoords(point);

            return w_A >= 0 && w_A <= 1
                && w_B >= 0 && w_B <= 1
                && w_C >= 0 && w_C <= 1;
        }

        public Triangle Offset(Vector2 delta)
            => new Triangle(A + delta, B + delta, C + delta);
        public Triangle Rotate(Point2 pivot, float angle) 
            => new Triangle(
                LinearAlgebra.RotateAbout(A, pivot, angle),
                LinearAlgebra.RotateAbout(B, pivot, angle),
                LinearAlgebra.RotateAbout(C, pivot, angle));

        public static Triangle operator +(Triangle triangle, Vector2 delta)
            => triangle.Offset(delta);
        public static Triangle operator -(Triangle triangle, Vector2 delta)
            => triangle.Offset(-delta);

        public Contact GetClosestPoints(Point2 other)
        {
            var n_A = new Contact(A, other);
            var n_B = new Contact(B, other);
            var n_C = new Contact(C, other);
            var n_AB = AB.GetClosestPoints(other);
            var n_BC = BC.GetClosestPoints(other);
            var n_CA = CA.GetClosestPoints(other);

            float d_A = n_A.Distance;
            float d_B = n_B.Distance;
            float d_C = n_C.Distance;
            float d_AB = n_AB.Distance;
            float d_BC = n_BC.Distance;
            float d_CA = n_CA.Distance;

            float d_min = Math.Min(
                Math.Min(d_A, Math.Min(d_B, d_C)),
                Math.Min(d_AB, Math.Min(d_BC, d_CA)));

            if (d_min == d_A) return n_A;
            if (d_min == d_B) return n_B;
            if (d_min == d_C) return n_C;
            if (d_min == d_AB) return n_AB;
            if (d_min == d_BC) return n_BC;
            if (d_min == d_CA) return n_CA;

            throw new ArgumentException("Invalid inputs");
        }

        public Contact GetClosestPoints(Side other)
        {
            var n_A = GetClosestPoints(other.A);
            var n_B = GetClosestPoints(other.B);
            var n_OA = other.GetClosestPoints(A);
            var n_OB = other.GetClosestPoints(B);
            var n_OC = other.GetClosestPoints(C);

            float d_A = n_A.Distance;
            float d_B = n_B.Distance;
            float d_OA = n_OA.Distance;
            float d_OB = n_OB.Distance;
            float d_OC = n_OC.Distance;

            float d_min = Math.Min(
                Math.Min(d_OC, Math.Min(d_OA, d_OB)),
                Math.Min(d_A, d_B));

            if (d_min == d_OA) return n_OA.Flip();
            if (d_min == d_OB) return n_OB.Flip();
            if (d_min == d_OC) return n_OC.Flip();
            if (d_min == d_A) return n_A;
            if (d_min == d_B) return n_B;

            throw new ArgumentException("Invalid inputs");
        }

        public Contact GetClosestPoints(Triangle other)
        {
            var n_A = GetClosestPoints(other.A);
            var n_B = GetClosestPoints(other.B);
            var n_C = GetClosestPoints(other.C);

            var n_AB = GetClosestPoints(other.AB);
            var n_BC = GetClosestPoints(other.BC);
            var n_CA = GetClosestPoints(other.CA);

            float d_A = n_A.Distance;
            float d_B = n_B.Distance;
            float d_C = n_C.Distance;
            float d_AB = n_AB.Distance;
            float d_BC = n_BC.Distance;
            float d_CA = n_CA.Distance;

            float d_min = Math.Min(
                Math.Min(d_A, Math.Min(d_B, d_C)),
                Math.Min(d_AB, Math.Min(d_BC, d_CA)));

            if (d_min == d_AB) return n_AB;
            if (d_min == d_BC) return n_BC;
            if (d_min == d_CA) return n_CA;
            if (d_min == d_A) return n_A;
            if (d_min == d_B) return n_B;
            if (d_min == d_C) return n_C;

            throw new ArgumentException("Invalid inputs");
        }

        public float DistanceTo(Point2 other) => GetClosestPoints(other).Distance;
        public float DistanceTo(Side other) => GetClosestPoints(other).Distance;
        public float DistanceTo(Triangle other) => GetClosestPoints(other).Distance;

        public string ToString(string format, IFormatProvider formatProvider)
            => $"Triangle{{{A.ToString(format, formatProvider)},{B.ToString(format, formatProvider)},{C.ToString(format, formatProvider)}}}";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g");

    }
}
