using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CharacterDropper : MonoBehaviour {

    /// <summary>
    /// GENERAL VARS
    /// </summary>
    public GameObject avatar;
    private Camera mouseCam;
    public bool hasRaycastLock = false;
    private bool initializing = false;

    private List<DroppedCharacter> droppedCharacters = new List<DroppedCharacter>();
    private string characterFilePath;
    
    
    /// <summary>
    /// NEW CHAR DROP VARS
    /// </summary>
    public Toggle randomToggle;
    public Text modelLabel;
    public RectTransform modelList;
    public ToggleGroup modelToggleGroup;
    public RectTransform dropCharacterSelectPanel;
    public Dropdown newCharWanderSelect;
    public Image buttonImage;
    public GameObject charRoot;
    private List<GameObject> loadedCharacters = new List<GameObject>();
    private List<Toggle> toggleList = new List<Toggle>();
    private GameObject charToDrop;
    private bool modelOptionsGreyed;
    private Vector3 dropLocation = Vector3.zero;
    private int randomCharIndex = -1;

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
    public Image radiusSelectMask;
    public CharacterWander.WanderMode selectedMode;

    private GameObject charToEdit;
    private CharacterWander wanderToEdit;

    /// <summary>
    /// STATES
    /// </summary>
    private bool newCharDrop = false;
    private bool charInfoOpen = false;
    private bool charEditOpen = false;
    private bool charRadiusSelect = false;
    private bool userSetRadius = false;
    private bool firstFrameOpen = false;
    private bool firstFrameRadiusSelect = false;


    void Start()
    {
        //characterFilePath = Application.dataPath + "/FullPackage/Settings/SavedChars.characters";
        //LoadCharacters();

        radiusInput.onValueChanged.AddListener(SetRadiusProjectorFromInputValueChanged);
        ResetMenu();
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

        if (firstFrameRadiusSelect && Input.GetMouseButtonDown(0))
            firstFrameRadiusSelect = false;

        if (firstFrameOpen && Input.GetMouseButtonDown(0))
            firstFrameOpen = false;

        // this finds the camera whose viewport contains the mouse cursor
        mouseCam = FindMouseCamera();

        // if the char edit group is set to patrol, disable the mask
        if ( charEditOpen  && charEditWanderToggleGroup.ActiveToggles().ToArray().Count() != 0 
            && charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Patrol")
        {
            radiusProjector.transform.position = wanderToEdit.localWanderCenter + new Vector3(0, 2, 0);
            if (!userSetRadius)
                radiusProjector.orthographicSize = wanderToEdit.localWanderRadius;

            radiusSelectMask.enabled = false;
        }
        else if (charEditOpen)
        {

            radiusProjector.orthographicSize = 0;

            radiusSelectMask.enabled = true;
        }

        //manage raycast lock
        if (charRadiusSelect || newCharDrop || charInfoOpen)
        {
            if (!hasRaycastLock && RaycastLock.GetLock())
                hasRaycastLock = true;
        }
        else if (hasRaycastLock)
        {
            RaycastLock.GiveLock();
            hasRaycastLock = false;
        }


        if (charRadiusSelect || charInfoOpen)
        {
            radiusProjector.gameObject.SetActive(true);
            radiusProjector.transform.position = wanderToEdit.localWanderCenter + new Vector3(0, 2, 0);
        }
        else
        {

            radiusProjector.gameObject.SetActive(false);
        }
        //radius select mode
        if (charRadiusSelect)
        {

            if (!userSetRadius)
            {
                //set size of the projector to the distance between the character being editted and the raycast hit at the mouse location
                if (mouseCam != null && hasRaycastLock)
                {
                    RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 9 | 1 << 8));
                    radiusProjector.orthographicSize = (charToEdit.transform.position - RaycastLock.hit.point).magnitude;
                }

                else
                    radiusProjector.orthographicSize = wanderToEdit.localWanderRadius;
            }

            radiusInput.text = radiusProjector.orthographicSize.ToString("F2");

            if (Input.GetMouseButtonUp(0) && !firstFrameRadiusSelect)
            {
                StopCharRadiusSelect(!charEditOpen);
            }

        }
        // if we are dropping a new character
        else if (newCharDrop)
        {
            #region dropping new char
            #region toggles
            if (randomToggle.isOn)
            {
                if (modelToggleGroup.AnyTogglesOn())
                {
                    modelToggleGroup.SetAllTogglesOff();
                }

                if (!modelOptionsGreyed)
                {
                    foreach (Toggle tog in modelToggleGroup.GetComponentsInChildren<Toggle>())
                    {
                        tog.GetComponentInChildren<Text>().color = Color.grey;
                    }
                    modelOptionsGreyed = true;
                }
            }
            else
            {
                if (modelOptionsGreyed)
                {
                    foreach (Toggle tog in modelToggleGroup.GetComponentsInChildren<Toggle>())
                    {
                        tog.GetComponentInChildren<Text>().color = Color.black;
                    }
                    modelOptionsGreyed = false;
                }
            }

            // this if is to make sure that a toggle in the toggle group is on if the random toggle is off
            if (!modelToggleGroup.AnyTogglesOn() && !randomToggle.isOn)
            {
                modelToggleGroup.GetComponentInChildren<Toggle>().isOn = true;
                modelToggleGroup.NotifyToggleOn(modelToggleGroup.GetComponentInChildren<Toggle>());
            }
            #endregion

            //raycast that ignores characters in the scene
            if (mouseCam != null && hasRaycastLock)
                RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 9 | 1 << 8));

            #region makes sure the displayed char is correct
            if (charToDrop == null)
                charToDrop = GetCharacter();

            if (modelToggleGroup.ActiveToggles().Count() > 0
                && charToDrop.name != modelToggleGroup.ActiveToggles().ToList()[0].name + "(Clone)"
                && !randomToggle.isOn)
            {
                Destroy(charToDrop);
                charToDrop = GetCharacter();
            }
            #endregion

            //sets the position of the temp avatar
            if (mouseCam != null)
            {
                if (RaycastLock.hit.point != null)
                    dropLocation = RaycastLock.hit.point;
                else
                    dropLocation = avatar.transform.position + avatar.transform.forward * 2f;

                //Debug.Log("drop location: " + dropLocation + " haslock: " + hasRaycastLock);
            }

            if (charToDrop != null)
                charToDrop.transform.position = dropLocation;

            //if we left click at a valid location, drop the character
            if (Input.GetMouseButtonUp(0) && RaycastLock.hit.transform != null && !firstFrameOpen && !EventSystem.current.IsPointerOverGameObject())
            {
                DropCharacter();
                Debug.Log("dropping character");
            }

            #endregion
        }
        else // this is when we are not dropping a new character
        {
            if (mouseCam != null && Input.GetMouseButtonDown(0))
            {
                if (!hasRaycastLock && RaycastLock.GetLock())
                    hasRaycastLock = true;

                if (hasRaycastLock)
                {
                    RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 9));
                    //if we are pointing at an existing avatar and left click, open char info
                    if (RaycastLock.hit.transform != null && RaycastLock.hit.transform.GetComponent<CharacterWander>() != null && !charInfoOpen)
                    {
                        mouseDownOnChar = true;
                        mouseDownChar = RaycastLock.hit.transform.gameObject;
                        RaycastLock.GiveLock();
                        hasRaycastLock = false;
                    }
                    else
                    {
                        RaycastLock.GiveLock();
                        hasRaycastLock = false;
                        mouseDownOnChar = false;
                        mouseDownChar = null;
                    }
                }
            }
            //raycast that ignores the user avatar
            if (mouseCam != null && Input.GetMouseButtonUp(0) && mouseDownOnChar)
            {
                if (!hasRaycastLock && RaycastLock.GetLock())
                {
                    hasRaycastLock = true;
                }

                if (hasRaycastLock)
                {
                    RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 9));
                    //if we are pointing at an existing avatar and left click, open char info
                    if (RaycastLock.hit.transform != null && RaycastLock.hit.transform.GetComponent<CharacterWander>() != null && !charInfoOpen && mouseDownChar == RaycastLock.hit.transform.gameObject)
                        OpenCharacterInfo();
                    else
                    {
                        RaycastLock.GiveLock();
                        hasRaycastLock = false;
                        mouseDownOnChar = false;
                        mouseDownChar = null;
                    }
                }
            }
            
        }
    }


    void OnDisable()
    {
        ResetMenu();
    }

    void OnEnable()
    {
        characterFilePath = Application.dataPath + "/FullPackage/Settings/SavedChars.characters";
        LoadCharacters();
        ResetMenu();
    }

    public void ResetMenu()
    {
        initializing = true;
        Destroy(charToDrop);
        newCharDrop = false;
        charInfoOpen = false;
        charEditOpen = false;
        charRadiusSelect = false;
        modelOptionsGreyed = false;
        StopCharRadiusSelect(false);
        CloseCharacterDrop();
        CloseCharacterInfo();
        CloseCharacterEdit();
        initializing = false;
    }

    public void ToggleMenu()
    {
        newCharDrop = !newCharDrop;
        if (newCharDrop)
        {
            OpenCharacterDrop();
        }
        else
        {
            CloseCharacterDrop();
        }
    }

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

    public void SetWanderRadiusFromInput()
    {
        userSetRadius = true;
        radiusProjector.orthographicSize = float.Parse(radiusInput.text);
    }

    public void DropCharacter()
    {
        charToDrop.GetComponent<CapsuleCollider>().enabled = true;
        charToDrop.GetComponent<NavMeshAgent>().enabled = true;
        charToDrop.GetComponent<CharacterWander>().enabled = true;
        charToDrop.GetComponent<CharacterWander>().mode = (CharacterWander.WanderMode)newCharWanderSelect.value;
        charToDrop.GetComponent<CharacterWander>().dropPoint = charToDrop.transform.position;
        charToDrop.GetComponent<CharacterWander>().poiDestination = -1;
       
        charToDrop.transform.parent = charRoot.transform;

        if ((CharacterWander.WanderMode)newCharWanderSelect.value == CharacterWander.WanderMode.Patrol)
        {
            charToEdit = charToDrop;
            wanderToEdit = charToEdit.GetComponent<CharacterWander>();
            StartCharRadiusSelect();
        }

        droppedCharacters.Add(new DroppedCharacter(charToDrop.GetComponent<CharacterWander>()));
        SaveCharacters();
        charToDrop = null;
    }

    public void OpenCharacterDrop()
    {
        if (!hasRaycastLock && RaycastLock.GetLock())
            hasRaycastLock = true;

        dropCharacterSelectPanel.gameObject.SetActive(true);
        modelToggleGroup.SetAllTogglesOff();
        randomToggle.isOn = true;
        charToDrop = GetCharacter();
        buttonImage.color = Color.red;
        firstFrameOpen = true;

        CloseCharacterInfo();
    }

    public void CloseCharacterDrop()
    {

        if (hasRaycastLock)
        {
            hasRaycastLock = false;
            RaycastLock.GiveLock();
        }
        dropCharacterSelectPanel.gameObject.SetActive(false);
        Destroy(charToDrop);
        charToDrop = null;
        buttonImage.color = Color.white;
        CloseCharacterInfo();
        CloseCharacterEdit();
    }


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
        returnChar.GetComponent<NavMeshAgent>().enabled = false;
        returnChar.GetComponent<CharacterWander>().enabled = false;
        return returnChar;
    }

    private GameObject CreateRandomChar()
    {
        /*
        int randomIndex = (int)Random.Range(0, loadedCharacters.Count - 1);
        modelLabel.text = "Model: " + loadedCharacters[randomIndex].name;
        */
        randomCharIndex++;
        if (randomCharIndex >= loadedCharacters.Count)
            randomCharIndex = 0;
        modelLabel.text = "Model: " + loadedCharacters[randomCharIndex].name;
        return Instantiate(loadedCharacters[randomCharIndex], dropLocation, Quaternion.identity) as GameObject;
    }


    public void DeleteCharacter()
    {
        droppedCharacters.Remove(new DroppedCharacter(charToEdit.GetComponent<CharacterWander>()));
        SaveCharacters();
        Destroy(charToEdit);
        charToEdit = null;
        CloseCharacterInfo();
    }

    private Camera FindMouseCamera()
    {
        List<Camera> camList = (from cam in GameObject.FindObjectsOfType<Camera>() where cam.targetTexture == null select cam).ToList();
        foreach (Camera cam in camList)
        {
            if (Input.mousePosition.x > cam.pixelRect.xMin && Input.mousePosition.x < cam.pixelRect.xMax
                && Input.mousePosition.y > cam.pixelRect.yMin && Input.mousePosition.y < cam.pixelRect.yMax)
            {
                return cam;
            }
        }
        return null;
    }

    public void StartCharRadiusSelect()
    {
        userSetRadius = false;
        if (charToEdit == null && charToDrop != null)
        {
            charToEdit = charToDrop;
           
            charToDrop = null;
        }
        if(wanderToEdit == null)
            wanderToEdit = charToEdit.GetComponent<CharacterWander>();

        if (charToEdit != null)
            wanderToEdit.localWanderCenter = charToEdit.transform.position;

        charRadiusSelect = true;
        firstFrameRadiusSelect = true;
        
    }

    public void StopCharRadiusSelect(bool startMotion)
    {
        if (wanderToEdit != null)
        {
            wanderToEdit.localWanderRadius = radiusProjector.orthographicSize;
            if(startMotion)
                wanderToEdit.SetWanderMode();
        }
        

        //charToEdit = null;
        //wanderToEdit = null;
        charRadiusSelect = false;
        
    }

    public void OpenCharacterEdit()
    {
        userSetRadius = false;
        charEditOpen = true;
        charEditPanel.gameObject.SetActive(true);
        charEditWanderToggleGroup.SetAllTogglesOff();
        radiusInput.text = wanderToEdit.localWanderRadius.ToString();
        wanderToEdit.localWanderCenter = charToEdit.transform.position;
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
        //charEditPanel.localPosition = Input.mousePosition;
        charEditPanel.transform.position = UIUtilities.SetPopUpPanel(charEditPanel);
        charEditPanel.localScale = new Vector3(.01f, .01f, .01f);
        iTween.ScaleBy(charEditPanel.gameObject, iTween.Hash("x", 100, "y", 100, "easeType", "easeInOutExpo", "time", .5f));
    }

    public void CloseCharacterEdit()
    {
        if (wanderToEdit != null && charEditOpen)
        {
            if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Idle")
                selectedMode = (CharacterWander.WanderMode)0;
            else if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Patrol")
            {
                if (float.Parse(radiusInput.text) > .5f)
                {
                    selectedMode = (CharacterWander.WanderMode)1;
                    wanderToEdit.localWanderCenter = charToEdit.transform.position;
                    wanderToEdit.localWanderRadius = float.Parse(radiusInput.text);
                }
                else
                    selectedMode = (CharacterWander.WanderMode)0;
            }
            else if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Explore")
                selectedMode = (CharacterWander.WanderMode)2;
        }

        iTween.MoveBy(charInfoPanel.gameObject, iTween.Hash("y", -Screen.height, "easeType", "easeInOutExpo", "time", .5f));


        charEditOpen = false;
        charEditPanel.gameObject.SetActive(false);
    }

    public void OpenCharacterInfo()
    {
        Destroy(charToDrop);
        charInfoOpen = true;
        charRadiusSelect = false;
        mouseDownOnChar = false;
        EditModeManager.EnterEditMode(charInfoPanel);
        charToEdit = RaycastLock.hit.transform.gameObject;
        wanderToEdit = charToEdit.GetComponent<CharacterWander>();
        charToEdit.GetComponent<NavMeshAgent>().Stop();
        wanderToEdit.CancelMovement();
        charToEdit.GetComponent<Animator>().enabled = false;
        charInfoPanel.gameObject.SetActive(true);

        if (wanderToEdit.mode == CharacterWander.WanderMode.Bookmark)
            selectedMode = wanderToEdit.prevMode;
        else
            selectedMode = wanderToEdit.mode;
        /*
        if (Input.mousePosition.x > Screen.width - charInfoPanel.sizeDelta.x)
        {
            if (Input.mousePosition.y > Screen.height - charInfoPanel.sizeDelta.y)
            {
                charInfoPanel.transform.position = new Vector3(Screen.width - charInfoPanel.sizeDelta.x, Screen.height - charInfoPanel.sizeDelta.y, 0);
            }
            else if (Input.mousePosition.y < charEditPanel.sizeDelta.y)
            {
                charInfoPanel.transform.position = new Vector3(Screen.width - charInfoPanel.sizeDelta.x, charEditPanel.sizeDelta.y, 0);
            }
            else
                charInfoPanel.transform.position = new Vector3(Screen.width - charInfoPanel.sizeDelta.x, Input.mousePosition.y, 0);
        }
        else if (Input.mousePosition.y > Screen.height - charInfoPanel.sizeDelta.y)
            charInfoPanel.transform.position = new Vector3(Input.mousePosition.x, Screen.height - charInfoPanel.sizeDelta.y, 0);
        else if (Input.mousePosition.y < charEditPanel.sizeDelta.y)
            charInfoPanel.transform.position = new Vector3(Input.mousePosition.x, charEditPanel.sizeDelta.y, 0);
        else
            charInfoPanel.transform.position = Input.mousePosition;
            */
        charInfoPanel.transform.position = UIUtilities.SetPopUpPanel(charInfoPanel);

        iTween.Stop(charInfoPanel.gameObject, true);
        charInfoPanel.localScale = new Vector3(.01f, .01f, .01f);
        iTween.ScaleBy(charInfoPanel.gameObject, iTween.Hash("x", 100, "y", 100, "easeType", "easeInOutExpo", "time", .5f));

        UpdateCharInfoLabels();

        destinationDropDown.ClearOptions();
        destinationDropDown.AddOptions(new List<string>() { "None" });

        destinationDropDown.AddOptions(new List<string>( POIButtonManager.originalHandler.projectPOIs.Select(e => e.buttonName).ToList()));
    }

    public void CloseCharacterInfo()
    {
        if (hasRaycastLock)
        {
            RaycastLock.GiveLock();
            hasRaycastLock = false;
        }
        if(charEditOpen)
            CloseCharacterEdit();

        if (charToEdit != null)
            charToEdit.GetComponent<Animator>().enabled = true;
        

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
        charToEdit = null;
        wanderToEdit = null;

        if(charInfoOpen)
            EditModeManager.ExitEditMode();

        charInfoOpen = false;
        charInfoPanel.gameObject.SetActive(false);
    }

    public void UpdateCharInfoLabels()
    {
        if(charToEdit != null)
            modelNameLabel.text = charToEdit.gameObject.name.Remove(charToEdit.gameObject.name.IndexOf("("));

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

    public void ApplyOptions()
    {
        if (charToEdit != null)
        {
            if (wanderToEdit.mode == CharacterWander.WanderMode.Bookmark)
                wanderToEdit.prevMode = selectedMode;
            else
                wanderToEdit.mode = selectedMode;

            wanderToEdit.SetWanderMode();
        }
    }

    public void LoadCharacters()
    {
        if (!File.Exists(characterFilePath))
            XmlIO.Save(droppedCharacters, characterFilePath);
        droppedCharacters = XmlIO.Load(characterFilePath, typeof(List<DroppedCharacter>)) as List<DroppedCharacter>;

        foreach (DroppedCharacter character in droppedCharacters)
        {
            GameObject newChar = GameObject.Instantiate(Resources.Load("Characters/" + character.modelName)) as GameObject;
            newChar.transform.parent = charRoot.transform;
            newChar.transform.localPosition = character.dropPoint;
            //Debug.Log("droppping character: " + character.dropPoint);
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

    public void DeleteCharacters()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in charRoot.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
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