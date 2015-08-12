using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

public class WidgetSettingsLoader : MonoBehaviour{
	
	/// this is the script that will actually load the xml file 
	/// this script will be attached to WidgetRoot

	public string settingsFileFolderPath;

	void Start()
	{
		LoadSettingsFiles ();
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

		}

	}
}
