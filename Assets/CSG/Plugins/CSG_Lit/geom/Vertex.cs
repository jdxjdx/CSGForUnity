using UnityEngine;

namespace ConstructiveSolidGeometry
{
    /// <summary>
    /// Represents a vertex of a polygon. Use your own vertex class instead of this
    /// one to provide additional features like texture coordinates and vertex
    /// colors. Custom vertex classes need to implement the Vertex interface.
    /// 
    /// </summary>
    public class Vertex
    {
        public Vector3 position { get; set; }
        public Vector3 normal;
        public Vector2 uv;
        public int textureId;
        // TODO: Memomry optimization - this could probably be a short rather than int, 
        //       accessed as a get/set int property that converts the -32,768 to 32,767 range to 0 to 65535
        public int index = -1;

        public Vertex(Vector3 position)
        {
            //this.pos = (pos != Vector3.zero) ? pos : Vector3.zero;
            this.position = position;
            //this.normal = (this.normal != Vector3.zero) ? normal : Vector3.zero;
        }

        public Vertex(Vector3 position, Vector3 normal)
        {
            //this.pos = (pos != Vector3.zero) ? pos : Vector3.zero;
            this.position = position;
            //this.normal = (normal != Vector3.zero) ? normal : Vector3.zero;
            this.normal = normal;
        }
        
        public Vertex(Vector3 position, Vector3 normal, Vector2 uv, int texId)
        {
            //this.pos = (pos != Vector3.zero) ? pos : Vector3.zero;
            this.position = position;
            //this.normal = (normal != Vector3.zero) ? normal : Vector3.zero;
            this.normal = normal;
            
            this.uv = uv;
            
            this.textureId = texId;
        }

        public Vertex clone()
        {
            return new Vertex(this.position, this.normal, this.uv, this.textureId);
        }

        public void flip()
        {
            this.normal *= -1f;
        }

        public Vertex interpolate(Vertex other, float t)
        {
            return new Vertex(
                Vector3.Lerp(this.position, other.position, t),
                Vector3.Lerp(this.normal, (other as Vertex).normal, t),
                Vector2.Lerp(this.uv, (other as Vertex).uv, t),
                (other as Vertex).textureId
            );

        }
    }
}