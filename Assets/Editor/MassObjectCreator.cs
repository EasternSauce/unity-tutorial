// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;

// public class MassObjectCreator
// {
//     [MenuItem("Tools/Mass Create GameObjects")]
//     public static void CreateGameObjectsInScene()
//     {
//         GameObject prefab = Resources.Load<GameObject>("RockTile2");

//         int side = 100;
//         int count = side * side;
//         float spacing = 1f;

//         Debug.Log("side is " + side);

//         GameObject parent = new GameObject("GeneratedObjects");

//         for (int i = 0; i < count; i++)
//         {
//             Vector3 position = new Vector3(i % side * spacing, 0, i / side * spacing);
//             GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

//             obj.transform.position = position;
//             obj.transform.parent = parent.transform;

//             Renderer rend = obj.GetComponentInChildren<Renderer>();
//             if (rend != null)
//             {
//                 Vector3 boundsCenter = rend.bounds.center;
//                 float bottomY = rend.bounds.min.y;

//                 // Move object up so its bottom is at Y = 0
//                 float heightOffset = boundsCenter.y - bottomY;
//                 obj.transform.position += Vector3.up * heightOffset;
//             }

//             Undo.RegisterCreatedObjectUndo(obj, "Create Prefab Object");
//         }

//         // Mark scene as dirty to enable saving
//         EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
//     }
// }

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class MassObjectCreator
{
    // === Configurable Parameters ===
    private static int mapWidth = 200;
    private static int mapHeight = 200;
    private static float tileSpacing = 1f;

    private static int numLandmarks = 9;
    private static float roomRadius = 15f;
    private static float landmarkMinDistance = 70f;

    private static int tunnelWidthMain = 6;
    private static int tunnelWidthDeadEnd = 4;
    private static int numDeadEnds = 10;

    private static Vector2Int landmarkPlacementMargin = new Vector2Int(10, 10);
    private static string prefabResourcePath = "RockTile2";

    private static List<Vector2Int> carvedPathPoints = new List<Vector2Int>();
    private static int wallMargin = 2;
    private static Vector2Int? guaranteedLandmark = new Vector2Int(25, 25);

    // === Main Menu Entry Point ===
    [MenuItem("Tools/Generate Cave System")]
    public static void CreateCaveSystem()
    {
        carvedPathPoints = new List<Vector2Int>();
        GameObject prefab = Resources.Load<GameObject>(prefabResourcePath);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found in Resources folder.");
            return;
        }

        GameObject parent = new GameObject("GeneratedCave");

        bool[,] isEmpty = new bool[mapWidth, mapHeight];
        System.Random rand = new System.Random();

        List<Vector2Int> landmarks = GenerateLandmarks(rand);

        CarveRooms(landmarks, isEmpty);
        ConnectLandmarksWithTunnels(landmarks, isEmpty, rand);
        AddRandomDeadEnds(landmarks, isEmpty, rand);
        InstantiateTiles(prefab, isEmpty, parent);

        CreateGroundPlane(parent);

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

    }

    private static List<Vector2Int> GenerateLandmarks(System.Random rand)
    {
        List<Vector2Int> landmarks = new List<Vector2Int>();
        int attempts = 0;

        // === Insert guaranteed landmark if provided and valid ===
        if (guaranteedLandmark.HasValue)
        {
            Vector2Int pos = guaranteedLandmark.Value;
            if (IsInBounds(pos))
            {
                landmarks.Add(pos);
            }
            else
            {
                Debug.LogWarning("Guaranteed landmark position is out of bounds and was ignored.");
            }
        }

        while (landmarks.Count < numLandmarks && attempts < 1000)
        {
            attempts++;
            Vector2Int pos = new Vector2Int(
                rand.Next(landmarkPlacementMargin.x, mapWidth - landmarkPlacementMargin.x),
                rand.Next(landmarkPlacementMargin.y, mapHeight - landmarkPlacementMargin.y)
            );

            bool tooClose = false;
            foreach (var lm in landmarks)
            {
                if (Vector2Int.Distance(pos, lm) < landmarkMinDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                landmarks.Add(pos);
            }
        }

        if (landmarks.Count < numLandmarks)
        {
            Debug.LogWarning("Could not place all landmarks with the given minimum distance.");
        }

        return landmarks;
    }

    private static void CarveRooms(List<Vector2Int> landmarks, bool[,] isEmpty)
    {
        foreach (var pos in landmarks)
        {
            int carveRadius = Mathf.CeilToInt(roomRadius + 2f); // 2f allows margin for noise

            for (int y = -carveRadius; y <= carveRadius; y++)
            {
                for (int x = -carveRadius; x <= carveRadius; x++)
                {
                    Vector2Int tilePos = pos + new Vector2Int(x, y);
                    if (IsInBounds(tilePos))
                    {
                        float noise = Mathf.PerlinNoise(tilePos.x * 0.1f, tilePos.y * 0.1f);
                        float dist = new Vector2(x, y).magnitude;

                        if (dist < roomRadius + noise * 2f)
                        {
                            isEmpty[tilePos.x, tilePos.y] = true;
                            carvedPathPoints.Add(tilePos);
                        }
                    }
                }
            }
        }
    }

    private static void ConnectLandmarksWithTunnels(List<Vector2Int> landmarks, bool[,] isEmpty, System.Random rand)
    {
        for (int i = 0; i < landmarks.Count - 1; i++)
        {
            CarveTunnel(landmarks[i], landmarks[i + 1], isEmpty, tunnelWidthMain);
        }
    }

    private static void AddRandomDeadEnds(List<Vector2Int> landmarks, bool[,] isEmpty, System.Random rand)
    {
        if (carvedPathPoints.Count == 0) return;

        for (int i = 0; i < numDeadEnds; i++)
        {
            Vector2Int start = carvedPathPoints[rand.Next(carvedPathPoints.Count)];

            int length = rand.Next(30, 70);
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector2Int end = start + Vector2Int.RoundToInt(dir * length);

            CarveTunnel(start, end, isEmpty, tunnelWidthDeadEnd);
        }
    }

    private static void InstantiateTiles(GameObject prefab, bool[,] isEmpty, GameObject parent)
    {
        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (!isEmpty[x, z])
                {
                    Vector3 position = new Vector3(x * tileSpacing, 0, z * tileSpacing);
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    obj.transform.position = position;
                    obj.transform.parent = parent.transform;

                    Renderer rend = obj.GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        float heightOffset = rend.bounds.center.y - rend.bounds.min.y;
                        obj.transform.position += Vector3.up * heightOffset;
                    }

                    Undo.RegisterCreatedObjectUndo(obj, "Create Prefab Object");
                }
            }
        }
    }

    private static void CarveTunnel(Vector2Int start, Vector2Int end, bool[,] isEmpty, int tunnelWidth)
    {
        Vector2 current = start;
        Vector2 direction = ((Vector2)(end - start)).normalized;
        float distance = Vector2.Distance(start, end);
        int steps = Mathf.CeilToInt(distance);

        for (int i = 0; i < steps; i++)
        {
            int x = Mathf.RoundToInt(current.x);
            int y = Mathf.RoundToInt(current.y);

            for (int dx = -tunnelWidth; dx <= tunnelWidth; dx++)
            {
                for (int dy = -tunnelWidth; dy <= tunnelWidth; dy++)
                {
                    Vector2Int tilePos = new Vector2Int(x + dx, y + dy);
                    if (IsInBounds(tilePos))
                    {
                        float dist = new Vector2(dx, dy).magnitude;
                        float noise = Mathf.PerlinNoise(tilePos.x * 0.1f, tilePos.y * 0.1f);
                        if (dist < tunnelWidth + noise)
                        {
                            isEmpty[tilePos.x, tilePos.y] = true;
                        }
                    }
                }
            }

            Vector2 randomDir = new Vector2(
                Mathf.PerlinNoise(current.x * 0.1f, current.y * 0.1f) - 0.5f,
                Mathf.PerlinNoise(current.y * 0.1f, current.x * 0.1f) - 0.5f
            ) * 2f; // stronger directional noise

            if (i % 10 == 0)
            {
                direction = ((Vector2)(end - new Vector2Int(Mathf.RoundToInt(current.x), Mathf.RoundToInt(current.y)))).normalized;
            }

            current += (direction + randomDir).normalized;
        }
    }

    private static bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= wallMargin && pos.x < mapWidth - wallMargin &&
               pos.y >= wallMargin && pos.y < mapHeight - wallMargin;
    }

    private static void CreateGroundPlane(GameObject parent)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "GroundPlane";

        float totalWidth = mapWidth * tileSpacing;
        float totalHeight = mapHeight * tileSpacing;

        ground.transform.localScale = new Vector3(totalWidth / 10f, 1, totalHeight / 10f); // Plane is 10x10 units by default
        ground.transform.position = new Vector3(totalWidth / 2f, -0.1f, totalHeight / 2f); // Slightly below the tiles
        ground.transform.parent = parent.transform;

        Renderer rend = ground.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            rend.sharedMaterial.color = Color.gray;
        }

        Undo.RegisterCreatedObjectUndo(ground, "Create Ground Plane");
    }
}