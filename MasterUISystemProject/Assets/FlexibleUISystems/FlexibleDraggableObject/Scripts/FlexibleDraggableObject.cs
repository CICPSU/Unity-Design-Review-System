using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// the event trigger is required to handle dragging
/// the rigidbody2d and boxcollider2d are required to handle collision detection
/// </summary>
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class FlexibleDraggableObject : MonoBehaviour
{
    public GameObject Target;
    private EventTrigger _eventTrigger;
    private Rigidbody2D rigid;

    void Start ()
    {
        /// this initializes the event trigger to handle dragging and releasing
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        _eventTrigger.AddEventTrigger(OnEndDrag, EventTriggerType.EndDrag);

        // this sets up the rigidbody2d on the ui element
        rigid = GetComponent<Rigidbody2D>();
        // this freezes the position and rotation of ui elements in the scene
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /// <summary>
    /// this is called while the user is dragging the ui element
    /// </summary>
    /// <param name="data"></param>
    void OnDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData) data;
        Target.transform.Translate(ped.delta);
        // while the ui element is being dragged, freeze the rotation but let the static ui elements push it into place
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// this is called when the user releases the ui element
    /// </summary>
    /// <param name="data"></param>
    void OnEndDrag(BaseEventData data)
    {
        UIUtilities.PlaceMenuObject(Target.GetComponent<RectTransform>());
    }

    /// <summary>
    /// when the ui element is no longer colliding with any other ui elements, freeze it in place
    /// </summary>
    void OnCollisionExit()
    {
        // this will freeze position and rotation on the ui element once it is in place
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}