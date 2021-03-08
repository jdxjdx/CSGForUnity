using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace ConstructiveSolidGeometry
{
	class Model
	{
		public List<Vertex> vertices;
		public List<int> indices;
		public static List<List<int>> submeshIndices;
		public static bool keepSubmeshes = true;

		public Model()
		{
			this.vertices = new List<Vertex>();
			this.indices = new List<int>();
		}

		/**
		 * Initialize a Model with the mesh of a gameObject.
		 */
		public Model(GameObject go, bool reverseNormals, bool useCustomMaterial)
		{
			this.vertices = new List<Vertex>();
			this.indices = new List<int>();
			
			Mesh m = go.GetComponent<MeshFilter>().sharedMesh;
			Transform trans = go.GetComponent<Transform>();

			int vertexCount = m.vertexCount;
			
			Vector3[] v = m.vertices;
			Vector3[] n = m.normals;
			int[] t = m.triangles;

			if (reverseNormals == true)
			{
				for (int i = 0; i < n.Length; i++)
				{
					n[i] = -n[i];
				}
			}

			Vector2[] u = m.uv;
			Color[] c = m.colors;
			int count = 0;
			
			if (useCustomMaterial == true)
			{
				for (int i = 0; i < t.Length; i++)
				{
					vertices.Add( new Vertex(trans.TransformPoint(v[t[i]]), trans.TransformDirection(n[t[i]]), u == null ? Vector2.zero : u[t[i]], Model.submeshIndices.Count + 1));
					this.indices.Add(count++);
				}
				Model.submeshIndices.Add(this.indices);
			}
			else
			{
				for (int i = 0; i < m.subMeshCount; i++)
				{
					int[] submeshIndices = m.GetTriangles(i);
	
					Model.submeshIndices.Add(submeshIndices.ToList());
					for (int j = 0; j < submeshIndices.Length; j++)
					{
						vertices.Add( new Vertex(trans.TransformPoint(v[submeshIndices[j]]), trans.TransformDirection(n[submeshIndices[j]]), u == null ? Vector2.zero : u[submeshIndices[j]], Model.submeshIndices.Count));
						this.indices.Add(count++);
					}
				}
			}
		}

		public Model(List<Polygon> list)
		{
			this.vertices = new List<Vertex>();
			this.indices = new List<int>();

			for (int i = 0; i < list.Count; i++)
			{
				Polygon poly = list[i];

				for (int j = 2; j < poly.vertices.Length; j++)
				{
					this.vertices.Add(poly.vertices[0]);
					this.vertices.Add(poly.vertices[j - 1]);	
					this.vertices.Add(poly.vertices[j]);		
				}
			}
		}

		public List<Polygon> ToPolygons()
		{
			List<Polygon> list = new List<Polygon>();

			for (int i = 0; i < indices.Count; i += 3)
			{
				List<Vertex> triangle = new List<Vertex>()
				{
					vertices[indices[i]],
					vertices[indices[i + 1]],
					vertices[indices[i + 2]]
				};

				list.Add(new Polygon(triangle));
			}

			return list;
		}

		/**
		 * Converts a Model to a Unity mesh.
		 */
		public Mesh ToMesh(bool keepSubmeshes, Transform targetTransform)
		{			
			int count = 0;

			Mesh m = new Mesh();
			m.name = "New Mesh";

			int vc = vertices.Count;

			Vector3[] v = new Vector3[vc];
			Vector3[] n = new Vector3[vc];
			Vector2[] u = new Vector2[vc];
			Color[] c = new Color[vc];
			Vector3 tempVector3 = Vector3.zero;

			List<Vertex> orderedVertices = new List<Vertex>();

			for (int i = 0; i < Model.submeshIndices.Count; i++)
			{
				for (int j = 0; j < vc; j++)
				{
					if ((this.vertices[j].textureId - 1) == i)
					{
						tempVector3 = targetTransform.InverseTransformPoint(this.vertices[j].position);
						Vertex tempVertex = this.vertices[j];
						tempVertex.position = tempVector3;
						this.vertices[j] = tempVertex;
						orderedVertices.Add(this.vertices[j]);
					}
				}
			}

			this.indices = new List<int>();
			
			int[] indexCountPerSubmesh = new int[Model.submeshIndices.Count];
			
			for (int i = 0; i < vc; i++)
			{
				indexCountPerSubmesh[orderedVertices[i].textureId - 1] += 1;
				this.indices.Add(count++);
			}
			
			for (int i = 0; i < vc; i++)
			{
				v[i] = orderedVertices[i].position;
				n[i] = orderedVertices[i].normal;
				u[i] = orderedVertices[i].uv;
			}

			
			m.vertices = v;
			m.normals = n;
			m.colors = c;
			m.uv = u;
			m.triangles = this.indices.ToArray();

			if (keepSubmeshes == true)
			{
				count = 0;
				m.subMeshCount = 0;
	
				for (int i = 0; i < indexCountPerSubmesh.Length; i++)
				{
					if (indexCountPerSubmesh[i] > 0)
					{
						m.subMeshCount++;
						m.SetTriangles(this.indices.GetRange(count, indexCountPerSubmesh[i]), m.subMeshCount - 1);
						count += indexCountPerSubmesh[i];			
					}
				}
			}	

			return m;
		}
	}
}