using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PullCurrentTransform : MonoBehaviour {


    public List<InputField> poiTransformFields = new List<InputField>();

	void Start(){
		if(poiTransformFields.Count != 4 ){
			poiTransformFields = new List<InputField>();
			Debug.Log("field count: " + poiTransformFields.Count);

			poiTransformFields.Add(POI_ReferenceHub.Instance.POIEditWindow.FindChild("XPosField").GetComponent<InputField>());
			poiTransformFields.Add(POI_ReferenceHub.Instance.POIEditWindow.FindChild("YPosField").GetComponent<InputField>());
			poiTransformFields.Add(POI_ReferenceHub.Instance.POIEditWindow.FindChild("ZPosField").GetComponent<InputField>());
			poiTransformFields.Add(POI_ReferenceHub.Instance.POIEditWindow.FindChild("YRotField").GetComponent<InputField>());
		}
	}

	//called by the button On Click event.
    public void GatherCurrentTransform()
    {

        poiTransformFields[0].text = POI_ReferenceHub.Instance.Avatar.transform.position.x.ToString();
		poiTransformFields[1].text = POI_ReferenceHub.Instance.Avatar.transform.position.y.ToString();
		poiTransformFields[2].text = POI_ReferenceHub.Instance.Avatar.transform.position.z.ToString();
		poiTransformFields[3].text = POI_ReferenceHub.Instance.Avatar.transform.rotation.eulerAngles.y.ToString();
        
    }
}
