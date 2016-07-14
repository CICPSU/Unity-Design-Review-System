using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsMenusRefs : MonoBehaviour {

    public static SettingsMenusRefs Instance;
    
    // References for the KeybindingsMenu
    public InputField forwardInput;
    public InputField backwardInput;
    public InputField leftInput;
    public InputField rightInput;
    public InputField rotateLeftInput;
    public InputField rotateRightInput;
    public InputField rotateSensitivityInput;
    public InputField elevateInput;
    public InputField descendInput;
    public InputField gravityInput;
    public InputField increaseSpeedInput;
    public InputField decreaseSpeedInput;
    public InputField toggleCameraInput;

    // References for the WidgetControlMenu
    public Toggle minimapToggle;
    public Toggle bookmarkToggle;
    public Toggle sunlightToggle;

    // References for the AvatarSettingsMenu
    public Toggle trackingToggle;
    public Toggle gravityToggle;
    public InputField forwardSpeedInput;
    public InputField backwardSpeedInput;
    public InputField strafeSpeedInput;
    public InputField jumpSpeedInput;

    // References for the SettingsSelectMenu
    public RectTransform SettingsContentPanel;
    public RectTransform SettingsSelectMenu;
    public RectTransform SettingsHeader;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }
}
