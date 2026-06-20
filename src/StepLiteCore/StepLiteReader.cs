using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace StepLiteCore
{
    public sealed class StepLiteReader
    {
        private sealed class PolyLoopRecord
        {
            public long Id { get; set; }
            public List<long> PointIds { get; set; }
        }

        private sealed class FaceBoundRecord
        {
            public long Id { get; set; }
            public long LoopId { get; set; }
            public bool Orientation { get; set; }
            public bool IsOuter { get; set; }
        }

        private sealed class AdvancedFaceRecord
        {
            public long Id { get; set; }
            public List<long> BoundIds { get; set; }
            public bool SameSense { get; set; }
        }

        private sealed class OrientedLoopRecord
        {
            public long LoopId { get; set; }
            public long FaceId { get; set; }
            public long BoundId { get; set; }
            public List<long> PointIds { get; set; }
            public bool Reverse { get; set; }
            public bool IsOuter { get; set; }
        }

        private static readonly Regex EntityRegex = new Regex(
            @"#(?<id>\d+)\s*=\s*(?<body>.*?);",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex CartesianPointRegex = new Regex(
            @"CARTESIAN_POINT\s*\(.*?\(\s*(?<x>[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?)\s*,\s*(?<y>[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?)\s*,\s*(?<z>[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?)\s*\)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex VertexPointRegex = new Regex(
            @"VERTEX_POINT\s*\([^,]*,\s*#(?<point>\d+)\s*\)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex EdgeCurveRegex = new Regex(
            @"EDGE_CURVE\s*\([^,]*,\s*#(?<a>\d+)\s*,\s*#(?<b>\d+)\s*,",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex PolyLoopRegex = new Regex(
            @"POLY_LOOP\s*\([^,]*,\s*\((?<refs>[^\)]*)\)\s*\)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex FaceBoundRegex = new Regex(
            @"FACE_(?<outer>OUTER_)?BOUND\s*\([^,]*,\s*#(?<loop>\d+)\s*,\s*(?<orientation>\.[TF]\.)\s*\)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex AdvancedFaceRegex = new Regex(
            @"ADVANCED_FACE\s*\([^,]*,\s*\((?<bounds>[^\)]*)\)\s*,\s*#(?<surface>\d+)\s*,\s*(?<same>\.[TF]\.)\s*\)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RefRegex = new Regex(
            @"#(?<id>\d+)",
            RegexOptions.Compiled);

        private static readonly Regex FileSchemaRegex = new Regex(
            @"FILE_SCHEMA\s*\(\s*\(\s*'(?<schema>[^']+)'",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public StepLiteScene Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is empty.", "path");
            }

            string text = File.ReadAllText(path);
            return Parse(text, path);
        }

        public StepLiteScene Parse(string text, string sourcePath)
        {
            StepLiteScene scene = new StepLiteScene();
            scene.SourcePath = sourcePath;
            scene.Name = string.IsNullOrWhiteSpace(sourcePath) ? "STEP local scene" : Path.GetFileName(sourcePath);

            if (string.IsNullOrEmpty(text))
            {
                return scene;
            }

            Match schemaMatch = FileSchemaRegex.Match(text);

            if (schemaMatch.Success)
            {
                scene.FileSchema = schemaMatch.Groups["schema"].Value;
            }

            Dictionary<long, StepPoint3> pointById = new Dictionary<long, StepPoint3>();
            Dictionary<long, long> vertexToPointId = new Dictionary<long, long>();
            List<Tuple<long, long>> edgeVertexIds = new List<Tuple<long, long>>();
            Dictionary<long, PolyLoopRecord> polyLoopById = new Dictionary<long, PolyLoopRecord>();
            Dictionary<long, FaceBoundRecord> faceBoundById = new Dictionary<long, FaceBoundRecord>();
            List<AdvancedFaceRecord> advancedFaces = new List<AdvancedFaceRecord>();

            MatchCollection matches = EntityRegex.Matches(text);

            foreach (Match match in matches)
            {
                long id;

                if (!long.TryParse(
                    match.Groups["id"].Value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out id))
                {
                    continue;
                }

                string body = match.Groups["body"].Value ?? string.Empty;

                Match cp = CartesianPointRegex.Match(body);

                if (cp.Success)
                {
                    scene.CartesianPointEntityCount++;

                    double x = ParseDouble(cp.Groups["x"].Value);
                    double y = ParseDouble(cp.Groups["y"].Value);
                    double z = ParseDouble(cp.Groups["z"].Value);

                    pointById[id] = new StepPoint3(id, x, y, z);
                    continue;
                }

                Match polyLoop = PolyLoopRegex.Match(body);

                if (polyLoop.Success)
                {
                    List<long> loop = ParseReferences(polyLoop.Groups["refs"].Value);

                    if (loop.Count >= 3)
                    {
                        PolyLoopRecord record = new PolyLoopRecord();
                        record.Id = id;
                        record.PointIds = loop;
                        polyLoopById[id] = record;
                        scene.PolyLoopEntityCount++;
                    }

                    continue;
                }

                Match faceBound = FaceBoundRegex.Match(body);

                if (faceBound.Success)
                {
                    long loopId;

                    if (long.TryParse(
                        faceBound.Groups["loop"].Value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out loopId))
                    {
                        FaceBoundRecord record = new FaceBoundRecord();
                        record.Id = id;
                        record.LoopId = loopId;
                        record.Orientation = ParseStepBoolean(faceBound.Groups["orientation"].Value);
                        record.IsOuter = faceBound.Groups["outer"].Success;
                        faceBoundById[id] = record;

                        scene.FaceBoundEntityCount++;

                        if (record.IsOuter)
                        {
                            scene.FaceOuterBoundEntityCount++;
                        }
                        else
                        {
                            scene.FaceInnerBoundEntityCount++;
                        }

                        if (record.Orientation)
                        {
                            scene.FaceBoundOrientationTrueCount++;
                        }
                        else
                        {
                            scene.FaceBoundOrientationFalseCount++;
                        }
                    }

                    continue;
                }

                Match advancedFace = AdvancedFaceRegex.Match(body);

                if (advancedFace.Success)
                {
                    AdvancedFaceRecord record = new AdvancedFaceRecord();
                    record.Id = id;
                    record.BoundIds = ParseReferences(advancedFace.Groups["bounds"].Value);
                    record.SameSense = ParseStepBoolean(advancedFace.Groups["same"].Value);
                    advancedFaces.Add(record);

                    scene.AdvancedFaceEntityCount++;

                    if (record.SameSense)
                    {
                        scene.AdvancedFaceSameSenseTrueCount++;
                    }
                    else
                    {
                        scene.AdvancedFaceSameSenseFalseCount++;
                    }

                    continue;
                }

                Match vertex = VertexPointRegex.Match(body);

                if (vertex.Success)
                {
                    scene.VertexPointEntityCount++;

                    long pointId;

                    if (long.TryParse(
                        vertex.Groups["point"].Value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out pointId))
                    {
                        vertexToPointId[id] = pointId;
                    }

                    continue;
                }

                Match edge = EdgeCurveRegex.Match(body);

                if (edge.Success)
                {
                    scene.EdgeCurveEntityCount++;

                    long a;
                    long b;

                    if (long.TryParse(
                            edge.Groups["a"].Value,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out a) &&
                        long.TryParse(
                            edge.Groups["b"].Value,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out b))
                    {
                        edgeVertexIds.Add(new Tuple<long, long>(a, b));
                    }
                }
            }

            if (polyLoopById.Count > 0)
            {
                List<OrientedLoopRecord> orientedLoops = ResolveOrientedLoops(
                    scene,
                    polyLoopById,
                    faceBoundById,
                    advancedFaces);

                BuildSceneFromOrientedLoops(scene, pointById, orientedLoops);
                return scene;
            }

            BuildSceneFromLoosePointsAndEdgeCurves(
                scene,
                pointById,
                vertexToPointId,
                edgeVertexIds);

            return scene;
        }

        private static List<OrientedLoopRecord> ResolveOrientedLoops(
            StepLiteScene scene,
            Dictionary<long, PolyLoopRecord> polyLoopById,
            Dictionary<long, FaceBoundRecord> faceBoundById,
            List<AdvancedFaceRecord> advancedFaces)
        {
            List<OrientedLoopRecord> result = new List<OrientedLoopRecord>();
            HashSet<long> usedLoopIds = new HashSet<long>();

            foreach (AdvancedFaceRecord face in advancedFaces)
            {
                if (face == null || face.BoundIds == null)
                {
                    continue;
                }

                foreach (long boundId in face.BoundIds)
                {
                    FaceBoundRecord bound;

                    if (!faceBoundById.TryGetValue(boundId, out bound))
                    {
                        scene.UnresolvedFaceBoundReferenceCount++;
                        continue;
                    }

                    PolyLoopRecord loop;

                    if (!polyLoopById.TryGetValue(bound.LoopId, out loop))
                    {
                        scene.UnresolvedPolyLoopReferenceCount++;
                        continue;
                    }

                    // STEP orientation chain:
                    // FACE_BOUND.orientation controls use of POLY_LOOP direction.
                    // ADVANCED_FACE.same_sense flips the face orientation relative to its support surface.
                    // Effective forward winding is preserved when both flags are equal.
                    bool effectiveForward = bound.Orientation == face.SameSense;

                    OrientedLoopRecord oriented = new OrientedLoopRecord();
                    oriented.LoopId = loop.Id;
                    oriented.FaceId = face.Id;
                    oriented.BoundId = bound.Id;
                    oriented.PointIds = loop.PointIds;
                    oriented.Reverse = !effectiveForward;
                    oriented.IsOuter = bound.IsOuter;
                    result.Add(oriented);

                    usedLoopIds.Add(loop.Id);
                    scene.OrientedFaceLoopCount++;

                    if (oriented.Reverse)
                    {
                        scene.ReversedFaceLoopCount++;
                    }
                    else
                    {
                        scene.ForwardFaceLoopCount++;
                    }
                }
            }

            foreach (KeyValuePair<long, PolyLoopRecord> item in polyLoopById)
            {
                if (usedLoopIds.Contains(item.Key) || item.Value == null)
                {
                    continue;
                }

                OrientedLoopRecord orphan = new OrientedLoopRecord();
                orphan.LoopId = item.Value.Id;
                orphan.PointIds = item.Value.PointIds;
                orphan.Reverse = false;
                orphan.IsOuter = true;
                result.Add(orphan);
                scene.OrphanPolyLoopCount++;
            }

            return result;
        }

        private static void BuildSceneFromOrientedLoops(
            StepLiteScene scene,
            Dictionary<long, StepPoint3> pointById,
            List<OrientedLoopRecord> orientedLoops)
        {
            Dictionary<long, int> pointIdToIndex = new Dictionary<long, int>();
            HashSet<string> edgeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> triangleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (OrientedLoopRecord record in orientedLoops)
            {
                if (record == null || record.PointIds == null)
                {
                    continue;
                }

                List<int> loop = new List<int>();

                foreach (long pointId in record.PointIds)
                {
                    StepPoint3 point;

                    if (!pointById.TryGetValue(pointId, out point))
                    {
                        continue;
                    }

                    int index;

                    if (!pointIdToIndex.TryGetValue(pointId, out index))
                    {
                        index = scene.PointCount;
                        pointIdToIndex[pointId] = index;
                        scene.AddPoint(point);
                    }

                    if (loop.Count == 0 || loop[loop.Count - 1] != index)
                    {
                        loop.Add(index);
                    }
                }

                if (loop.Count > 1 && loop[0] == loop[loop.Count - 1])
                {
                    loop.RemoveAt(loop.Count - 1);
                }

                if (loop.Count < 3)
                {
                    continue;
                }

                if (record.Reverse)
                {
                    loop.Reverse();
                }

                for (int i = 0; i < loop.Count; i++)
                {
                    int a = loop[i];
                    int b = loop[(i + 1) % loop.Count];
                    AddUniqueEdge(scene, edgeKeys, a, b);
                }

                if (!record.IsOuter)
                {
                    // The current lightweight fan triangulator cannot subtract inner loops.
                    // Preserve the boundary edges, but do not triangulate the inner loop itself.
                    scene.InnerBoundTriangulationSkippedCount++;
                    continue;
                }

                for (int i = 1; i < loop.Count - 1; i++)
                {
                    AddUniqueTriangle(
                        scene,
                        triangleKeys,
                        loop[0],
                        loop[i],
                        loop[i + 1]);
                }
            }
        }

        private static void BuildSceneFromLoosePointsAndEdgeCurves(
            StepLiteScene scene,
            Dictionary<long, StepPoint3> pointById,
            Dictionary<long, long> vertexToPointId,
            List<Tuple<long, long>> edgeVertexIds)
        {
            List<long> ids = new List<long>(pointById.Keys);
            ids.Sort();

            Dictionary<long, int> pointIdToIndex = new Dictionary<long, int>();

            foreach (long id in ids)
            {
                StepPoint3 point = pointById[id];
                pointIdToIndex[id] = scene.PointCount;
                scene.AddPoint(point);
            }

            HashSet<string> edgeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Tuple<long, long> edge in edgeVertexIds)
            {
                long pointA;
                long pointB;

                if (!vertexToPointId.TryGetValue(edge.Item1, out pointA) ||
                    !vertexToPointId.TryGetValue(edge.Item2, out pointB))
                {
                    continue;
                }

                int indexA;
                int indexB;

                if (!pointIdToIndex.TryGetValue(pointA, out indexA) ||
                    !pointIdToIndex.TryGetValue(pointB, out indexB) ||
                    indexA == indexB)
                {
                    continue;
                }

                AddUniqueEdge(scene, edgeKeys, indexA, indexB);
            }
        }

        private static List<long> ParseReferences(string text)
        {
            List<long> result = new List<long>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return result;
            }

            foreach (Match match in RefRegex.Matches(text))
            {
                long id;

                if (long.TryParse(
                    match.Groups["id"].Value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out id))
                {
                    result.Add(id);
                }
            }

            return result;
        }

        private static bool ParseStepBoolean(string text)
        {
            return string.Equals(
                (text ?? string.Empty).Trim(),
                ".T.",
                StringComparison.OrdinalIgnoreCase);
        }

        private static void AddUniqueEdge(
            StepLiteScene scene,
            HashSet<string> edgeKeys,
            int a,
            int b)
        {
            if (a == b)
            {
                return;
            }

            int min = Math.Min(a, b);
            int max = Math.Max(a, b);
            string key =
                min.ToString(CultureInfo.InvariantCulture) + ":" +
                max.ToString(CultureInfo.InvariantCulture);

            if (edgeKeys.Add(key))
            {
                scene.AddEdge(new StepEdge(a, b));
            }
        }

        private static void AddUniqueTriangle(
            StepLiteScene scene,
            HashSet<string> triangleKeys,
            int a,
            int b,
            int c)
        {
            if (a == b || b == c || a == c)
            {
                return;
            }

            string key =
                a.ToString(CultureInfo.InvariantCulture) + ":" +
                b.ToString(CultureInfo.InvariantCulture) + ":" +
                c.ToString(CultureInfo.InvariantCulture);

            if (triangleKeys.Add(key))
            {
                scene.AddTriangle(new StepTriangle(a, b, c));
            }
        }

        private static double ParseDouble(string text)
        {
            return double.Parse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture);
        }
    }
}
