using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Reflection;

public class TestSettings : WidgetSettingsBase {

	/// <summary>
	/// Bool to tell whether the menu should be enabled or not.
	/// </summary>
	public bool isEnabled;

	public void Initialize()
	{
		base.Initialize ();
		widget.SetActive (isEnabled);
	}
}
