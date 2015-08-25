using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WidgetSettingsManager : MonoBehaviour{

	public string settingsFileFolderPath;

	public RectTransform widgetList;
	public InputField xmlFileDisplay;

	public string activeSettingsFile;

	private List<string> loadedFiles = new List<string>();

	void Start()
	{
		LoadSettingsFiles ();
		GenerateSettingsFileButtons();
		//XmlIO.Save(new WidgetSettings(), settingsFileFolderPath + "\\test1.sets");
	}

	public void CreateTempFile()
	{
		XmlTextWriter writer = new XmlTextWriter(settingsFileFolderPath + "\\" + activeSettingsFile.Remove(activeSettingsFile.LastIndexOf(".")) + ".tempsets", Encoding.ASCII);
		writer.WriteString (xmlFileDisplay.GetComponentInChildren<Text>().text);
		(XmlIO.Load (settingsFileFolderPath + "\\" + activeSettingsFile, typeof(WidgetSettings)) as WidgetSettings).ApplySettings();
	}

	public void OverwriteFile()
	{

	}

	public void LoadSettingsFiles()
	{
		if (string.IsNullOrEmpty (settingsFileFolderPath)) 
			settingsFileFolderPath = Application.dataPath;	
		List<string> settingsFiles = new List<string>(Directory.GetFiles(settingsFileFolderPath, "*.sets"));
		foreach(string file in settingsFiles)
		{
			WidgetSettings loadedFile = XmlIO.Load(file, typeof(WidgetSettings)) as WidgetSettings;
			loadedFile.ApplySettings();
			loadedFiles.Add(file.Substring(file.LastIndexOf("\\") + 1));

		}

	}

	public void GenerateSettingsFileButtons()
	{
		foreach(string name in loadedFiles)
		{
			GameObject newBut = Instantiate(Resources.Load("WidgetSettings/WidgetButton")) as GameObject;
			newBut.transform.SetParent(widgetList.transform);
			newBut.GetComponentInChildren<Text>().text = name;

			// code to add a listener to the button OnClicked() event
			EventTrigger eTrigger = newBut.GetComponent<EventTrigger>();
			EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
			
			// The following line adds the DisplaySettingsFile function as a listener to the EventTrigger on the button we instantiated.
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
		string xmlString = File.ReadAllText(settingsFileFolderPath + "\\" + clickedButton.GetComponentInChildren<Text>().text);
		activeSettingsFile = clickedButton.GetComponentInChildren<Text> ().text;
		int count = xmlString.Split(System.Environment.NewLine.ToCharArray()).Length - 1;
		xmlFileDisplay.text = xmlString;
		xmlFileDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(xmlFileDisplay.GetComponent<RectTransform>().sizeDelta.x, count * 15);
	}
}
