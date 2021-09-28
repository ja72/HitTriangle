using System;
using System.Numerics;

namespace JA.Geometry
{
    static class Pairs
    {
        public static (Vector2 A, Vector2 B) Nearest(this Vector2 point, Vector2 other)
            => (point, other);

        public static (Vector2 A, Vector2 B) Nearest(this Side side, Vector2 other)
        {
            float d_A = Vector2.Distance(side.A, other);
            float d_B = Vector2.Distance(side.B, other);

            // Project point to line of side
            Vector2 hit = LinearAlgebra.Project(side.OnLine, other);
            float d_hit = side.Contains(hit) ? Vector2.Distance(hit, other) : float.PositiveInfinity;

            // Find the closest point
            float d_min = Math.Min(d_hit, Math.Min(d_A, d_B));
            if (d_min == d_hit) return (hit, other);
            if (d_min == d_A) return (side.A, other);
            if (d_min == d_B) return (side.B, other);

            throw new ArgumentException("Invalid inputs");
        }

        public static (Vector2 A, Vector2 B) Nearest(this Triangle triangle, Vector2 other)
        {
            var n_A = Nearest(triangle.A, other);
            var n_B = Nearest(triangle.B, other);
            var n_C = Nearest(triangle.C, other);
            var n_AB = Nearest(triangle.AB, other);
            var n_BC = Nearest(triangle.BC, other);
            var n_CA = Nearest(triangle.CA, other);

            float d_A = Vector2.Distance(n_A.A, n_A.B);
            float d_B = Vector2.Distance(n_B.A, n_B.B);
            float d_C = Vector2.Distance(n_C.A, n_C.B);
            float d_AB = Vector2.Distance(n_AB.A, n_AB.B);
            float d_BC = Vector2.Distance(n_BC.A, n_BC.B);
            float d_CA = Vector2.Distance(n_CA.A, n_CA.B);

            float d_min = Math.Min(
                Math.Min(d_A, Math.Min(d_B,d_C)),
                Math.Min(d_AB, Math.Min(d_BC, d_CA)));

            if (d_min == d_A) return n_A;
            if (d_min == d_B) return n_B;
            if (d_min == d_C) return n_C;
            if (d_min == d_AB) return n_AB;
            if (d_min == d_BC) return n_BC;
            if (d_min == d_CA) return n_CA;

            throw new ArgumentException("Invalid inputs");
        }

        public static (Vector2 A, Vector2 B) Nearest(this Side side, Side other)
        {
            var n_AA = Nearest(side, other.A);
            var n_AB = Nearest(side, other.B);
            var n_BA = Nearest(other, side.A);
            var n_BB = Nearest(other, side.B);

            float d_AA = Vector2.Distance(n_AA.A, n_AA.B);
            float d_AB = Vector2.Distance(n_AB.A, n_AB.B);
            float d_BA = Vector2.Distance(n_BA.A, n_BA.B);
            float d_BB = Vector2.Distance(n_BB.A, n_BB.B);

            float d_min = Math.Min(Math.Min(d_AA, d_AB), Math.Min(d_BA, d_BB));

            if (d_min == d_AA) return n_AA;
            if (d_min == d_AB) return n_AB;
            if (d_min == d_BA) return (n_BA.B, n_BA.A);
            if (d_min == d_BB) return (n_BB.B, n_BB.A);

            throw new ArgumentException("Invalid inputs");
        }

        public static (Vector2 A, Vector2 B) Nearest(this Triangle triangle, Side other)
        {            
            var n_A = Nearest(triangle, other.A);
            var n_B = Nearest(triangle, other.B);
            var n_OA = Nearest(other, triangle.A);
            var n_OB = Nearest(other, triangle.B);
            var n_OC = Nearest(other, triangle.C);

            float d_A = Vector2.Distance(n_A.A, n_A.B);
            float d_B = Vector2.Distance(n_B.A, n_B.B);
            float d_OA = Vector2.Distance(n_OA.A, n_OA.B);
            float d_OB = Vector2.Distance(n_OB.A, n_OB.B);
            float d_OC = Vector2.Distance(n_OC.A, n_OC.B);

            float d_min = Math.Min(
                Math.Min(d_OC, Math.Min(d_OA, d_OB)),
                Math.Min(d_A, d_B));

            if (d_min == d_OA) return (n_OA.B, n_OA.A);
            if (d_min == d_OB) return (n_OB.B, n_OB.A);
            if (d_min == d_OC) return (n_OC.B, n_OC.A);
            if (d_min == d_A) return n_A;
            if (d_min == d_B) return n_B;

            throw new ArgumentException("Invalid inputs");
        }

        public static (Vector2 A, Vector2 B) Nearest(this Triangle triangle, Triangle other)
        {
            var n_A = Nearest(triangle, other.A);
            var n_B = Nearest(triangle, other.B);
            var n_C = Nearest(triangle, other.C);

            var n_AB = Nearest(triangle, other.AB);
            var n_BC = Nearest(triangle, other.BC);
            var n_CA = Nearest(triangle, other.CA);

            float d_A = Vector2.Distance(n_A.A, n_A.B);
            float d_B = Vector2.Distance(n_B.A, n_B.B);
            float d_C = Vector2.Distance(n_C.A, n_C.B);
            float d_AB = Vector2.Distance(n_AB.A, n_AB.B);
            float d_BC = Vector2.Distance(n_BC.A, n_BC.B);
            float d_CA = Vector2.Distance(n_CA.A, n_CA.B);

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

        public static float Distance(this Vector2 point, Vector2 other)
            => Vector2.Distance(point, other);

        public static float Distance(this Side side, Vector2 other)
        {
            var (A, B) = Nearest(side, other);
            return Vector2.Distance(A, B);
        }
        public static float Distance(this Side side, Side other)
        {
            var (A, B) = Nearest(side, other);
            return Vector2.Distance(A, B);
        }
        public static float Distance(this Triangle triangle, Vector2 other)
        {
            var (A, B) = Nearest(triangle, other);
            return Vector2.Distance(A, B);
        }
        public static float Distance(this Triangle triangle, Side other)
        {
            var (A, B) = Nearest(triangle, other);
            return Vector2.Distance(A, B);
        }
        public static float Distance(this Triangle triangle, Triangle other)
        {
            var (A, B) = Nearest(triangle, other);
            return Vector2.Distance(A, B);
        }
    }
}
