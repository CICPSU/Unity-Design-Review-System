using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("City")]
public class City{
	
	[XmlElement("CityName")]
	public string CityName{get;set;}
	
	[XmlElement("Longitude")]
	public float Longitude{get;set;}
	
	[XmlElement("Latitude")]
	public float Latitude{get;set;}
	
	[XmlElement("TimeZone")]
	public int TimeZone{get;set;}

	public City(){
		CityName = "";
		Longitude = 0;
		Latitude = 0;
		TimeZone = 0;
	}

	public City(string name, float longitude, float latitude, int timeZone){
		this.CityName = name;
		this.Longitude = longitude;
		this.Latitude =latitude;
		this.TimeZone = timeZone;
	}
	
}

[XmlRoot()]
public class SunlightWidgetData {
	[XmlElement("Year")]
	public int Year {get; set;}

	[XmlElement("Month")]
	public int Month {get;set;}

	[XmlElement("Date")]
	public int Date{get;set;}

	[XmlElement("CurrentTime")]
	public double CurrentTime{get;set;} //range [0 to 24), decimal is minute

	[XmlElement("CurrentCity")]
	public City CurrentCity;

	[XmlArray("ListOfCity")]
	[XmlArrayItem("City")]
	public List<City> ListOfCity;


	public SunlightWidgetData(){
		Year = 0;
		Month = 0;
		Date = 0;
		CurrentTime = 0;
		ListOfCity =  new List<City>();
		CurrentCity = new City();
	}

	public SunlightWidgetData(int year, int month, int date, int curreTime, City currentCity, List<City> listOfCity){
		this.Year = year;
		this.Month =month;
		this.Date = date;
		this.CurrentTime = curreTime;
		this.CurrentCity = currentCity;
		this.ListOfCity = listOfCity;
	}

	public void setCurrentCityByName(string cityName){
		foreach(City city in ListOfCity){
			if(city.CityName == cityName){
				CurrentCity = city;
				return;
			}
		}
		Debug.LogError("Dropdown list error: the city in the dropdown list is not initialized and set in the sunlightwidgetdata!");
	}

	public void updateDateMonYear(int date, int mon, int year){
		this.Date = date;
		this.Month = mon;
		this.Year = year;
	}

	public override string ToString(){
		return "year: " + Year.ToString() + ",month: " + Month.ToString() + ",time: " + CurrentTime.ToString() + ",Cities: " + ListOfCity.ToString();
	}
}