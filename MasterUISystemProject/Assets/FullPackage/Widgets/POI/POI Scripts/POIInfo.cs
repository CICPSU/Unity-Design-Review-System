using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class POIInfo : MonoBehaviour {
    
    public string sceneName;
    public string buttonName;
    public Vector3 position;
    public Vector3 rotation;
	public string markerPrefabName;

	private POI point = new POI();


//returns the POI by references, be aware of the implication of this when developing
    public POI Point{
     get{

         point.sceneFlag = sceneName;
         point.buttonName = buttonName;
         point.position = position;
         point.rotation = rotation;
		 point.markerModelPrefab = markerPrefabName;
         return point;
     }

        set
        {
            point = value;
            sceneName = point.sceneFlag;
            buttonName = point.buttonName;
            position = point.position;
            rotation = point.rotation;
            if (value.markerModelPrefab != "" && value.markerModelPrefab != null)
                markerPrefabName = point.markerModelPrefab;
            else
                point.markerModelPrefab = markerPrefabName;
        }
    }

   
}
