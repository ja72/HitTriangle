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
        public static float Dot(Line2 a, Point2 b)
            => Vector2.Dot(a.Vector, b.Vector) + a.Scalar+ b.Scalar;
        public static float Dot(Point2 a, Line2 b)
            => Vector2.Dot(a.Vector, b.Vector) + a.Scalar + b.Scalar;
        public static Vector2 Cross(Vector2 v, float s)
            => new Vector2(v.Y * s, -v.X * s);
        public static Vector2 Cross(float s, Vector2 v)
            => new Vector2(-v.Y * s, v.X * s);
        public static float Cross(Vector2 a, Vector2 b)
            => a.X * b.Y - a.Y * b.X;
        public static Vector2 Rotate(Vector2 vector, float angle)
        {
            float c = (float)Math.Cos(angle), s = (float)Math.Sin(angle);
            return new Vector2(
                c * vector.X - s * vector.Y,
                s * vector.X + c * vector.Y);
        }
        public static Vector2 RotateAbout(Vector2 vector, Point2 pivot, float angle)
            => pivot.Position + Rotate(vector - pivot.Position, angle);
        public static Point2 RotateAbout(Point2 point, Point2 pivot, float angle)
            => pivot + Rotate(Point2.Span(point, pivot), angle);

        public static float Area(Vector2 a, Vector2 b, Vector2 c)
            // Area = 1/2*(B-A)×(C-A) = 1/2*(B×C+C×A+A×B)
            => 0.5f * (Cross(a, b) + Cross(b, c) + Cross(c, a));
        public static Vector2 Centroid(Vector2 a, Vector2 b, Vector2 c)
            => (a + b + c) / 3;
        public static float AreaMoment(Vector2 a, Vector2 b, Vector2 c)
            => Area(a, b, c) / 18 * (
                Dot(a, a) + Dot(b, b) + Dot(c, c) 
                -Dot(a, b) - Dot(b, c) - Dot(c, a));


    }
}
