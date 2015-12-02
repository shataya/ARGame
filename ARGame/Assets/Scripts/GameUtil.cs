using UnityEngine;
using System.Collections;

public static class GameUtil
{
    public static bool IsInRadius(Vector3 center, float radius, Vector3 toValidate)
    {
        float dx = Mathf.Abs (toValidate.x - center.x);
        if (dx > radius)
            return false;
        float dz = Mathf.Abs (toValidate.z - center.z);
        if (dz > radius)
            return false;
        if (dx + dz <= radius)
            return true;
        return (dx * dx + dz * dz <= radius * radius);
    }

    public static bool IsOutOfRadius(Vector3 center, float radius, Vector3 toValidate)
    {
        return !IsInRadius (center, radius, toValidate);
    }
}
