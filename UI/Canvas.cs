using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace JA.UI
{
    using System.Linq;

    using JA.Geometry;

    public class GraphicsEventArgs : EventArgs
    {
        public GraphicsEventArgs(Graphics graphics)
        {
            Graphics= graphics??throw new ArgumentNullException(nameof(graphics));
        }

        public Graphics Graphics { get; }
    }

    public delegate void CanvasDrawHandler(object sender, GraphicsEventArgs eventArgs);

    public class Canvas : IDisposable
    {
        readonly Control control;
        readonly List<IVisualObject> visualObjects;
        public Canvas(Control target, float size)
            : this(target, size, Point2.Origin) { }
        public Canvas(Control target, float modelSize, Point2 worldCenter)
        {
            Target = target;
            ModelSize = modelSize;
            WorldCenter = worldCenter;
            visualObjects = new List<IVisualObject>();

            Form onForm = target.FindForm();
            onForm.Load += (s, ev) =>
            {
                SetupHandlers(onForm);
            };
        }
        void SetupHandlers(Form onForm)
        {
            Target.Resize += (obj, ev) =>
            {
                Target.Refresh();
            };

            Target.KeyDown += (obj, ev) =>
            {
                if (ev.KeyData == Keys.Escape)
                {
                    onForm.Close();
                }
            };

            Target.MouseMove += (obj, ev) =>
            {
                DoMouseMove(ev.Location, ev.Button);
                Target.Refresh();
            };
            Target.MouseDown += (obj, ev) =>
            {
                DoMouseDown(ev.Location, ev.Button);
                Target.Refresh();
            };
        }
        public Point2 MouseDown { get; set; }
        public Point2 MouseMove { get; set; }
        public MouseButtons Buttons { get; set; }
        public Control Target { get; }
        public Rectangle TargetRect { get => Target.ClientRectangle; }
        public float ModelSize { get; set; }
        public Point2 WorldCenter { get; set; }
        public int TargetSize { get => Math.Min(TargetRect.Width, TargetRect.Height); }
        public float Scale { get => TargetSize / ModelSize; }
        public PointF GetPixel(Point2 point)
        {
            Vector2 pt = Scale * Point2.Span(point, WorldCenter);

            return new PointF(
                TargetRect.X + TargetRect.Width / 2 + pt.X,
                TargetRect.Y + TargetRect.Height / 2 - pt.Y);
        }
        public PointF[] GetPixels(Point2[] points)
        {
            PointF[] pixels = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                pixels[i] = GetPixel(points[i]);
            }
            return pixels;
        }
        public Point2 GetPoint(PointF pixel)
        {
            var pt = new Vector2(
                pixel.X - TargetRect.X - TargetRect.Width / 2,
                TargetRect.Y + TargetRect.Height / 2 - pixel.Y);
            return WorldCenter + pt / Scale;
        }

        public void DoMouseDown(Point mouse, MouseButtons buttons)
        {
            Buttons = buttons;
            MouseDown = GetPoint(mouse);
            MouseMove = MouseDown;
        }
        public void DoMouseMove(Point mouse, MouseButtons buttons)
        {
            Buttons = buttons;
            MouseMove = GetPoint(mouse);
        }
        public IReadOnlyList<IVisualObject> VisualObjects { get => visualObjects; }
        public void AddTriangle(Color color, Triangle2 triangle)
            => visualObjects.Add(new VisualTriangle(color, triangle));
        public void AddCircle(Color color, Circle2 circle)
            => visualObjects.Add(new VisualCircle(color, circle));

        public void DrawObjects(Graphics g)
        {
            foreach (var item in visualObjects)
            {
                item.Draw(g, this);
            }
        }
        public void DrawPoint(Graphics g, Color color, Point2 point, float pointSize = 4f)
        {
            var pixel = GetPixel(point);
            Gdi.DrawPoint(g, color, pixel, pointSize);
        }
        public void FillPoint(Graphics g, Color color, Point2 point, float pointSize = 4f)
        {
            var pixel = GetPixel(point);
            Gdi.FillPoint(g, color, pixel, pointSize);
        }
        public void DrawPoints(Graphics g, Color color, Point2[] points, float pointSize = 4f)
        {
            var pixel = GetPixels(points);
            Gdi.DrawPoints(g, color, pixel, pointSize);
        }
        public void FillPoints(Graphics g, Color color, Point2[] point, float pointSize = 4f)
        {
            var pixels = GetPixels(point);
            Gdi.FillPoints(g, color, pixels, pointSize);
        }
        public void DrawLine(Graphics g, Color color, Point2 start, Point2 end, bool startArrow = false, bool endArrow = false, float arrowSize = 8f)
        {
            var px1 = GetPixel(start);
            var px2 = GetPixel(end);
            Gdi.DrawLine(g, color, px1, px2, startArrow, endArrow, arrowSize);
        }
        public void DrawLines(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            Gdi.DrawLines(g, color, pxs);
        }
        public void DrawCurve(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            Gdi.DrawCurve(g, color, pxs);
        }
        public void DrawPolygon(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            Gdi.DrawPolygon(g, color, pxs);
        }
        public void FillPolygon(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            Gdi.FillPolygon(g, color, pxs);
        }
        public void DrawClosedCurve(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            Gdi.StrokeColor = color;
            g.DrawClosedCurve(Gdi.Stroke, pxs);
        }
        public void FillClosedCurve(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            Gdi.FillColor = color;
            g.FillClosedCurve(Gdi.Fill, pxs);
        }
        public void DrawLabel(Graphics g, Color color, string label, Point2 point, int Xoffset = 0, int Yoffset = 0, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            var pt = GetPixel(point);
            Gdi.DrawLabel(g, color, label, pt, Xoffset, Yoffset, alignment);
        }
        public void DrawTriangle(Graphics g, Color color, Triangle2 triangle)
        {
            DrawLine(g, color, triangle.A, triangle.B);
            DrawLine(g, color, triangle.B, triangle.C);
            DrawLine(g, color, triangle.C, triangle.A);

            DrawPoint(g, color, triangle.A);
            DrawPoint(g, color, triangle.B);
            DrawPoint(g, color, triangle.C);
        }
        public void DrawCircle(Graphics g, Color color, Circle2 circle)
        {
            var cen = GetPixel(circle.Center);
            var rad = Scale * circle.Radius;

            Gdi.DrawCircle(g, color, cen, rad);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose GDI objects
                }

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Events
        public void HandleMouse()
        {
            foreach (var item in VisualObjects.Reverse())
            {
                if (item.Geometry.Contains(MouseDown))
                {
                    switch (Buttons)
                    {
                        case MouseButtons.Left:
                        {
                            var delta = Point2.Span(MouseMove, MouseDown);
                            MouseDown = MouseMove;
                            item.UpdatePosition(delta);
                            Target.Invalidate();
                            break;
                        }
                        case MouseButtons.Right:
                        {
                            break;
                        }
                        case MouseButtons.Middle:
                        {
                            Point2 pivot = MouseDown;
                            float angle = 0.2f;
                            item.UpdateRotation(pivot, angle);
                            Target.Invalidate();
                            break;
                        }
                    }
                }

            }
            
        }


        #endregion
    }

}
