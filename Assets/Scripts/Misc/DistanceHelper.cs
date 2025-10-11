using UnityEngine;

public static class DistanceHelper
{
    public static float Distance(Vector3 a, Vector3 b)
    {
        Vector3 diff = a - b;
        diff.y = 0f;
        return diff.magnitude;
    }
}
