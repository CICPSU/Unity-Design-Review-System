using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {
    private GameObject avatar;

    public void teleport()
    {
        avatar = GameObject.FindWithTag("Player");
        avatar.transform.position = this.GetComponent<POIInfo>().position;
        avatar.transform.eulerAngles = this.GetComponent<POIInfo>().rotation;
    }
}
