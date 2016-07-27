using UnityEngine;
using System.Collections;

public class UIDemoManager : MonoBehaviour {

    public Animator demoAnimator;

	// Use this for initialization
	void Start () {

        demoAnimator.SetBool("Slide", false);
        demoAnimator.SetBool("Fade", false);
        demoAnimator.SetBool("Grow", false);

	}
	
    public void PlaySlide()
    {
        demoAnimator.Play("Slide");
    }

    public void PlayFade()
    {
        demoAnimator.Play("Fade");
    }
    public void PlayGrow()
    {
        demoAnimator.Play("Grow");
    }

    // Update is called once per frame
    void Update () {
        
       
    }
}
