using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class S_EditDateBut : MonoBehaviour {
	public RectTransform labelPanel;
	public RectTransform inputPanel;
	public Sprite saveIcon;
	public Sprite editIcon;

	private RectTransform monthInput;
	private RectTransform dateInput;
	private RectTransform yearInput;

	private Text monthLabel;
	private Text dateLabel;
	private Text yearLabel;

	bool isEditMode = false;

	public void editDateClicked(){
		if(isEditMode){
			isEditMode = false;
			transform.FindChild("Icon").GetComponent<Image>().sprite = editIcon;
			this.GetComponent<Image>().color = new Color(1,1,1);
			labelPanel.gameObject.SetActive(true);
			inputPanel.gameObject.SetActive(false);

			establishRefToDateUIs();
			monthLabel.text = monthInput.GetComponent<InputField>().text;
			dateLabel.text = dateInput.GetComponent<InputField>().text;
			yearLabel.text = yearInput.GetComponent<InputField>().text;

			//save changes to the database
			SunLightWidget.Instance.InputData.updateDateMonYear(int.Parse(dateLabel.text), int.Parse(monthLabel.text), int.Parse(yearLabel.text));
			//save changes to xml
			SunLightWidget.Instance.saveDataToXML();
			//recalc sun angle
			SunLightWidget.Instance.calcSunCoordination();
			
		}else{//was not in edit mode, now in editmode
			isEditMode = true;

			transform.FindChild("Icon").GetComponent<Image>().sprite = saveIcon;
			this.GetComponent<Image>().color = new Color(0.67f, 0.67f, 0.67f);
			labelPanel.gameObject.SetActive(false);
			inputPanel.gameObject.SetActive(true);

			establishRefToDateUIs();
			monthInput.GetComponent<InputField>().text = monthLabel.text;
			dateInput.GetComponent<InputField>().text = dateLabel.text;
			yearInput.GetComponent<InputField>().text = yearLabel.text;

		}

	}

	//find the reference to the date input fields and date labels, run only once in each game
	void establishRefToDateUIs(){
		if(monthInput == null || dateInput == null || yearInput == null){
			monthInput = inputPanel.FindChild("MonthInput") as RectTransform;
			dateInput = inputPanel.FindChild("DateInput") as RectTransform;
			yearInput = inputPanel.FindChild("YearInput") as RectTransform;
		}

		if(monthLabel == null || dateLabel == null || yearLabel == null){
			monthLabel = labelPanel.FindChild("Month").GetComponent<Text>();
			dateLabel = labelPanel.FindChild("Date").GetComponent<Text>();
			yearLabel = labelPanel.FindChild("Year").GetComponent<Text>();
		}

		return;
	}
}
