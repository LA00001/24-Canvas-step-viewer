using System.Collections.Generic;

namespace BoxGridCore
{
    public sealed class BoxGridCell
    {
        internal BoxGridCell(int ix, int iy, int iz, Box3 bounds)
        {
            Ix = ix;
            Iy = iy;
            Iz = iz;
            Id = MakeId(ix, iy, iz);
            Bounds = bounds;
            Items = new List<BoxGridItem>();
        }

        public int Ix { get; private set; }
        public int Iy { get; private set; }
        public int Iz { get; private set; }
        public string Id { get; private set; }
        public Box3 Bounds { get; private set; }
        public List<BoxGridItem> Items { get; private set; }

        public double Volume
        {
            get { return Bounds == null ? 0.0 : Bounds.Volume; }
        }

        public static string MakeId(int ix, int iy, int iz)
        {
            return "CUBE_X" + ix.ToString() + "_Y" + iy.ToString() + "_Z" + iz.ToString();
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
