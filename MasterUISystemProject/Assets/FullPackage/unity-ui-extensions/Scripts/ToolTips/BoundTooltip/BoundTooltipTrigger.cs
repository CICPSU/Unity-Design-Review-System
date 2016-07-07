///Credit Martin Nerurkar // www.martin.nerurkar.de // www.sharkbombs.com
///Sourced from - http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Trigger")]
	public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
	{
		[TextAreaAttribute]
		public string text;

        public Text textObject;

        public string infoString = "";

		public void OnPointerEnter(PointerEventData eventData)
		{
			StartHover();
			
		}

		public void OnSelect(BaseEventData eventData)
		{
			StartHover();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			StopHover();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StopHover();
		}

		void StartHover()
		{
            Invoke("TriggerShowTooltip", 2);
            Invoke("StopHover", 7);
		}

        private void TriggerShowTooltip()
        {
            if (infoString != "")
                BoundTooltipItem.Instance.ShowTooltip(infoString);
            else if (textObject != null && LayoutUtility.GetPreferredWidth(textObject.rectTransform) > GetComponent<RectTransform>().rect.width)
                BoundTooltipItem.Instance.ShowTooltip(textObject.text);
        }

		void StopHover()
		{
            CancelInvoke();
			BoundTooltipItem.Instance.HideTooltip();
		}
	}
}
