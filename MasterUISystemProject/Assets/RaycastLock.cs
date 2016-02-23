using UnityEngine;
using System.Collections;

public static class RaycastLock{

    public static bool raycastAvailable = true;
    public static RaycastHit hit;

    public static bool GetLock()
    {

        if (raycastAvailable)
        {
            raycastAvailable = false;
            return true;
        }

        return false;
    }

    public static void GiveLock()
    {
        raycastAvailable = true;
    }

    public static void Raycast(Vector3 startPosition, Vector3 direction, int ignoreLayers)
    {
        Physics.Raycast(startPosition, direction, out hit, 1000, ignoreLayers);
    }

    public static void Raycast(Ray ray, int ignoreLayers)
    {
        Physics.Raycast(ray, out hit, 1000, ignoreLayers);
    }
}
