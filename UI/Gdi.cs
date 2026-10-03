using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace JA.UI
{
    public static class Gdi
    {
        #region Properties
        public static Pen Stroke { get; private set; } = new Pen(Color.Black, 0);
        public static SolidBrush Fill { get; private set; } = new SolidBrush(Color.Black);
        public static Font Type { get; private set; } = new Font(SystemFonts.CaptionFont.FontFamily, 8f);
        public static Color StrokeColor { get => Stroke.Color; set => Stroke.Color = value; }
        public static Color FillColor { get => Fill.Color; set => Fill.Color = value; }
        public static float FontSize { get => Type.Size; set => Type = new Font(Type.FontFamily, value); }
        public static FontStyle FontStyle { get => Type.Style; set => new Font(Type, value); }
        public static void SetEndArrow(float arrowSize) => Stroke.CustomEndCap = new AdjustableArrowCap(arrowSize / 2, arrowSize);
        public static void SetStartArrow(float arrowSize) => Stroke.CustomStartCap = new AdjustableArrowCap(arrowSize / 2, arrowSize);
        public static void SetEndNone() => Stroke.EndCap = LineCap.NoAnchor;
        public static void SetStartNone() => Stroke.StartCap = LineCap.NoAnchor;
        public static void SetNoCaps()
        {
            SetEndNone();
            SetStartNone();
        }
        #endregion

        #region Drawing
        public static void DrawLine(Graphics g, Color color, PointF start, PointF end, bool startArrow = false, bool endArrow = false, float arrowSize = 8f)
        {
            if (startArrow)
            {
                SetStartArrow(arrowSize);
            }
            if (endArrow)
            {
                SetEndArrow(arrowSize);
            }
            StrokeColor = color;
            g.DrawLine(Stroke, start, end);
            SetNoCaps();
        }
        public static void DrawPoint(Graphics g, Color color, PointF pixel, float pointSize = 4f)
        {
            Gdi.StrokeColor = color;
            g.DrawEllipse(Gdi.Stroke,
                pixel.X - pointSize / 2,
                pixel.Y - pointSize / 2,
                pointSize,
                pointSize);
        }
        public static void FillPoint(Graphics g, Color color, PointF pixel, float pointSize = 4f)
        {
            Gdi.FillColor = color;
            g.FillEllipse(Gdi.Fill,
                pixel.X - pointSize / 2,
                pixel.Y - pointSize / 2,
                pointSize,
                pointSize);
        }
        public static void DrawPoints(Graphics g, Color color, PointF[] points, float pointSize = 4f)
        {
            Gdi.StrokeColor = color;
            foreach (var pixel in points)
            {
                g.DrawEllipse(Gdi.Stroke,
                    pixel.X - pointSize / 2,
                    pixel.Y - pointSize / 2,
                    pointSize,
                    pointSize);
            }
        }
        public static void FillPoints(Graphics g, Color color, PointF[] points, float pointSize = 4f)
        {
            Gdi.FillColor = color;
            foreach (var pixel in points)
            {
                g.FillEllipse(Gdi.Fill,
                    pixel.X - pointSize / 2,
                    pixel.Y - pointSize / 2,
                    pointSize,
                    pointSize);
            }
        }
        public static void DrawLines(Graphics g, Color color, params PointF[] points)
        {
            Gdi.StrokeColor = color;
            g.DrawLines(Gdi.Stroke, points);
        }
        public static void DrawCurve(Graphics g, Color color, params PointF[] points)
        {
            Gdi.StrokeColor = color;
            g.DrawCurve(Gdi.Stroke, points);
        }
        public static void DrawClosedCurve(Graphics g, Color color, params PointF[] points)
        {
            Gdi.StrokeColor = color;
            g.DrawClosedCurve(Gdi.Stroke, points);
        }
        public static void FillClosedCurve(Graphics g, Color color, params PointF[] points)
        {
            Gdi.FillColor = color;
            g.FillClosedCurve(Gdi.Fill, points);
        }
        public static void DrawPolygon(Graphics g, Color color, params PointF[] points)
        {
            Gdi.StrokeColor = color;
            g.DrawPolygon(Gdi.Stroke, points);
        }
        public static void FillPolygon(Graphics g, Color color, params PointF[] points)
        {
            Gdi.FillColor = color;
            g.FillPolygon(Gdi.Fill, points);
        }
        public static void DrawLabel(Graphics g, Color color, string label, PointF point, int Xoffset = 0, int Yoffset = 0, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            point.X += Xoffset;
            point.Y += Yoffset;
            var sz = g.MeasureString(label, Gdi.Type);
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
                    point.X -= 0;
                    point.Y -= 0;
                    break;
                case ContentAlignment.TopCenter:
                    point.X -= sz.Width / 2;
                    point.Y -= 0;
                    break;
                case ContentAlignment.TopRight:
                    point.X -= sz.Width;
                    point.Y -= 0;
                    break;
                case ContentAlignment.MiddleLeft:
                    point.X -= 0;
                    point.Y -= sz.Height / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    point.Y -= sz.Height / 2;
                    point.X -= sz.Width / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    point.Y -= sz.Height / 2;
                    point.X -= sz.Width;
                    break;
                case ContentAlignment.BottomLeft:
                    point.X -= 0;
                    point.Y -= sz.Height;
                    break;
                case ContentAlignment.BottomCenter:
                    point.Y -= sz.Height;
                    point.X -= sz.Width / 2;
                    break;
                case ContentAlignment.BottomRight:
                    point.Y -= sz.Height;
                    point.X -= sz.Width;
                    break;
                default:
                    throw new NotSupportedException($"Unknown alignment {alignment}");
            }
            if (color.GetBrightness() >= 0.77f)
            {
                Gdi.FillColor = Color.FromArgb(22, 32, 32);
            }
            else
            {
                Gdi.FillColor = Color.FromArgb(224, 234, 224);
            }
            var box = new RectangleF(point, sz);
            g.FillRectangle(Gdi.Fill, box);
            Gdi.FillColor = color;
            g.DrawString(label, Gdi.Type, Gdi.Fill, box, sf);
        }

        public static void DrawCircle(Graphics g, Color color, PointF center, float radius)
        {
            StrokeColor = color;
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2f, radius * 2f);
            g.DrawEllipse(Stroke, rect);
            g.DrawLine(Stroke, center.X, center.Y, center.X+radius, center.Y);
        }
        public static void DrawCircle(Graphics g, Color color, PointF center, float radius, float angleDeg)
        {
            var state = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(-angleDeg);
            DrawCircle(g, color, PointF.Empty, radius);
            g.Restore(state);
        }
        public static void FillCircle(Graphics g, Color color, PointF center, float radius)
        {
            FillColor = color;
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2f, radius * 2f);
            g.FillEllipse(Fill, rect);
        }
        public static void DrawEllipse(Graphics g, Color color, PointF center, float majorRadius, float minorRadius)
        {
            StrokeColor = color;
            var rect = new RectangleF(center.X - majorRadius, center.Y - minorRadius, majorRadius * 2f, minorRadius * 2f);
            g.DrawEllipse(Stroke, rect);
            g.DrawLine(Stroke, center.X, center.Y, center.X+majorRadius, center.Y);
            //g.DrawLine(Stroke, center.X, center.Y, center.X, center.Y+minorRadius);
        }
        public static void DrawEllipse(Graphics g, Color color, PointF center, float majorRadius, float minorRadius, float angleDeg)
        {
            var state = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(-angleDeg);
            DrawEllipse(g, color, PointF.Empty, majorRadius, minorRadius);
            g.Restore(state);
        }
        public static void FillEllipse(Graphics g, Color color, PointF center, float majorRadius, float minorRadius)
        {
            FillColor = color;
            var rect = new RectangleF(center.X - majorRadius, center.Y - minorRadius, majorRadius * 2f, minorRadius * 2f);
            g.FillEllipse(Fill, rect);
        }
        public static void FillEllipse(Graphics g, Color color, PointF center, float majorRadius, float minorRadius, float angleDeg)
        {
            var state = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(-angleDeg);
            FillEllipse(g, color, PointF.Empty, majorRadius, minorRadius);
            g.Restore(state);
        }

        #endregion
    }
}
