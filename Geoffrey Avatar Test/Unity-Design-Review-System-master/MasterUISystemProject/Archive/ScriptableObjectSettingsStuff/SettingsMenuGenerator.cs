using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class will look for a WidgetSettings component on each of its children.  It will read the settings from that component 
/// and generate a UI to allow the user to edit the settings.
/// </summary>
public class SettingsMenuGenerator : MonoBehaviour {

	/// <summary>
	/// Dictionary to hold the references to the settings components and the gameobjects they are attached to.  The root object of each widget should
	/// be an empty object with a single script attached.  That script should contain properties to set all of the parameters that the developer wants the
	/// user to have access to in the menu.
	/// </summary>
	private Dictionary<String, MonoBehaviour> componentList = new Dictionary<String, MonoBehaviour>();

	void Start()
	{
		SaveSettingsFiles ();
	}

	public void SaveSettingsFiles()
	{
		for (int i = 0; i < transform.childCount; i++) 
		{

			///////// THIS NEEDS TO BE CHANGED SINCE WIDGETSETTINGSBASE IS NOW A SCRIPTABLE OBJECT
			object componentToSave = transform.GetChild(i).GetComponent<WidgetSettingsBase>();
			XmlIO.Save (componentToSave, transform.GetChild(i).name + "_settings.xml");
		}
	}

	public void GenerateMenus()
	{
		/// the first thing we need to do is get references to all of the MenuSettings objects.
		/// we will store the references in a dictionary.  the key will be the full name of the gameobject that the settings component is attached to
		/// and the value will be the reference to the component

		for (int i = 0; i < transform.childCount; i++) 
		{
			componentList.Add(transform.GetChild (i).name, transform.GetChild(i).GetComponent<MonoBehaviour>());
		}
	}
}