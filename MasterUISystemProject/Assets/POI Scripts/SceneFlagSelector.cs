using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SceneFlagSelector : MonoBehaviour {

	[SerializeField]
	public List<string> sceneNames = new List<string>();

	private List<Toggle> sceneCheckBoxes = new List<Toggle>();

	void Start()
	{
#if UNITY_EDITOR
		sceneNames = (from scene in UnityEditor.EditorBuildSettings.scenes where scene.enabled select scene.path.Substring(7,scene.path.Length-13)).ToList();

#endif
	}

	public void GenerateSceneCheckBoxes()
	{
		if(POI_ReferenceHub.Instance.SceneFlagList.childCount == 0)
		{
			foreach(string scene in sceneNames)
			{
				GameObject newToggle = Instantiate(Resources.Load("POIPanel/SceneCheckBox")) as GameObject;
				newToggle.GetComponentInChildren<Text>().text = scene;
				newToggle.transform.parent = POI_ReferenceHub.Instance.SceneFlagList;
				LayoutRebuilder.MarkLayoutForRebuild(POI_ReferenceHub.Instance.SceneFlagList.GetComponent<RectTransform>());

				sceneCheckBoxes.Add(newToggle.GetComponent<Toggle>());
			}
		}
	}

	public List<string> GetSceneFlags()
	{
		if(sceneCheckBoxes ==null){
			return null;
		}
		return((from toggle in sceneCheckBoxes where toggle.isOn select toggle.gameObject.GetComponentInChildren<Text>().text).ToList());
	}
}
