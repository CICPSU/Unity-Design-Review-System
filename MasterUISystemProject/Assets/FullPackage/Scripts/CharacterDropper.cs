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

    /// <summary>
    /// CHAR EDIT VARS
    /// </summary>
    public RectTransform charEditPanel;
    public Projector radiusProjector;
    public InputField radiusInput;
    public ToggleGroup charEditWanderToggleGroup;
    public Image radiusSelectMask;
    public NavMeshWander.WanderMode selectedMode;

    private GameObject charToEdit;
    private NavMeshWander navMeshWanderToEdit;

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
            radiusProjector.transform.position = navMeshWanderToEdit.localWanderCenter + new Vector3(0, 2, 0);
            if (!userSetRadius)
                radiusProjector.orthographicSize = navMeshWanderToEdit.localWanderRadius;

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
            radiusProjector.transform.position = navMeshWanderToEdit.localWanderCenter + new Vector3(0, 2, 0);
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
                    RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 2 | 1 << 8));
                    radiusProjector.orthographicSize = (charToEdit.transform.position - RaycastLock.hit.point).magnitude;
                }

                else
                    radiusProjector.orthographicSize = navMeshWanderToEdit.localWanderRadius;
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
                RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 2 | 1 << 8));

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
            //raycast that ignores the user avatar
            if (mouseCam != null && Input.GetMouseButtonDown(0))
            {
                if (!hasRaycastLock && RaycastLock.GetLock())
                {
                    hasRaycastLock = true;
                }

                if (hasRaycastLock)
                {
                    RaycastLock.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), ~(1 << 9 | 1 << 7));
                    //if we are pointing at an existing avatar and left click, open char info
                    if (RaycastLock.hit.transform != null && RaycastLock.hit.transform.GetComponent<NavMeshWander>() != null && !charInfoOpen)
                        OpenCharacterInfo();
                    else
                    {
                        RaycastLock.GiveLock();
                        hasRaycastLock = false;
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
                value = radiusProjector.orthographicSize.ToString();
                radiusInput.text = value;
            }
            radiusProjector.orthographicSize = float.Parse(value);
        }
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
        charToDrop.GetComponent<NavMeshWander>().enabled = true;
        charToDrop.GetComponent<NavMeshWander>().mode = (NavMeshWander.WanderMode)newCharWanderSelect.value;
        charToDrop.GetComponent<NavMeshWander>().dropPoint = charToDrop.transform.position;
       
        charToDrop.transform.parent = charRoot.transform;

        if ((NavMeshWander.WanderMode)newCharWanderSelect.value == NavMeshWander.WanderMode.Patrol)
        {
            charToEdit = charToDrop;
            navMeshWanderToEdit = charToEdit.GetComponent<NavMeshWander>();
            StartCharRadiusSelect();
        }

        droppedCharacters.Add(new DroppedCharacter(charToDrop.GetComponent<NavMeshWander>()));
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
        returnChar.GetComponent<NavMeshWander>().enabled = false;
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
        droppedCharacters.Remove(new DroppedCharacter(charToEdit.GetComponent<NavMeshWander>()));
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
        if(navMeshWanderToEdit == null)
            navMeshWanderToEdit = charToEdit.GetComponent<NavMeshWander>();

        if (charToEdit != null)
            navMeshWanderToEdit.localWanderCenter = charToEdit.transform.position;

        charRadiusSelect = true;
        firstFrameRadiusSelect = true;
        
    }

    public void StopCharRadiusSelect(bool startMotion)
    {
        if (navMeshWanderToEdit != null)
        {
            navMeshWanderToEdit.localWanderRadius = radiusProjector.orthographicSize;
            if(startMotion)
                navMeshWanderToEdit.ConfigureDestination();
        }
        

        //charToEdit = null;
        //navMeshWanderToEdit = null;
        charRadiusSelect = false;
        
    }

    public void OpenCharacterEdit()
    {
        userSetRadius = false;
        charEditOpen = true;
        charEditPanel.gameObject.SetActive(true);
        charEditWanderToggleGroup.SetAllTogglesOff();
        radiusInput.text = navMeshWanderToEdit.localWanderRadius.ToString();
        navMeshWanderToEdit.localWanderCenter = charToEdit.transform.position;
        Toggle toggleToActivate = charEditWanderToggleGroup.transform.GetChild((int)navMeshWanderToEdit.mode).GetComponent<Toggle>();
        toggleToActivate.isOn = true;
        charEditWanderToggleGroup.NotifyToggleOn(toggleToActivate);

        // move info panel off screen
        iTween.MoveBy(charInfoPanel.gameObject, iTween.Hash("y", Screen.height, "easeType", "easeInOutExpo", "delay", .1));

        // grow edit panel
        charEditPanel.localPosition = Input.mousePosition;
        charEditPanel.localScale = new Vector3(.01f, .01f, .01f);
        iTween.ScaleBy(charEditPanel.gameObject, iTween.Hash("x", 100, "y", 100, "easeType", "easeInOutExpo", "delay", .1));
    }

    public void CloseCharacterEdit()
    {
        if (navMeshWanderToEdit != null && charEditOpen)
        {
            if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Idle")
                selectedMode = (NavMeshWander.WanderMode)0;
            else if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Explore")
                selectedMode = (NavMeshWander.WanderMode)2;
            else if (charEditWanderToggleGroup.ActiveToggles().ToArray()[0].name == "Patrol")
            {
                selectedMode = (NavMeshWander.WanderMode)1;
                navMeshWanderToEdit.localWanderCenter = charToEdit.transform.position;
                navMeshWanderToEdit.localWanderRadius = float.Parse(radiusInput.text);

            }
        }

        iTween.MoveBy(charInfoPanel.gameObject, iTween.Hash("y", -Screen.height, "easeType", "easeInOutExpo", "delay", .1));


        charEditOpen = false;
        charEditPanel.gameObject.SetActive(false);
    }

    public void OpenCharacterInfo()
    {
        Destroy(charToDrop);
        charInfoOpen = true;
        charRadiusSelect = false;
        charToEdit = RaycastLock.hit.transform.gameObject;
        navMeshWanderToEdit = charToEdit.GetComponent<NavMeshWander>();
        charToEdit.GetComponent<NavMeshAgent>().Stop();
        charToEdit.GetComponent<Animator>().enabled = false;
        charInfoPanel.gameObject.SetActive(true);
        selectedMode = navMeshWanderToEdit.mode;

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


        iTween.Stop(charInfoPanel.gameObject, true);
        charInfoPanel.localScale = new Vector3(.01f, .01f, .01f);
        iTween.ScaleBy(charInfoPanel.gameObject, iTween.Hash("x", 100, "y", 100, "easeType", "easeInOutExpo"));

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

        CloseCharacterEdit();
        if (charToEdit != null)
            charToEdit.GetComponent<Animator>().enabled = true;
        if (navMeshWanderToEdit != null)
        {
            if(destinationDropDown.value == 0)
                navMeshWanderToEdit.ConfigureDestination();
            else
                navMeshWanderToEdit.ConfigureDestination(destinationDropDown.value - 1);
        }
        ApplyOptions();
        SaveCharacters();
        charToEdit = null;
        navMeshWanderToEdit = null;
        charInfoOpen = false;
        charInfoPanel.gameObject.SetActive(false);
        
    }

    public void UpdateCharInfoLabels()
    {
        modelNameLabel.text = charToEdit.gameObject.name.Remove(charToEdit.gameObject.name.IndexOf("("));

        wanderModeLabel.text = navMeshWanderToEdit.mode.ToString();

        if ((int)navMeshWanderToEdit.mode == 0)
        {
            radiusProjector.orthographicSize = 0;
            wanderRangeLabel.text = "0";
        }
        else if ((int)navMeshWanderToEdit.mode == 1)
        {
            radiusProjector.orthographicSize = navMeshWanderToEdit.localWanderRadius;
            wanderRangeLabel.text = navMeshWanderToEdit.localWanderRadius.ToString();
        }
        else if ((int)navMeshWanderToEdit.mode == 2)
        {
            radiusProjector.orthographicSize = 0;
            wanderRangeLabel.text = "Infinite";
        }

        destinationDropDown.value = navMeshWanderToEdit.poiDestination;
    }

    public void ApplyOptions()
    {
        if (charToEdit != null)
        {
            navMeshWanderToEdit.mode = selectedMode;

            UpdateCharInfoLabels();
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
            Debug.Log("droppping character: " + character.dropPoint);
            newChar.transform.localScale = Vector3.one;
            NavMeshWander newWander = newChar.GetComponent<NavMeshWander>();
            newWander.dropPoint = character.dropPoint;
            newWander.localWanderCenter = character.localWanderCenter;
            newWander.localWanderRadius = character.localWanderRadius;
            newWander.mode = character.mode;
            newWander.normalSpeedRadius = character.normalSpeedRadius;
            newWander.defaultSpeed = character.defaultSpeed;
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
                droppedCharacters.Add(new DroppedCharacter(charRoot.transform.GetChild(i).GetComponent<NavMeshWander>()));
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
    public NavMeshWander.WanderMode mode;
    public float defaultSpeed;
    public float normalSpeedRadius;

    public DroppedCharacter()
    {

    }

    public DroppedCharacter(NavMeshWander wanderScript)
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