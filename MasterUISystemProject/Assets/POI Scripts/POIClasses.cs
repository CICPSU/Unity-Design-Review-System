using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class POI
{
	[XmlElement("SceneFlag")]
	public string sceneFlag;

    [XmlElement("Name")]
    public string buttonName;

    [XmlElement("Position")]
    public Vector3 position;

    [XmlElement("Rotation")]
    public Vector3 rotation;

	[XmlElement("MarkerPrefabName")]
	public string markerModelPrefab;

	[XmlIgnore]
	public GameObject marker;

    public POI()
    {
    }

    public POI(string sFlag, string bName, Vector3 pos, Vector3 rot, string marker)
    {
        sceneFlag = sFlag;
        buttonName = bName;
        position = pos;
        rotation = rot;
		markerModelPrefab = marker;
    }

	public void Update(string sFlag, string bName, Vector3 pos, Vector3 rot, string marker){
		sceneFlag = sFlag;
		buttonName = bName;
		position = pos;
		rotation = rot;
		markerModelPrefab = marker;
	}

	//copy by value
	public void UpdateByValue(POI point){
		sceneFlag = point.sceneFlag;
		buttonName = point.buttonName;
		position = point.position;
		rotation = point.rotation;
		markerModelPrefab = point.markerModelPrefab;
	}
}

[XmlRoot]
public class POIHandler
{
    [XmlArray("ProjectPOIs")]
    [XmlArrayItem("POI")]
    public List<POI> projectPOIs;

	public POIHandler()
	{
		projectPOIs = new List<POI>();
	}

    public void AddPoint(POI point)
    {
		projectPOIs.Add(point);
    }

	public void RemovePoint(POI point)
	{
		projectPOIs.Remove(point);
	}

	public void UpdatePoint(POI oldPoint, POI newPoint)
	{
		for(int i = 0; i< projectPOIs.Count; i++)
		{
			if(projectPOIs[i] == oldPoint){
				projectPOIs[i] = newPoint;
				oldPoint = newPoint;
			}
		}
	}

}