﻿using UnityEngine;
using System.Collections;

namespace CurvedUI
{
    public class CUI_CameraController : MonoBehaviour
    {




        public static CUI_CameraController instance;


        [SerializeField]
        Transform CameraObject;

		[SerializeField]
        float rotationMargin = 25;

        // Use this for initialization
        void Awake()
        {
            instance = this;
        }


        #if UNITY_EDITOR
        // Update is called once per frame
        void Update()
        {
            CameraObject.localEulerAngles = new Vector3(Input.mousePosition.y.Remap(0, Screen.height, rotationMargin, -rotationMargin),
                                                        Input.mousePosition.x.Remap(0, Screen.width, -rotationMargin, rotationMargin),
                                                           0);
        }
        #endif
    }
}
