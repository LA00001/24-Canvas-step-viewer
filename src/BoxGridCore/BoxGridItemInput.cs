namespace BoxGridCore
{
    public sealed class BoxGridItemInput
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Box3 Bounds { get; set; }
        public object Tag { get; set; }
    }
}
