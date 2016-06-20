using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// the event trigger is required to handle dragging
/// the rigidbody2d and boxcollider2d are required to handle collision detection
/// </summary>
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(BoxCollider2D))]
public class FlexibleDraggableObject : MonoBehaviour
{
    public GameObject Target;
    private RectTransform targetRectTrans;
    private EventTrigger _eventTrigger;
    
    void Start ()
    {
        /// this initializes the event trigger to handle dragging and releasing
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        targetRectTrans = GetComponent<RectTransform>();
    }

    /// <summary>
    /// this is called while the user is dragging the ui element
    /// </summary>
    /// <param name="data"></param>
    void OnDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData) data;
        
        targetRectTrans.Translate(ped.delta);
        UIUtilities.PlaceMenuObject(targetRectTrans);
    }

    /// <summary>
    /// when the ui element is no longer colliding with any other ui elements, freeze it in place
    /// </summary>
    void OnCollisionEnter2D(Collision2D coll)
    {

    }
}