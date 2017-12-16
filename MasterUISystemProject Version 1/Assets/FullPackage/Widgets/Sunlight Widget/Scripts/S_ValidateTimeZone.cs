using UnityEngine;
using System.Collections;

//attached to the timezone input field
public class S_ValidateTimeZone : MonoBehaviour {
	public RectTransform timeZoneWarning;
	private int timeZoneMax = 12;
	private int timeZoneMin = -11;

	public void validateTimeZone(){
		string timeZoneInput = GetComponent<UnityEngine.UI.InputField>().text;
		int timeZone = int.Parse(timeZoneInput);
		if(timeZone <= timeZoneMax && timeZone >=timeZoneMin){
			return;
		}else{
			timeZoneWarning.gameObject.SetActive(true);
		}
	}
}
