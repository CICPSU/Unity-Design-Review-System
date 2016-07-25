using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RaycastControl : MonoBehaviour{
    
    public static RaycastHit hit;
    public static Camera mouseCam = null;

    void Update()
    {
        // this finds the camera whose viewport contains the mouse cursor
        mouseCam = FindMouseCamera();

        // when we left click, if the raycast hits an object that has a class that implements IAcceptRaycast, trigger it
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 9));
            if (hit.transform != null && hit.transform.gameObject.GetComponent<IAcceptRaycast>() != null)
                hit.transform.gameObject.GetComponent<IAcceptRaycast>().RaycastTrigger();
        }
    }
    
    /// <summary>
    /// This function performs a raycast from the cursor's location.
    /// The ignoreLayers argument allows the user to specify what layers the raycast should ignore.
    /// </summary>
    /// <param name="ignoreLayers"></param>
    public static void RaycastCursor(int ignoreLayers)
    {
        mouseCam = FindMouseCamera();
        Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ignoreLayers);
    }

    /// <summary>
    /// This is a function wrapper for the Physics.Raycast function.
    /// The wrapper is used so that the data is stored in RaycastControl as hit.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="direction"></param>
    /// <param name="ignoreLayers"></param>
    public static void Raycast(Vector3 startPosition, Vector3 direction, int ignoreLayers)
    {
        Physics.Raycast(startPosition, direction, out hit, 1000, ignoreLayers);
    }

    /// <summary>
    /// A wrapped like above, but for a different function signature.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="ignoreLayers"></param>
    public static void Raycast(Ray ray, int ignoreLayers)
    {
        Physics.Raycast(ray, out hit, 1000, ignoreLayers);
    }

    /// <summary>
    /// This function is used to determine which camera's viewport the cursor is currently in.
    /// This is used to make the raycast through the correct camera.
    /// </summary>
    /// <returns></returns>
    public static Camera FindMouseCamera()
    {
        List<Camera> camList = (from cam in FindObjectsOfType<Camera>() where cam.targetTexture == null select cam).ToList();
        foreach (Camera cam in camList)
        {
            if (Input.mousePosition.x > cam.pixelRect.xMin && Input.mousePosition.x < cam.pixelRect.xMax
                && Input.mousePosition.y > cam.pixelRect.yMin && Input.mousePosition.y < cam.pixelRect.yMax)
            {
                return cam;
            }
        }
        return camList[0];
    }
}

interface IAcceptRaycast
{
    void RaycastTrigger();
}