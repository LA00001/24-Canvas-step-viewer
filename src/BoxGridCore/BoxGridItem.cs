using System.Collections.Generic;

namespace BoxGridCore
{
    public sealed class BoxGridItem
    {
        internal BoxGridItem()
        {
            CellIds = new List<string>();
        }

        public int Index { get; internal set; }
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public Box3 Bounds { get; internal set; }
        public object Tag { get; internal set; }
        public List<string> CellIds { get; private set; }

        public string CellIdsText
        {
            get
            {
                if (CellIds == null || CellIds.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(", ", CellIds.ToArray());
            }
        }
    }
}
