using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PullCurrentTransform : MonoBehaviour {

    public Transform characterTransform = null;
    public List<InputField> poiTransformFields = new List<InputField>();

	void Start(){
		if(poiTransformFields.Count != 4 ){
			poiTransformFields = new List<InputField>();
			Debug.Log("field count: " + poiTransformFields.Count);
			poiTransformFields.Add(transform.parent.FindChild("XPosField").GetComponent<InputField>());
			poiTransformFields.Add(transform.parent.FindChild("YPosField").GetComponent<InputField>());
			poiTransformFields.Add(transform.parent.FindChild("ZPosField").GetComponent<InputField>());
			poiTransformFields.Add(transform.parent.FindChild("YRotField").GetComponent<InputField>());
		}
	}

	//called by the button On Click event.
    public void GatherCurrentTransform()
    {

        poiTransformFields[0].text = characterTransform.position.x.ToString();
		poiTransformFields[1].text = characterTransform.position.y.ToString();
		poiTransformFields[2].text = characterTransform.position.z.ToString();
		poiTransformFields[3].text = characterTransform.rotation.eulerAngles.y.ToString();
        
    }
}
