using System;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Circle2 : 
        IGeometry,
        IFormattable
    {
        #region Factory
        public Circle2(Point2 center, float radius)
        {
            Center=center;
            Radius=radius;
        } 
        #endregion

        #region Properties
        public Point2 Center { get; }
        public float Radius { get; } 
        #endregion

        #region Formatting
        public string ToString(string format, IFormatProvider formatProvider)
           => $"Circle({Center.ToString(format, formatProvider)}, {Radius.ToString(format, formatProvider)})";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g");
        #endregion

        #region Geometry
        public Circle2 Offset(Vector2 delta)
            => new Circle2(Center + delta, Radius);
        public Circle2 Rotate(Point2 pivot, float angle) 
            => new Circle2(
                LinearAlgebra.RotateAbout(Center, pivot, angle),
                Radius);

        public static Circle2 operator +(Circle2 circle, Vector2 delta)
            => circle.Offset(delta);
        public static Circle2 operator -(Circle2 circle, Vector2 delta)
            => circle.Offset(-delta);

        public bool Contains(Point2 point) => Center.DistanceTo(point) <= Radius;
        public float DistanceTo(Point2 other) => throw new NotImplementedException();
        public Contact GetClosestPoint(Point2 target)
        {
            var direction = Vector2.Normalize( Point2.Span(Center, target) );
            var point = Center - Radius * direction;
            return new Contact(point, target);
        }
        public Contact GetClosestPoint(Circle2 target)
        {
            var contact = target.GetClosestPoint(Center);
            return GetClosestPoint(contact.Source);
        }
        public Contact GetClosestPoint(Triangle2 target)
        {
            var contact = target.GetClosestPoint(Center);
            return GetClosestPoint(contact.Source);
        }

        #endregion
    }

}
