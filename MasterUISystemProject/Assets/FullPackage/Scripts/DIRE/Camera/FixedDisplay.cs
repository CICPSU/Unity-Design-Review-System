using UnityEngine;
using System.Collections;

using System.IO;

/// <summary>
/// This script is should be attached to a Unity camera and is responsible for setting
/// the left/right eye transformation and projection matrices used in OpenGL stereo 
/// rendering.  It is intended for fixed position displays where the position of the 
/// users' head with respect to the display will change.  It should work with Head Mounted 
/// Displays but that has not been tested.
/// 
/// The script uses a references to WallObject (a GameObject with an attached Wall
/// Definition script) to get the position, orientation, and extents of the physical 
/// display surface.
/// 
/// At run time, it creates a GameObject for the left and right eye (using the cameras'
/// eye separation valud) and attaches them to the camera.  It uses the position of those
/// eye objects to calculate eye positions relative to the display wall.
/// 
/// Eye transforms are also set so that the view direction is always normal to the wall.
/// </summary>
public class FixedDisplay : MonoBehaviour 
{
    /// <summary>
    /// GameObject that represents the position and orientation of a display surface.
    /// It MUST have an attached WallDefinition script to define the extents (width 
    /// and height).
    /// </summary>
    public GameObject WallObject;

    /// <summary>
    /// WallDefinition for WallObject.  Saved from WallObject component. 
    /// </summary>
    private WallDefinition wallDef;
   
    /// <summary>
    /// Left and right eye objects.  Created at runtime and attached to camera to 
    /// get eye positions.
    /// </summary>
    
	// objects for two eyes
	//private GameObject leftEyeObj;
    //private GameObject rightEyeObj;

	private GameObject eyeObj;

    /// <summary>
    /// Some debug variables.  Usually disabled but show useful algorithmic information
    /// on screen when enabled.
    /// </summary>
    public bool ShowDebug = false;
    private StringWriter WallInfo = new StringWriter();
    private StringWriter UpdateInfo = new StringWriter();

	/// <summary>
	/// Initialization.  Create eye objects and save wall definition.
	/// </summary>
	void Start () 
    {
        // Create a debug string for wall information
        WallInfo = new StringWriter();

        // Debug
        WallInfo.WriteLine("Wall: " + WallObject.name);
        WallInfo.WriteLine("Position: " + WallObject.transform.localPosition);
        WallInfo.WriteLine("Orientation: " + WallObject.transform.localRotation.eulerAngles);
        
        // Get the wall definition object for the wall object.
        // There should probably be some error checking if it doesn't exist.
        wallDef = WallObject.GetComponent(typeof(WallDefinition)) as WallDefinition;
        WallInfo.WriteLine("Size: " + wallDef.Width + ", " + wallDef.Height);

        // calculate eye offset based on camera eye separation value.
        //float eyeOffset = camera.stereoSeparation / 2.0f;

        // create objects to represent left and right eye positions.
        //leftEyeObj = CreateEyeObject(-eyeOffset, camera.name + ":Left Eye");
        //rightEyeObj = CreateEyeObject(eyeOffset, camera.name + ":Right Eye");
		eyeObj = CreateEyeObject (0, GetComponent<Camera>().name + ":Eye");
    }

    /// <summary>
    /// Create eye object.
    /// </summary>
    /// <param name="offset">Offset distance for eye.  This should be in the same units as camera and wall parameters.</param>
    /// <param name="name">Name to give the eye object.</param>
    /// <returns></returns>
    GameObject CreateEyeObject(float offset, string name)
    {
        GameObject eyeObject;

        // Create and return a new GameObject representing an eye.  Attach to this
        // transform and set its TRS appropriately.

        eyeObject = new GameObject(name);
        eyeObject.transform.parent = gameObject.transform;
        eyeObject.transform.localPosition = new Vector3(offset, 0, 0);
        eyeObject.transform.localRotation = Quaternion.identity;
        eyeObject.transform.localScale = Vector3.one;

        return (eyeObject);
    }

    /// <summary>
    /// Show on screen GUI
    /// </summary>

    void OnGUI()
    {
        // if requested show debug on wall and camera information.
        if (ShowDebug)
        {
            Vector3 origin = GetComponent<Camera>().ViewportToScreenPoint(new Vector3(0, 1, 0));
            GUI.Box(new Rect(origin.x, 0, 300, 100), WallInfo.ToString());
            GUI.Box(new Rect(origin.x, 100, 300, 300), UpdateInfo.ToString());
        }
    }

    /// <summary>
    /// Update stereo camera
    /// </summary>
	/// 
	/// 
	void LateUpdate () 
    {
        if (WallObject == null)
        {
			if ( Time.frameCount <= 1 )
	            Debug.LogError("No Wall assigned to " + name, this);
            return;
        }

        // Debug - clear last frame information.
        UpdateInfo = new StringWriter();

        // Create camera offset matrices for the left and right eye
        //Matrix4x4 leftEyeMatrix = new Matrix4x4();
        //Matrix4x4 rightEyeMatrix = new Matrix4x4();

		Matrix4x4 eyeMatrix = new Matrix4x4 ();

        // Create and calculate an offset rotation matrix for left and right eye (same)
        // Camera render should face the same direction as the display wall
        Quaternion rotation = Quaternion.identity;
		rotation = Quaternion.Inverse (transform.rotation) * WallObject.transform.rotation;

        // Debug
		UpdateInfo.WriteLine ("Camera->Wall Rotation: " + rotation.eulerAngles);

        // populate L/R matrices
        //leftEyeMatrix.SetTRS(leftEyeObj.transform.localPosition, rotation, Vector3.one);
        //rightEyeMatrix.SetTRS(rightEyeObj.transform.localPosition, rotation, Vector3.one);

		eyeMatrix.SetTRS (eyeObj.transform.localPosition, rotation, Vector3.one);

        // Set stereo camera transforms
        //camera.SetStereoCameraTransforms(leftEyeMatrix, rightEyeMatrix);

        // Compute the L/R eye view frustums
        //leftEyeMatrix = ComputeViewFrustum(leftEyeObj);
        //rightEyeMatrix = ComputeViewFrustum(rightEyeObj);
		if (GetComponent<Camera>().transform.localEulerAngles == Vector3.zero)
			GetComponent<Camera>().transform.localEulerAngles = rotation.eulerAngles;

		eyeMatrix = ComputeViewFrustum (eyeObj);

		// set projection matrix of the camera after calculating frustums
		GetComponent<Camera>().projectionMatrix = eyeMatrix;

        // set the camera projection matrix
        //camera.SetStereoProjectionMatrices(leftEyeMatrix, rightEyeMatrix);

	}

    /// <summary>
    /// Compute a view frustum for a given eye.
    /// </summary>
    /// <param name="eyeObject">Eye object requested</param>
    /// <returns></returns>
    Matrix4x4 ComputeViewFrustum(GameObject eyeObject)
    {
        // Debug
        UpdateInfo.WriteLine(eyeObject.name + ": " + 
		                     eyeObject.transform.TransformPoint( Vector3.zero ) );

        // Calculate the position of the eye relative to the wall.
        Vector3 wallEyePos = WallObject.transform.InverseTransformPoint(eyeObject.transform.TransformPoint(Vector3.zero));
        UpdateInfo.WriteLine("Wall Relative Eye Pos: " + wallEyePos);

        // calculate frustum
        float near = GetComponent<Camera>().nearClipPlane;
        float far = GetComponent<Camera>().farClipPlane;
        float left = -wallEyePos.x / -wallEyePos.z * near;
        float right = (wallDef.Width - wallEyePos.x) / -wallEyePos.z * near;
        float bottom = -wallEyePos.y / -wallEyePos.z * near;
        float top = (wallDef.Height - wallEyePos.y) / -wallEyePos.z * near;

		//Debug.DrawLine (camera.transform.position, camera.transform.position + 10.0f * new Vector3 (Mathf.Sin(left),0,Mathf.Cos(left)));
		//Debug.DrawLine (camera.transform.position, camera.transform.position + 10.0f * new Vector3 (Mathf.Sin (right),0,Mathf.Cos (right)));
		//Debug.DrawLine (camera.transform.position, camera.transform.position + 10.0f * new Vector3 (0,Mathf.Sin(top),Mathf.Cos(top)));
		//Debug.DrawLine (camera.transform.position, camera.transform.position + 10.0f * new Vector3 (0,Mathf.Sin(bottom),Mathf.Cos(bottom)));

        // Debug
        UpdateInfo.WriteLine("L/R: " + left + ", " + right );
        UpdateInfo.WriteLine("B/T: " + bottom + ", " + top);
        UpdateInfo.WriteLine("N/F: " + near + ", " + far);

        // fill in the custom view frustum projection matrix and return
        // This is the standard OpenGL projection matrix
        Matrix4x4 frustum = new Matrix4x4();
        frustum.SetColumn(0, new Vector4(2 * near / (right - left), 0, 0, 0));
        frustum.SetColumn(1, new Vector4(0, 2 * near / (top - bottom), 0, 0));
        frustum.SetColumn(2, new Vector4((right + left) / (right - left),
                                           (top + bottom) / (top - bottom),
                                           -(far + near) / (far - near),
                                           -1));
        frustum.SetColumn(3, new Vector4(0, 0, -2 * far * near / (far - near), 0));

        return (frustum);
    }
}
