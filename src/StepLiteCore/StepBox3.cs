using System;

namespace StepLiteCore
{
    public sealed class StepBox3
    {
        public StepBox3()
        {
            MinX = MinY = MinZ = double.MaxValue;
            MaxX = MaxY = MaxZ = double.MinValue;
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

        public bool IsValid
        {
            get { return MinX != double.MaxValue && MaxX != double.MinValue; }
        }

        public void Include(StepPoint3 point)
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
    }
}
