using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjLoader
{
    // Private fields
    Mesh mesh;
    string filePath;

    // Constructor
    public ObjLoader(Mesh mesh, string filePath)
    {
        this.mesh = mesh;
        this.filePath = filePath;
    }

    public void ConstructMesh()
    {
        // Using lists so we can dynamically load, then later generate arrays for Unity's mesh
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        System.IO.StreamReader file = new System.IO.StreamReader(filePath);
        string line;
        int unparsed = 0;
        while ((line = file.ReadLine()) != null)
        {
            switch (line[0])
            {
                case 'v':
                    if (line[1] == ' ')
                    {
                        string[] param = line.Split(' ').Skip(1).ToArray();
                        List<float> fParam = new List<float>();
                        foreach(string vert in param)
                        {
                            fParam.Add(float.Parse(vert));
                        }
                        if (fParam.Count == 3)
                        {
                            vertices.Add(new Vector3(fParam[0], fParam[1], fParam[2]));
                        } else if (fParam.Count == 2)
                        {
                            vertices.Add(new Vector3(fParam[0], fParam[1]));
                        } else
                        {
                            Debug.LogWarning("Vertex was unable to be processed.");
                        }
                        break;
                    }
                    unparsed++;
                    break;
                case 'f':
                    if (line[1] == ' ')
                    {
                        // Leaving texture and normal vertex code in case we need it later
                        string[] param = line.Split(' ').Skip(1).ToArray();
                        //List<int> textureVertexes = new List<int>();
                        //List<int> normalVertexes = new List<int>();

                        if (param.Length == 3)
                        {

                            foreach (string set in param)
                            {
                                string[] symbols = set.Split('/');
                                int v;
                                //int vt, vn;
                                int.TryParse(symbols[0], out v);
                                //int.TryParse(symbols[1], out vt);
                                //int.TryParse(symbols[2], out vn);
                                if (v > 0)
                                {
                                    triangles.Add(v - 1);
                                } else
                                {
                                    triangles.Add(0);
                                }
                                //textureVertexes.Add(vt - 1);
                                //normalVertexes.Add(vn - 1);
                            }
                            break;
                        } else if (param.Length == 4)
                        {
                            List<int> square = new List<int>();

                            foreach(string set in param)
                            {
                                string[] symbols = set.Split('/');
                                int v = int.Parse(symbols[0]);

                                if (v <= 0) v = 1;
                                square.Add(v);
                            }

                            // I hope this is right????
                            triangles.Add(square[0] - 1);
                            triangles.Add(square[2] - 1);
                            triangles.Add(square[3] - 1);

                            triangles.Add(square[0] - 1);
                            triangles.Add(square[1] - 1);
                            triangles.Add(square[3] - 1);
                        } else
                        {
                            Debug.LogWarning("Face was unable to be parsed.");
                        }
                    } else
                    {
                        Debug.LogWarning("Face was unable to be processed.");
                    }
                    unparsed++;
                    break;
                default:
                    unparsed++;
                    break;
            }
        }
        Debug.LogWarning(unparsed + " lines left unparsed.");
        file.Close();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
