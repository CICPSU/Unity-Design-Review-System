using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterDropper : MonoBehaviour {

    public Toggle randomToggle;
    public Text modelLabel;
    public RectTransform modelList;
    public ToggleGroup modelToggleGroup;
    public RectTransform dropCharacterSelectPanel;
    public GameObject avatar;
    public Dropdown newCharWanderSelect;
    public RectTransform charOptionsPanel;
    public Dropdown charOptionsWanderSelect;
    public Image buttonImage;
    public Projector radiusProjector;

    private List<GameObject> loadedCharacters = new List<GameObject>();
    private List<Toggle> toggleList = new List<Toggle>();
    private GameObject charToDrop;
    private GameObject charToEdit;
    private NavMeshWander navMeshWanderToEdit;
    private bool dropModeOn;
    private bool charEditModeOn;
    private bool radiusSelectMode;
    private bool modelOptionsGreyed;
    private Vector3 dropLocation = Vector3.zero;
    private Camera mouseCam;
    private RaycastHit hit;

    private NavMeshWander.WanderMode prevWanderMode = NavMeshWander.WanderMode.Idle;

    void OnDisable()
    {
        Destroy(charToDrop);
        ToggleMode(false);
        dropModeOn = false;
        charEditModeOn = false;
        radiusSelectMode = false;
        modelOptionsGreyed = false;
        radiusProjector.gameObject.SetActive(false);
        CloseCharacterOptions();
    }

    void OnEnable()
    {
        ToggleMode(false);
        dropModeOn = false;
        charEditModeOn = false;
        radiusSelectMode = false;
        modelOptionsGreyed = false;
        radiusProjector.gameObject.SetActive(false);
        CloseCharacterOptions();
    }

    public void ToggleMode()
    {
        dropModeOn = !dropModeOn;
        ToggleMode(dropModeOn);
    }

    private void ToggleMode(bool mode)
    {
        if (mode)
        {
            dropCharacterSelectPanel.gameObject.SetActive(true);
            CloseCharacterOptions();
            modelToggleGroup.SetAllTogglesOff();
            randomToggle.isOn = true;
            charToDrop = CreateRandomChar();
            buttonImage.color = Color.red;
        }
        else
        {
            dropCharacterSelectPanel.gameObject.SetActive(false);
            CloseCharacterOptions();
            Destroy(charToDrop);
            buttonImage.color = Color.white;
        }
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
                if(character.name == toggleList[0].name)
                {
                    modelLabel.text = "Model: " + character.name;
                    returnChar = Instantiate(character, dropLocation, Quaternion.identity) as GameObject;
                }
            }
        }
        returnChar.GetComponent<CapsuleCollider>().enabled = false;
        returnChar.GetComponent<NavMeshAgent>().enabled = false;
        return returnChar;
    }

    private GameObject CreateRandomChar()
    {
        int randomIndex = (int)Random.Range(0, loadedCharacters.Count - 1);
        modelLabel.text = "Model: " + loadedCharacters[randomIndex].name;
        return Instantiate(loadedCharacters[randomIndex], dropLocation, Quaternion.identity) as GameObject;
    }

    void Start()
    {
        charOptionsPanel.gameObject.SetActive(false);
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
        /// here is where we will do the raycast and show a temporary character where it will be dropped
        /// we will have a reference to the temporary character and update its position to wherever the raycast from the mouse is pointing
        /// when "dropping" we will just stop updating the position
        /// need to make sure the temporary character is deleted/removed whenever the dropcharacter button is disabled (this script)   

        if (randomToggle.isOn)
        {
            if(modelToggleGroup.AnyTogglesOn())
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
        if (!modelToggleGroup.AnyTogglesOn() && !randomToggle.isOn)
        {
            modelToggleGroup.GetComponentInChildren<Toggle>().isOn = true;
            modelToggleGroup.NotifyToggleOn(modelToggleGroup.GetComponentInChildren<Toggle>());
        }

        if (dropModeOn)
        {
            if (!charEditModeOn)
            {
                if (charToDrop == null)
                    charToDrop = GetCharacter();

                if (modelToggleGroup.ActiveToggles().Count() > 0 
                    && charToDrop.name != modelToggleGroup.ActiveToggles().ToList()[0].name + "(Clone)" 
                    && !randomToggle.isOn)
                {
                    Destroy(charToDrop);
                    charToDrop = GetCharacter();
                }

                mouseCam = FindMouseCamera();
                if (mouseCam != null)
                {
                    if (Physics.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), out hit))
                        dropLocation = hit.point;
                    else
                        dropLocation = avatar.transform.position + avatar.transform.forward * 2f;
                }

                if (charToDrop != null && !radiusSelectMode)
                    charToDrop.transform.position = dropLocation;

                if (hit.transform != null && hit.transform.GetComponent<NavMeshAgent>() != null)
                {
                    if (Input.GetMouseButtonDown(0))
                        OpenCharacterOptions();
                    charToDrop.SetActive(false);
                }
                else
                {
                    if (!charToDrop.activeSelf)
                        charToDrop.SetActive(true);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (!radiusSelectMode)
                    {
                        charToDrop.GetComponent<CapsuleCollider>().enabled = true;
                        charToDrop.GetComponent<NavMeshAgent>().enabled = true;
                        
                        //need to implement the expanding circle to show/select local wander radius
                        if ((NavMeshWander.WanderMode)newCharWanderSelect.value == NavMeshWander.WanderMode.Local)
                        {
                            charToDrop.GetComponent<NavMeshWander>().localWanderCenter = hit.point;
                            radiusProjector.gameObject.SetActive(true);
                            radiusProjector.transform.position = charToDrop.transform.position + new Vector3(0, 2, 0);
                            radiusSelectMode = true;
                        }
                        else
                        {
                            charToDrop.GetComponent<NavMeshWander>().mode = (NavMeshWander.WanderMode)newCharWanderSelect.value;
                            charToDrop = GetCharacter();
                        }
                    }
                    else
                    {
                        charToDrop.GetComponent<NavMeshWander>().localWanderRadius = radiusProjector.orthographicSize;
                        charToDrop.GetComponent<NavMeshWander>().mode = (NavMeshWander.WanderMode)newCharWanderSelect.value;
                        radiusProjector.gameObject.SetActive(false);
                        radiusSelectMode = false;
                        charToDrop = GetCharacter();
                    }
                }
                if (radiusSelectMode)
                {
                    radiusProjector.orthographicSize = (charToDrop.transform.position - hit.point).magnitude;
                }


                if (Input.GetMouseButtonDown(2))
                {
                    Destroy(charToDrop);
                    charToDrop = GetCharacter();
                }
            }// chareditmode
        }
    }

    public void OpenCharacterOptions()
    {
        Destroy(charToDrop);
        charEditModeOn = true;
        charToEdit = hit.transform.gameObject;
        navMeshWanderToEdit = charToEdit.GetComponent<NavMeshWander>();
        prevWanderMode = navMeshWanderToEdit.mode;
        navMeshWanderToEdit.mode = NavMeshWander.WanderMode.Idle;
        charOptionsPanel.gameObject.SetActive(true);
        charOptionsPanel.transform.position = Input.mousePosition;
        charOptionsWanderSelect.value = (int)navMeshWanderToEdit.mode;

    }

    public void ApplyOptions()
    {
        prevWanderMode = (NavMeshWander.WanderMode)charOptionsWanderSelect.value;
        CloseCharacterOptions();
    }

    public void CloseCharacterOptions()
    {
        charToEdit = null;
        charEditModeOn = false;
        if(navMeshWanderToEdit != null)
        navMeshWanderToEdit.mode = prevWanderMode;
        charOptionsPanel.gameObject.SetActive(false);
    }

    public void DeleteCharacter()
    {
        Destroy(charToEdit);
        charToEdit = null;
        CloseCharacterOptions();
    }

    private Camera FindMouseCamera()
    {
        List<Camera> camList = (from cam in GameObject.FindObjectsOfType<Camera>() where cam.targetTexture == null select cam).ToList();
        foreach(Camera cam in camList)
        {
            if(Input.mousePosition.x > cam.pixelRect.xMin && Input.mousePosition.x < cam.pixelRect.xMax
                && Input.mousePosition.y > cam.pixelRect.yMin && Input.mousePosition.y < cam.pixelRect.yMax)
            {
                return cam;
            }
        }
        return null;
    }
}
