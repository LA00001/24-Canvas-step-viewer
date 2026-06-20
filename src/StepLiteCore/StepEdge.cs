namespace StepLiteCore
{
    public sealed class StepEdge
    {
        public StepEdge()
        {
        }

        public StepEdge(int a, int b)
        {
            A = a;
            B = b;
        }

        public int A { get; set; }
        public int B { get; set; }
    }
}
