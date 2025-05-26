using System.Collections.Generic;
using UnityEngine;

public class NaturalCorridorBuilder : MonoBehaviour
{
    [Header("Landmarks to Connect (in order)")]
    public List<GameObject> landmarks;

    [Header("Prefabs")]
    public GameObject groundPrefab;
    public GameObject wallPrefab;

    [Header("Corridor Settings")]
    public float corridorWidth = 6f;
    public float tileSpacing = 1f;
    public float noiseStrength = 2f;
    public float pathSmoothness = 0.2f;

    private HashSet<Vector3> placedGroundTiles = new();
    private HashSet<Vector3> placedWallTiles = new();

    private void Start()
    {
        if (landmarks.Count < 2)
        {
            Debug.LogWarning("Need at least 2 landmarks to connect.");
            return;
        }

        for (int i = 0; i < landmarks.Count - 1; i++)
        {
            Vector3 from = landmarks[i].transform.position;
            Vector3 to = landmarks[i + 1].transform.position;
            GenerateCorridor(from, to);
        }

        PrintTileGrid();
    }

    void GenerateCorridor(Vector3 start, Vector3 end)
    {
        List<Vector3> path = GeneratePath(start, end);

        foreach (Vector3 point in path)
        {
            Vector3 groundPos = RoundToGrid(point);
            if (placedGroundTiles.Add(groundPos))
            {
                Instantiate(groundPrefab, groundPos, Quaternion.identity, transform);
            }

            int radialSteps = 12;
            for (int i = 0; i < radialSteps; i++)
            {
                float angle = i * Mathf.PI * 2 / radialSteps;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * corridorWidth * 0.5f;
                Vector3 wallPos = RoundToGrid(point + offset);

                if (!placedGroundTiles.Contains(wallPos) && placedWallTiles.Add(wallPos))
                {
                    if (!Physics.CheckSphere(wallPos, tileSpacing * 0.4f))
                    {
                        Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    List<Vector3> GeneratePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        float totalDistance = Vector3.Distance(start, end);
        int steps = Mathf.CeilToInt(totalDistance / tileSpacing);

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 point = Vector3.Lerp(start, end, t);

            float noiseX = (Mathf.PerlinNoise(t * pathSmoothness, 0f) - 0.5f) * noiseStrength;
            float noiseZ = (Mathf.PerlinNoise(0f, t * pathSmoothness) - 0.5f) * noiseStrength;
            Vector3 noiseOffset = new Vector3(noiseX, 0, noiseZ);

            path.Add(point + noiseOffset);
        }

        return path;
    }

    Vector3 RoundToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / tileSpacing) * tileSpacing,
            0f,
            Mathf.Round(pos.z / tileSpacing) * tileSpacing
        );
    }

    void PrintTileGrid()
    {
        // Get bounds of tile map
        List<Vector3> allTiles = new List<Vector3>();
        allTiles.AddRange(placedGroundTiles);
        allTiles.AddRange(placedWallTiles);

        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (Vector3 pos in allTiles)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minZ = Mathf.Min(minZ, pos.z);
            maxZ = Mathf.Max(maxZ, pos.z);
        }

        int width = Mathf.RoundToInt((maxX - minX) / tileSpacing) + 1;
        int height = Mathf.RoundToInt((maxZ - minZ) / tileSpacing) + 1;

        char[,] grid = new char[width, height];

        // Fill grid with '.'
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                grid[x, z] = '.';

        // Mark walls
        foreach (Vector3 pos in placedWallTiles)
        {
            int x = Mathf.RoundToInt((pos.x - minX) / tileSpacing);
            int z = Mathf.RoundToInt((pos.z - minZ) / tileSpacing);
            grid[x, z] = 'W';
        }

        // Mark ground (overrides wall)
        foreach (Vector3 pos in placedGroundTiles)
        {
            int x = Mathf.RoundToInt((pos.x - minX) / tileSpacing);
            int z = Mathf.RoundToInt((pos.z - minZ) / tileSpacing);
            grid[x, z] = '#';
        }

        // Print grid from top to bottom
        System.Text.StringBuilder sb = new();
        for (int z = height - 1; z >= 0; z--)
        {
            for (int x = 0; x < width; x++)
            {
                sb.Append(grid[x, z]);
            }
            sb.AppendLine();
        }

        Debug.Log("=== TILE GRID ===\n" + sb.ToString());
    }
}
