using UnityEngine;
using System.Collections;

public class GazeCrosshairOnOff : MonoBehaviour 
{

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			GazeInputModuleCrosshair.DisplayCrosshair = !GazeInputModuleCrosshair.DisplayCrosshair;
		}
	}
}
