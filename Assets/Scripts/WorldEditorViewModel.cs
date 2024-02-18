using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditorViewModel : MonoBehaviour
{

    private string worldName;
    private int meshSizeX;
    private int meshSizeZ;
    private Gradient gradient;
    private GameObject worldObject;
    private Mesh mesh;
    private List<Vector3> verticies;
    private List<int> triangles;
    private List<Color> colors;
    private Vector3[] normals;
    private float minWorldHeight;
    private float maxWorldHeight;

    private int octaves;
    private float heightMultiplier;
    private float elevationMultiplier;

    public GameObject GenerateMesh(string _worldName, int _meshSizeX, int _meshSizeZ, int _octaves, float _heightMultiplier, float _elevationMultiplier, Gradient _gradient, GameObject _worldObject)
    {
        worldObject = _worldObject;
        worldName = _worldName;
        meshSizeX = _meshSizeX;
        meshSizeZ = _meshSizeZ;
        octaves = _octaves;
        heightMultiplier = _heightMultiplier;
        elevationMultiplier = _elevationMultiplier;
        gradient = _gradient;

        mesh = new Mesh();
        worldObject.GetComponent<MeshFilter>().mesh = mesh;

        verticies = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();

        //Create verticies
        for (int z = 0; z <= meshSizeZ; z++)
        {
            for (int x = 0; x <= meshSizeX; x++)
            {

                float y = GetNoise((float)x / meshSizeX, (float)z / meshSizeZ);

                verticies.Add(new Vector3(x, y, z));
                float color = Mathf.InverseLerp(minWorldHeight, maxWorldHeight, y);
                colors.Add(gradient.Evaluate(color));

            }

        }

        int vertex = 0;
        int triangle = 0;

        //Create triangles
        for (int z = 0; z < meshSizeZ; z++)
        {
            for (int x = 0; x < meshSizeX; x++)
            {
                triangles.Add(vertex + 0);
                triangles.Add(vertex + meshSizeX + 1);
                triangles.Add(vertex + 1);
                triangles.Add(vertex + 1);
                triangles.Add(vertex + meshSizeX + 1);
                triangles.Add(vertex + meshSizeX + 2);

                vertex++;
                triangle += 6;
            }
            vertex++;
        }

        mesh.Clear();

        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        normals = new Vector3[verticies.Count];
        CalculateNormals();
        mesh.SetNormals(normals);

        return worldObject;

    }

    public void SpawnWorld()
    {
        GameObject newWorldObject = Instantiate(worldObject, worldObject.transform.position, Quaternion.identity);
        newWorldObject.name = worldName;
    }

    private void CalculateNormals()
    {
        //Calculate normals
        for (int i = 0; i < triangles.Count; i += 3)
        {
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            Vector3 side1 = verticies[index2] - verticies[index1];
            Vector3 side2 = verticies[index3] - verticies[index1];
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            normals[index1] = normal;
            normals[index2] = normal;
            normals[index3] = normal;
        }
    }

    private float GetNoise(float _x, float _z)
    {

        float amplitude = 1;
        float scale = 1;
        float noise = 0;

        //Layers Perlin noise
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = _x * scale;
            float sampleY = _z * scale;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noise += perlinValue * amplitude;

            amplitude *= heightMultiplier;
            scale *= elevationMultiplier;
        }

        if (noise > maxWorldHeight)
        {
            maxWorldHeight = noise;
        }
        else if (noise < minWorldHeight)
        {
            minWorldHeight = noise;
        }

        return noise;
    }
}
