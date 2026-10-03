using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JA.UI
{
    using JA.Geometry;

    public partial class MainForm : Form
    {
        Canvas canvas;

        public MainForm()
        {
            InitializeComponent();

            float b = 5, h = 8;
            float x = -4, y = -3;
            var triangle = new Triangle2(
                Point2.Cartesian(x + 0, y + 0),
                Point2.Cartesian(x + b, y + 0),
                Point2.Cartesian(x + 0, y + h));
            var other = new Triangle2(
                Point2.Cartesian(8, 0),
                Point2.Cartesian(9, 2),
                Point2.Cartesian(8, 2));
            var circ = new Circle2(
                Point2.Cartesian(3, 7),
                2);

            canvas = new Canvas(this, 20);
            canvas.AddTriangle(Color.Blue, triangle);
            canvas.AddTriangle(Color.DarkCyan, other);
            canvas.AddCircle(Color.DarkGoldenrod, circ);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Paint += RenderFormHandler;
        }

        private void DrawClosestDistance(Graphics g, Color color, string format, IVisualObject source, IVisualObject target, out Contact contact)
        {
            contact = Geometry.GetClosestPoint(source.Geometry, target.Geometry);
            float d = contact.Distance;

            Gdi.Stroke.DashStyle = DashStyle.Solid;
            canvas.DrawLine(g, color, contact.Target, contact.Source, false, true);
            Gdi.Stroke.DashStyle = DashStyle.Solid;
            string label = d.ToString(format);
            Vector2 mid_leader = (contact.Target +contact.Source).Position; 
            canvas.DrawLabel(g, color, label, mid_leader, alignment: ContentAlignment.MiddleCenter);
            canvas.FillPoint(g, Color.Black, contact.Source, 4f);
            canvas.FillPoint(g, Color.Black, contact.Target, 4f);
        }

        private void RenderFormHandler(object sender, PaintEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.High;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // handle left, right and middle clicks
            // - left click/drag is pan
            // - middle click is rotate
            canvas.HandleMouse();

            sb.Append("Scene");
            // draw all objects in scene
            canvas.DrawObjects(e.Graphics);

            // draw closest distance arrrow
            VisualTriangle triangle = canvas.VisualObjects[0] as VisualTriangle;
            VisualTriangle other  = canvas.VisualObjects[1] as VisualTriangle;
            VisualCircle circle = canvas.VisualObjects[2] as VisualCircle;

            DrawClosestDistance(e.Graphics, Color.DarkGray, "g4", triangle, other, out _);

            //float d = contact.Distance;
            //sb.Append($", Pair: {contact.Source:g4}-{contact.Target:g4}, Distance: {d:g4}");

            DrawClosestDistance(e.Graphics, Color.DarkGoldenrod, "g4", other, circle, out _);

            DrawClosestDistance(e.Graphics, Color.DarkOliveGreen, "g4", circle, triangle, out _);

            // draw point at mouse position (filled/open)
            Point2 point = canvas.MouseMove;
            if (canvas.VisualObjects.Any(vo => vo.Geometry.Contains(point)))
            {
                sb.Append($", Inside");
                canvas.FillPoint(e.Graphics, Color.Red, point, 6f);
            }
            else
            {
                sb.Append($", Outside");
                canvas.DrawPoint(e.Graphics, Color.Red, point, 6f);
            }

            //var hit = source.Circle.Contains(point) || target.Circle.Contains(point);
            //if (hit)
            //{
            //    sb.Append($", Inside");
            //    canvas.FillPoint(e.Graphics, Color.Red, point, 6f);
            //}
            //else
            //{
            //    sb.Append($", Outside");
            //    canvas.DrawPoint(e.Graphics, Color.Red, point, 6f);
            //}

            statusLabel1.Text = sb.ToString();
        }
    }
}
