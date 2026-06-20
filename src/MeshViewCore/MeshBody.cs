using System.Collections.Generic;

namespace MeshViewCore
{
    public sealed class MeshBody
    {
        public MeshBody()
        {
            Vertices = new List<MeshPoint3>();
            Triangles = new List<MeshTriangle>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public MeshBox3 Bounds { get; set; }
        public List<MeshPoint3> Vertices { get; private set; }
        public List<MeshTriangle> Triangles { get; private set; }

        public int VertexCount
        {
            get { return Vertices == null ? 0 : Vertices.Count; }
        }

        public int TriangleCount
        {
            get { return Triangles == null ? 0 : Triangles.Count; }
        }

        public void UpdateBounds()
        {
            Bounds = MeshBox3.FromPoints(Vertices);
        }
    }
}
