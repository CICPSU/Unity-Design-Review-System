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

}
