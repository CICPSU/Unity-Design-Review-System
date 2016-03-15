using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Vector2UI : FieldUIs {

	public InputField xInput;
	public InputField yInput;

	public override object GetFieldValue()
	{
		try
		{
			Vector2 parsed = new Vector2(float.Parse(xInput.text), float.Parse(yInput.text));
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
        Vector2 vectorValue = (value as Vector2?).Value;
		xInput.text = vectorValue.x.ToString ();
		yInput.text = vectorValue.y.ToString ();
	}

}
