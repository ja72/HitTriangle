using System;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Contact : IFormattable
    {
        public Contact(Point2 source, Point2 target) : this()
        {
            Source = source;
            Target = target;
            Distance = Source.DistanceTo(Target);
            Direction = (target - source) / Distance;
        }

        public Point2 Source { get; }
        public Point2 Target { get; }
        public Vector2 Direction { get; }
        public float Distance { get; }

        public Contact Flip() => new Contact(Target, Source);

        public string ToString(string format, IFormatProvider formatProvider) 
            => $"Contact(Source={Source.ToString(format, formatProvider)}, Target={Target.ToString(format, formatProvider)})";
        public string ToString(string format)
            => ToString(format, null);
        public override string ToString()
            => ToString("g");

    }

}
