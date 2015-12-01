using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterDropper : MonoBehaviour {

    private List<GameObject> loadedCharacters = new List<GameObject>();
    private GameObject tmpChar;

    private bool dropModeOn;
    private Vector3 dropLocation = Vector3.zero;
    private Camera mouseCam;
    private RaycastHit hit;

    public void ToggleMode()
    {
        dropModeOn = !dropModeOn;
        if (dropModeOn)
        {
            tmpChar = CreateRandomChar();
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            Destroy(tmpChar);
            GetComponent<Image>().color = Color.white;
        }
    }

    private GameObject CreateRandomChar()
    {
        return Instantiate(loadedCharacters[(int)Random.Range(0, loadedCharacters.Count - 1)], dropLocation, Quaternion.identity) as GameObject;
       
    }

    void Start()
    {
        Object[] tmpArray = Resources.LoadAll("Characters/");
        foreach(Object obj in tmpArray)
            loadedCharacters.Add(obj as GameObject);
    }

    void Update()
    {
        /// here is where we will do the raycast and show a temporary character where it will be dropped
        /// we will have a reference to the temporary character and update its position to wherever the raycast from the mouse is pointing
        /// when "dropping" we will just stop updating the position
        /// need to make sure the temporary character is deleted/removed whenever the dropcharacter button is disabled (this script)   
        if (dropModeOn)
        {
			mouseCam = FindMouseCamera();
			if (mouseCam != null)
			{
				Physics.Raycast(mouseCam.ScreenPointToRay(Input.mousePosition), out hit);
				dropLocation = hit.point;
			}

            if(tmpChar != null)
                tmpChar.transform.position = dropLocation;

            if (Input.GetMouseButtonDown(1))
                tmpChar = CreateRandomChar();
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
