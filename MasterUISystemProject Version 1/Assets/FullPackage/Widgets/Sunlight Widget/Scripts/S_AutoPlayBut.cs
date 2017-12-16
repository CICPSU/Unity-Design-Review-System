using UnityEngine;
using System.Collections;

public class S_AutoPlayBut : MonoBehaviour {
	bool isPlaying;
	public RectTransform playBut;
	public RectTransform stopBut;
	public RectTransform hourSlider;
	public float AutoPlaySpeed; // How fast time elapse: hour per second

	// Use this for initialization
	void Start () {
		isPlaying = true;
	}

	public void clicked(){
		if(isPlaying){
			playBut.gameObject.SetActive(true);
			stopBut.gameObject.SetActive(false);
			isPlaying = false;

		}else{
			playBut.gameObject.SetActive(false);
			stopBut.gameObject.SetActive(true);
			isPlaying = true;
		}
	}


	void Update(){
		if(isPlaying){
			float time = hourSlider.GetComponent<UnityEngine.UI.Slider>().value; //get the current time

			if(SunLightWidget.Instance.altitude > -10){ //during the day, play normal speed. altitude < 10 ==> completely dark
				time += (AutoPlaySpeed/24) * Time.deltaTime; // 24 is to convert the input to hour/s
			}else{ // during night, play double speed
				time += (2 * AutoPlaySpeed/24) * Time.deltaTime;
			}
			time = time - (int)time; // get the decimal part of time, as time is always [0,1]
			hourSlider.GetComponent<UnityEngine.UI.Slider>().value = time;
		}
	}
}
