using System;
using System.Collections.Generic;
using BoxGridCore;
using Inventor;

namespace InventorIptOrg
{
    /// <summary>
    /// Пространственный индекс для IPT SurfaceBodies.
    ///
    /// Идея:
    /// 1) Вручную по кнопке берём RangeBox всей модели.
    /// 2) Делим его на N x N x N виртуальных ячеек.
    ///    Для первого практического режима N = 2, то есть 2x2x2 = 8 кубиков.
    /// 3) Каждое тело записываем во ВСЕ кубики, которые пересекает его body.RangeBox.
    /// 4) При рамочном выборе сначала ищем задетые кубики, потом проверяем только тела-кандидаты.
    ///
    /// Важно: кубики дают кандидатов, а не окончательный результат.
    /// Финальная проверка selectionLimits.Intersects(body.RangeBox, tolerance) всё равно остаётся.
    /// </summary>
    internal sealed class SpatialCubesIndex
    {
        private readonly Dictionary<string, SpatialCubeCell> _cellById = new Dictionary<string, SpatialCubeCell>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<IntPtr, SpatialBodyRecord> _bodyByIdentityKey = new Dictionary<IntPtr, SpatialBodyRecord>();

        private SpatialCubesIndex()
        {
            Cells = new List<SpatialCubeCell>();
            Bodies = new List<SpatialBodyRecord>();
            DivisionsPerAxis = 2;
            Tolerance = 0.001;
        }

        public bool IsReady { get; private set; }
        public int DivisionsPerAxis { get; private set; }
        public double Tolerance { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string DocumentDisplayName { get; private set; }
        public string DocumentFullFileName { get; private set; }
        public SpatialBox ModelBox { get; private set; }
        public List<SpatialCubeCell> Cells { get; private set; }
        public List<SpatialBodyRecord> Bodies { get; private set; }

        public static SpatialCubesIndex Build(
            PartDocument partDoc,
            int divisionsPerAxis,
            double tolerance,
            Func<object, IntPtr> identityKeyProvider,
            Func<SurfaceBody, int, string> displayNameProvider)
        {
            using (AppLogger.Scope("SpatialCubesIndex.Build"))
            {
                if (partDoc == null)
                {
                    throw new ArgumentNullException("partDoc");
                }

                if (divisionsPerAxis < 1)
                {
                    divisionsPerAxis = 1;
                }

                SpatialCubesIndex index = new SpatialCubesIndex();
                index.CreatedAt = DateTime.Now;
                index.DivisionsPerAxis = divisionsPerAxis;
                index.Tolerance = tolerance;
                index.DocumentDisplayName = SafeDocumentDisplayName(partDoc);
                index.DocumentFullFileName = SafeDocumentFullFileName(partDoc);

                SpatialBox modelBox = null;
                int bodyIndex = 0;

                foreach (SurfaceBody body in partDoc.ComponentDefinition.SurfaceBodies)
                {
                    if (body == null)
                    {
                        continue;
                    }

                    Box inventorBox = null;

                    try
                    {
                        inventorBox = body.RangeBox;
                    }
                    catch
                    {
                        inventorBox = null;
                    }

                    if (inventorBox == null)
                    {
                        continue;
                    }

                    SpatialBox bodyBox = SpatialBox.FromInventorBox(inventorBox);
                    bodyIndex++;

                    IntPtr key = IntPtr.Zero;
                    if (identityKeyProvider != null)
                    {
                        try
                        {
                            key = identityKeyProvider(body);
                        }
                        catch
                        {
                            key = IntPtr.Zero;
                        }
                    }

                    string displayName = "SurfaceBody " + bodyIndex.ToString();
                    if (displayNameProvider != null)
                    {
                        try
                        {
                            displayName = displayNameProvider(body, bodyIndex);
                        }
                        catch
                        {
                            displayName = "SurfaceBody " + bodyIndex.ToString();
                        }
                    }

                    SpatialBodyRecord record = new SpatialBodyRecord();
                    record.BodyIndex = bodyIndex;
                    record.Body = body;
                    record.IdentityKey = key;
                    record.DisplayName = displayName;
                    record.BodyBox = bodyBox;
                    record.CubeIds = new List<string>();

                    index.Bodies.Add(record);

                    if (key != IntPtr.Zero && !index._bodyByIdentityKey.ContainsKey(key))
                    {
                        index._bodyByIdentityKey.Add(key, record);
                    }

                    if (modelBox == null)
                    {
                        modelBox = bodyBox.Clone();
                    }
                    else
                    {
                        modelBox.Include(bodyBox);
                    }
                }

                if (modelBox == null)
                {
                    modelBox = new SpatialBox(0, 0, 0, 1, 1, 1);
                }

                modelBox.EnsureNonZeroSize(1.0);
                index.ModelBox = modelBox;
                index.BuildCellsWithBoxGridCore();
                index.IsReady = true;

                AppLogger.Log(
                    "SPATIAL_CUBES_COMPILE_FIX_BUILT",
                    "SpatialCubesIndex.Build",
                    "Bodies=" + index.Bodies.Count.ToString() +
                    "; Cells=" + index.Cells.Count.ToString() +
                    "; DivisionsPerAxis=" + index.DivisionsPerAxis.ToString() +
                    "; Tolerance=" + index.Tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture));

                return index;
            }
        }

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / MatchesDocument — 0.000649 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Проверяет, что построенная база кубиков относится к текущему открытому PartDocument.
        public bool MatchesDocument(PartDocument partDoc)
        {
            using (AppLogger.Scope("SpatialCubesIndex.MatchesDocument"))
            {
                if (partDoc == null)
                {
                    return false;
                }

                string currentFullName = SafeDocumentFullFileName(partDoc);

                if (!string.IsNullOrWhiteSpace(DocumentFullFileName) || !string.IsNullOrWhiteSpace(currentFullName))
                {
                    return string.Equals(DocumentFullFileName, currentFullName, StringComparison.OrdinalIgnoreCase);
                }

                return string.Equals(DocumentDisplayName, SafeDocumentDisplayName(partDoc), StringComparison.OrdinalIgnoreCase);
            }
        }

        public SpatialBodyRecord FindBodyRecord(IntPtr identityKey)
        {
            using (AppLogger.Scope("SpatialCubesIndex.FindBodyRecord"))
            {
                if (identityKey == IntPtr.Zero)
                {
                    return null;
                }

                SpatialBodyRecord record;
                if (_bodyByIdentityKey.TryGetValue(identityKey, out record))
                {
                    return record;
                }

                return null;
            }
        }

        public SpatialCubeCell FindCell(string id)
        {
            using (AppLogger.Scope("SpatialCubesIndex.FindCell"))
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return null;
                }

                SpatialCubeCell cell;
                if (_cellById.TryGetValue(id, out cell))
                {
                    return cell;
                }

                return null;
            }
        }

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / SpatialCubesIndex.Query — 0.001361 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX В контрольном запуске Query по selectionLimits вернул 1 задетый кубик CUBE_X0_Y1_Z1 и 37 тел-кандидатов.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Это список кубиков, задетых рамкой, а не полный список кубиков каждого тела.
        public SpatialQueryResult Query(SpatialBox selectionBox)
        {
            using (AppLogger.Scope("SpatialCubesIndex.Query"))
            {
                SpatialQueryResult result = new SpatialQueryResult();
                result.TouchedCells = new List<SpatialCubeCell>();
                result.CandidateBodies = new List<SpatialBodyRecord>();

                if (!IsReady || selectionBox == null)
                {
                    return result;
                }

                IndexRange xr = GetIndexRange(selectionBox.MinX, selectionBox.MaxX, AxisKind.X);
                IndexRange yr = GetIndexRange(selectionBox.MinY, selectionBox.MaxY, AxisKind.Y);
                IndexRange zr = GetIndexRange(selectionBox.MinZ, selectionBox.MaxZ, AxisKind.Z);

                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / HashSet не даёт обработать одно тело дважды,
                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX если его RangeBox лежит сразу в нескольких задетых кубиках.
                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Пример: Тело 11 хранится в CUBE_X0_Y0_Z1 и CUBE_X0_Y1_Z1; если Query заденет оба, тело всё равно попадёт в CandidateBodies один раз.
                HashSet<int> bodyIndexes = new HashSet<int>();

                for (int ix = xr.Min; ix <= xr.Max; ix++)
                {
                    for (int iy = yr.Min; iy <= yr.Max; iy++)
                    {
                        for (int iz = zr.Min; iz <= zr.Max; iz++)
                        {
                            string id = SpatialCubeCell.MakeId(ix, iy, iz);
                            SpatialCubeCell cell;
                            if (!_cellById.TryGetValue(id, out cell))
                            {
                                continue;
                            }

                            // Кубик может попасть в диапазон по индексам, но из-за допуска безопаснее оставить
                            // дополнительную проверку пересечения с реальной коробкой кубика.
                            if (!cell.Bounds.Intersects(selectionBox, Tolerance))
                            {
                                continue;
                            }

                            result.TouchedCells.Add(cell);

                            foreach (SpatialBodyRecord record in cell.Bodies)
                            {
                                if (record == null)
                                {
                                    continue;
                                }

                                if (bodyIndexes.Add(record.BodyIndex))
                                {
                                    result.CandidateBodies.Add(record);
                                }
                            }
                        }
                    }
                }

                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / Query возвращает не окончательный выбор, а candidateBodies.
                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Финальное решение остаётся в OnSelect через selectionLimits.Intersects(bodyRangeBox, 0.001).
                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Поэтому "задето кубиков: 1" означает только кубики selectionLimits, а строка тела "cubes=2" означает принадлежность тела к двум кубикам индекса.
                result.CandidateBodiesFromCellsBeforeBodyFilter = result.CandidateBodies.Count;
                result.RejectedBodiesByBodyFilter = 0;
                result.HitMode = "SpatialBoxIntersectsCells";
                result.BodyFilterMode = "FinalSelectionLimitsIntersectsBodyRangeBoxInOnSelect";
                return result;
            }
        }

        // 20:55 18.06.2026 InventorIptOrg_v0_4_73_BOXGRID_CORE_DLL
        // Адаптер приложения: берём уже прочитанные body boxes и передаём их в нейтральную DLL BoxGridCore.
        // Ядро DLL ничего не знает о конкретном источнике объектов: только Box3 + items + divisions + tolerance.
        private void BuildCellsWithBoxGridCore()
        {
            using (AppLogger.Scope("SpatialCubesIndex.BuildCellsWithBoxGridCore"))
            {
                Cells.Clear();
                _cellById.Clear();

                List<BoxGridItemInput> inputs = new List<BoxGridItemInput>();

                foreach (SpatialBodyRecord record in Bodies)
                {
                    if (record == null || record.BodyBox == null)
                    {
                        continue;
                    }

                    BoxGridItemInput input = new BoxGridItemInput();
                    input.Id = record.BodyIndex.ToString();
                    input.Name = record.DisplayName;
                    input.Bounds = new Box3(
                        record.BodyBox.MinX,
                        record.BodyBox.MinY,
                        record.BodyBox.MinZ,
                        record.BodyBox.MaxX,
                        record.BodyBox.MaxY,
                        record.BodyBox.MaxZ);
                    input.Tag = record.BodyIndex;
                    inputs.Add(input);

                    if (record.CubeIds == null)
                    {
                        record.CubeIds = new List<string>();
                    }
                    else
                    {
                        record.CubeIds.Clear();
                    }
                }

                BoxGridBuilder builder = new BoxGridBuilder();
                BoxGrid grid = builder.Build(inputs, DivisionsPerAxis, Tolerance);

                Dictionary<int, SpatialBodyRecord> recordByBodyIndex = new Dictionary<int, SpatialBodyRecord>();

                foreach (SpatialBodyRecord record in Bodies)
                {
                    if (record != null && !recordByBodyIndex.ContainsKey(record.BodyIndex))
                    {
                        recordByBodyIndex.Add(record.BodyIndex, record);
                    }
                }

                foreach (BoxGridCell gridCell in grid.Cells)
                {
                    SpatialCubeCell cell = new SpatialCubeCell(
                        gridCell.Ix,
                        gridCell.Iy,
                        gridCell.Iz,
                        new SpatialBox(
                            gridCell.Bounds.MinX,
                            gridCell.Bounds.MinY,
                            gridCell.Bounds.MinZ,
                            gridCell.Bounds.MaxX,
                            gridCell.Bounds.MaxY,
                            gridCell.Bounds.MaxZ));

                    Cells.Add(cell);
                    _cellById[cell.Id] = cell;

                    foreach (BoxGridItem gridItem in gridCell.Items)
                    {
                        int bodyIndex;

                        if (!int.TryParse(gridItem.Id, out bodyIndex))
                        {
                            continue;
                        }

                        SpatialBodyRecord record;

                        if (!recordByBodyIndex.TryGetValue(bodyIndex, out record) || record == null)
                        {
                            continue;
                        }

                        cell.Bodies.Add(record);

                        if (record.CubeIds == null)
                        {
                            record.CubeIds = new List<string>();
                        }

                        if (!record.CubeIds.Contains(cell.Id))
                        {
                            record.CubeIds.Add(cell.Id);
                        }
                    }
                }

                AppLogger.Log(
                    "BOXGRID_CORE_DLL_USED",
                    "SpatialCubesIndex.BuildCellsWithBoxGridCore",
                    "InputItems=" + inputs.Count.ToString() +
                    "; Cells=" + Cells.Count.ToString() +
                    "; DivisionsPerAxis=" + DivisionsPerAxis.ToString() +
                    "; Tolerance=" + Tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "; CoreAssembly=BoxGridCore");
            }
        }

        private void CreateCells()
        {
            using (AppLogger.Scope("SpatialCubesIndex.CreateCells"))
            {
                Cells.Clear();
                _cellById.Clear();

                double sx = SafeCellSize(ModelBox.SizeX, DivisionsPerAxis);
                double sy = SafeCellSize(ModelBox.SizeY, DivisionsPerAxis);
                double sz = SafeCellSize(ModelBox.SizeZ, DivisionsPerAxis);

                for (int ix = 0; ix < DivisionsPerAxis; ix++)
                {
                    for (int iy = 0; iy < DivisionsPerAxis; iy++)
                    {
                        for (int iz = 0; iz < DivisionsPerAxis; iz++)
                        {
                            double minX = ModelBox.MinX + sx * ix;
                            double minY = ModelBox.MinY + sy * iy;
                            double minZ = ModelBox.MinZ + sz * iz;

                            double maxX = (ix == DivisionsPerAxis - 1) ? ModelBox.MaxX : minX + sx;
                            double maxY = (iy == DivisionsPerAxis - 1) ? ModelBox.MaxY : minY + sy;
                            double maxZ = (iz == DivisionsPerAxis - 1) ? ModelBox.MaxZ : minZ + sz;

                            SpatialCubeCell cell = new SpatialCubeCell(ix, iy, iz, new SpatialBox(minX, minY, minZ, maxX, maxY, maxZ));
                            Cells.Add(cell);
                            _cellById[cell.Id] = cell;
                        }
                    }
                }
            }
        }

        private void FillCellsWithBodies()
        {
            using (AppLogger.Scope("SpatialCubesIndex.FillCellsWithBodies"))
            {
                foreach (SpatialBodyRecord record in Bodies)
                {
                    if (record == null || record.BodyBox == null)
                    {
                        continue;
                    }

                    IndexRange xr = GetIndexRange(record.BodyBox.MinX, record.BodyBox.MaxX, AxisKind.X);
                    IndexRange yr = GetIndexRange(record.BodyBox.MinY, record.BodyBox.MaxY, AxisKind.Y);
                    IndexRange zr = GetIndexRange(record.BodyBox.MinZ, record.BodyBox.MaxZ, AxisKind.Z);

                    for (int ix = xr.Min; ix <= xr.Max; ix++)
                    {
                        for (int iy = yr.Min; iy <= yr.Max; iy++)
                        {
                            for (int iz = zr.Min; iz <= zr.Max; iz++)
                            {
                                string id = SpatialCubeCell.MakeId(ix, iy, iz);
                                SpatialCubeCell cell;
                                if (!_cellById.TryGetValue(id, out cell))
                                {
                                    continue;
                                }

                                // Тело пишется во все кубики, которые пересекает его RangeBox.
                                // Это не даёт потерять длинные/большие тела на границе ячеек.
                                if (!cell.Bounds.Intersects(record.BodyBox, Tolerance))
                                {
                                    continue;
                                }

                                cell.Bodies.Add(record);
                                record.CubeIds.Add(cell.Id);
                            }
                        }
                    }
                }
            }
        }

        private IndexRange GetIndexRange(double minValue, double maxValue, AxisKind axis)
        {
            double modelMin;
            double cellSize;

            if (axis == AxisKind.Y)
            {
                modelMin = ModelBox.MinY;
                cellSize = SafeCellSize(ModelBox.SizeY, DivisionsPerAxis);
            }
            else if (axis == AxisKind.Z)
            {
                modelMin = ModelBox.MinZ;
                cellSize = SafeCellSize(ModelBox.SizeZ, DivisionsPerAxis);
            }
            else
            {
                modelMin = ModelBox.MinX;
                cellSize = SafeCellSize(ModelBox.SizeX, DivisionsPerAxis);
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

        private static double SafeCellSize(double totalSize, int divisions)
        {
            double size = totalSize / Math.Max(1, divisions);
            if (Math.Abs(size) < 0.000000001)
            {
                return 1.0;
            }

            return size;
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

        private static string SafeDocumentDisplayName(PartDocument partDoc)
        {
            try
            {
                return Convert.ToString(partDoc.DisplayName);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SafeDocumentFullFileName(PartDocument partDoc)
        {
            try
            {
                return Convert.ToString(partDoc.FullFileName);
            }
            catch
            {
                return string.Empty;
            }
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

    internal sealed class SpatialQueryResult
    {
        public List<SpatialCubeCell> TouchedCells { get; set; }
        public List<SpatialBodyRecord> CandidateBodies { get; set; }
        public int CandidateBodiesFromCellsBeforeBodyFilter { get; set; }
        public int RejectedBodiesByBodyFilter { get; set; }
        public string HitMode { get; set; }
        public string BodyFilterMode { get; set; }
        public string RequestedBodySelectionMode { get; set; }
        public string ResolvedBodySelectionMode { get; set; }
        public double AutoWholeCellCoveragePercent { get; set; }
        public string ProjectionMode { get; set; }
        public string ProjectionDetails { get; set; }
        public int ProjectionFallbackCount { get; set; }
        public int ProjectedCubeRectCount { get; set; }
        public int ProjectedBodyRectCount { get; set; }
    }

    internal sealed class SpatialCubeCell
    {
        public SpatialCubeCell(int ix, int iy, int iz, SpatialBox bounds)
        {
            Ix = ix;
            Iy = iy;
            Iz = iz;
            Id = MakeId(ix, iy, iz);
            Bounds = bounds;
            Bodies = new List<SpatialBodyRecord>();
        }

        public int Ix { get; private set; }
        public int Iy { get; private set; }
        public int Iz { get; private set; }
        public string Id { get; private set; }
        public SpatialBox Bounds { get; private set; }
        public List<SpatialBodyRecord> Bodies { get; private set; }

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

    internal sealed class SpatialBodyRecord
    {
        public int BodyIndex { get; set; }
        public SurfaceBody Body { get; set; }
        public IntPtr IdentityKey { get; set; }
        public string DisplayName { get; set; }
        public SpatialBox BodyBox { get; set; }
        public List<string> CubeIds { get; set; }

        public string CubeIdsText
        {
            get
            {
                if (CubeIds == null || CubeIds.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(", ", CubeIds.ToArray());
            }
        }
    }

    internal sealed class SpatialBox
    {
        public SpatialBox(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            MinX = Math.Min(minX, maxX);
            MinY = Math.Min(minY, maxY);
            MinZ = Math.Min(minZ, maxZ);
            MaxX = Math.Max(minX, maxX);
            MaxY = Math.Max(minY, maxY);
            MaxZ = Math.Max(minZ, maxZ);
        }

        public double MinX { get; private set; }
        public double MinY { get; private set; }
        public double MinZ { get; private set; }
        public double MaxX { get; private set; }
        public double MaxY { get; private set; }
        public double MaxZ { get; private set; }

        public double SizeX { get { return MaxX - MinX; } }
        public double SizeY { get { return MaxY - MinY; } }
        public double SizeZ { get { return MaxZ - MinZ; } }
        public double Volume { get { return Math.Max(0.0, SizeX) * Math.Max(0.0, SizeY) * Math.Max(0.0, SizeZ); } }

        public static SpatialBox FromInventorBox(Box box)
        {
            if (box == null)
            {
                return null;
            }

            Inventor.Point min = box.MinPoint;
            Inventor.Point max = box.MaxPoint;
            return new SpatialBox(min.X, min.Y, min.Z, max.X, max.Y, max.Z);
        }

        public SpatialBox Clone()
        {
            return new SpatialBox(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
        }

        public void Include(SpatialBox other)
        {
            if (other == null)
            {
                return;
            }

            MinX = Math.Min(MinX, other.MinX);
            MinY = Math.Min(MinY, other.MinY);
            MinZ = Math.Min(MinZ, other.MinZ);
            MaxX = Math.Max(MaxX, other.MaxX);
            MaxY = Math.Max(MaxY, other.MaxY);
            MaxZ = Math.Max(MaxZ, other.MaxZ);
        }

        public void EnsureNonZeroSize(double fallbackSize)
        {
            if (Math.Abs(SizeX) < 0.000000001)
            {
                MaxX = MinX + fallbackSize;
            }

            if (Math.Abs(SizeY) < 0.000000001)
            {
                MaxY = MinY + fallbackSize;
            }

            if (Math.Abs(SizeZ) < 0.000000001)
            {
                MaxZ = MinZ + fallbackSize;
            }
        }

        public bool Intersects(SpatialBox other, double tolerance)
        {
            if (other == null)
            {
                return false;
            }

            return !(other.MaxX < MinX - tolerance || other.MinX > MaxX + tolerance ||
                     other.MaxY < MinY - tolerance || other.MinY > MaxY + tolerance ||
                     other.MaxZ < MinZ - tolerance || other.MinZ > MaxZ + tolerance);
        }
    }
}
