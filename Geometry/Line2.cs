using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    using static LinearAlgebra;

    public readonly struct Line2 : 
        IGeometry, 
        IFormattable
    {
        #region Factory
        public Line2(Vector2 normal, float neg_offset) : this()
        {
            if (normal.LengthSquared() != 0)
            {
                float m = normal.Length();
                Vector = normal/m;
                Scalar = neg_offset/m;
            }
            else
            {
                Vector = normal;
                Scalar = neg_offset;
            }
            var direction = new Vector2(normal.Y, -normal.X);
            Direction = Vector2.Normalize(direction);
            Normal = Vector2.Normalize(normal);
        }
        public static Line2 Ray(Point2 origin, Vector2 direction)
        {
            direction = Vector2.Normalize(direction);
            var normal = new Vector2(-direction.Y, direction.X);
            return new Line2(
                normal,
                LinearAlgebra.Cross(origin.Position, normal));
        } 
        public static Line2 Zeros { get; } = new Line2(Vector2.Zero, 0);
        public static Line2 XAxis { get; } = new Line2(Vector2.UnitY, 0);
        public static Line2 YAxis { get; } = new Line2(-Vector2.UnitX, 0);
        public static Line2 AtInfinity { get; } = new Line2(Vector2.Zero, 1);
        #endregion

        #region Properties
        public Vector2 Vector { get; }
        public float Scalar { get; }
        public Vector2 Direction { get; }
        public Vector2 Normal { get; }
        #endregion

        #region Algebra
        public static Line2 operator *(float factor, Line2 A) => new Line2(factor*A.Vector, factor*A.Scalar);
        public static Line2 operator -(Line2 A) => new Line2(-A.Vector, -A.Scalar);
        public static Line2 operator +(Line2 A, Line2 B) => new Line2(A.Vector + B.Vector, A.Scalar + B.Scalar);
        public static Line2 operator -(Line2 A, Line2 B) => new Line2(A.Vector - B.Vector, A.Scalar - B.Scalar);

        #endregion

        #region Formatting
        public string ToString(string format, IFormatProvider formatProvider)
            => $"Line[{Vector.X.ToString(format, formatProvider)}x+{Vector.Y.ToString(format, formatProvider)}y+{Scalar.ToString(format, formatProvider)}=0]";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g"); 
        #endregion

        #region Geometry
        public static Line2 Offset(Line2 A, float distance) => new Line2(A.Vector, A.Scalar + distance * A.Vector.Length());
        public static Line2 Through(Line2 line, Point2 point) => Ray(point, line.Direction);

        /// <summary>
        /// Joints two points to form an infinite line.
        /// </summary>
        /// <param name="A">The first point</param>
        /// <param name="B">The second point</param>
        /// <returns>
        /// The <code>(a,b,c)</code> coordinates of the line
        /// such that <code>a*x+b*y+c=0</code> is the equation of the line.
        /// </returns>
        public static Line2 Join(Point2 A, Point2 B)
        {
            return new Line2(
                Cross(A.Vector, B.Scalar) + Cross(A.Scalar, B.Vector),
                Cross(A.Vector, B.Vector));
        }
        public Point2 PointAlong(float distance)
        {
            float v_sq = Vector.LengthSquared();
            float t = distance * LinearAlgebra.Sqrt(v_sq);
            return new Point2(-Vector * Scalar + LinearAlgebra.Cross(t, Vector), v_sq);
        }
        /// <summary>
        /// Projects a point onto the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="point">The point.</param>
        public Point2 Project(Point2 point)
        {
            float t = LinearAlgebra.Cross(Vector, point.Vector);
            return new Point2(
                -Vector * Scalar * point.Scalar + LinearAlgebra.Cross(t, Vector),
                Vector.LengthSquared() * point.Scalar);
        }

        public float DistanceTo(Point2 point)
        {
            return LinearAlgebra.Dot(this, point)
                / ( point.Scalar * Vector.Length() );
        }

        public bool Contains(Point2 point)
        {
            float d_sign = DistanceTo(point);

            return Math.Abs(d_sign) <= Geometry.Tolerance;
        }

        public Contact GetClosestPoint(Point2 other)
        {
            var hit = this.Project(other);
            //float d_hit = hit.DistanceTo(other);
            return new Contact(hit, other);

        }
        #endregion

    }
}
