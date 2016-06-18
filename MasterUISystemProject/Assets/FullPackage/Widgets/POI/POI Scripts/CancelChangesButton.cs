using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CancelChangesButton : MonoBehaviour {

	public void CancelClicked()
	{


        POI_ReferenceHub.Instance.FillPOIInfoFields(new POI("", "Name",Vector3.zero,Vector3.zero, ""));
		POI_ReferenceHub.Instance.CloseEditWindow();
	}
}
