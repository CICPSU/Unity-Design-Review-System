using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class S_AddCityBut : MonoBehaviour {
	public RectTransform newCityPanel;
	public RectTransform emptyInputWarningPanel;
	public InputField cityNameInput;
	public InputField longitudeInput;
	public InputField latitudeInput;
	public InputField timeZoneInput;
	public Text cityOnDisplay;
	public RectTransform contentPanel;
	public RectTransform CityExistedPanel;

    /// <summary>
    /// This button is called to add a city from the NewCityPanel.
    /// </summary>
	public void addCity(){
        // If all of the fields are filled out, add the city.
		if(cityNameInput.text != "" && longitudeInput.text != "" && latitudeInput.text != "" && timeZoneInput.text != "")
        {
			City city = new City(cityNameInput.text, float.Parse(longitudeInput.text), float.Parse(latitudeInput.text), int.Parse(timeZoneInput.text));
			if(cityExisted(city.CityName)){
				//overwrite, continue?
				CityExistedPanel.gameObject.SetActive(true);
			}else{
				SunLightWidget.Instance.InputData.CurrentCity = city;
				SunLightWidget.Instance.InputData.ListOfCity.Add(city); //add new city to database
				SunLightWidget.Instance.saveDataToXML();// save to xml
				cityOnDisplay.text = city.CityName; //change the display city
				SunLightWidget.Instance.AddNewCityToDropDown(city.CityName);
				newCityPanel.gameObject.SetActive(false);
				if(! SunLightWidget.Instance.CityDropdownLongEnough()){
					SunLightWidget.Instance.IncreaseCityDropdownSize();
				}
			}
			SunLightWidget.Instance.calcSunCoordination();

            ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.Sunlight);
        }
        // If all of the fields are not filled out, show the warning window.
        else
        {
			emptyInputWarningPanel.gameObject.SetActive(true);
		}
	}

    /// <summary>
    /// Overwrites a city with new data.
    /// </summary>
	public void overwriteCity(){
		City city = new City(cityNameInput.text, float.Parse(longitudeInput.text), float.Parse(latitudeInput.text), int.Parse(timeZoneInput.text));
		SunLightWidget.Instance.InputData.CurrentCity = city;
		//overwrite the city
		foreach(City item in SunLightWidget.Instance.InputData.ListOfCity){
			if(item.CityName == city.CityName){
				item.Latitude = city.Latitude;
				item.Longitude = city.Longitude;
				item.TimeZone = city.TimeZone;
			}
		}
		//save the changes
		SunLightWidget.Instance.saveDataToXML();
	}

	//return the city if found duplicate, return null if not found
	bool cityExisted(string cityName){
		foreach(City city in SunLightWidget.Instance.InputData.ListOfCity){
			if(city.CityName == cityName){
				return true;
			}
		}
		return false;
	}
}
