using UnityEngine;
using System.Collections.Generic;

public class S_CitySelected : MonoBehaviour {
	string cityName = "";

	//called by the On Click() on each item of the city dropdown list
	public void clicked(){
		cityName = this.transform.FindChild("Text").GetComponent<UnityEngine.UI.Text>().text;
		//update database and save to xml
		SunLightWidget.Instance.InputData.setCurrentCityByName(cityName);
		SunLightWidget.Instance.saveDataToXML();
		//update UI
		transform.parent.parent.parent.FindChild("CityOnDisplay").GetComponent<UnityEngine.UI.Text>().text = cityName;
		transform.parent.parent.gameObject.SetActive(false);
		//update sun angle
		SunLightWidget.Instance.calcSunCoordination(); 
	}
	
}
