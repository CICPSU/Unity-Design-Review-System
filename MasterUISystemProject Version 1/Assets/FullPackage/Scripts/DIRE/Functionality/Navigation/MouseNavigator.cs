using UnityEngine;
using System.Collections;


/// <summary>
/// This is a mouse navigation model for moving objects around.
/// 
/// Movement is initiated by holding down one or more mouse buttons and moving the
/// mouse in the X and Y direction.
/// 
/// Left:  Move forward and backward
///        Rotate left and right
/// Right:  Rotate up and down
///         Rotate left and right
/// Left and Right:  Move up and down
///                  Move left and right
///                  
/// </summary>
/// 
 
public class MouseNavigator : MonoBehaviour 
{
	/// <summary>
	/// Rotation speed
	/// </summary>
	public float rotationSpeed = .25f;

    /// <summary>
    /// Translation speed
    /// </summary>
	public float translationSpeed = .1f;
	
    /// <summary>
    /// X/Y position of mouse when motion is initiated
    /// </summary>
	private Vector2 startPosition = new Vector2(0,0);	
	
	// Use this for initialization
	void Start () {
	}
	
    /// <summary>
    /// Get current position of mouse
    /// </summary>
    /// <returns></returns>
	Vector2 getAxes()
	{
		return( new Vector2(Input.mousePosition[0], 
                            Input.mousePosition[1] ) );
	}
	 
	// Update is called once per frame
	void Update () 
	{
        // indices of mouse buttons used to trigger motion
        // They correspond to the left and right mouse buttons.
		int trigger1 = 0;
		int trigger2 = 1;
		bool startTrigger = Input.GetMouseButtonDown(trigger1) || 
			                Input.GetMouseButtonDown(trigger2);
		
        // save position if start of new motion
		if ( startTrigger )
			startPosition = getAxes();
		
        // calculate how far the mouse has moved since start of motion
		Vector2 delta = getAxes() - startPosition;
		
        // Process left button mouse motion
		if ( Input.GetMouseButton(trigger1) &&
			 !Input.GetMouseButton(trigger2) )
		{
			transform.localEulerAngles = transform.localEulerAngles + 
				new Vector3( 0, delta.x * rotationSpeed * Time.deltaTime, 0 );
			transform.Translate( 0, 0, delta.y * translationSpeed * Time.deltaTime, Space.Self );
		}

        // Process right button mouse motion
		if ( !Input.GetMouseButton(trigger1) &&
			 Input.GetMouseButton(trigger2) )
		{
			transform.localEulerAngles = transform.localEulerAngles + 
				new Vector3( -delta.y * rotationSpeed * Time.deltaTime, 
					delta.x * rotationSpeed * Time.deltaTime, 0 );
		}

        // Process left and right button mouse motion
		if ( Input.GetMouseButton(trigger1) &&
			 Input.GetMouseButton(trigger2) )
		{
			transform.Translate( delta.x * translationSpeed * Time.deltaTime, 
				delta.y * translationSpeed * Time.deltaTime, 0, Space.Self );
		}
	}
}
