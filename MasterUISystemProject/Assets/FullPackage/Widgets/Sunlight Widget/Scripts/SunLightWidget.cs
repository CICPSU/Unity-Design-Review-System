//solar shading calculator
//written by Yifan Liu - ivenaieseccqu@gmail.com
//April. 26th, 2013
// based on fomula from http://www.esrl.noaa.gov/gmd/grad/solcalc/calcdetails.html

// this script calculates the zimuth and altitude angle, which matches(+-1 difference): http://pveducation.org/pvcdrom/properties-of-sunlight/sun-position-calculator
// also matches (+-2): http://www.esrl.noaa.gov/gmd/grad/solcalc/
// the difference is caused by compiler lose of decimal precision when calculating julianday. The compiler loses precision when handling floats with more than seven digits

//INSTRUCTION: Positive z axis is the default NORTH, positive x axis is the default EAST
//euler.y of sun = Azmuith angle of sun -180;
//euler.x of sun = Altitude angle of sun;
//!!!Make sure align the north of the site to the Z axis before using this script.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SunLightWidget : MonoBehaviour {
	public static SunLightWidget Instance;

	public SunlightWidgetData InputData; // The center database of all the input data needed by the calculation algorithm
	public string xmlPath;

	public RectTransform cityButPrefab;
	public RectTransform cityDropdown;
    public RectTransform newCityPanel;

	public Text yearLabelText;
	public Text monthLabelText;
	public Text dateLabelText;
	public Text currentCityOnDisplay;

	public Transform sun;
	public static DateTime time = new DateTime(); // see the DateTime script for class definition
	public float longitude;
	public float latitude;
	public float localTime;	
	public float timeZone;
	
	public float altitude;
	public float azimuth;
	
	public float julianDay;
	public float julianCentury;
	public float geomMeanLongSun;
	public float geomMeanAnomSun;
	public float eccentEarthOrbit;
	public float sunEqOfCtr;
	public float sunTrueLong;
	public float sunAppLong;
	public float meanObliqEcliptic;
	public float ObliqCorr;
	public float declinationAngle;
	public float varY;
	public float eqOfTime;
	public float trueSolarTime;
	public float hourAngle;
	public float zenithAngle;

	private RectTransform hourSlider; // the hour of day is not saved into the inputData and xml. It is directly read from the slider value, range [0,1]


//	private bool widgetStart; // default is false, turn to true when the "start widget" is clicked
	public bool dayLightSaving; // default is false, true -> daylight saving -> deduct an hour from localtime during calculation

	void Awake(){
		xmlPath = Application.dataPath + "/sunlightWidgetData.xml";
		if(Instance == null){
			Instance = this;
		}
		else{
			Debug.LogError("Error: two instances of sunlightwidget");
		}
	}

	// Use this for initialization
	//PRE: Assign panel to the panel variable in inspector
	void Start () {
		if(true){//need to check for file path validity
			InputData  = XmlIO.Load(xmlPath, typeof(SunlightWidgetData)) as SunlightWidgetData;
		}
//		Debug.Log("input loaded: " + InputData);

		//this section initializes the UI with the inputData loaded from the xml
		populateCityDropDown(InputData.ListOfCity, cityDropdown,cityButPrefab);
		populateTimeLabels(InputData);
		currentCityOnDisplay.text = InputData.CurrentCity.CityName;

		//set up reference to hourSlider
		hourSlider = transform.FindChild("mainPanel").FindChild("Slider") as RectTransform;

		dayLightSaving = false; 
	}

    public void CloseSunlightWidget()
    {
        ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.Sunlight);
        CloseNewCityPanel();
        gameObject.SetActive(false);
        SettingsManager.Instance.wc_Settings.sl_Enabled = false;
        SettingsManager.Instance.SaveWidgetControlSettings();
    }

    public void CloseNewCityPanel()
    {
        newCityPanel.gameObject.SetActive(false);
        ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.Sunlight);
    }

    public void OpenNewCityPanel()
    {
        if (ActiveWidgetManager.ActivateWidget(ActiveWidgetManager.ActiveWidget.Sunlight))
        {
            newCityPanel.gameObject.SetActive(true);
            newCityPanel.anchoredPosition3D = new Vector3(350, -80, 0);

            Vector3[] corners = new Vector3[4];
            newCityPanel.GetWorldCorners(corners);
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
            if (!screenRect.Contains(corners[2]))
                newCityPanel.anchoredPosition3D = new Vector3(-105, -80, 0);
        }
    }

	public void AddNewCityToDropDown(string cityName){
		RectTransform cityItem = Instantiate(cityButPrefab) as RectTransform;
		cityItem.parent = cityDropdown;
		cityItem.FindChild("Text").GetComponent<UnityEngine.UI.Text>().text = cityName;
	}

	void populateCityDropDown(List<City> listOfCity, RectTransform dropdownPanel, RectTransform cityButPrefab){
		//remove existing item in the dropdown
		foreach(RectTransform item in dropdownPanel){
			Destroy (item.gameObject);
		}

		foreach(City city in listOfCity){
			RectTransform cityItem = Instantiate(cityButPrefab) as RectTransform;
			cityItem.SetParent(dropdownPanel);
			cityItem.FindChild("Text").GetComponent<UnityEngine.UI.Text>().text = city.CityName;
			if(!CityDropdownLongEnough()){
				IncreaseCityDropdownSize();
			}
		}
	}

	public void IncreaseCityDropdownSize(){
		cityDropdown.sizeDelta = new Vector2(121.3f, cityDropdown.sizeDelta.y + 18.0f);
	}

	//overload method allows specify size
	public void IncreaseCityDropdownSize(float incrementSize){
		cityDropdown.sizeDelta = new Vector2(121.3f, cityDropdown.sizeDelta.y + incrementSize);
	}


	//check if cityDropdown(contenPanel)'s height is enought to hold all the cities
	public bool CityDropdownLongEnough(){
		if(cityDropdown.sizeDelta.y - 16 * InputData.ListOfCity.Count <= 2){
			return false;
		}else{
			return true;
		}
	}


	void populateTimeLabels(SunlightWidgetData database){
		dateLabelText.text = database.Date.ToString();
		monthLabelText.text = database.Month.ToString();
		yearLabelText.text = database.Year.ToString();
	}

	//save the inputDate to the xml file at path Application.dataPath + "/sunlightWidgetData.xml"
	public void saveDataToXML(){
		XmlIO.Save(InputData, xmlPath);
	}
	
	//This method calls all the functions needed to calculate the sun position and rotate the sun
	//called by the hour slider
	public void calcSunCoordination(){
		time = new DateTime(InputData.Year, InputData.Month, InputData.Date);
		localTime = hourSlider.GetComponent<Slider>().value;
		longitude = InputData.CurrentCity.Longitude;
		latitude = InputData.CurrentCity.Latitude;
		timeZone = InputData.CurrentCity.TimeZone;

		julianDay = dateToJulian(time);
		julianCentury = calcJulianCentury(time);
		geomMeanAnomSun = calcGeoMeanAnomSun(time);
		eccentEarthOrbit = calcEccentEarthOrbit(time);
		meanObliqEcliptic = calcMeanObliqEcliptic(time);
		ObliqCorr = calcObliqCorr(meanObliqEcliptic);
		varY = calcVarY(ObliqCorr);
		geomMeanLongSun = calcMeanLongSun(julianCentury);
		eqOfTime = calcEqOfTime(geomMeanLongSun,geomMeanAnomSun,eccentEarthOrbit,varY);
		if(!dayLightSaving){
			trueSolarTime = calcTrueSolarTime(localTime,eqOfTime,longitude,timeZone);
		}
		else{
			trueSolarTime = calcTrueSolarTime(localTime - 0.041667f,eqOfTime,longitude,timeZone);
		}
		hourAngle = calcHourAngle(trueSolarTime);
		sunEqOfCtr = calcSunEqOfCtr(julianCentury,geomMeanAnomSun);
		sunTrueLong = calcSunTrueLong(geomMeanLongSun,sunEqOfCtr);
		sunAppLong = calcSunAppLong(sunTrueLong);
		declinationAngle = calcDeclinationAngle(ObliqCorr,sunAppLong);
		zenithAngle = calcZenithAngle(latitude, declinationAngle,hourAngle);
		azimuth = calcAzimuthAngle(hourAngle,latitude,zenithAngle,declinationAngle);
//		Debug.Log("azimuth: " + azimuth);
		altitude = calcAltitudeAngle(zenithAngle);
//		Debug.Log("altitude: " + altitude);
		//rotate the sun		
		rotateSun(azimuth, altitude);

		adjustSunIntensity(altitude);
	}

	//reduce sun intensity during dust and increase sun intensity during dawn
	private void adjustSunIntensity(float altitude){
		//check if sun is below ground, if so reduce sun intensity
		if(altitude <= 0){
				float intensity = sun.GetComponent<Light>().intensity;

					sun.GetComponent<Light>().intensity = Mathf.Lerp(1,0.01f,(0f - altitude)/10);

		}else{ // this is to prevent a jump in time, i.e. non-continuous time change
			if(sun.GetComponent<Light>().intensity < 0.9f){
				sun.GetComponent<Light>().intensity = 1;
			}
		}
	}
	
	//PRE: azimuth and altitude angle must be initialized
	//POST: rotate the sun based on the azimuth and altitude angle
	public void rotateSun(float azimuth, float altitude){
		while(azimuth < 0){
			azimuth += 360;
		}
		while (azimuth > 360){
			azimuth -= 360;	
		}
		//set azimuth angle
		sun.eulerAngles = new Vector3(0, azimuth - 180, 0);  // to set z be north, y should be azimuth - 180, if z is south , y should be azimuth
		//set altitude angle
		sun.eulerAngles = new Vector3(altitude, sun.eulerAngles.y, sun.eulerAngles.z);
		 
	}
	
	//converts the current date to Julianday
	public float dateToJulian(DateTime time){
		float month = time.Month;
		float day = time.Day;
		float year = time.Year;
		
		if(month < 3){
			month = month + 12;
			year = year -1;
		}
		
		// NOTE that julianDay here loses precision. It is truncated into an int after adding "1721119". The float or double cannot hold decimals when storing
		//numbers with more than 7 digits.
		float julianDay = day + (153.0f * month - 457.0f) / 5.0f + 365.0f * year + (year/4.0f) - (year / 100.0f) + (year/400.0f) + 1721119.0f + localTime - timeZone/24.0f;

		//print out julianDay
	//	Debug.Log(julianDay);
		return julianDay;
	}
	
	
	public float calcAzimuthAngle(float hourAngle, float latitude, float zenithAngle, float declinationAngle){
		float radLatitude = latitude * Mathf.Deg2Rad;
		float radZenithAngle = zenithAngle * Mathf.Deg2Rad;
		float radDeclinationAngle = declinationAngle * Mathf.Deg2Rad;
		float sinRadLatitude = Mathf.Sin(radLatitude);
		float sinRadDeclinationAngle = Mathf.Sin(radDeclinationAngle);
		float sinRadZenithAngle = Mathf.Sin(radZenithAngle);
		float cosRadLatitude = Mathf.Cos(radLatitude);
	//	float cosRadDeclinationAngle = Mathf.Cos(radDeclinationAngle);
		float cosRadZenithAngle = Mathf.Cos(radZenithAngle);
		float azimuth;
		if(hourAngle > 0){
			 azimuth = 
			(Mathf.Acos(((sinRadLatitude * cosRadZenithAngle) - sinRadDeclinationAngle)/(cosRadLatitude * sinRadZenithAngle))*Mathf.Rad2Deg + 180) % 360;
		}
		else{
			 azimuth = 
			(540 - Mathf.Acos(((sinRadLatitude * cosRadZenithAngle)- sinRadDeclinationAngle)/ (cosRadLatitude * sinRadZenithAngle))*Mathf.Rad2Deg) % 360;
		}
		return azimuth;
	}
	
	public float calcAltitudeAngle(float zenithAngle){
		return 90-zenithAngle;
	}
		
	public float calcHourAngle(float trueSolarTime){
		float hourAngle;
		if(trueSolarTime /4 < 0){
			hourAngle = trueSolarTime/4 + 180;
		}
		else{
			hourAngle = trueSolarTime/4 -180;
		}
		return hourAngle;
	}
	
	public float calcTrueSolarTime(float localTime, float eqOfTime, float longitude, float timeZone){
		float trueSolarTime1;
		if((localTime * 1440 + eqOfTime + 4* longitude - 60* timeZone) < 0){
			trueSolarTime1 = (localTime * 1440 + eqOfTime + 4* longitude - 60* timeZone) - 1440 * Mathf.Floor((localTime * 1440 + eqOfTime + 4* longitude - 60* timeZone)/1440);
		}
		else{
			trueSolarTime1 = ((localTime * 1440 + eqOfTime + 4* longitude - 60* timeZone) % 1440);
		}
		return trueSolarTime1;
	}
	
	public float calcEqOfTime(float geomMeanLongSun, float geomMeanAnomSun, float eccentEarthOrbit, float varY){
		float radMeanLongSun = geomMeanLongSun * Mathf.Deg2Rad;
		float radGeoMeanAnomSun = geomMeanAnomSun * Mathf.Deg2Rad;
		float eqOfTime;
		
		eqOfTime = 
			4*(varY * Mathf.Sin(2 * radMeanLongSun) - 2 * eccentEarthOrbit * Mathf.Sin(radGeoMeanAnomSun) + 4 * eccentEarthOrbit * varY * Mathf.Sin(radGeoMeanAnomSun) * Mathf.Cos(2 * radMeanLongSun) - 0.5f *varY * varY*Mathf.Sin(4 * radMeanLongSun) - 1.25f * eccentEarthOrbit * eccentEarthOrbit * Mathf.Sin(2 * radGeoMeanAnomSun))*Mathf.Rad2Deg;
		return eqOfTime;
	}
	
	public float calcMeanLongSun(float julianCentury){
		float geomMeanLongSun = (280.46646f + julianCentury * (36000.76983f + julianCentury * 0.0003032f))% 360;
		return geomMeanLongSun;
		
	}
		
	public float calcJulianCentury(DateTime time){
		float julianCentury = (dateToJulian(time) - 2451545) / 36525;
		return julianCentury;
	}
	
	public float calcVarY(float obliqCorr){
		float varY = Mathf.Tan((obliqCorr/2)*Mathf.Deg2Rad)*Mathf.Tan((obliqCorr/2)*Mathf.Deg2Rad);
		return varY;
	}
	
	public float calcObliqCorr(float meanObliqEcliptic){
		float obliqCorr = 
			meanObliqEcliptic + 0.00256f * Mathf.Cos((125.04f - 1934.136f * calcJulianCentury(time))*Mathf.Deg2Rad);
		return obliqCorr;
	}
	
	public float calcMeanObliqEcliptic(DateTime time){
		float julianCentury = calcJulianCentury(time);
		float meanObliqEcliptic = 23+(26+((21.448f-julianCentury*(46.815f+julianCentury*(0.00059f - julianCentury * 0.001813f))))/60)/60;
		return meanObliqEcliptic; 
	}
	
	public float calcEccentEarthOrbit(DateTime time){
		float julianCentury = calcJulianCentury(time);
		float eccenEarthOrbit = 0.016708634f - julianCentury * (0.000042037f+0.0000001267f* julianCentury);
		return eccenEarthOrbit;
	}
	
	public float calcGeoMeanAnomSun(DateTime time){
		float julianCentury = calcJulianCentury(time);
		float geoMeanAnomSun = 357.52911f+julianCentury*(35999.05029f - 0.0001537f*julianCentury);
		return geoMeanAnomSun;
	}
	
	public float calcDeclinationAngle(float obliqCorr, float sunAppLong){
		float declinationAngle =  Mathf.Asin(Mathf.Sin(obliqCorr* Mathf.Deg2Rad)* Mathf.Sin(sunAppLong* Mathf.Deg2Rad))*Mathf.Rad2Deg;
		return declinationAngle;
	}
	
	public float calcSunAppLong(float sunTrueLong){
		float sunAppLong = sunTrueLong - 0.00569f-0.00478f*Mathf.Sin((125.04f-1934.136f*this.julianCentury)*Mathf.Deg2Rad);
		return sunAppLong;
	}
	
	public float calcSunTrueLong(float geomMeanLongSun, float sunEqOfCtr){
		return geomMeanLongSun + sunEqOfCtr;
	}
	
	public float calcSunEqOfCtr(float julianCentury, float geomMeanAnomSun){
		return (Mathf.Sin(geomMeanAnomSun*Mathf.Deg2Rad)*(1.914602f - julianCentury*(0.004817f+0.000014f*julianCentury)) + 
			Mathf.Sin(geomMeanAnomSun *2 *Mathf.Deg2Rad)*(0.019993f-0.000101f*julianCentury) + Mathf.Sin(3*geomMeanAnomSun*Mathf.Deg2Rad* 0.000289f));
	}
	
	public float calcZenithAngle(float latitude, float declinationAngle, float hourAngle){
		return ((Mathf.Acos(Mathf.Sin(latitude*Mathf.Deg2Rad)*Mathf.Sin(declinationAngle *Mathf.Deg2Rad)+ 
			Mathf.Cos(latitude*Mathf.Deg2Rad)*Mathf.Cos(declinationAngle*Mathf.Deg2Rad)*Mathf.Cos(hourAngle*Mathf.Deg2Rad)))*Mathf.Rad2Deg);	
	}
	
}
	