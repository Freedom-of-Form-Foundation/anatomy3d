using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGenerator : MonoBehaviour
{
    [SerializeField, HideInInspector]
    MeshFilter meshFilter;
    ObjLoader loader;

    [SerializeField]
    string modelFile;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (meshFilter == null)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
        }

        loader = new ObjLoader(meshFilter.sharedMesh, modelFile);
    }

    void GenerateMesh()
    {
        loader.ConstructMesh();
    }
}
