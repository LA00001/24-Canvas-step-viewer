using System;
using System.Collections.Generic;

namespace BoxGridCore
{
    public sealed class BoxGrid
    {
        private readonly Dictionary<string, BoxGridCell> _cellById;

        internal BoxGrid(Box3 bounds, int divisionsPerAxis, double tolerance, List<BoxGridCell> cells, List<BoxGridItem> items)
        {
            Bounds = bounds;
            DivisionsPerAxis = divisionsPerAxis;
            Tolerance = tolerance;
            Cells = cells ?? new List<BoxGridCell>();
            Items = items ?? new List<BoxGridItem>();
            _cellById = new Dictionary<string, BoxGridCell>(StringComparer.OrdinalIgnoreCase);

            foreach (BoxGridCell cell in Cells)
            {
                if (cell != null)
                {
                    _cellById[cell.Id] = cell;
                }
            }
        }

        public Box3 Bounds { get; private set; }
        public int DivisionsPerAxis { get; private set; }
        public double Tolerance { get; private set; }
        public List<BoxGridCell> Cells { get; private set; }
        public List<BoxGridItem> Items { get; private set; }

        public BoxGridCell FindCell(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            BoxGridCell cell;
            if (_cellById.TryGetValue(id, out cell))
            {
                return cell;
            }

            return null;
        }

        public BoxGridQueryResult Query(Box3 selection)
        {
            BoxGridQueryResult result = new BoxGridQueryResult();

            if (selection == null || Bounds == null)
            {
                return result;
            }

            IndexRange xr = GetIndexRange(selection.MinX, selection.MaxX, AxisKind.X);
            IndexRange yr = GetIndexRange(selection.MinY, selection.MaxY, AxisKind.Y);
            IndexRange zr = GetIndexRange(selection.MinZ, selection.MaxZ, AxisKind.Z);
            HashSet<int> indexes = new HashSet<int>();

            for (int ix = xr.Min; ix <= xr.Max; ix++)
            {
                for (int iy = yr.Min; iy <= yr.Max; iy++)
                {
                    for (int iz = zr.Min; iz <= zr.Max; iz++)
                    {
                        BoxGridCell cell;
                        string id = BoxGridCell.MakeId(ix, iy, iz);

                        if (!_cellById.TryGetValue(id, out cell))
                        {
                            continue;
                        }

                        if (!cell.Bounds.Intersects(selection, Tolerance))
                        {
                            continue;
                        }

                        result.TouchedCells.Add(cell);

                        foreach (BoxGridItem item in cell.Items)
                        {
                            if (item == null)
                            {
                                continue;
                            }

                            if (indexes.Add(item.Index))
                            {
                                result.CandidateItems.Add(item);

                                if (item.Bounds != null && item.Bounds.Intersects(selection, Tolerance))
                                {
                                    result.IntersectingItems.Add(item);
                                }

                                if (item.Bounds != null && selection.Contains(item.Bounds, Tolerance))
                                {
                                    result.FullyInsideItems.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private IndexRange GetIndexRange(double minValue, double maxValue, AxisKind axis)
        {
            double modelMin;
            double cellSize;

            if (axis == AxisKind.Y)
            {
                modelMin = Bounds.MinY;
                cellSize = SafeCellSize(Bounds.SizeY, DivisionsPerAxis);
            }
            else if (axis == AxisKind.Z)
            {
                modelMin = Bounds.MinZ;
                cellSize = SafeCellSize(Bounds.SizeZ, DivisionsPerAxis);
            }
            else
            {
                modelMin = Bounds.MinX;
                cellSize = SafeCellSize(Bounds.SizeX, DivisionsPerAxis);
            }

            int minIndex = (int)Math.Floor(((minValue - Tolerance) - modelMin) / cellSize);
            int maxIndex = (int)Math.Floor(((maxValue + Tolerance) - modelMin) / cellSize);

            minIndex = ClampIndex(minIndex);
            maxIndex = ClampIndex(maxIndex);

            if (maxIndex < minIndex)
            {
                int temp = minIndex;
                minIndex = maxIndex;
                maxIndex = temp;
            }

            return new IndexRange(minIndex, maxIndex);
        }

        private int ClampIndex(int index)
        {
            if (index < 0)
            {
                return 0;
            }

            if (index >= DivisionsPerAxis)
            {
                return DivisionsPerAxis - 1;
            }

            return index;
        }

        internal static double SafeCellSize(double totalSize, int divisions)
        {
            double size = totalSize / Math.Max(1, divisions);

            if (Math.Abs(size) < 0.000000001)
            {
                return 1.0;
            }

            return size;
        }

        private enum AxisKind
        {
            X,
            Y,
            Z
        }

        private sealed class IndexRange
        {
            public IndexRange(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public int Min { get; private set; }
            public int Max { get; private set; }
        }
    }
}
