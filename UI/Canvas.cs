using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

namespace JA.UI
{
    using JA.Geometry;

    public class Canvas : IDisposable
    {
        public readonly Pen pen = new Pen(Color.Black, 0);
        public readonly SolidBrush brush = new SolidBrush(Color.Black);
        public readonly Font font = new Font(SystemFonts.CaptionFont.FontFamily, 8f);

        public Canvas(Rectangle target, float size)
            : this(target, size, Vector2.Zero) { }
        public Canvas(Rectangle target, float modelSize, Point2 worldCenter)
        {
            Target = target;
            ModelSize = modelSize;
            WorldCenter = worldCenter;
        }
        public Point2 MouseDown { get; set; }
        public Point2 MouseMove { get; set; }
        public MouseButtons Buttons { get; set; }

        public Rectangle Target { get; }
        public float ModelSize { get; }
        public Point2 WorldCenter { get; }
        public int TargetSize { get => Math.Min(Target.Width, Target.Height); }
        public PointF GetPixel(Point2 point)
        {
            Vector2 pt = TargetSize / ModelSize * (point - WorldCenter);

            return new PointF(
                Target.X + Target.Width / 2 + pt.X,
                Target.Y + Target.Height / 2 - pt.Y);
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
                pixel.X - Target.X - Target.Width / 2,
                Target.Y + Target.Height / 2 - pixel.Y);
            return WorldCenter + (ModelSize / TargetSize) * pt;
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

        public void DrawLine(Graphics g, Color color, Point2 start, Point2 end, bool startArrow = false, bool endArrow = false, float arrowSize = 8f)
        {
            var px1 = GetPixel(start);
            var px2 = GetPixel(end);
            if (startArrow)
            {
                pen.CustomStartCap = new AdjustableArrowCap(arrowSize / 2, arrowSize);
            }
            if (endArrow)
            {
                pen.CustomEndCap = new AdjustableArrowCap(arrowSize / 2, arrowSize);
            }
            pen.Color = color;
            g.DrawLine(pen, px1, px2);
            pen.EndCap = LineCap.NoAnchor;
            pen.StartCap = LineCap.NoAnchor;
        }

        public void DrawLabel(Graphics g, Color color, string label, Point2 point, int Xoffset = 0, int Yoffset = 0, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            var pt = GetPixel(point);
            pt.X += Xoffset;
            pt.Y += Yoffset;
            var sz = g.MeasureString(label, font);
            sz.Height += 1;
            sz.Width += 3;
            var sf = new StringFormat()
            {
                Trimming = StringTrimming.EllipsisWord,
                FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap,
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    pt.X -= 0;
                    pt.Y -= 0;
                    break;
                case ContentAlignment.TopCenter:
                    pt.X -= sz.Width / 2;
                    pt.Y -= 0;
                    break;
                case ContentAlignment.TopRight:
                    pt.X -= sz.Width;
                    pt.Y -= 0;
                    break;
                case ContentAlignment.MiddleLeft:
                    pt.X -= 0;
                    pt.Y -= sz.Height / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    pt.Y -= sz.Height / 2;
                    pt.X -= sz.Width / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    pt.Y -= sz.Height / 2;
                    pt.X -= sz.Width;
                    break;
                case ContentAlignment.BottomLeft:
                    pt.X -= 0;
                    pt.Y -= sz.Height;
                    break;
                case ContentAlignment.BottomCenter:
                    pt.Y -= sz.Height;
                    pt.X -= sz.Width / 2;
                    break;
                case ContentAlignment.BottomRight:
                    pt.Y -= sz.Height;
                    pt.X -= sz.Width;
                    break;
                default:
                    throw new NotSupportedException($"Unknown alignment {alignment}");
            }
            if (color.GetBrightness() >= 0.77f)
            {
                brush.Color = Color.FromArgb(22, 32, 32);
            }
            else
            {
                brush.Color = Color.FromArgb(224, 234, 224);
            }
            var box = new RectangleF(pt, sz);
            g.FillRectangle(brush, box);
            brush.Color = color;
            g.DrawString(label, font, brush, box, sf);
        }

        public void DrawPoint(Graphics g, Color color, Vector2 point, float pointSize = 4f)
        {
            var pixel = GetPixel(point);
            pen.Color = color;
            g.DrawEllipse(pen,
                pixel.X - pointSize / 2,
                pixel.Y - pointSize / 2,
                pointSize,
                pointSize);
        }
        public void FillPoint(Graphics g, Color color, Vector2 point, float pointSize = 4f)
        {
            var pixel = GetPixel(point);
            brush.Color = color;
            g.FillEllipse(brush,
                pixel.X - pointSize / 2,
                pixel.Y - pointSize / 2,
                pointSize,
                pointSize);
        }

        public void DrawLines(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            pen.Color = color;
            g.DrawLines(pen, pxs);
        }
        public void DrawCurve(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            pen.Color = color;
            g.DrawCurve(pen, pxs);
        }
        public void DrawPolygon(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            pen.Color = color;
            g.DrawPolygon(pen, pxs);
        }
        public void DrawClosedCurve(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            pen.Color = color;
            g.DrawClosedCurve(pen, pxs);
        }
        public void FillPolygon(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            brush.Color = color;
            g.FillPolygon(brush, pxs);
        }
        public void FillClosedCurve(Graphics g, Color color, params Point2[] points)
        {
            var pxs = GetPixels(points);
            brush.Color = color;
            g.FillClosedCurve(brush, pxs);
        }
        public void DrawLine(Graphics g, Color color, Side side)
        {
            DrawLine(g, color, side.A, side.B);
        }
        public void DrawTriangle(Graphics g, Color color, Triangle triangle)
        {
            DrawLine(g, color, triangle.AB);
            DrawLine(g, color, triangle.BC);
            DrawLine(g, color, triangle.CA);
            DrawPoint(g, color, triangle.A);
            DrawPoint(g, color, triangle.B);
            DrawPoint(g, color, triangle.C);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    pen.Dispose();
                    brush.Dispose();
                    font.Dispose();
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
    }
}
