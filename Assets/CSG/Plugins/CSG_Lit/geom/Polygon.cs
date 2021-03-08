using System;
using System.Collections.Generic;

namespace ConstructiveSolidGeometry
{
    /// <summary>
    /// Represents a convex polygon. The vertices used to initialize a polygon must
    /// be coplanar and form a convex loop.
    /// 
    /// Each convex polygon has a `shared` property, which is shared between all
    /// polygons that are clones of each other or were split from the same polygon.
    /// This can be used to define per-polygon properties (such as surface color).
    /// </summary>
    public class Polygon
    {
        public Vertex[] vertices;
        public System.Object shared; // TODO: maybe this should be a Dictionary ??
        public Plane plane;


        public Polygon(Vertex[] vertices)
        {
            this.vertices = vertices;
            this.plane = Plane.fromPoints(vertices[0].position, vertices[1].position, vertices[2].position);
        }

        public Polygon(Vertex[] vertices, System.Object shared)
        {
            this.vertices = vertices;
            this.shared = shared;
            this.plane = Plane.fromPoints(vertices[0].position, vertices[1].position, vertices[2].position);
        }

        public Polygon(List<Vertex> vertices)
        {
            this.vertices = vertices.ToArray();
            this.plane = Plane.fromPoints(vertices[0].position, vertices[1].position, vertices[2].position);
        }


        public Polygon(List<Vertex> vertices, System.Object shared)
        {
            this.vertices = vertices.ToArray();
            this.shared = shared;
            this.plane = Plane.fromPoints(vertices[0].position, vertices[1].position, vertices[2].position);
        }

        public Polygon clone()
        {
            List<Vertex> vs = new List<Vertex>();
            foreach (Vertex v in this.vertices)
            {
                vs.Add(v.clone());
            }
            return new Polygon(vs.ToArray(), this.shared);
        }

        public void flip()
        {
            Array.Reverse(this.vertices, 0, this.vertices.Length);
            foreach (Vertex v in this.vertices)
            {
                v.flip();
            }
            this.plane.flip();
        }
    }
}