using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WidgetSettingsManager : MonoBehaviour{

	public string settingsFileFolderPath;

	public RectTransform widgetList;
	public RectTransform fieldsList;
    public RectTransform errorWindow;

	public GameObject displaySettingsPanel;

	private Type activeSettingsFileType;

	private List<string> loadedFiles = new List<string>();
    private List<Type> settingsTypes = new List<Type>();
    private List<Type> loadedTypes = new List<Type>();

	void Awake()
	{
		LoadSettingsFiles ();
		//XmlIO.Save(new AvatarSettings(), settingsFileFolderPath + "\\AvatarSettings.sets");
	}

	public void LoadSettingsFiles()
	{
        settingsTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assembly.GetTypes()
                         where type.IsSubclassOf(typeof(WidgetSettings))
                         select type).ToList<Type>();

		if (string.IsNullOrEmpty (settingsFileFolderPath)) 
			settingsFileFolderPath = Application.dataPath + "/FullPackage/Settings";	

		loadedFiles = new List<string> ();

		List<string> settingsFiles = new List<string>(Directory.GetFiles(settingsFileFolderPath, "*.sets"));
		foreach(string file in settingsFiles)
		{
			OperatingSystem systemInfo = Environment.OSVersion;
			string tmpFile = string.Empty;
			if(System.PlatformID.Unix == systemInfo.Platform){
				tmpFile = file.Substring(file.LastIndexOf("/") + 1);
			}else{
				tmpFile = file.Substring(file.LastIndexOf("\\") + 1);
			}
			string typeName = tmpFile.Substring(0,tmpFile.Length - 5);
			Type fileType = System.Type.GetType(typeName);
            loadedTypes.Add(fileType);
			WidgetSettings loadedFile = XmlIO.Load(file, fileType) as WidgetSettings;
			loadedFile.ApplySettings();
			loadedFiles.Add(tmpFile);
			Debug.Log("loading: " + tmpFile);
		}
        foreach(Type missingType in (settingsTypes.Except(loadedTypes)))
        {
            WidgetSettings settingObj = Activator.CreateInstance(missingType) as WidgetSettings;
            XmlIO.Save(settingObj, settingsFileFolderPath + "\\" + missingType.Name + ".sets");
        }

	}

	public void GenerateSettingsFileButtons()
	{

		// we need to clear out the children in the list before we generate new ones
		for (int i = 0; i < widgetList.transform.childCount; i ++)
		{
			//widgetList.transform.GetChild(i).gameObject.SetActive(false);
			Debug.Log("destroying: " + widgetList.transform.GetChild(i).name);
			Destroy(widgetList.transform.GetChild(i).gameObject);

		}

		// we need to clear out the children in the list before we generate new ones
		for (int i = 0; i < fieldsList.transform.childCount; i ++)
		{
			fieldsList.transform.GetChild(i).gameObject.SetActive(false);
			Debug.Log("destroying: " + fieldsList.transform.GetChild(i).name);
			Destroy(fieldsList.transform.GetChild(i).gameObject);
			
		}

		foreach(string name in loadedFiles)
		{
			GameObject newBut = Instantiate(Resources.Load("WidgetSettings/WidgetButton")) as GameObject;
			newBut.transform.SetParent(widgetList.transform);
			newBut.GetComponentInChildren<Text>().text = name;

			// code to add a listener to the button OnClicked() event
			EventTrigger eTrigger = newBut.GetComponent<EventTrigger>();
			EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
			
			// The following line adds the DisplaySettingsFile function as a listener to the EventTrigger on the button we instantiated.
			trigger.AddListener((eventData)=>displaySettingsPanel.SetActive(true));
			trigger.AddListener((eventData)=>DisplaySettingsFile(newBut));


			// The next line adds the entry we created to the Event Trigger of the instantiated button.
			// The entry consists of two parts, the listener we set up earlier, and the EventTriggerType.
			// The EventTriggerType tells the EventTrigger when to send out the message that the event has occured.
			// We use PointerClick so we know when the used has clicked on a button.
			EventTrigger.Entry entry = new EventTrigger.Entry(){callback = trigger, eventID = EventTriggerType.PointerClick};
			eTrigger.triggers.Add(entry);

		}
	}

	public void DisplaySettingsFile(GameObject clickedButton)
	{

		// we need to clear out the children in the list before we generate new ones
		for (int i = 0; i < fieldsList.transform.childCount; i ++)
		{
			fieldsList.transform.GetChild(i).gameObject.SetActive(false);
			Debug.Log("destroying: " + fieldsList.transform.GetChild(i).name);
			Destroy(fieldsList.transform.GetChild(i).gameObject);
			
		}

		string file = settingsFileFolderPath + "/" + clickedButton.GetComponentInChildren<Text> ().text;

		string tmpFile = clickedButton.GetComponentInChildren<Text> ().text.Substring(0, clickedButton.GetComponentInChildren<Text> ().text.Length - 5);
		Type fileType = System.Type.GetType(tmpFile);
		activeSettingsFileType = fileType;

		WidgetSettings displayedFile = XmlIO.Load (file, fileType) as WidgetSettings;

		object[] displayedValues = displayedFile.GetValues ();

		FieldInfo[] fieldsArray = fileType.GetFields ();

		for (int i = 0; i < fieldsArray.Length - 2; i+=2)
		{
			GameObject fieldUI = Instantiate (Resources.Load ("WidgetSettings/" + fieldsArray [i].FieldType.Name + "_UI")) as GameObject;
			fieldUI.transform.SetParent (fieldsList.transform);
			fieldUI.transform.FindChild("Title").GetComponent<Text>().text = fieldsArray[i].Name;
            fieldUI.GetComponent<FieldUIs>().description = displayedValues[i + 1].ToString();
			fieldUI.GetComponent<FieldUIs>().SetFieldValue(displayedValues[i]);

		}

	}

	public void SaveSettingsFile()
	{
		WidgetSettings objToSave = (WidgetSettings)System.Activator.CreateInstance(activeSettingsFileType);

		object[] valuesToSave = new object[fieldsList.childCount];

		for (int i = 0; i < fieldsList.childCount; i ++)
		{
			valuesToSave[i] = fieldsList.GetChild(i).GetComponent<FieldUIs>().GetFieldValue();
			if(valuesToSave[i] == null)
			{
                //Debug.Log("need to decide how to implement the error message that should be displayed here");
                errorWindow.gameObject.SetActive(true);
                errorWindow.GetChild(1).GetComponentInChildren<Text>().text = "Error: Inalid input, please correct settings.";
				return;
			}
		}

		objToSave.SetValues(valuesToSave);

		string file = settingsFileFolderPath + "/" + activeSettingsFileType.Name + ".sets";

		Debug.Log("saving: " + file);
		XmlIO.Save (objToSave, file);
        //LoadSettingsFiles();
	}

    public WidgetSettings GetSettingsFile(string fileName, Type settingsType)
    {
        return XmlIO.Load(settingsFileFolderPath + "/" + fileName + ".sets", settingsType) as WidgetSettings;
    }

    // need to make an overload of the savesettingsfile function that takes in an widgetsettings object
    // that contains all of the settings that need saved
    // this can be used to save off the minimap settings whenever a size or zoom button is clicked by passing in 
    // an object that contains the new minimap settings
    public void SaveSettingsFile(WidgetSettings objToSave, Type settingsType)
    {
        XmlIO.Save(objToSave, settingsFileFolderPath + "/" + settingsType.Name + ".sets");
    }
}
