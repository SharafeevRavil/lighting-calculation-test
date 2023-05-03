using System.Collections.Generic;
using System.Linq;
using GeometRi;

namespace Utils
{
    public static class MeshCreation
    {
        public static IEnumerable<IReadOnlyList<Point3d>> CreateWalls(Point3d size)
        {
            return new[]
            {
                new List<Point3d>
                {
                    new(-size.X / 2, 0, -size.Z / 2), new(-size.X / 2, size.Y, -size.Z / 2),
                    new(-size.X / 2, size.Y, size.Z / 2), new(-size.X / 2, 0, size.Z / 2),
                },
                new List<Point3d>
                {
                    new(-size.X / 2, 0, size.Z / 2), new(-size.X / 2, size.Y, size.Z / 2),
                    new(size.X / 2, size.Y, size.Z / 2), new(size.X / 2, 0, size.Z / 2),
                },
                new List<Point3d>
                {
                    new(size.X / 2, 0, size.Z / 2), new(size.X / 2, size.Y, size.Z / 2),
                    new(size.X / 2, size.Y, -size.Z / 2), new(size.X / 2, 0, -size.Z / 2),
                },
                new List<Point3d>
                {
                    new(size.X / 2, 0, -size.Z / 2), new(size.X / 2, size.Y, -size.Z / 2),
                    new(-size.X / 2, size.Y, -size.Z / 2), new(-size.X / 2, 0, -size.Z / 2),
                },
                new List<Point3d>
                {
                    new(-size.X / 2, 0, -size.Z / 2), new(-size.X / 2, 0, size.Z / 2),
                    new(size.X / 2, 0, size.Z / 2), new(size.X / 2, 0, -size.Z / 2),
                },
                new List<Point3d>
                {
                    new(-size.X / 2, size.Y, -size.Z / 2), new(size.X / 2, size.Y, -size.Z / 2),
                    new(size.X / 2, size.Y, size.Z / 2), new(-size.X / 2, size.Y, size.Z / 2),
                },
            };
        }

        public static IEnumerable<IReadOnlyList<Point3d>> CreateCube(Point3d position, Point3d size)
        {
            return new List<IReadOnlyList<Point3d>>
            {
                new List<Point3d>
                {
                    position + new Point3d(size.X / 2, size.Y / 2, -size.Z / 2),
                    position + new Point3d(-size.X / 2, size.Y / 2, -size.Z / 2),
                    position + new Point3d(-size.X / 2, size.Y / 2, size.Z / 2),
                    position + new Point3d(size.X / 2, size.Y / 2, size.Z / 2)
                },
                new List<Point3d>
                {
                    position + new Point3d(size.X / 2, -size.Y / 2, -size.Z / 2),
                    position + new Point3d(size.X / 2, size.Y / 2, -size.Z / 2),
                    position + new Point3d(size.X / 2, size.Y / 2, size.Z / 2),
                    position + new Point3d(size.X / 2, -size.Y / 2, size.Z / 2)
                },
                new List<Point3d>
                {
                    position + new Point3d(-size.X / 2, -size.Y / 2, -size.Z / 2),
                    position + new Point3d(-size.X / 2, size.Y / 2, -size.Z / 2),
                    position + new Point3d(size.X / 2, size.Y / 2, -size.Z / 2),
                    position + new Point3d(size.X / 2, -size.Y / 2, -size.Z / 2)
                },
                new List<Point3d>
                {
                    position + new Point3d(-size.X / 2, -size.Y / 2, size.Z / 2),
                    position + new Point3d(-size.X / 2, size.Y / 2, size.Z / 2),
                    position + new Point3d(-size.X / 2, size.Y / 2, -size.Z / 2),
                    position + new Point3d(-size.X / 2, -size.Y / 2, -size.Z / 2)
                },
                new List<Point3d>
                {
                    position + new Point3d(size.X / 2, -size.Y / 2, size.Z / 2),
                    position + new Point3d(size.X / 2, size.Y / 2, size.Z / 2),
                    position + new Point3d(-size.X / 2, size.Y / 2, size.Z / 2),
                    position + new Point3d(-size.X / 2, -size.Y / 2, size.Z / 2)
                },
                new List<Point3d>
                {
                    position + new Point3d(size.X / 2, -size.Y / 2, size.Z / 2),
                    position + new Point3d(-size.X / 2, -size.Y / 2, size.Z / 2),
                    position + new Point3d(-size.X / 2, -size.Y / 2, -size.Z / 2),
                    position + new Point3d(size.X / 2, -size.Y / 2, -size.Z / 2)
                },
            };
        }
    }
}