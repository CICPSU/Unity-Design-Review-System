using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StringUI : FieldUIs {

	public InputField stringInput;

	public override object GetFieldValue()
	{
		return stringInput.text;
	}
}
