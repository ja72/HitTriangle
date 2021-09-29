using System;
using System.Numerics;

namespace JA.Geometry
{
    static class LinearAlgebra
    {
        public static readonly float PI = (float)Math.PI;
        public static readonly float Deg = PI/180;
        public static float Sqrt(float a) => (float)Math.Sqrt(a);
        public static float Hypot(float a, float b) => (float)Math.Sqrt(a*a+b*b);
        public static float Dot(Vector2 a, Vector2 b)
            => Vector2.Dot(a, b);
        public static float Cross(Vector2 a, Vector2 b)
            => a.X * b.Y - a.Y * b.X;
        public static Vector2 Rotate(Vector2 vector, float angle)
        {
            float c = (float)Math.Cos(angle), s = (float)Math.Sin(angle);
            return new Vector2(
                c * vector.X - s * vector.Y,
                s * vector.X + c * vector.Y);
        }
        public static Vector2 RotateAbout(Vector2 vector, Vector2 pivot, float angle)
            => pivot + Rotate(vector - pivot, angle);

        public static float Area(Vector2 a, Vector2 b, Vector2 c)
            // Area = 1/2*(B-A)×(C-A) = 1/2*(B×C+C×A+A×B)
            => 0.5f * (Cross(a, b) + Cross(b, c) + Cross(c, a));
        public static Vector2 Centroid(Vector2 a, Vector2 b, Vector2 c)
            => (a + b + c) / 3;
        public static float AreaMoment(Vector2 a, Vector2 b, Vector2 c)
            => Area(a, b, c) / 18 * (
                Dot(a, a) + Dot(b, b) + Dot(c, c) 
                -Dot(a, b) - Dot(b, c) - Dot(c, a));
        /// <summary>
        /// Joints two points to form an infinite line.
        /// </summary>
        /// <param name="A">The first point</param>
        /// <param name="B">The second point</param>
        /// <returns>
        /// The <code>(a,b,c)</code> coordinates of the line
        /// such that <code>a*x+b*y+c=0</code> is the equation of the line.
        /// </returns>
        public static Line Join(Vector2 A, Vector2 B)
        {
            return new Line(A.Y - B.Y, B.X - A.X, Cross(A, B));
        }

        /// <summary>
        /// Finds the point where two lines meet.
        /// </summary>
        /// <param name="lineA">The first line.</param>
        /// <param name="lineB">The second line.</param>
        /// <returns>A point</returns>
        public static Vector2 Meet(Line lineA, Line lineB)
        {
            float d = lineA.A * lineB.B - lineA.B * lineB.A;
            return new Vector2(
                (lineA.B * lineB.C - lineA.C * lineB.B) / d,
                (lineA.C * lineB.A - lineA.A * lineB.C) / d);
        }

        /// <summary>
        /// Projects a point onto a line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="point">The point.</param>
        public static Vector2 Project(Line line, Vector2 point)
        {
            float t = line.A * point.Y - line.B * point.X;
            float d = line.A * line.A + line.B * line.B;
            return new Vector2(
                (-line.A * line.C - line.B * t) / d,
                (-line.B * line.C + line.A * t) / d);
        }
    }
}
