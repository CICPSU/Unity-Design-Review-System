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
    public float manualWidth = -1;
    private RectTransform targetRectTrans;
    private Vector3 prevPos = new Vector3();
    private Vector3[] corners = new Vector3[4];
    private EventTrigger _eventTrigger;

    public bool dragging = false;
    public bool colliding = false;
    
    void Start ()
    {
        /// this initializes the event trigger to handle dragging and releasing
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        _eventTrigger.AddEventTrigger(OnDragEnd, EventTriggerType.EndDrag);
        targetRectTrans = GetComponent<RectTransform>();
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /// <summary>
    /// this is called while the user is dragging the ui element
    /// </summary>
    /// <param name="data"></param>
    void OnDrag(BaseEventData data)
    {
        dragging = true;
        PointerEventData ped = (PointerEventData) data;
        if (!colliding)
        {
            prevPos = targetRectTrans.anchoredPosition3D;
            targetRectTrans.Translate(ped.delta);
            targetRectTrans.GetWorldCorners(corners);
            if (manualWidth != -1)
            {
                corners[2] = corners[1] + new Vector3(manualWidth, 0, 0);
                corners[3] = corners[0] + new Vector3(manualWidth, 0, 0);
            }
            UIUtilities.PlaceMenuObject(targetRectTrans, corners);
        }
    }

    void OnDragEnd(BaseEventData data)
    {
        dragging = false;
    }

    /// <summary>
    /// when the ui element is no longer colliding with any other ui elements, freeze it in place
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dragging)
        {
            Debug.Log("hit");
            colliding = true;
            targetRectTrans.Translate((prevPos - targetRectTrans.anchoredPosition3D) * 5);
            
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (colliding)
        {
            Debug.Log("stop");
            colliding = false;
        }
    }
}