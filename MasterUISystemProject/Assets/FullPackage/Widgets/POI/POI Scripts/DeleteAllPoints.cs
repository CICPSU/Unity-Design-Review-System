using UnityEngine;
using System.Collections;

public class DeleteAllPoints : MonoBehaviour {

    public Transform poiList;

    public void RemoveAllPointsFromMenuAndHandler()
    {
        //remove the poinst from the original handler
        for (int i = 0; i < POIButtonManager.originalHandler.projectPOIs.Count; i++)
        {
            POIButtonManager.originalHandler.RemovePoint(POIButtonManager.originalHandler.projectPOIs[i]);
        }
        for(int i = 0; i < POI_ReferenceHub.Instance.markerRoot.transform.childCount; i++)
        { 
            //remove the marker from the marker root
            Destroy(POI_ReferenceHub.Instance.markerRoot.transform.GetChild(i).gameObject);
            
        }

        for (int i = 0; i < poiList.childCount; i++)
        {
            //remove the button from the button manager
            POIButtonManager.instance.RemoveButton(poiList.GetChild(i).gameObject);
        }
        //close the edit window
        POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);

        /*
        //grey out delete point button
        Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
        deleteBut.GetComponent<Button>().enabled = false; //disable delete button
        Transform deleteButText = deleteBut.FindChild("Text") as Transform;
        deleteButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);
        //grey out edit bookmark
        Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
        editBut.GetComponent<Button>().enabled = false; //disable edit button
        Transform editButText = editBut.FindChild("Text") as Transform;
        editButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);
        */
    }
}
