using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CharacterDropper : MonoBehaviour {

    public static CharacterDropper Instance;

    /// <summary>
    /// GENERAL VARS
    /// </summary>
    public GameObject avatar;
    public bool hasRaycastLock = false;
    public GameObject activeChar = null;

    private bool initializing = false;
    private Camera mouseCam;
    private List<DroppedCharacter> droppedCharacters = new List<DroppedCharacter>();
    private string characterFilePath;
    
    
    /// <summary>
    /// NEW CHAR DROP VARS
    /// </summary>
    public Toggle randomToggle;
    public Text modelLabel;
    public RectTransform modelList;
    public Image modelSelectMask;
    public ToggleGroup modelToggleGroup;
    public RectTransform dropCharacterSelectPanel;
    public Dropdown newCharWanderSelect;
    public Image buttonImage;
    public GameObject charRoot;

    private List<GameObject> loadedCharacters = new List<GameObject>();
    private List<Toggle> toggleList = new List<Toggle>();
    private Vector3 dropLocation = Vector3.zero;
    private int randomCharIndex = -1;
    private bool mouseDownDrop = false;

    /// <summary>
    /// CHAR INFO VARS
    /// </summary>
    public RectTransform charInfoPanel;
    public Text modelNameLabel;
    public Text wanderModeLabel;
    public Text wanderRangeLabel;
    public Dropdown destinationDropDown;
    public bool mouseDownOnChar = false;
    public GameObject mouseDownChar = null;

    /// <summary>
    /// CHAR EDIT VARS
    /// </summary>
    public RectTransform charEditPanel;
    public Projector radiusProjector;
    public InputField radiusInput;
    public ToggleGroup charEditWanderToggleGroup;
    public Toggle idleToggle;
    public Toggle patrolToggle;
    public Toggle exploreToggle;
    public Image radiusSelectMask;
    public CharacterWander.WanderMode selectedMode;

    private CharacterWander wanderToEdit;

    /// <summary>
    /// STATES
    /// </summary>
    public enum CharacterDropperState { None, SelectExisting, DroppingNew, EditCharacter, CharRadiusSelect}
    public CharacterDropperState currentState = CharacterDropperState.None;
    private CharacterDropperState prevState = CharacterDropperState.None;

    private bool userSetRadius = false;
    
    void Start()
    {
        patrolToggle.onValueChanged.AddListener(OnPatrolChanged);
        randomToggle.onValueChanged.AddListener(OnRandomChanged);
        //characterFilePath = Application.dataPath + "/FullPackage/Settings/SavedChars.characters";
        //LoadCharacters();

        if (Instance == null)
            Instance = this;

        radiusInput.onValueChanged.AddListener(SetRadiusProjectorFromInputValueChanged);

        ResetMenu();
        currentState = CharacterDropperState.SelectExisting;

        if (avatar == null)
        {
            Debug.Log("CharacterDropper.Avatar was null! Searching for tag: Player");
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                Debug.Log("Found object tagged: Player");
                avatar = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                Debug.Log("CharacterDropper.Avatar was null and no object tagged: Player");
            }
        }

        Object[] tmpArray = Resources.LoadAll("Characters/");
        foreach (Object obj in tmpArray)
        {
            loadedCharacters.Add(obj as GameObject);
            GameObject newToggle = Instantiate(Resources.Load("CustomToggle"), Vector3.zero, Quaternion.identity) as GameObject;
            newToggle.name = obj.name;
            newToggle.transform.SetParent(modelList.transform);
            newToggle.transform.SetAsLastSibling();
            newToggle.transform.FindChild("Label").GetComponent<Text>().text = obj.name;
            newToggle.GetComponent<Toggle>().group = modelToggleGroup;
            modelToggleGroup.RegisterToggle(newToggle.GetComponent<Toggle>());
        }
    }

    void Update()
    {
        // if characterdrop is the active widget, we can either drop a new character or be in charinfo/charedit/radiusselect
        // for charinfo/charedit we dont need to do anything on update
        if (ActiveWidgetManager.currentActive == ActiveWidgetManager.ActiveWidget.CharacterDrop)
        {
            // if we are dropping a new char, we need to raycast for the placement of the temp char
            // we also need to manage the random toggle and character model toggles
            if (currentState == CharacterDropperState.DroppingNew)
            {
                RaycastControl.RaycastCursor(~(1 << 9 | 1 << 8));
                dropLocation = RaycastControl.hit.point;
                activeChar.transform.position = dropLocation;

                // This if checks if the pointer is over a menu object.
                // If not, we can drop a char.
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (Input.GetMouseButtonDown(0))
                        mouseDownDrop = true;
                    if (Input.GetMouseButtonUp(0) && mouseDownDrop)
                        DropCharacter();
                }

                // If we are not dropping random chars, this statement makes sure that the temp character that is shown matches the selected model.
                if(!randomToggle.isOn && activeChar.gameObject.name.Substring(0, activeChar.gameObject.name.IndexOf("(")) != modelToggleGroup.ActiveToggles().ToArray()[0].gameObject.name)
                {
                    DestroyImmediate(activeChar);
                    activeChar = GetCharacter();
                }
                
            }
            else if (mouseDownDrop)
                mouseDownDrop = false;

            // If we are in radius select mode, we need to activate the radius projector and set its radius based on a raycast through the cursor.
            // When we left click, we stop the radius select and change states.
            if(currentState == CharacterDropperState.CharRadiusSelect)
            {
                if (!radiusProjector.gameObject.activeSelf)
                {
                    radiusProjector.gameObject.SetActive(true);
                    radiusProjector.transform.position = new Vector3(activeChar.transform.position.x, activeChar.transform.position.y + 2, activeChar.transform.position.z);
                }
                RaycastControl.RaycastCursor(~(1 << 9 | 1 << 8));
                radiusProjector.orthographicSize = (RaycastControl.hit.point - wanderToEdit.localWanderCenter).magnitude;

                if (Input.GetMouseButtonDown(0))
                    StopCharRadiusSelect(false);
            }
            else if (radiusProjector.gameObject.activeSelf)
                    radiusProjector.gameObject.SetActive(false);
        }
        
    }
    
    /// <summary>
    /// This function resets the menu whenever the WidgetRoot, where this script is attached, is disabled.
    /// </summary>
    void OnDisable()
    {
        ResetMenu();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        characterFilePath = Application.dataPath + "/FullPackage/Settings/SavedChars.characters";
        LoadCharacters();
        ResetMenu();
    }

    /// <summary>
    /// This function is called whenever the Random toggle is changed when dropping new characters.
    /// </summary>
    /// <param name="newValue"></param>
    public void OnRandomChanged(bool newValue)
    {
        // If the value is turned on, we enable the mask to hide the model options.
        if(newValue)
        {
            modelSelectMask.gameObject.SetActive(true);
        }
        // If the value is turned off, we disable the mask and turn on the correct model toggle.
        else
        {
            modelSelectMask.gameObject.SetActive(false);
            Toggle activeCharToggle = (from tog in modelToggleGroup.gameObject.GetComponentsInChildren<Toggle>() where tog.name.Equals(activeChar.gameObject.name.Substring(0, activeChar.gameObject.name.IndexOf("("))) select tog).ToArray()[0];
            activeCharToggle.isOn = true;
            modelToggleGroup.NotifyToggleOn(activeCharToggle);
        }
    }

    /// <summary>
    /// This function is called whenever the Patrol toggle changes in the CharacterEditPanel.
    /// </summary>
    /// <param name="newValue"></param>
    public void OnPatrolChanged(bool newValue)
    {
        // If the value is turned on, we disable the mask hiding the radius select section.
        if(newValue)
        {
            radiusSelectMask.gameObject.SetActive(false);
        }
        // If the value is turned off, we enable the mask and change the state.
        else
        {
            radiusSelectMask.gameObject.SetActive(true);
            if (currentState == CharacterDropperState.CharRadiusSelect)
                currentState = CharacterDropperState.EditCharacter;
        }
    }

    /// <summary>
    /// Closes and resets the menu.
    /// </summary>
    public void ResetMenu()
    {
        initializing = true;
        Destroy(activeChar);
        StopCharRadiusSelect(false);
        CloseCharacterDrop();
        CloseCharacterInfo();
        CloseCharacterEdit();
        initializing = false;
        currentState = CharacterDropperState.None;
    }

    /// <summary>
    /// Switches between the menu being open and closed.
    /// </summary>
    public void ToggleMenu()
    {
        if(currentState == CharacterDropperState.DroppingNew)
        {
            CloseCharacterDrop();
        }
        else
        {
            OpenCharacterDrop();
        }
    }

    /// <summary>
    /// Updates the radius projector's radius value whenver the input field changes.
    /// </summary>
    /// <param name="value"></param>
    public void SetRadiusProjectorFromInputValueChanged(string value)
    {
        if (radiusInput.isFocused)
        {
            userSetRadius = true;
            if (value == null || value == "")
            {
                radiusInput.text = 0.ToString();
            }
            else
                radiusInput.text = value.ToString();
        }

        radiusProjector.orthographicSize = float.Parse(radiusInput.text);
    }

    /// <summary>
    /// Sets the wander radius from the input field.
    /// </summary>
    public void SetWanderRadiusFromInput()
    {
        userSetRadius = true;
        radiusProjector.orthographicSize = float.Parse(radiusInput.text);
    }

    /// <summary>
    /// Drops the active character at the selected location and updates its CharacterWander script.
    /// </summary>
    public void DropCharacter()
    {
        mouseDownDrop = false;
        activeChar.GetComponent<CapsuleCollider>().enabled = true;
        activeChar.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        activeChar.GetComponent<CharacterWander>().enabled = true;
        activeChar.GetComponent<CharacterWander>().mode = (CharacterWander.WanderMode)newCharWanderSelect.value;
        activeChar.GetComponent<CharacterWander>().dropPoint = activeChar.transform.position;
        activeChar.GetComponent<CharacterWander>().poiDestination = -1;

        activeChar.transform.parent = charRoot.transform;
        ;

        if ((CharacterWander.WanderMode)newCharWanderSelect.value == CharacterWander.WanderMode.Patrol)
        {
            wanderToEdit = activeChar.GetComponent<CharacterWander>();
            StartCharRadiusSelect();
        }
        else
        {
            activeChar = GetCharacter();
        }

        droppedCharacters.Add(new DroppedCharacter(activeChar.GetComponent<CharacterWander>()));
        SaveCharacters();
        
    }

    /// <summary>
    /// Opens the character drop menu.
    /// </summary>
    public void OpenCharacterDrop()
    {
        if (ActiveWidgetManager.currentActive == ActiveWidgetManager.ActiveWidget.WidgetConfig)
        {
            dropCharacterSelectPanel.gameObject.SetActive(true);
            modelToggleGroup.SetAllTogglesOff();
            randomToggle.isOn = true;
            activeChar = GetCharacter();
            buttonImage.color = Color.red;
            currentState = CharacterDropperState.DroppingNew;
            ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.WidgetConfig);
            ActiveWidgetManager.ActivateWidget(ActiveWidgetManager.ActiveWidget.CharacterDrop);
        }
    }

    /// <summary>
    /// Closes the character drop menu.
    /// </summary>
    public void CloseCharacterDrop()
    {
        if (ActiveWidgetManager.currentActive == ActiveWidgetManager.ActiveWidget.CharacterDrop)
        {
            dropCharacterSelectPanel.gameObject.SetActive(false);
            Destroy(activeChar);
            activeChar = null;
            buttonImage.color = Color.white;
            currentState = CharacterDropperState.SelectExisting;
            ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.CharacterDrop);
            ActiveWidgetManager.ActivateWidget(ActiveWidgetManager.ActiveWidget.WidgetConfig);
        }
    }
    
    /// <summary>
    /// Gets a character to use for the active character.
    /// Makes sure the correct character is chosen based on the active toggles.
    /// </summary>
    /// <returns></returns>
    private GameObject GetCharacter()
    {
        GameObject returnChar = null;
        if (randomToggle.isOn)
            returnChar = CreateRandomChar();
        else
        {
            toggleList = modelToggleGroup.ActiveToggles().ToList<Toggle>();
            foreach (GameObject character in loadedCharacters)
            {
                if (character.name == toggleList[0].name)
                {
                    modelLabel.text = "Model: " + character.name;
                    returnChar = Instantiate(character, dropLocation, Quaternion.identity) as GameObject;
                }
            }
        }

        returnChar.GetComponent<CapsuleCollider>().enabled = false;
        returnChar.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        returnChar.GetComponent<CharacterWander>().enabled = false;
        return returnChar;
    }

    /// <summary>
    /// Creates a "random" character from the list of loaded characters.
    /// Increments through the list sequentially.
    /// </summary>
    /// <returns></returns>
    private GameObject CreateRandomChar()
    {
        randomCharIndex++;
        if (randomCharIndex >= loadedCharacters.Count)
            randomCharIndex = 0;
        modelLabel.text = "Model: " + loadedCharacters[randomCharIndex].name;
        return Instantiate(loadedCharacters[randomCharIndex], dropLocation, Quaternion.identity) as GameObject;
    }
    
    /// <summary>
    /// Deletes the active character.
    /// </summary>
    public void DeleteCharacter()
    {
        droppedCharacters.Remove(new DroppedCharacter(activeChar.GetComponent<CharacterWander>()));
        SaveCharacters();
        Destroy(activeChar);
        activeChar = null;
        CloseCharacterInfo();
    }

    /// <summary>
    /// Starts the radius selection process.
    /// Changes the state to CharRadiusSelect.
    /// </summary>
    public void StartCharRadiusSelect()
    {
        prevState = currentState;
        currentState = CharacterDropperState.CharRadiusSelect;

        userSetRadius = false;
        
        if(wanderToEdit == null)
            wanderToEdit = activeChar.GetComponent<CharacterWander>();

        if (activeChar != null)
        {
            wanderToEdit.localWanderCenter = activeChar.transform.position;
            radiusProjector.transform.position = activeChar.transform.position + new Vector3(0, 2, 0);
        }
    }

    /// <summary>
    /// Stops the radius selection process.
    /// </summary>
    /// <param name="startMotion"></param>
    public void StopCharRadiusSelect(bool startMotion)
    {
        if (wanderToEdit != null)
        {
            wanderToEdit.localWanderRadius = radiusProjector.orthographicSize;

            if (startMotion)
                wanderToEdit.SetWanderMode();

            radiusInput.text = wanderToEdit.localWanderRadius.ToString("F2");
        }
        currentState = prevState;


        if (currentState == CharacterDropperState.DroppingNew)
            activeChar = GetCharacter();
    }

    /// <summary>
    /// Opens the charater edit panel.
    /// Initializes elements in the panel.
    /// Triggers transitions for the panel.
    /// </summary>
    public void OpenCharacterEdit()
    {
        userSetRadius = false;
        charEditPanel.gameObject.SetActive(true);
        charEditWanderToggleGroup.SetAllTogglesOff();
        radiusInput.text = wanderToEdit.localWanderRadius.ToString();
        wanderToEdit.localWanderCenter = activeChar.transform.position;
        Toggle toggleToActivate;
        if(wanderToEdit.mode == CharacterWander.WanderMode.Bookmark)
            toggleToActivate = charEditWanderToggleGroup.transform.GetChild((int)wanderToEdit.prevMode).GetComponent<Toggle>();
        else
            toggleToActivate = charEditWanderToggleGroup.transform.GetChild((int)wanderToEdit.mode).GetComponent<Toggle>();
        toggleToActivate.isOn = true;
        charEditWanderToggleGroup.NotifyToggleOn(toggleToActivate);

        // move info panel off screen
        iTween.MoveBy(charInfoPanel.gameObject, iTween.Hash("y", Screen.height, "easeType", "easeInOutExpo", "time", .5f));

        // grow edit panel
        charEditPanel.transform.position = UIUtilities.SetPopUpPanel(charEditPanel);
        charEditPanel.localScale = new Vector3(.01f, .01f, .01f);
        iTween.ScaleBy(charEditPanel.gameObject, iTween.Hash("x", 100, "y", 100, "easeType", "easeInOutExpo", "time", .5f));
    }

    /// <summary>
    /// Closes the character edit panel.
    /// Triggers transitions.
    /// Sets the CharacterWander script values.
    /// </summary>
    public void CloseCharacterEdit()
    {
        if (wanderToEdit != null)
        {
            if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Idle")
            {
                wanderToEdit.dropPoint = activeChar.transform.position;
                wanderToEdit.localWanderCenter = activeChar.transform.position;
                selectedMode = (CharacterWander.WanderMode)0;
            }
            else if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Patrol")
            {
                if (float.Parse(radiusInput.text) > .5f)
                {
                    selectedMode = (CharacterWander.WanderMode)1;
                    wanderToEdit.localWanderCenter = activeChar.transform.position;
                    wanderToEdit.localWanderRadius = float.Parse(radiusInput.text);
                }
                else
                    selectedMode = (CharacterWander.WanderMode)0;
            }
            else if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Explore")
                selectedMode = (CharacterWander.WanderMode)2;
        }

        iTween.MoveBy(charInfoPanel.gameObject, iTween.Hash("y", -Screen.height, "easeType", "easeInOutExpo", "time", .5f));


        charEditPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the character info panel.
    /// </summary>
    /// <param name="charToOpen"></param>
    public void OpenCharacterInfo(GameObject charToOpen)
    {
        Destroy(activeChar);
        activeChar = charToOpen;

        mouseDownOnChar = false;
        wanderToEdit = activeChar.GetComponent<CharacterWander>();
        activeChar.GetComponent<UnityEngine.AI.NavMeshAgent>().Stop();
        wanderToEdit.CancelMovement();
        activeChar.GetComponent<Animator>().enabled = false;
        charInfoPanel.gameObject.SetActive(true);

        if (wanderToEdit.mode == CharacterWander.WanderMode.Bookmark)
            selectedMode = wanderToEdit.prevMode;
        else
            selectedMode = wanderToEdit.mode;
       
        charInfoPanel.transform.position = UIUtilities.SetPopUpPanel(charInfoPanel);

        iTween.Stop(charInfoPanel.gameObject, true);
        charInfoPanel.localScale = new Vector3(.01f, .01f, .01f);
        iTween.ScaleBy(charInfoPanel.gameObject, iTween.Hash("x", 100, "y", 100, "easeType", "easeInOutExpo", "time", .5f));

        UpdateCharInfoLabels();

        destinationDropDown.ClearOptions();
        destinationDropDown.AddOptions(new List<string>() { "None" });

        destinationDropDown.AddOptions(new List<string>( POIButtonManager.originalHandler.projectPOIs.Select(e => e.buttonName).ToList()));

        ActiveWidgetManager.ActivateWidget(ActiveWidgetManager.ActiveWidget.CharacterDrop);

        currentState = CharacterDropperState.EditCharacter;
    }

    /// <summary>
    /// Closes the character info panel.
    /// </summary>
    public void CloseCharacterInfo()
    {
        if (activeChar != null)
            activeChar.GetComponent<Animator>().enabled = true;
        

        ApplyOptions();

        if (wanderToEdit != null)
        {
            wanderToEdit.poiDestination = destinationDropDown.value - 1;

            if (destinationDropDown.value == 0)
                wanderToEdit.SetWanderMode();
            else
            {
                wanderToEdit.SetBookmarkMode();
            }
        }

        SaveCharacters();
        activeChar = null;
        wanderToEdit = null;

        iTween.Stop(charInfoPanel.gameObject);
        charInfoPanel.localScale = new Vector3(1,1,1);
        charInfoPanel.gameObject.SetActive(false);
        ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.CharacterDrop);

        currentState = CharacterDropperState.SelectExisting;
    }

    /// <summary>
    /// Updates the fields in the character info panel from the CharacterWander script.
    /// </summary>
    public void UpdateCharInfoLabels()
    {
        if(activeChar != null)
            modelNameLabel.text = activeChar.gameObject.name.Remove(activeChar.gameObject.name.IndexOf("("));

        if (wanderToEdit != null)
        {
            if (wanderToEdit.mode == CharacterWander.WanderMode.Bookmark)
                wanderModeLabel.text = wanderToEdit.prevMode.ToString();
            else
                wanderModeLabel.text = wanderToEdit.mode.ToString();

            if ((int)wanderToEdit.mode == 0)
            {
                radiusProjector.orthographicSize = 0;
                wanderRangeLabel.text = "0";
            }
            else if ((int)wanderToEdit.mode == 1)
            {
                radiusProjector.orthographicSize = wanderToEdit.localWanderRadius;
                wanderRangeLabel.text = wanderToEdit.localWanderRadius.ToString("F2");
            }
            else if ((int)wanderToEdit.mode == 2)
            {
                radiusProjector.orthographicSize = 0;
                wanderRangeLabel.text = "Infinite";
            }

            destinationDropDown.value = wanderToEdit.poiDestination + 1;
        }
    }

    /// <summary>
    /// Sets the wander mode on the activeChar.
    /// </summary>
    public void ApplyOptions()
    {
        if (activeChar != null)
        {
            if (wanderToEdit.mode == CharacterWander.WanderMode.Bookmark)
                wanderToEdit.prevMode = selectedMode;
            else
                wanderToEdit.mode = selectedMode;

            wanderToEdit.SetWanderMode();
        }
    }

    /// <summary>
    /// Loads characters from file.
    /// </summary>
    public void LoadCharacters()
    {
        if (!File.Exists(characterFilePath))
            XmlIO.Save(droppedCharacters, characterFilePath);
        droppedCharacters = XmlIO.Load(characterFilePath, typeof(List<DroppedCharacter>)) as List<DroppedCharacter>;

        foreach (DroppedCharacter character in droppedCharacters)
        {
            GameObject newChar = GameObject.Instantiate(Resources.Load("Characters/" + character.modelName), character.dropPoint, Quaternion.identity) as GameObject;
            newChar.transform.parent = charRoot.transform;
            newChar.transform.localPosition = character.dropPoint;
            newChar.transform.localScale = Vector3.one;
            CharacterWander newWander = newChar.GetComponent<CharacterWander>();
            newWander.dropPoint = character.dropPoint;
            newWander.localWanderCenter = character.localWanderCenter;
            newWander.localWanderRadius = character.localWanderRadius;
            newWander.mode = character.mode;
            newWander.normalSpeedRadius = character.normalSpeedRadius;
            newWander.defaultSpeed = character.defaultSpeed;
            newWander.poiDestination = -1;
        }
    }

    /// <summary>
    /// Saves character to file.
    /// </summary>
    public void SaveCharacters()
    {
        if (!initializing)
        {
            droppedCharacters = new List<DroppedCharacter>();
            Debug.Log("saving " + charRoot.transform.childCount + " characters");
            for (int i = 0; i < charRoot.transform.childCount; i++)
            {
                droppedCharacters.Add(new DroppedCharacter(charRoot.transform.GetChild(i).GetComponent<CharacterWander>()));
            }
            XmlIO.Save(droppedCharacters, characterFilePath);
        }
    }

    /// <summary>
    /// Deletes all characters in the scene.
    /// </summary>
    public void DeleteCharacters()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in charRoot.transform) children.Add(child.gameObject);
        children.ForEach(child =>
        {
            droppedCharacters.Remove(new DroppedCharacter(child.GetComponent<CharacterWander>()));
            DestroyImmediate(child);
        }
        );
        SaveCharacters();
    }
}

public class DroppedCharacter
{
    public string modelName;
    public Vector3 dropPoint;
    public Vector3 localWanderCenter;
    public float localWanderRadius;
    public CharacterWander.WanderMode mode;
    public float defaultSpeed;
    public float normalSpeedRadius;

    public DroppedCharacter()
    {

    }

    public DroppedCharacter(CharacterWander wanderScript)
    {
        modelName = wanderScript.gameObject.name.Substring(0, wanderScript.gameObject.name.IndexOf("("));
        localWanderCenter = wanderScript.localWanderCenter;
        localWanderRadius = wanderScript.localWanderRadius;
        mode = wanderScript.mode;
        defaultSpeed = wanderScript.defaultSpeed;
        normalSpeedRadius = wanderScript.normalSpeedRadius;
        dropPoint = wanderScript.dropPoint;
    }

}