using UnityEngine;
using System.Collections;

public class DeleteAllPoints : MonoBehaviour {

    public Transform poiList;

    /// <summary>
    /// This function is used to delete all of the POIs.
    /// </summary>
    public void RemoveAllPointsFromMenuAndHandler()
    {
        int limit = POI_ReferenceHub.Instance.markerRoot.transform.childCount;
        for (int i = 0; i < limit; i++)
        { 
            //remove the marker from the marker root
            DestroyImmediate(POI_ReferenceHub.Instance.markerRoot.transform.GetChild(0).gameObject);
            
        }

        limit = poiList.childCount;
        for (int i = 0; i < limit; i++)
        {
            //remove the button from the button manager
            POIButtonManager.instance.RemoveButton(poiList.GetChild(0).gameObject);
        }

        //close the edit window
        POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);
    }
}
