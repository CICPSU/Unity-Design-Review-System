using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HintManager : MonoBehaviour {

	public List<string> hints = new List<string>();

	private Text hintText = null;

	private bool textUpdated = false;

	void Start()
	{
		hintText = gameObject.GetComponentInChildren<Text> ();
		hintText.text = hints [0];
	}

	public void ChangeHint(int hint)
	{
		hintText.text = hints [hint];
	}

}
