using UnityEngine;
using System.Collections;

public abstract class FieldUIs : MonoBehaviour {

	public abstract object GetFieldValue();

	public abstract void SetFieldValue(object value);
}
