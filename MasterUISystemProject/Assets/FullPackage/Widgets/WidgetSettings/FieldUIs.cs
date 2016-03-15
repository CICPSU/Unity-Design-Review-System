using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class FieldUIs : MonoBehaviour {

	public abstract object GetFieldValue();

	public abstract void SetFieldValue(object value);

    public Text descriptionText;

    public string description;
}
