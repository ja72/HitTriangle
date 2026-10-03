using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace JA.Geometry
{
    public interface ICheckContains
    {
        bool Contains(Point2 point);
    }

    public interface IMeasureDistance
    {
        float DistanceTo(Point2 other);
        Contact GetClosestPoint(Point2 other);
    }
    public interface IGeometry : ICheckContains, IMeasureDistance
    {
        // empty
    }

    public enum ReferenceAngle
    {
        HorizontalCCW,
        HorizontalCW,
        VerticalCCW,
        VerticalCW,

        Default = HorizontalCCW,
        Clock = VerticalCW
    }

    public static class Geometry
    {
        public static float Tolerance { get; } = 1e-6f;

        public static Vector2 Cartesian(float x, float y) => new Vector2(x, y);
        public static Vector2 Polar(float r, float θ, ReferenceAngle referenceAngle = ReferenceAngle.Default)
        {
            switch (referenceAngle)
            {
                case ReferenceAngle.HorizontalCCW:
                {
                    return new Vector2(r*(float)Math.Cos(θ), r*(float)Math.Sin(θ));
                }
                case ReferenceAngle.HorizontalCW:
                {
                    return new Vector2(r*(float)Math.Cos(θ), -r*(float)Math.Sin(θ));
                }
                case ReferenceAngle.VerticalCCW:
                {
                    return new Vector2(-r*(float)Math.Sin(θ), r*(float)Math.Cos(θ));
                }
                case ReferenceAngle.VerticalCW:
                {
                    return new Vector2(r*(float)Math.Sin(θ), r*(float)Math.Cos(θ));
                }
                default:
                throw new NotSupportedException();
            }
        }

        public static Contact GetClosestPoint(IGeometry source, IGeometry target)
        {
            if (source is Triangle2 sourceTriangle)
            {
                if (target is Triangle2 targetTriangle)
                {
                    return sourceTriangle.GetClosestPoint(targetTriangle);
                }
                else if (target is Circle2 targetCircle)
                {
                    return sourceTriangle.GetClosestPoint(targetCircle);
                }
            }
            else if (source is Circle2 sourceCircle)
            {
                if (target is Triangle2 targetTriangle)
                {
                    return sourceCircle.GetClosestPoint(targetTriangle);
                }
                else if (target is Circle2 targetCircle)
                {
                    return sourceCircle.GetClosestPoint(targetCircle);
                }
            }
            throw new NotImplementedException();
        }
    }
}