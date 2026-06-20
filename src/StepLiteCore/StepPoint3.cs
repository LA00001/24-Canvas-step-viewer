namespace StepLiteCore
{
    public sealed class StepPoint3
    {
        public StepPoint3()
        {
        }

        public StepPoint3(long sourceId, double x, double y, double z)
        {
            SourceId = sourceId;
            X = x;
            Y = y;
            Z = z;
        }

        public long SourceId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
