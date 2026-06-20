using System.Collections.Generic;

namespace BoxGridCore
{
    public sealed class BoxGridQueryResult
    {
        public BoxGridQueryResult()
        {
            TouchedCells = new List<BoxGridCell>();
            CandidateItems = new List<BoxGridItem>();
            IntersectingItems = new List<BoxGridItem>();
            FullyInsideItems = new List<BoxGridItem>();
        }

        public List<BoxGridCell> TouchedCells { get; private set; }
        public List<BoxGridItem> CandidateItems { get; private set; }
        public List<BoxGridItem> IntersectingItems { get; private set; }
        public List<BoxGridItem> FullyInsideItems { get; private set; }
    }
}
