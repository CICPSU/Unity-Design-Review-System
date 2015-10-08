using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SingleUI : FieldUIs {

	public InputField singleInput;

	public override object GetFieldValue()
	{
		return float.Parse(singleInput.text);
	}

}
