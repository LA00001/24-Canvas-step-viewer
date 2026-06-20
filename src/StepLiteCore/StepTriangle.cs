namespace StepLiteCore
{
    public sealed class StepTriangle
    {
        public StepTriangle()
        {
        }

        public StepTriangle(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }
}
