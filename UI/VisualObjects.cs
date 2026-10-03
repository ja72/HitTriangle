using System.Drawing;
using System.Windows.Forms;

namespace JA.UI
{
    using System.Numerics;

    using JA.Geometry;
    public interface IVisualObject
    {
        IGeometry Geometry { get; }
        void Draw(Graphics g, Canvas canvas);
        void UpdatePosition(Vector2 delta);
        void UpdateRotation(Point2 pivot, float angleDeg);
    }

    public class VisualTriangle : IVisualObject
    {
        public VisualTriangle(Color color, Triangle2 triangle)
        {
            this.Color = color;
            this.Triangle = triangle;
        }
        public Color Color { get; set; }
        public Triangle2 Triangle { get; set; }
        public IGeometry Geometry => Triangle;
        public void Draw(Graphics g, Canvas canvas)
        {
            Point2 a = Triangle.A, b = Triangle.B, c = Triangle.C;
            Side2 ab = Triangle.AB, bc = Triangle.BC, ca = Triangle.CA;
            canvas.DrawPolygon(g, Color, a, b, c);
            canvas.DrawPoints(g, Color, [a, b, c], 4f);
        }
        public void UpdatePosition(Vector2 delta) => Triangle += delta;
        public void UpdateRotation(Point2 pivot, float angleDeg) => Triangle = Triangle.Rotate(pivot, angleDeg * LinearAlgebra.Deg);
    }

    public class VisualCircle : IVisualObject
    {
        public VisualCircle(Color color, Circle2 circle)
        {
            Color=color;
            Circle=circle;
        }

        public Color Color { get; set; }
        public Circle2 Circle { get; set; }
        public IGeometry Geometry => Circle;
        public void Draw(Graphics g, Canvas canvas)
        {
            Point2 cen = Circle.Center;
            canvas.DrawCircle(g, Color, Circle);
        }
        public void UpdatePosition(Vector2 delta) => Circle += delta;
        public void UpdateRotation(Point2 pivot, float angleDeg) => Circle = Circle.Rotate(pivot, angleDeg * LinearAlgebra.Deg);
    }
}
