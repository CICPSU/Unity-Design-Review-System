using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SingleUI : FieldUIs {

	public InputField singleInput;

	public override object GetFieldValue()
	{
		try
		{
			float parsed = float.Parse(singleInput.text);
			return parsed;
		}
		catch(Exception e)
		{
			return null;
		}
	}

	public override void SetFieldValue(object value)
	{
        descriptionText.text = description;
        singleInput.text = value.ToString();
	}

}
