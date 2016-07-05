using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// the event trigger is required to handle dragging
/// the rigidbody2d and boxcollider2d are required to handle collision detection
/// </summary>
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FlexibleDraggableObject : MonoBehaviour
{
    public GameObject Target;
    private RectTransform targetRectTrans;
    private BoxCollider2D targetCollider2D;
    private Vector3[] corners = new Vector3[4];
    private EventTrigger _eventTrigger;

    public bool draggable = true;

    public bool dragging = false;
    public bool colliding = false;
    
    void Start ()
    {
        /// this initializes the event trigger to handle dragging and releasing
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        _eventTrigger.AddEventTrigger(OnDragEnd, EventTriggerType.EndDrag);
        targetRectTrans = GetComponent<RectTransform>();
        targetCollider2D = GetComponent<BoxCollider2D>();
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /// <summary>
    /// this is called while the user is dragging the ui element
    /// </summary>
    /// <param name="data"></param>
    void OnDrag(BaseEventData data)
    {
        if (draggable)
        {
            dragging = true;

            // these two lines turn off camera rotation while dragging a menu object
            TP_Motor.Instance.stopRotation = true;
            TP_Camera.Instance.stopRotation = true;

            PointerEventData ped = (PointerEventData)data;

            if(!targetCollider2D.IsTouchingLayers())
                targetRectTrans.Translate(ped.delta);

            if (targetCollider2D.IsTouchingLayers())
                targetRectTrans.Translate(-ped.delta);
            
            // this function returns the four corners of the rectTransform in world coordinates
            // order: bottom left, top left, top right, bottom right
            targetRectTrans.GetWorldCorners(corners);

            // this calls the function to make sure the menu object stays on the screen
            UIUtilities.PlaceMenuObject(targetRectTrans, corners);
            
        }
    }

    void OnDragEnd(BaseEventData data)
    {
        if (draggable)
        {
            dragging = false;
            TP_Motor.Instance.stopRotation = false;
            TP_Camera.Instance.stopRotation = false;
        }
    }

}