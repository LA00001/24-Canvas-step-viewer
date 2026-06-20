using System.Collections.Generic;

namespace StepLiteCore
{
    public sealed class StepLiteScene
    {
        public StepLiteScene()
        {
            Points = new List<StepPoint3>();
            Edges = new List<StepEdge>();
            Triangles = new List<StepTriangle>();
            Bounds = new StepBox3();
        }

        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string FileSchema { get; set; }
        public List<StepPoint3> Points { get; private set; }
        public List<StepEdge> Edges { get; private set; }
        public List<StepTriangle> Triangles { get; private set; }
        public StepBox3 Bounds { get; private set; }
        public int CartesianPointEntityCount { get; set; }
        public int VertexPointEntityCount { get; set; }
        public int EdgeCurveEntityCount { get; set; }
        public int PolyLoopEntityCount { get; set; }
        public int AdvancedFaceEntityCount { get; set; }
        public int AdvancedFaceSameSenseTrueCount { get; set; }
        public int AdvancedFaceSameSenseFalseCount { get; set; }
        public int FaceBoundEntityCount { get; set; }
        public int FaceOuterBoundEntityCount { get; set; }
        public int FaceInnerBoundEntityCount { get; set; }
        public int FaceBoundOrientationTrueCount { get; set; }
        public int FaceBoundOrientationFalseCount { get; set; }
        public int OrientedFaceLoopCount { get; set; }
        public int ForwardFaceLoopCount { get; set; }
        public int ReversedFaceLoopCount { get; set; }
        public int OrphanPolyLoopCount { get; set; }
        public int UnresolvedFaceBoundReferenceCount { get; set; }
        public int UnresolvedPolyLoopReferenceCount { get; set; }
        public int InnerBoundTriangulationSkippedCount { get; set; }

        public int PointCount
        {
            get { return Points == null ? 0 : Points.Count; }
        }

        public int EdgeCount
        {
            get { return Edges == null ? 0 : Edges.Count; }
        }

        public int TriangleCount
        {
            get { return Triangles == null ? 0 : Triangles.Count; }
        }

        public void AddPoint(StepPoint3 point)
        {
            if (point == null)
            {
                return;
            }

            Points.Add(point);
            Bounds.Include(point);
        }

        public void AddEdge(StepEdge edge)
        {
            if (edge == null)
            {
                return;
            }

            if (edge.A < 0 || edge.B < 0 || edge.A >= PointCount || edge.B >= PointCount || edge.A == edge.B)
            {
                return;
            }

            Edges.Add(edge);
        }

        public void AddTriangle(StepTriangle triangle)
        {
            if (triangle == null)
            {
                return;
            }

            if (triangle.A < 0 || triangle.B < 0 || triangle.C < 0 ||
                triangle.A >= PointCount || triangle.B >= PointCount || triangle.C >= PointCount ||
                triangle.A == triangle.B || triangle.B == triangle.C || triangle.A == triangle.C)
            {
                return;
            }

            Triangles.Add(triangle);
        }
    }
}
