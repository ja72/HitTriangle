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
            Normal = Vector2.Normalize(
                    new Vector2(-(B.Y - A.Y), (B.X - A.X)));
            OnLine = LinearAlgebra.Join(a, b);
        }

        public Vector2 A { get; }
        public Vector2 B { get; }
        [Browsable(false)]
        public Vector2 Normal { get; }
        [Browsable(false)]
        public float Length { get; }
        public Line OnLine { get; }

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

        public float LineDistanceTo(Vector2 point) 
            => Vector2.Dot(Normal, point - A);

        public bool Contains(Vector2 point)
        {
            float d_sign = LineDistanceTo(point);

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

        public string ToString(string format, IFormatProvider formatProvider) 
            => $"{A.ToString(format, formatProvider)}-{B.ToString(format, formatProvider)}";

        public string ToString(string format)
            => ToString(format, null);

        public override string ToString()
            => ToString("g");

    }
}
