using UnityEngine;
using System.IO;
using System.Collections;
using System.Xml;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public AvatarSettings a_Settings = new AvatarSettings();
    public KeyBindings kb_Settings = new KeyBindings();
    public WidgetControlSettings wc_Settings = new WidgetControlSettings();

    public TP_Motor tp_Motor_Ref;
    public TP_InputManager tp_InputManager_Ref;
    public MiniMapManager mm_Manager_Ref;
    public GameObject mm_GameObject;
    public GameObject sl_GameObject;
    public GameObject bm_GameObject;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    // there are only two settings files we need to load
    // AvatarSettings and KeyBindings
    public void LoadSettingsFiles()
    {
        if (File.Exists(Application.dataPath + "/FullPackage/Settings/AvatarSettings.sets"))
        {
            a_Settings = XmlIO.Load(Application.dataPath + "/FullPackage/Settings/AvatarSettings.sets", typeof(AvatarSettings)) as AvatarSettings;
            ApplyAvatarSettings();
        }
        else
        {
            SaveAvatarSettings();
        }


        if (File.Exists(Application.dataPath + "/FullPackage/Settings/KeyBindings.sets"))
        {
            kb_Settings = XmlIO.Load(Application.dataPath + "/FullPackage/Settings/KeyBindings.sets", typeof(KeyBindings)) as KeyBindings;
            ApplyKeyBindings();
        }
        else
        {
            SaveKeyBindings();
        }

        if (File.Exists(Application.dataPath + "/FullPackage/Settings/WidgetControlSettings.sets"))
        {
            wc_Settings = XmlIO.Load(Application.dataPath + "/FullPackage/Settings/WidgetControlSettings.sets", typeof(WidgetControlSettings)) as WidgetControlSettings;
            ApplyWidgetControlSettings();
        }
        else
        {
            SaveWidgetControlSettings();
        }
    }

	public void ApplyAvatarSettings()
    {
        tp_Motor_Ref.gravityOn = a_Settings.a_GravColl;
        tp_Motor_Ref.ForwardSpeed = a_Settings.a_ForwardSpeed;
        tp_Motor_Ref.BackwardSpeed = a_Settings.a_BackwardSpeed;
        tp_Motor_Ref.StrafingSpeed = a_Settings.a_StrafeSpeed;
        tp_Motor_Ref.VerticalSpeed = a_Settings.a_VerticalSpeed;
        tp_Motor_Ref.JumpSpeed = a_Settings.a_JumpSpeed;
    }

    public void SaveAvatarSettings()
    {
        XmlIO.Save(a_Settings, Application.dataPath + "/FullPackage/Settings/AvatarSettings.sets");
    }

    public void ApplyKeyBindings()
    {
        tp_InputManager_Ref.forward = kb_Settings.kb_Forward;
        tp_InputManager_Ref.backward = kb_Settings.kb_Backward;
        tp_InputManager_Ref.leftward = kb_Settings.kb_Leftward;
        tp_InputManager_Ref.rightward = kb_Settings.kb_Rightward;
        tp_InputManager_Ref.elevate = kb_Settings.kb_Elevate;
        tp_InputManager_Ref.descend = kb_Settings.kb_Descend;
        tp_InputManager_Ref.gravity = kb_Settings.kb_Gravity;
        tp_InputManager_Ref.rotateLeft = kb_Settings.kb_RotateLeft;
        tp_InputManager_Ref.rotateRight = kb_Settings.kb_RotateRight;
        tp_InputManager_Ref.roateKeySensitivity = kb_Settings.kb_RotateKeySensitivity;
        tp_InputManager_Ref.increaseSpeed = kb_Settings.kb_IncreaseSpeed;
        tp_InputManager_Ref.decreaseSpeed = kb_Settings.kb_DecreaseSpeed;
        tp_InputManager_Ref.toggleCamera = kb_Settings.kb_ToggleCamera;
    }

    public void SaveKeyBindings()
    {
        XmlIO.Save(kb_Settings, Application.dataPath + "/FullPackage/Settings/KeyBindings.sets");
    }

    public void ApplyWidgetControlSettings()
    {
        bm_GameObject.GetComponent<RectTransform>().anchoredPosition = wc_Settings.bm_DefaultPosition;
        bm_GameObject.SetActive(wc_Settings.bm_Enabled);
        sl_GameObject.GetComponent<RectTransform>().anchoredPosition = wc_Settings.sl_DefaultPosition;
        sl_GameObject.SetActive(wc_Settings.sl_Enabled);
        mm_GameObject.GetComponent<RectTransform>().anchoredPosition = wc_Settings.mm_DefaultPosition;
        mm_GameObject.SetActive(wc_Settings.mm_Enabled);

        mm_Manager_Ref.mapProportionOfScreen = wc_Settings.mm_ScreenSize;
        mm_Manager_Ref.orthoCamRadiusFeet = wc_Settings.mm_ScopeRadius;
        mm_Manager_Ref.SetMiniMapCam();
    }

    public void SaveWidgetControlSettings()
    {
        XmlIO.Save(wc_Settings, Application.dataPath + "/FullPackage/Settings/WidgetControlSettings.sets");
    }

    public void SetMiniMapFields(float screenSize, float scopeRadius)
    {
        wc_Settings.mm_ScreenSize = screenSize;
        wc_Settings.mm_ScopeRadius = scopeRadius;

    }

    void OnApplicationQuit()
    {
        SaveAvatarSettings();
        SaveKeyBindings();
        SaveWidgetControlSettings();
    }
}

public class AvatarSettings
{

    public bool a_Tracking = true;
    public bool a_GravColl = true;
    public float a_ForwardSpeed = 3;
    public float a_BackwardSpeed = 3;
    public float a_StrafeSpeed = 3;
    public float a_VerticalSpeed = 2;
    public float a_JumpSpeed = 6;

    public AvatarSettings()
    {
    }
}

public class WidgetControlSettings
{

    public bool bm_Enabled = true;
    public Vector2 bm_DefaultPosition = new Vector2(5, -5);
    public bool sl_Enabled = true;
    public Vector2 sl_DefaultPosition = new Vector2(155, -5);

    public bool mm_Enabled = true;
    public Vector2 mm_DefaultPosition = Vector2.zero;
    public float mm_ScreenSize = .2f;
    public float mm_ScopeRadius = 5f;

    public WidgetControlSettings()
    {
    }
}

public class KeyBindings
{

    public string kb_Forward = "w";
    public string kb_Backward = "s";
    public string kb_Leftward = "q";
    public string kb_Rightward = "e";
    public string kb_Elevate = "up";
    public string kb_Descend = "down";
    public string kb_Gravity = "g";
    public string kb_RotateLeft = "a";
    public string kb_RotateRight = "d";
    public float kb_RotateKeySensitivity = .8f;
    public string kb_IncreaseSpeed = "=";
    public string kb_DecreaseSpeed = "-";
    public string kb_ToggleCamera = "c";

    public KeyBindings()
    {

    }
}
