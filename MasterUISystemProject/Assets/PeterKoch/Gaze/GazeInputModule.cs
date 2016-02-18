// Gaze Input Module by Peter Koch <peterept@gmail.com>
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// To use:
// 1. Drag onto your EventSystem game object.
// 2. Disable any other Input Modules (eg: StandaloneInputModule & TouchInputModule) as they will fight over selections.
// 3. Make sure your Canvas is in world space and has a GraphicRaycaster (should by default).
// 4. If you have multiple cameras then make sure to drag your VR (center eye) camera into the canvas.
public class GazeInputModule : PointerInputModule 
{
	public enum Mode { Click = 0, Gaze };
	public Mode mode;

	[Header("Click Settings")]
	public string ClickInputName = "Submit";
	[Header("Gaze Settings")]
	public float GazeTimeInSeconds = 2f;

	public RaycastResult CurrentRaycast;

	private PointerEventData pointerEventData;
	private GameObject currentLookAtHandler;
	private float currentLookAtHandlerClickTime;

	public override void Process()
	{ 
		HandleLook();
		HandleSelection();
	}

	void HandleLook()
	{
		if (pointerEventData == null)
		{
			pointerEventData = new PointerEventData(eventSystem);
		}
		// fake a pointer always being at the center of the screen
		pointerEventData.position = new Vector2(Screen.width/2, Screen.height/2);
		pointerEventData.delta = Vector2.zero;
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		eventSystem.RaycastAll(pointerEventData, raycastResults);
		CurrentRaycast = pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
		ProcessMove(pointerEventData);
	}
	
	void HandleSelection()
	{
		if (pointerEventData.pointerEnter != null)
		{
			// if the ui receiver has changed, reset the gaze delay timer
			GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerEnter);
			if (currentLookAtHandler != handler)
			{
				currentLookAtHandler = handler;

				currentLookAtHandlerClickTime = Time.realtimeSinceStartup + GazeTimeInSeconds;
			}
			
			// if we have a handler and it's time to click, do it now
			if (currentLookAtHandler != null && 
			    (mode == Mode.Gaze && Time.realtimeSinceStartup > currentLookAtHandlerClickTime) || 
			    (mode == Mode.Click && Input.GetButtonDown(ClickInputName)))
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					Debug.Log ("old" + EventSystem.current.currentSelectedGameObject.name);
		//			ExecuteEvents.ExecuteHierarchy(EventSystem.current.currentSelectedGameObject, pointerEventData, ExecuteEvents.deselectHandler);
				}

				EventSystem.current.SetSelectedGameObject(currentLookAtHandler);

				ExecuteEvents.ExecuteHierarchy(currentLookAtHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
				currentLookAtHandlerClickTime = float.MaxValue;
							ExecuteEvents.ExecuteHierarchy(EventSystem.current.currentSelectedGameObject, pointerEventData, ExecuteEvents.deselectHandler);
			}
		}
		else
		{
			currentLookAtHandler = null;
		}
	}


}