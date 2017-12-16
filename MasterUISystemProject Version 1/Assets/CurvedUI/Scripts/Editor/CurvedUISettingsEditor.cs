﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

#if CURVEDUI_TMP
using TMPro;
#endif 


namespace CurvedUI { 

[ExecuteInEditMode]
[CustomEditor(typeof(CurvedUISettings))]
public class CurvedUISettingsEditor : Editor {

        ////GUI
        //GUISkin editorSkin;
        //GUIStyle GUIPhoto;

        ////Editor options
        //[SerializeField] bool openFeedback = false;
        //[SerializeField] bool openAdvanced = false;

        ////internal values
        //string message = "";

        bool ShowRemoveCurvedUI = false;
        static bool ShowAdvaced = false;

    void Start(){
		AddCurvedUIComponents();
	}

        public override void OnInspectorGUI() {
        CurvedUISettings myTarget = (CurvedUISettings)target;

        if (target == null) return;
        ////load up editor skin
        //if (editorSkin == null) {
        //    editorSkin = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/CurvedUI/Scripts/Editor/CurvedUIGUISkin.guiskin", typeof(GUISkin)));
        //    GUIPhoto = editorSkin.FindStyle("DanielPhoto");
        //}
        //GUI.skin = editorSkin;

        //initial settings
        GUI.changed = false;
        EditorGUIUtility.labelWidth = 150;

        //shape settings
        GUILayout.Label("Shape", EditorStyles.boldLabel);
        myTarget.Shape = (CurvedUISettings.CurvedUIShape)EditorGUILayout.EnumPopup("Canvas Shape", myTarget.Shape);
        switch (myTarget.Shape) {
            case CurvedUISettings.CurvedUIShape.CYLINDER: {
                myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, -360, 360);
                myTarget.PreserveAspect = EditorGUILayout.Toggle("Preserve Aspect", myTarget.PreserveAspect);

                break;
            }
            case CurvedUISettings.CurvedUIShape.CYLINDER_VERTICAL:
            {
                myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, -360, 360);
                myTarget.PreserveAspect = EditorGUILayout.Toggle("Preserve Aspect", myTarget.PreserveAspect);

                break;
            }
            case CurvedUISettings.CurvedUIShape.RING: {
                myTarget.RingExternalDiameter = Mathf.Clamp(EditorGUILayout.IntField("External Diameter", myTarget.RingExternalDiameter), 1, 100000);
                myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, 0, 360);
                myTarget.RingFill = EditorGUILayout.Slider("Fill", myTarget.RingFill, 0.0f, 1.0f);
                myTarget.RingFlipVertical = EditorGUILayout.Toggle("Flip Canvas Vertically", myTarget.RingFlipVertical);
                break;
            }
            case CurvedUISettings.CurvedUIShape.SPHERE: {
                GUILayout.BeginHorizontal();
                GUILayout.Space(150);
                GUILayout.Label("Sphere shape is more expensive than a Cyllinder shape. Keep this in mind when working on mobile VR.", EditorStyles.helpBox);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                if (myTarget.PreserveAspect) {
                    myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, -360, 360);
                } else {
                    myTarget.Angle = EditorGUILayout.IntSlider("Horizontal Angle", myTarget.Angle, 0, 360);
                    myTarget.VerticalAngle = EditorGUILayout.IntSlider("Vertical Angle", myTarget.VerticalAngle, 0, 180);
                }
                myTarget.PreserveAspect = EditorGUILayout.Toggle("Preserve Aspect", myTarget.PreserveAspect);

                break;
            }
        }


           

        //advanced settings
        GUILayout.Space(10);

        if (!ShowAdvaced) {
            if (GUILayout.Button("Show Advanced Settings")) ShowAdvaced = true;
        } else {
            if (GUILayout.Button("Hide Advanced Settings")) ShowAdvaced = false;

            //GUILayout.Label("Advanced Settings", EditorStyles.boldLabel);


            //controller
            myTarget.Controller = (CurvedUISettings.CurvedUIController)EditorGUILayout.EnumPopup("Control Method", myTarget.Controller);
            GUILayout.BeginHorizontal();
            GUILayout.Space(150);
            switch (myTarget.Controller)
            {
                case CurvedUISettings.CurvedUIController.MOUSE:
                {

                    GUILayout.Label("Basic Controller. Mouse in screen space.", EditorStyles.helpBox);
                    break;
                }
                case CurvedUISettings.CurvedUIController.GAZE:
                {
                    GUILayout.Label("Center of Canvas's World Camera acts as a pointer.", EditorStyles.helpBox);
                    break;
                }
                case CurvedUISettings.CurvedUIController.WORLD_MOUSE:
                {
                    GUILayout.Label("Mouse controller that is independent of the camera view. Use WorldSpaceMouseOnCanvas function to get its position.", EditorStyles.helpBox);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(150);
                    myTarget.WorldSpaceMouseSensitivity = EditorGUILayout.FloatField("Mouse Sensitivity", myTarget.WorldSpaceMouseSensitivity);
                    break;
                }
                case CurvedUISettings.CurvedUIController.CUSTOM_RAY:
                {
                    GUILayout.Label("You can set a custom ray as a controller with CustomControllerRay function. Raycaster will use that ray to find selected objects.", EditorStyles.helpBox);
                    break;
                }
            }
            GUILayout.EndHorizontal();


            myTarget.Interactable = EditorGUILayout.Toggle("Interactable", myTarget.Interactable);
            myTarget.RaycastMyLayerOnly = EditorGUILayout.Toggle("Raycast My Layer Only", myTarget.RaycastMyLayerOnly);
            myTarget.Quality = EditorGUILayout.Slider("Quality", myTarget.Quality, 0.1f, 3.0f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(150);
            GUILayout.Label("Smoothness of the curve. Bigger values mean more subdivisions. Decrease for better performance. Default 1", EditorStyles.helpBox);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Components", GUILayout.Width(146));
            if (GUILayout.Button("Add Effect To Children"))
            {
                AddCurvedUIComponents();
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(146));

            if (!ShowRemoveCurvedUI) {
                if (GUILayout.Button("Remove CurvedUI from Canvas")) ShowRemoveCurvedUI = true;
            } else {
                if (GUILayout.Button("Remove CurvedUI"))  {
                    RemoveCurvedUIComponents();
                }
                if (GUILayout.Button("Cancel"))  {
                    ShowRemoveCurvedUI = false;
                }
            }
            GUILayout.EndHorizontal();

        }  // end of advanced settings

        //final settings
        if (GUI.changed)
		EditorUtility.SetDirty(myTarget);

	}


	void OnEnable()
	{
		EditorApplication.hierarchyWindowChanged += AddCurvedUIComponents;
	}

	void OnDisable() 
	{
		EditorApplication.hierarchyWindowChanged -= AddCurvedUIComponents;
	}

	//Travel the hierarchy and add CurvedUIVertexEffect to every gameobject that can be bent.
	private void AddCurvedUIComponents()
	{
		if(target == null)return;
		
		(target as CurvedUISettings).AddEffectToChildren();

	}

    private void RemoveCurvedUIComponents()
    {
        if (target == null) return;

        //destroy componenets
        List<CurvedUIVertexEffect> comps = new List<CurvedUIVertexEffect>();
        comps.AddRange((target as CurvedUISettings).GetComponentsInChildren<CurvedUIVertexEffect>(true));
        for (int i = 0; i < comps.Count; i++)
        {
            if (comps[i].GetComponent<UnityEngine.UI.Graphic>() != null) comps[i].GetComponent<UnityEngine.UI.Graphic>().SetAllDirty();
            DestroyImmediate(comps[i]);
            
        }

        //destroy raycasters
        List<CurvedUIRaycaster> raycasters = new List<CurvedUIRaycaster>();
        raycasters.AddRange((target as CurvedUISettings).GetComponents<CurvedUIRaycaster>());
        for (int i = 0; i < raycasters.Count; i++)
        {
            DestroyImmediate(raycasters[i]);
        }

        DestroyImmediate(target);
    }

     
    }
}

