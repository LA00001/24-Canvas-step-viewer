using System.Collections.Generic;

namespace MeshViewCore
{
    public sealed class MeshScene
    {
        public MeshScene()
        {
            Bodies = new List<MeshBody>();
        }

        public string Name { get; set; }
        public MeshBox3 Bounds { get; private set; }
        public List<MeshBody> Bodies { get; private set; }

        public int BodyCount
        {
            get { return Bodies == null ? 0 : Bodies.Count; }
        }

        public int VertexCount
        {
            get
            {
                int count = 0;
                foreach (MeshBody body in Bodies)
                {
                    if (body != null)
                    {
                        count += body.VertexCount;
                    }
                }

                return count;
            }
        }

        public int TriangleCount
        {
            get
            {
                int count = 0;
                foreach (MeshBody body in Bodies)
                {
                    if (body != null)
                    {
                        count += body.TriangleCount;
                    }
                }

                return count;
            }
        }

        public void AddBody(MeshBody body)
        {
            if (body == null)
            {
                return;
            }

            if (body.Bounds == null)
            {
                body.UpdateBounds();
            }

            Bodies.Add(body);

            if (Bounds == null)
            {
                Bounds = new MeshBox3(body.Bounds.MinX, body.Bounds.MinY, body.Bounds.MinZ, body.Bounds.MaxX, body.Bounds.MaxY, body.Bounds.MaxZ);
            }
            else
            {
                Bounds.Include(body.Bounds);
            }
        }
    }
}
