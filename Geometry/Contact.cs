using System;
using System.Numerics;

namespace JA.Geometry
{
    public readonly struct Contact : IFormattable
    {
        public Contact(Vector2 source, Vector2 target) : this()
        {
            Source = source;
            Target = target;
            Distance = Vector2.Distance(Source, Target);
            Direction = (target - source) / Distance;
        }

        public Vector2 Source { get; }
        public Vector2 Target { get; }
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
