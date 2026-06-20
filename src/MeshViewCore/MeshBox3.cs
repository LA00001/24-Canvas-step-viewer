using System;

namespace MeshViewCore
{
    public sealed class MeshBox3
    {
        public MeshBox3()
        {
        }

        public MeshBox3(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            MinX = Math.Min(minX, maxX);
            MinY = Math.Min(minY, maxY);
            MinZ = Math.Min(minZ, maxZ);
            MaxX = Math.Max(minX, maxX);
            MaxY = Math.Max(minY, maxY);
            MaxZ = Math.Max(minZ, maxZ);
        }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MinZ { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MaxZ { get; set; }

        public double SizeX { get { return MaxX - MinX; } }
        public double SizeY { get { return MaxY - MinY; } }
        public double SizeZ { get { return MaxZ - MinZ; } }

        public void Include(MeshPoint3 point)
        {
            if (point == null)
            {
                return;
            }

            MinX = Math.Min(MinX, point.X);
            MinY = Math.Min(MinY, point.Y);
            MinZ = Math.Min(MinZ, point.Z);
            MaxX = Math.Max(MaxX, point.X);
            MaxY = Math.Max(MaxY, point.Y);
            MaxZ = Math.Max(MaxZ, point.Z);
        }

        public void Include(MeshBox3 box)
        {
            if (box == null)
            {
                return;
            }

            MinX = Math.Min(MinX, box.MinX);
            MinY = Math.Min(MinY, box.MinY);
            MinZ = Math.Min(MinZ, box.MinZ);
            MaxX = Math.Max(MaxX, box.MaxX);
            MaxY = Math.Max(MaxY, box.MaxY);
            MaxZ = Math.Max(MaxZ, box.MaxZ);
        }

        public static MeshBox3 FromPoints(System.Collections.Generic.IEnumerable<MeshPoint3> points)
        {
            MeshBox3 box = null;

            if (points != null)
            {
                foreach (MeshPoint3 point in points)
                {
                    if (point == null)
                    {
                        continue;
                    }

                    if (box == null)
                    {
                        box = new MeshBox3(point.X, point.Y, point.Z, point.X, point.Y, point.Z);
                    }
                    else
                    {
                        box.Include(point);
                    }
                }
            }

            if (box == null)
            {
                box = new MeshBox3(0, 0, 0, 1, 1, 1);
            }

            return box;
        }
    }
}
