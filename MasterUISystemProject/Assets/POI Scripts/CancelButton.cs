using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CancelButton : MonoBehaviour {

	//reload the xml file to regenerate buttons and markers
	public void cancelClicked(){
		POIButtonManager.instance.LoadAndGenerateButs();

		POIMenuStateManager.EditModeState = false;

		POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);
		POI_ReferenceHub.Instance.POIMenu.GetComponent<Image>().color = Color.white;
		POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive (false);
		POI_ReferenceHub.Instance.EditBut.gameObject.SetActive(true);
		POI_ReferenceHub.Instance.ApplyBut.gameObject.SetActive (false);
		POI_ReferenceHub.Instance.CancelBut.gameObject.SetActive(false);
		POI_ReferenceHub.Instance.HintText.gameObject.SetActive (true);

	}
}
