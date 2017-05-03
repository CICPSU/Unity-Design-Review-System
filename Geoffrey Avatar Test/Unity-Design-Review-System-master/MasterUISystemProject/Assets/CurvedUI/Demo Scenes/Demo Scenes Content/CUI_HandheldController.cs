using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CurvedUI
{
    /// <summary>
    /// This class contains code that controls the mockup vive controller. 
    /// Its made to make demo sceen look better. Its not made to be used with actual vive controller.
    /// </summary>
    public class CUI_HandheldController : MonoBehaviour
    {

        [SerializeField]
        CurvedUISettings ControlledCanvas;
        [SerializeField]
        Transform LaserBeamTransform;
        [SerializeField]
        Transform LaserBeamDot;


        // Update is called once per frame
        void Update()
        {


            //tell canvas to use the direction of the gun as a ray controller
            Ray myRay = new Ray(this.transform.position, this.transform.forward);

            if (ControlledCanvas)
                ControlledCanvas.CustomControllerRay = myRay;


            //change the laser's length depending on where it hits
            float length = 10000;

            RaycastHit hit;
            if (Physics.Raycast(myRay, out hit, length))
            {

                //check for graphic under pointer if we hit curved canvas. We only want transforms with graphics that are drawn by canvas (depth not -1) to block the pointer.
                int SelectablesUnderPointer = 0;
                if (hit.transform.GetComponent<CurvedUIRaycaster>() != null)
                {
                    SelectablesUnderPointer = hit.transform.GetComponent<CurvedUIRaycaster>().GetObjectsUnderPointer().FindAll(x => x.GetComponent<Graphic>() != null && x.GetComponent<Graphic>().depth != -1).Count;
                }

                //Debug.Log("found graphics: " + SelectablesUnderPointer);
                length = SelectablesUnderPointer == 0 ? 10000 : Vector3.Distance(hit.point, this.transform.position) ;

            }

            LaserBeamTransform.localScale = LaserBeamTransform.localScale.ModifyZ(length);

        }
    }
}
