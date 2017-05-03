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

	public override void SetFieldValue(object value)
	{
        descriptionText.text = description;

		toggleInput.isOn = (value as bool?).Value;
	}
}
