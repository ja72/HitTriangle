using System;
using System.ComponentModel;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Side2 : 
        IGeometry, 
        IFormattable
    {
        #region Factory
        public Side2(Point2 a, Point2 b) : this()
        {
            A = a;
            B = b;
            Length = A.DistanceTo(B);
            Tangent = Vector2.Normalize(Point2.Span(B, A));
            Normal = new Vector2(-Tangent.Y, Tangent.X);
        } 
        #endregion

        #region Properties
        public Point2 A { get; }
        public Point2 B { get; }
        [Browsable(false)] public Vector2 Tangent { get; }
        [Browsable(false)] public Vector2 Normal { get; }
        [Browsable(false)] public float Length { get; } 
        #endregion

        public Side2 Offset(Vector2 delta)
            => new Side2(A + delta, B + delta);

        public Side2 Rotate(Vector2 pivot, float angle)
            => new Side2(
                LinearAlgebra.RotateAbout(A, pivot, angle),
                LinearAlgebra.RotateAbout(B, pivot, angle));

        #region Algebra
        public static Side2 operator +(Side2 side, Vector2 delta)
            => side.Offset(delta);
        public static Side2 operator -(Side2 side, Vector2 delta)
            => side.Offset(-delta);
        #endregion

        #region Formatting
        public string ToString(string format, IFormatProvider formatProvider)
            => $"Side({A.ToString(format, formatProvider)}-{B.ToString(format, formatProvider)})";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g"); 
        #endregion

        #region Geometry
        public bool Contains(Point2 point)
        {
            float d_sign = Vector2.Dot(Normal, Point2.Span(point, A));

            if (Math.Abs(d_sign) <= Geometry.Tolerance)
            {
                point -= d_sign * Normal;
                Vector2 ba = Point2.Span(B, A);
                Vector2 pa = Point2.Span(point, A);
                var w = Vector2.Dot(ba, pa) / Vector2.Dot(ba, ba);
                return w >= 0 && w <= 1;
            }

            return false;
        }

        public (float w_A, float w_B) GetBaryCoords(Point2 point)
        {
            Vector2 pa = Point2.Span(point, A);
            float d_side = Vector2.Dot(Normal, pa);
            point -= d_side * Normal;

            Vector2 ba = Point2.Span(B, A);
            float w_B = Vector2.Dot(ba, pa) / Vector2.Dot(ba, ba);
            float w_A = 1 - w_B;

            return (w_A, w_B);
        }

        public Contact GetClosestPoint(Point2 other)
        {
            float d_A = A.DistanceTo(other);
            float d_B = B.DistanceTo(other);

            // Project point to line of side
            var hit = Line2.Join(A, B).Project(other);
            float d_hit = Contains(hit) ? hit.DistanceTo(other) : float.PositiveInfinity;

            // Find the closest point
            float d_min = Math.Min(d_hit, Math.Min(d_A, d_B));
            if (d_min == d_hit) return new Contact(hit, other);
            if (d_min == d_A) return new Contact(A, other);
            if (d_min == d_B) return new Contact(B, other);

            throw new ArgumentException("Invalid inputs");
        }
        public Contact GetClosestPoint(Side2 other)
        {
            var n_AA = GetClosestPoint(other.A);
            var n_AB = GetClosestPoint(other.B);
            var n_BA = other.GetClosestPoint(A);
            var n_BB = other.GetClosestPoint(B);

            float d_AA = n_AA.Source.DistanceTo(n_AA.Target);
            float d_AB = n_AB.Source.DistanceTo(n_AB.Target);
            float d_BA = n_BA.Source.DistanceTo(n_BA.Target);
            float d_BB = n_BB.Source.DistanceTo(n_BB.Target);

            float d_min = Math.Min(
                Math.Min(d_AA, d_AB),
                Math.Min(d_BA, d_BB));

            if (d_min == d_AA) return n_AA;
            if (d_min == d_AB) return n_AB;
            if (d_min == d_BA) return n_BA.Flip();
            if (d_min == d_BB) return n_BB.Flip();

            throw new ArgumentException("Invalid inputs");
        }

        public float DistanceTo(Point2 other) => GetClosestPoint(other).Distance;
        public float DistanceTo(Side2 other) => GetClosestPoint(other).Distance;
        #endregion

    }
}
