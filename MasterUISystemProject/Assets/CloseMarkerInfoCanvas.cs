using UnityEngine;
using System.Collections;

public class CloseMarkerInfoCanvas : MonoBehaviour {

	public void OnMouseUp()
	{
		transform.parent.parent.gameObject.SetActive (false);
	}
}
