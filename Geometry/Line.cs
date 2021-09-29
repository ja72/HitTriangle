using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    public readonly struct Line : IFormattable
    {
        public Line(float a, float b, float c) : this()
        {
            A = a;
            B = b;
            C = c;

            Tangent = Vector2.Normalize(
                new Vector2(b, -a));
            Normal = -Vector2.Normalize(
                new Vector2(a, b));
        }

        public float A { get; }
        public float B { get; }
        public float C { get; }

        public Vector2 Tangent { get; }
        public Vector2 Normal { get; }

        public Vector2 PointAlong(float distance)
        {
            float absq = A * A + B * B;
            float t = distance * LinearAlgebra.Sqrt(absq);
            return new Vector2(
                -(A * C + B * t) / absq,
                (A * t - B * C) / absq);
        }

        public float DistanceTo(Vector2 point)
        {
            return (A * point.X + B * point.Y + C) / LinearAlgebra.Sqrt(A * A + B * B);
        }

        public bool Contains(Vector2 point)
        {
            float d_sign = DistanceTo(point);

            return Math.Abs(d_sign) <= 1e-6f;
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => $"Line[{A.ToString(format, formatProvider)}x+{B.ToString(format, formatProvider)}y+{C.ToString(format, formatProvider)}=0]";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g");


    }
}
