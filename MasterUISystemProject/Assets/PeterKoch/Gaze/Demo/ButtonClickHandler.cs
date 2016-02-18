using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClickHandler : MonoBehaviour, IPointerClickHandler 
{
	public void OnPointerClick(PointerEventData eventData)
	{
		GetComponent<Renderer>().material.color = new Color (Random.value, Random.value, Random.value, 1.0f);
	}
}
