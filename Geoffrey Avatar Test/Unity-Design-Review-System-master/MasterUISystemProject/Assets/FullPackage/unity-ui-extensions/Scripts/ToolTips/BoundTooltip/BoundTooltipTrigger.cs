///Credit Martin Nerurkar // www.martin.nerurkar.de // www.sharkbombs.com
///Sourced from - http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Trigger")]
	public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[TextAreaAttribute]
		public string text;

        public Text textObject;

        public string infoString = "";

		public void OnPointerEnter(PointerEventData eventData)
		{
            //Debug.Log("pointer enter");
			StartHover();
			
		}
        
		public void OnPointerExit(PointerEventData eventData)
		{
            //Debug.Log("pointer exit");
			StopHover();
		}
        
		void StartHover()
		{
            //Debug.Log("start hover");
            Invoke("TriggerShowTooltip", 2f);
            
		}

        private void TriggerShowTooltip()
        {
            //Debug.Log("trigger show");
            if (infoString != "")
                BoundTooltipItem.Instance.ShowTooltip(infoString);
            else if (textObject != null && LayoutUtility.GetPreferredWidth(textObject.rectTransform) > GetComponent<RectTransform>().rect.width)
                BoundTooltipItem.Instance.ShowTooltip(textObject.text);
            Invoke("StopHover", 5f);
        }

		void StopHover()
		{
            //Debug.Log("stop hover");
            CancelInvoke();
			BoundTooltipItem.Instance.HideTooltip();
		}
	}
}
