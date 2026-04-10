using System;
using System.ComponentModel;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Side : IFormattable
    {
        public Side(Point2 a, Point2 b) : this()
        {
            A = a;
            B = b;
            Length = A.DistanceTo(B);
            Tangent = Vector2.Normalize(B - A);                
            Normal = new Vector2(-Tangent.Y, Tangent.X);
        }

        public Point2 A { get; }
        public Point2 B { get; }
        [Browsable(false)] public Vector2 Tangent { get; }
        [Browsable(false)] public Vector2 Normal { get; }
        [Browsable(false)] public float Length { get; }

        public Side Offset(Vector2 delta)
            => new Side(A + delta, B + delta);

        public Side Rotate(Vector2 pivot, float angle)
            => new Side(
                LinearAlgebra.RotateAbout(A, pivot, angle),
                LinearAlgebra.RotateAbout(B, pivot, angle));

        public static Side operator +(Side side, Vector2 delta)
            => side.Offset(delta);
        public static Side operator -(Side side, Vector2 delta)
            => side.Offset(-delta);

        public bool Contains(Point2 point)
        {
            float d_sign = Vector2.Dot(Normal, point - A);
            
            if (Math.Abs(d_sign) <= 1e-6f)
            {
                point -= d_sign * Normal;
                var w = Vector2.Dot(B - A, point - A) / Vector2.Dot(B - A, B - A);
                return w >= 0 && w <= 1;
            }

            return false;
        }

        public (float w_A, float w_B) GetBaryCoords(Point2 point)
        {
            float d_side = Vector2.Dot(Normal, point - A);
            point -= d_side * Normal;

            float w_B = Vector2.Dot(B - A, point - A) / Vector2.Dot(B - A, B - A);
            float w_A = 1 - w_B;

            return (w_A, w_B);
        }

        public Contact GetClosestPoints(Point2 other)
        {
            float d_A = A.DistanceTo(other);
            float d_B = B.DistanceTo(other);

            // Project point to line of side
            var hit = LinearAlgebra.Join(A,B).Project(other);
            float d_hit = Contains(hit) ? hit.DistanceTo(other) : float.PositiveInfinity;

            // Find the closest point
            float d_min = Math.Min(d_hit, Math.Min(d_A, d_B));
            if (d_min == d_hit) return new Contact(hit, other);
            if (d_min == d_A) return new Contact(A, other);
            if (d_min == d_B) return new Contact(B, other);

            throw new ArgumentException("Invalid inputs");
        }
        public Contact GetClosestPoints(Side other)
        {
            var n_AA = GetClosestPoints(other.A);
            var n_AB = GetClosestPoints(other.B);
            var n_BA = other.GetClosestPoints(A);
            var n_BB = other.GetClosestPoints(B);

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


        public float DistanceTo(Point2 other) => GetClosestPoints(other).Distance;
        public float DistanceTo(Side other) => GetClosestPoints(other).Distance;

        public string ToString(string format, IFormatProvider formatProvider) 
            => $"Side({A.ToString(format, formatProvider)}-{B.ToString(format, formatProvider)})";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g");

    }
}
