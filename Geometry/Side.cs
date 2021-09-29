using System;
using System.ComponentModel;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Side : IFormattable
    {
        public Side(Vector2 a, Vector2 b) : this()
        {
            A = a;
            B = b;
            Length = Vector2.Distance(A, B);
            Tangent = Vector2.Normalize(B - A);                
            Normal = Vector2.Normalize(
                    new Vector2(-(B.Y - A.Y), (B.X - A.X)));
        }

        public Vector2 A { get; }
        public Vector2 B { get; }
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

        public bool Contains(Vector2 point)
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

        public (float w_A, float w_B) GetBaryCoords(Vector2 point)
        {
            float d_side = Vector2.Dot(Normal, point - A);
            point -= d_side * Normal;

            float w_B = Vector2.Dot(B - A, point - A) / Vector2.Dot(B - A, B - A);
            float w_A = 1 - w_B;

            return (w_A, w_B);
        }

        public Contact GetClosestPoints(Vector2 other)
        {
            float d_A = Vector2.Distance(A, other);
            float d_B = Vector2.Distance(B, other);

            // Project point to line of side
            Vector2 hit = LinearAlgebra.Join(A,B).Project(other);
            float d_hit = Contains(hit) ? Vector2.Distance(hit, other) : float.PositiveInfinity;

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

            float d_AA = Vector2.Distance(n_AA.Source, n_AA.Target);
            float d_AB = Vector2.Distance(n_AB.Source, n_AB.Target);
            float d_BA = Vector2.Distance(n_BA.Source, n_BA.Target);
            float d_BB = Vector2.Distance(n_BB.Source, n_BB.Target);

            float d_min = Math.Min(Math.Min(d_AA, d_AB), Math.Min(d_BA, d_BB));

            if (d_min == d_AA) return n_AA;
            if (d_min == d_AB) return n_AB;
            if (d_min == d_BA) return n_BA.Flip();
            if (d_min == d_BB) return n_BB.Flip();

            throw new ArgumentException("Invalid inputs");
        }


        public float DistanceTo(Vector2 other) => GetClosestPoints(other).Distance;
        public float DistanceTo(Side other) => GetClosestPoints(other).Distance;

        public string ToString(string format, IFormatProvider formatProvider) 
            => $"Side({A.ToString(format, formatProvider)}-{B.ToString(format, formatProvider)})";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g");

    }
}
