using UnityEngine;
using System.Collections;

/// <summary>
/// Eine Hilfsklasse, die nützliche Methoden bereit stellt
/// </summary>
public static class GameUtil
{
    /// <summary>
    /// Gibt an, ob sich ein Punkt innerhalb eines Radius,
    /// ausgehend von einem Zentrum, befindet
    /// </summary>
    /// <param name="center">Zentrum</param>
    /// <param name="radius">Radius</param>
    /// <param name="toValidate">zu validierender Punkt</param>
    /// <returns></returns>
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

    /// <summary>
    /// Gibt an, ob sich ein Punkt außerhalb eines Radius,
    /// ausgehend von einem Zentrum, befindet
    /// </summary>
    /// <param name="center">Zentrum</param>
    /// <param name="radius">Radius</param>
    /// <param name="toValidate">zu validierender Punkt</param>
    /// <returns></returns>
    public static bool IsOutOfRadius(Vector3 center, float radius, Vector3 toValidate)
    {
        return !IsInRadius (center, radius, toValidate);
    }
}
