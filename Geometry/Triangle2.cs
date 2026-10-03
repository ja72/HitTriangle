using System;
using System.ComponentModel;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Triangle2 : 
        IGeometry,
        IFormattable
    {
        public Triangle2(Point2 a, Point2 b, Point2 c) : this()
        {
            A = a;
            B = b;
            C = c;

            Vector2 va = a.Position, vb = b.Position, vc = c.Position;

            Area = LinearAlgebra.Area(va, vb, vc);
            Centroid = LinearAlgebra.Centroid(va, vb, vc);
            AreaMoment = LinearAlgebra.AreaMoment(va, vb, vc);
        }

        public Point2 A { get; }
        public Point2 B { get; }
        public Point2 C { get; }

        [Browsable(false)] public Side2 AB { get => new Side2(A, B); }
        [Browsable(false)] public Side2 BC { get => new Side2(B, C); }
        [Browsable(false)] public Side2 CA { get => new Side2(C, A); }

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
            Vector2 va = A.Position, vb = B.Position, vc = C.Position;
            Vector2 vp = point.Position;

            float d = LinearAlgebra.Area(va, vb, vc);

            return (LinearAlgebra.Area(vp, vb, vc) / d,
                    LinearAlgebra.Area(va, vp, vc) / d,
                    LinearAlgebra.Area(va, vb, vp) / d);
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

        public Triangle2 Offset(Vector2 delta)
            => new Triangle2(A + delta, B + delta, C + delta);
        public Triangle2 Rotate(Point2 pivot, float angle) 
            => new Triangle2(
                LinearAlgebra.RotateAbout(A, pivot, angle),
                LinearAlgebra.RotateAbout(B, pivot, angle),
                LinearAlgebra.RotateAbout(C, pivot, angle));

        public static Triangle2 operator +(Triangle2 triangle, Vector2 delta)
            => triangle.Offset(delta);
        public static Triangle2 operator -(Triangle2 triangle, Vector2 delta)
            => triangle.Offset(-delta);

        public Contact GetClosestPoint(Point2 target)
        {
            var n_A = new Contact(A, target);
            var n_B = new Contact(B, target);
            var n_C = new Contact(C, target);
            var n_AB = AB.GetClosestPoint(target);
            var n_BC = BC.GetClosestPoint(target);
            var n_CA = CA.GetClosestPoint(target);

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
        public Contact GetClosestPoint(Circle2 target)
        {
            return target.GetClosestPoint(this).Flip();
        }
        public Contact GetClosestPoint(Side2 target)
        {
            var n_A = GetClosestPoint(target.A);
            var n_B = GetClosestPoint(target.B);
            var n_OA = target.GetClosestPoint(A);
            var n_OB = target.GetClosestPoint(B);
            var n_OC = target.GetClosestPoint(C);

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

        public Contact GetClosestPoint(Triangle2 other)
        {
            var n_A = GetClosestPoint(other.A);
            var n_B = GetClosestPoint(other.B);
            var n_C = GetClosestPoint(other.C);

            var n_AB = GetClosestPoint(other.AB);
            var n_BC = GetClosestPoint(other.BC);
            var n_CA = GetClosestPoint(other.CA);

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

        public float DistanceTo(Point2 other) => GetClosestPoint(other).Distance;
        public float DistanceTo(Side2 other) => GetClosestPoint(other).Distance;
        public float DistanceTo(Triangle2 other) => GetClosestPoint(other).Distance;

        #region Formatting
        public string ToString(string format, IFormatProvider formatProvider)
            => $"Triangle{{{A.ToString(format, formatProvider)},{B.ToString(format, formatProvider)},{C.ToString(format, formatProvider)}}}";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g"); 
        #endregion

    }
}
