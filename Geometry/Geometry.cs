using System;
using System.Collections.Generic;
using System.Linq;
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

    public static class Geometry
    {
        public static float Tolerance { get; } = 1e-6f;
    }
}
