using System;
using System.Collections.Generic;

namespace BoxGridCore
{
    public sealed class BoxGridBuilder
    {
        public BoxGrid Build(IEnumerable<BoxGridItemInput> inputItems, int divisionsPerAxis, double tolerance)
        {
            if (divisionsPerAxis < 1)
            {
                divisionsPerAxis = 1;
            }

            List<BoxGridItem> items = new List<BoxGridItem>();
            Box3 modelBox = null;
            int index = 0;

            if (inputItems != null)
            {
                foreach (BoxGridItemInput input in inputItems)
                {
                    if (input == null || input.Bounds == null)
                    {
                        continue;
                    }

                    index++;

                    BoxGridItem item = new BoxGridItem();
                    item.Index = index;
                    item.Id = string.IsNullOrWhiteSpace(input.Id) ? index.ToString() : input.Id;
                    item.Name = input.Name ?? string.Empty;
                    item.Bounds = input.Bounds.Clone();
                    item.Tag = input.Tag;
                    items.Add(item);

                    if (modelBox == null)
                    {
                        modelBox = item.Bounds.Clone();
                    }
                    else
                    {
                        modelBox.Include(item.Bounds);
                    }
                }
            }

            if (modelBox == null)
            {
                modelBox = new Box3(0, 0, 0, 1, 1, 1);
            }

            modelBox.EnsureNonZeroSize(1.0);

            List<BoxGridCell> cells = CreateCells(modelBox, divisionsPerAxis);
            Dictionary<string, BoxGridCell> cellById = new Dictionary<string, BoxGridCell>(StringComparer.OrdinalIgnoreCase);

            foreach (BoxGridCell cell in cells)
            {
                cellById[cell.Id] = cell;
            }

            foreach (BoxGridItem item in items)
            {
                FillCellsForItem(modelBox, divisionsPerAxis, tolerance, cellById, item);
            }

            return new BoxGrid(modelBox, divisionsPerAxis, tolerance, cells, items);
        }

        private static List<BoxGridCell> CreateCells(Box3 modelBox, int divisionsPerAxis)
        {
            List<BoxGridCell> cells = new List<BoxGridCell>();
            double sx = BoxGrid.SafeCellSize(modelBox.SizeX, divisionsPerAxis);
            double sy = BoxGrid.SafeCellSize(modelBox.SizeY, divisionsPerAxis);
            double sz = BoxGrid.SafeCellSize(modelBox.SizeZ, divisionsPerAxis);

            for (int ix = 0; ix < divisionsPerAxis; ix++)
            {
                for (int iy = 0; iy < divisionsPerAxis; iy++)
                {
                    for (int iz = 0; iz < divisionsPerAxis; iz++)
                    {
                        double minX = modelBox.MinX + sx * ix;
                        double minY = modelBox.MinY + sy * iy;
                        double minZ = modelBox.MinZ + sz * iz;

                        double maxX = (ix == divisionsPerAxis - 1) ? modelBox.MaxX : minX + sx;
                        double maxY = (iy == divisionsPerAxis - 1) ? modelBox.MaxY : minY + sy;
                        double maxZ = (iz == divisionsPerAxis - 1) ? modelBox.MaxZ : minZ + sz;

                        cells.Add(new BoxGridCell(ix, iy, iz, new Box3(minX, minY, minZ, maxX, maxY, maxZ)));
                    }
                }
            }

            return cells;
        }

        private static void FillCellsForItem(
            Box3 modelBox,
            int divisionsPerAxis,
            double tolerance,
            Dictionary<string, BoxGridCell> cellById,
            BoxGridItem item)
        {
            if (item == null || item.Bounds == null)
            {
                return;
            }

            IndexRange xr = GetIndexRange(modelBox, divisionsPerAxis, tolerance, item.Bounds.MinX, item.Bounds.MaxX, AxisKind.X);
            IndexRange yr = GetIndexRange(modelBox, divisionsPerAxis, tolerance, item.Bounds.MinY, item.Bounds.MaxY, AxisKind.Y);
            IndexRange zr = GetIndexRange(modelBox, divisionsPerAxis, tolerance, item.Bounds.MinZ, item.Bounds.MaxZ, AxisKind.Z);

            for (int ix = xr.Min; ix <= xr.Max; ix++)
            {
                for (int iy = yr.Min; iy <= yr.Max; iy++)
                {
                    for (int iz = zr.Min; iz <= zr.Max; iz++)
                    {
                        string id = BoxGridCell.MakeId(ix, iy, iz);
                        BoxGridCell cell;

                        if (!cellById.TryGetValue(id, out cell))
                        {
                            continue;
                        }

                        if (!cell.Bounds.Intersects(item.Bounds, tolerance))
                        {
                            continue;
                        }

                        cell.Items.Add(item);
                        item.CellIds.Add(cell.Id);
                    }
                }
            }
        }

        private static IndexRange GetIndexRange(Box3 modelBox, int divisionsPerAxis, double tolerance, double minValue, double maxValue, AxisKind axis)
        {
            double modelMin;
            double cellSize;

            if (axis == AxisKind.Y)
            {
                modelMin = modelBox.MinY;
                cellSize = BoxGrid.SafeCellSize(modelBox.SizeY, divisionsPerAxis);
            }
            else if (axis == AxisKind.Z)
            {
                modelMin = modelBox.MinZ;
                cellSize = BoxGrid.SafeCellSize(modelBox.SizeZ, divisionsPerAxis);
            }
            else
            {
                modelMin = modelBox.MinX;
                cellSize = BoxGrid.SafeCellSize(modelBox.SizeX, divisionsPerAxis);
            }

            int minIndex = (int)Math.Floor(((minValue - tolerance) - modelMin) / cellSize);
            int maxIndex = (int)Math.Floor(((maxValue + tolerance) - modelMin) / cellSize);

            minIndex = ClampIndex(minIndex, divisionsPerAxis);
            maxIndex = ClampIndex(maxIndex, divisionsPerAxis);

            if (maxIndex < minIndex)
            {
                int temp = minIndex;
                minIndex = maxIndex;
                maxIndex = temp;
            }

            return new IndexRange(minIndex, maxIndex);
        }

        private static int ClampIndex(int index, int divisionsPerAxis)
        {
            if (index < 0)
            {
                return 0;
            }

            if (index >= divisionsPerAxis)
            {
                return divisionsPerAxis - 1;
            }

            return index;
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
