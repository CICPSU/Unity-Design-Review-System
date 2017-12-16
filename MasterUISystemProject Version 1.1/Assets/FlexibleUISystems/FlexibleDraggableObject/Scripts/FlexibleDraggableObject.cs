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
    private BoxCollider2D targetCollider2D;
    private Vector3 startPos = new Vector3();
    private EventTrigger _eventTrigger;
    private Vector3 mouseMovement = new Vector3();
    
    void Start ()
    {
        /// this initializes the event trigger to handle dragging and releasing
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDragBegin,EventTriggerType.BeginDrag);
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
        PointerEventData ped = (PointerEventData)data;

        targetRectTrans.Translate(ped.delta);
    }

    void OnDragBegin(BaseEventData data)
    {
        // these two lines turn off camera rotation while dragging a menu object
        TP_Motor.Instance.stopRotation = true;
        TP_Camera.Instance.stopRotation = true;

        startPos = targetRectTrans.anchoredPosition3D;
        
    }

    void OnDragEnd(BaseEventData data)
    {
        TP_Motor.Instance.stopRotation = false;
        TP_Camera.Instance.stopRotation = false;

        if (targetCollider2D.IsTouchingLayers())
            targetRectTrans.anchoredPosition3D = startPos;
        
    }
}