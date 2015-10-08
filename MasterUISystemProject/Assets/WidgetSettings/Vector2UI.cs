using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Vector2UI : FieldUIs {

	public InputField xInput;
	public InputField yInput;

	public override object GetFieldValue()
	{
		return (new Vector2(float.Parse(xInput.text), float.Parse(yInput.text)));
	}

}
