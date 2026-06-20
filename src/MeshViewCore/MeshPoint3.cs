namespace MeshViewCore
{
    public sealed class MeshPoint3
    {
        public MeshPoint3()
        {
        }

        public MeshPoint3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
