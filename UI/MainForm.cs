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
            float x = -1, y = -3;
            var triangle = new Triangle2(
                new Vector2(x + 0, y + 0),
                new Vector2(x + b, y + 0),
                new Vector2(x + 0, y + h));
            var other = new Triangle2(
                new Vector2(8, 0),
                new Vector2(9, 2),
                new Vector2(8, 2));

            canvas = new Canvas(this, 20);
            canvas.AddTriangle(Color.Blue, triangle);
            canvas.AddTriangle(Color.DarkCyan, other);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            this.Paint += RenderFormHandler;
        }        

        private void RenderFormHandler(object sender, PaintEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.High;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            VisualTriangle triangle = canvas.VisualObjects[0] as VisualTriangle;
            VisualTriangle other  = canvas.VisualObjects[1] as VisualTriangle;

            triangle.UpdatePosition(canvas);
            other.UpdatePosition(canvas);
            
            sb.Append("Scene");
            canvas.DrawObjects(e.Graphics);
            var n = triangle.Triangle.GetClosestPoint(other.Triangle);
            float d = n.Distance;

            sb.Append($", Pair: {n.Source:g4}-{n.Target:g4}, Distance: {d:g4}");

            Gdi.Stroke.DashStyle = DashStyle.Solid;
            canvas.DrawLine(e.Graphics, Color.DarkGray, n.Target, n.Source, false, true);
            Gdi.Stroke.DashStyle = DashStyle.Solid;
            string label = $"{d:g4}";
            Vector2 mid_leader = (n.Target +n.Source).Position; //0.5f*n.Target.Position+0.5f*n.Source.Position;
            canvas.DrawLabel(e.Graphics, Color.DarkGray, label, mid_leader, alignment:  ContentAlignment.MiddleCenter);
            canvas.FillPoint(e.Graphics, Color.Black, n.Source, 4f);
            canvas.FillPoint(e.Graphics, Color.Black, n.Target, 4f);

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

            //var hit = triangle.Triangle.Contains(point) || other.Triangle.Contains(point);
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
