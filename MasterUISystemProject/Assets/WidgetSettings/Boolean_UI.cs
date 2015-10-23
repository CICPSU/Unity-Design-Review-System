using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Boolean_UI : FieldUIs {

	public Toggle toggleInput;
	
	public override object GetFieldValue()
	{
		
		try
		{
			return toggleInput.isOn;
		}
		catch(Exception e)
		{
			return null;
		}
	}
}
