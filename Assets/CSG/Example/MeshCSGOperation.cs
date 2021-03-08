using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ConstructiveSolidGeometry;

public class MeshCSGOperation : MonoBehaviour
{
    public GameObject a;
    public GameObject b;

    void Start()
    {
        GameObject newGameObject = CreateNewObject();
        Mesh newMesh = new CSG().subtract(a, b).toMesh();
        newGameObject.GetComponent<MeshRenderer>().sharedMaterial = a.GetComponent<MeshRenderer>().sharedMaterial;
        newGameObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(false);
    }

    Mesh SubtractTest(GameObject targetGo, GameObject brushGo)
    {
        Model.submeshIndices = new List<List<int>>();
        
        Model csg_model_a = new Model(targetGo.gameObject, false, false); 
        Model csg_model_b = new Model(brushGo.gameObject, true, false);
			
        Node a = new Node( csg_model_a.ToPolygons());
        Node b = new Node( csg_model_b.ToPolygons());
        
        a.invert();
        a.clipTo(b);
        b.clipTo(a);
        b.invert();
        b.clipTo(a);
        b.invert();
        a.build(b.allPolygons());
        a.invert();
        return CSG.fromPolygons(a.allPolygons()).toMesh();
    }
    
    private GameObject CreateNewObject()
    {
        GameObject tempObject = new GameObject();
        MeshRenderer meshRenderer = tempObject.AddComponent<MeshRenderer>();
        //EditorUtility.SetSelectedWireframeHidden(meshRenderer, true);
        Material[] material = new Material[1];
        material[0] = new Material(Shader.Find("Standard"));
        meshRenderer.sharedMaterials = material; 
        MeshFilter meshFilter = tempObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new UnityEngine.Mesh();
        meshFilter.sharedMesh.name = "Level Editor Mesh";
        return tempObject;
    }
}
