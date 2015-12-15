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

    private List<GameObject> loadedCharacters = new List<GameObject>();
    private List<Toggle> toggleList = new List<Toggle>();
    private GameObject selectedChar;
    private bool dropModeOn;
    private Vector3 dropLocation = Vector3.zero;
    private Camera mouseCam;
    private RaycastHit hit;

    void OnDisable()
    {
        Destroy(selectedChar);
        ToggleMode(false);
        dropModeOn = false;
    }

    void OnEnable()
    {
        ToggleMode(false);
        dropModeOn = false;
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
            modelToggleGroup.SetAllTogglesOff();
            randomToggle.isOn = true;
            modelToggleGroup.NotifyToggleOn(randomToggle);
            selectedChar = CreateRandomChar();
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            dropCharacterSelectPanel.gameObject.SetActive(false);
            Destroy(selectedChar);
            GetComponent<Image>().color = Color.white;
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
  
        if (dropModeOn)
        {
            if (selectedChar == null)
                selectedChar = GetCharacter();

            if (selectedChar.name != modelToggleGroup.ActiveToggles().ToList()[0].name + "(Clone)" && !randomToggle.isOn)
            {
                Destroy(selectedChar);
                selectedChar = GetCharacter();
            }

			mouseCam = FindMouseCamera();
			if (mouseCam != null)
			{
                if (Physics.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), out hit))
                    dropLocation = hit.point;
                else
                    dropLocation = avatar.transform.position + avatar.transform.forward * 2f;
			}

            if(selectedChar != null)
                selectedChar.transform.position = dropLocation;

            if (Input.GetMouseButtonDown(1))
                selectedChar = GetCharacter();

            if (Input.GetMouseButtonDown(2))
            {
                Destroy(selectedChar);
                selectedChar = GetCharacter();
            }
        }
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
