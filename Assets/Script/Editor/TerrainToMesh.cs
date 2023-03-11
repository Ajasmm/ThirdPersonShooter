using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainToMesh : MonoBehaviour
{
    Terrain m_Terrain;
    TerrainData data;
    int resolution, index;

    public void BuildMesh()
    {
        m_Terrain = GetComponent<Terrain>();
        data = m_Terrain.terrainData;
        resolution = data.heightmapResolution;
        index = 0;

        float[,] heights = data.GetHeights(0, 0, resolution, resolution);

        Vector3[] verticies = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        for(int i = 0; i < resolution; i++)
        {
            for(int j = 0; j < resolution; j++)
            {
                verticies[index++] = new Vector3(i, heights[i, j], j);
            }
        }
    }
}
