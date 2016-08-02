using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class StringUI : FieldUIs {

	public InputField stringInput;

	public override object GetFieldValue()
	{

		try
		{
			return stringInput.text;
		}
		catch(Exception e)
		{
			return null;
		}
	}

	public override void SetFieldValue(object value)
	{
        descriptionText.text = description;
        stringInput.text = value.ToString();
	}
}
