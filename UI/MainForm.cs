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

        Triangle triangle;
        Triangle other;

        public MainForm()
        {
            InitializeComponent();

            float b = 5, h = 8;
            float x = -1, y = -3;
            triangle = new Triangle(
                new Vector2(x + 0, y + 0),
                new Vector2(x + b, y + 0),
                new Vector2(x + 0, y + h));
            other = new Triangle(
                new Vector2(8, 0),
                new Vector2(9, 2),
                new Vector2(8, 2));

            canvas = new Canvas(ClientRectangle, 20);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Resize += (obj, ev) =>
            {
                if (canvas.Target != ClientRectangle)
                {
                    canvas = new Canvas(ClientRectangle, 20);
                }
                Refresh();
            };

            this.KeyDown += (obj, ev) =>
            {
                if (ev.KeyData == Keys.Escape)
                {
                    Close();
                }
            };

            this.MouseMove += (obj, ev) =>
            {
                canvas.DoMouseMove(ev.Location, ev.Button);
                Refresh();
            };
            this.MouseDown += (obj, ev) =>
            {
                canvas.DoMouseDown(ev.Location, ev.Button);
                Refresh();
            };

            this.Paint += RenderFormHandler;
        }

        private void RenderFormHandler(object sender, PaintEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (canvas.Buttons == MouseButtons.Left)
            {
                if (other.Contains(canvas.MouseDown))
                {
                    var delta = canvas.MouseMove - canvas.MouseDown;
                    canvas.MouseDown = canvas.MouseMove;
                    other += delta;
                    Invalidate();
                }
                else if (triangle.Contains(canvas.MouseDown))
                {
                    var delta = canvas.MouseMove - canvas.MouseDown;
                    canvas.MouseDown = canvas.MouseMove;
                    triangle += delta;
                    Invalidate();
                }
            }
            if (canvas.Buttons == MouseButtons.Middle)
            {
                if (other.Contains(canvas.MouseDown))
                {
                    other = other.Rotate(canvas.MouseDown, 0.2f * LinearAlgebra.Deg);
                    Invalidate();
                } else if (triangle.Contains(canvas.MouseDown))
                {
                    triangle = triangle.Rotate(canvas.MouseDown, 0.2f * LinearAlgebra.Deg);
                    Invalidate();
                }
            }
            canvas.DrawTriangle(e.Graphics, Color.Blue, triangle);
            canvas.DrawTriangle(e.Graphics, Color.DarkGreen, other);
            sb.Append($"Triangle: {triangle:g4}");
            Vector2 point;
            if (canvas.Buttons == MouseButtons.Right)
            {
                var n = Pairs.Nearest(triangle, other);
                float d = Vector2.Distance(n.A, n.B);

                sb.Append($", Pair: {n.A:g4}-{n.B:g4}, Distance: {d:g4}");

                canvas.pen.DashStyle = DashStyle.Dash;
                canvas.DrawLine(e.Graphics, Color.Black, n.B, n.A, false, true);
                canvas.pen.DashStyle = DashStyle.Solid;
                canvas.DrawLabel(e.Graphics, Color.LightBlue, $"{d:g4}", n.B+0.2f*n.A, 0, 0, n.B.X-n.A.X >=0 ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight);
                canvas.FillPoint(e.Graphics, Color.Black, n.A, 4f);
                canvas.FillPoint(e.Graphics, Color.Black, n.B, 4f);
            }
            point = canvas.MouseMove;
            var hit = triangle.Contains(point);
            if (hit)
            {
                sb.Append($", Inside");
                canvas.FillPoint(e.Graphics, Color.Red, point, 6f);
            }
            else
            {
                sb.Append($", Outside");
                canvas.DrawPoint(e.Graphics, Color.Red, point, 6f);
            }

            statusLabel1.Text = sb.ToString();
        }
    }
}
