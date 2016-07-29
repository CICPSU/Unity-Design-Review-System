using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///all the physics/3D raycast in this scene should be done through calling corresponding methods in this class
/// </summary>
public class RaycastControl : MonoBehaviour{
    
    public static RaycastHit hit;
    
    //the cam whose viewport contains the mouse cursor
    private static Camera mouseCam = null;

    public static Camera MouseCam
    {
        get
        {
            mouseCam = FindMouseCamera();
            return mouseCam;
        }
    }


    void Update()
    {

        // when we left click, if the raycast hits an object that has a class that implements IAcceptRaycast, trigger it
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // this finds the camera whose viewport contains the mouse cursor
            mouseCam = FindMouseCamera();

            RaycastCursor(~(1 << 9)); //raycast should ignore layer 9, avatar layer

            if (hit.transform != null && hit.transform.gameObject.GetComponent<IAcceptRaycast>() != null)
                hit.transform.gameObject.GetComponent<IAcceptRaycast>().RaycastTrigger();
        }
    }
    
    /// <summary>
    /// This function performs a raycast from the cursor's location.
    /// The ignoreLayers argument allows the user to specify what layers the raycast should ignore.
    /// </summary>
    /// <param name="ignoreLayers">it's a bitmask, where each bit means collide when set to 1, ingoren when 0. Layer number corresponds to bits shifted, so layer 9 is 1 shifted left 9 times </param>
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



/// <summary>
///All 3D objects that respond to raycast should have a class that implements this interface. 
/// </summary>
interface IAcceptRaycast
{
    /// <summary>
    /// this function is called whenever an object that implements IAcceptRaycast interface is clicked
    /// </summary>
    void RaycastTrigger();
}