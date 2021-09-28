using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    public readonly struct Line
    {
        public Line(float a, float b, float c) : this()
        {
            A = a;
            B = b;
            C = c;

            Tangent = Vector2.Normalize(
                new Vector2(b, -a));
            Normal = Vector2.Normalize(
                new Vector2(a, b));
        }

        public float A { get; }
        public float B { get; }
        public float C { get; }

        public Vector2 Tangent { get; }
        public Vector2 Normal { get; }
    }
}
