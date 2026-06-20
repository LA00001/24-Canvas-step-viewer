using System;

namespace BoxGridCore
{
    public sealed class Box3
    {
        public Box3(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            MinX = Math.Min(minX, maxX);
            MinY = Math.Min(minY, maxY);
            MinZ = Math.Min(minZ, maxZ);
            MaxX = Math.Max(minX, maxX);
            MaxY = Math.Max(minY, maxY);
            MaxZ = Math.Max(minZ, maxZ);
        }

        public double MinX { get; private set; }
        public double MinY { get; private set; }
        public double MinZ { get; private set; }
        public double MaxX { get; private set; }
        public double MaxY { get; private set; }
        public double MaxZ { get; private set; }

        public double SizeX { get { return MaxX - MinX; } }
        public double SizeY { get { return MaxY - MinY; } }
        public double SizeZ { get { return MaxZ - MinZ; } }

        public double Volume
        {
            get
            {
                return Math.Max(0.0, SizeX) * Math.Max(0.0, SizeY) * Math.Max(0.0, SizeZ);
            }
        }

        public Box3 Clone()
        {
            return new Box3(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
        }

        public void Include(Box3 other)
        {
            if (other == null)
            {
                return;
            }

            MinX = Math.Min(MinX, other.MinX);
            MinY = Math.Min(MinY, other.MinY);
            MinZ = Math.Min(MinZ, other.MinZ);
            MaxX = Math.Max(MaxX, other.MaxX);
            MaxY = Math.Max(MaxY, other.MaxY);
            MaxZ = Math.Max(MaxZ, other.MaxZ);
        }

        public void EnsureNonZeroSize(double fallbackSize)
        {
            if (Math.Abs(SizeX) < 0.000000001)
            {
                MaxX = MinX + fallbackSize;
            }

            if (Math.Abs(SizeY) < 0.000000001)
            {
                MaxY = MinY + fallbackSize;
            }

            if (Math.Abs(SizeZ) < 0.000000001)
            {
                MaxZ = MinZ + fallbackSize;
            }
        }

        public bool Intersects(Box3 other, double tolerance)
        {
            if (other == null)
            {
                return false;
            }

            return !(other.MaxX < MinX - tolerance || other.MinX > MaxX + tolerance ||
                     other.MaxY < MinY - tolerance || other.MinY > MaxY + tolerance ||
                     other.MaxZ < MinZ - tolerance || other.MinZ > MaxZ + tolerance);
        }

        public bool Contains(Box3 other, double tolerance)
        {
            if (other == null)
            {
                return false;
            }

            return other.MinX >= MinX - tolerance && other.MaxX <= MaxX + tolerance &&
                   other.MinY >= MinY - tolerance && other.MaxY <= MaxY + tolerance &&
                   other.MinZ >= MinZ - tolerance && other.MaxZ <= MaxZ + tolerance;
        }

        public override string ToString()
        {
            return "X[" + MinX.ToString("0.###") + ".." + MaxX.ToString("0.###") + "] " +
                   "Y[" + MinY.ToString("0.###") + ".." + MaxY.ToString("0.###") + "] " +
                   "Z[" + MinZ.ToString("0.###") + ".." + MaxZ.ToString("0.###") + "]";
        }
    }
}
