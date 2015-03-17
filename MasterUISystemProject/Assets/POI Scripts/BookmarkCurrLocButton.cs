using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BookmarkCurrLocButton : MonoBehaviour {

	public Text posValues;

	public void Update()
	{
		posValues.text = POI_ReferenceHub.Instance.Avatar.transform.position.ToString ();
	}

	public void ToggleBookmarkCurrentWindow()
	{
		POI_ReferenceHub.Instance.BookmarkCurrentLocationWindow.gameObject.SetActive (!POI_ReferenceHub.Instance.BookmarkCurrentLocationWindow.gameObject.activeSelf);
		POI_ReferenceHub.Instance.BookmarkCurrentLocationNameField.GetComponent<InputField> ().value = "";
	}
}
