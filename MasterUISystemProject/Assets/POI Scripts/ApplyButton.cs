using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ApplyButton : MonoBehaviour {

	public void applyChangesToXML(){

		XmlIO.Save(POIButtonManager.originalHandler, POI_GlobalVariables.XMLpath);

		POIMenuStateManager.EditModeState = false;

		//restoring the original window
		POI_ReferenceHub.Instance.POIMenu.gameObject.GetComponent<Image>().color = Color.white;
		POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);
		POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive (false);
		POI_ReferenceHub.Instance.EditBut.gameObject.SetActive(true);
		POI_ReferenceHub.Instance.CancelBut.gameObject.SetActive(false);
		POI_ReferenceHub.Instance.ApplyBut.gameObject.SetActive(false); 
		POI_ReferenceHub.Instance.HintText.gameObject.SetActive (true);

			//don't put code after this line
	}
}
