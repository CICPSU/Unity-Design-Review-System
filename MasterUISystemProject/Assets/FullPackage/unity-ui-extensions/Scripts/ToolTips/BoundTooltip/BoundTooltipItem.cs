///Credit Martin Nerurkar // www.martin.nerurkar.de // www.sharkbombs.com
///Sourced from - http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Item")]
    public class BoundTooltipItem : MonoBehaviour
    {
        public bool IsActive
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public UnityEngine.UI.Text TooltipText;
        public Vector3 ToolTipOffset;

        void Awake()
        {
            instance = this;
            if(!TooltipText) TooltipText = GetComponentInChildren<Text>();
            HideTooltip();
        }

        /// <summary>
        /// this function is called from the TooltipTrigger component
        /// this function displays the tooltip and sizes it to fit the text inside
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        public void ShowTooltip(string text, Vector3 pos)
        {
            if (TooltipText.text != text)
                TooltipText.text = text;
            Vector3 finalPos = pos + ToolTipOffset;
            
            // this code is used to keep the tooltip on the screen, but causes flickering when it moves the tooltip under the mouse
            /*
            if (finalPos.y + .5f * GetComponent<RectTransform>().sizeDelta.y > Screen.height)
                finalPos -= new Vector3(0, finalPos.y + .5f * GetComponent<RectTransform>().sizeDelta.y - Screen.height,0);
            if (finalPos.x + .5f * GetComponent<RectTransform>().sizeDelta.x > Screen.width)
                finalPos -= new Vector3(0, finalPos.x + .5f * GetComponent<RectTransform>().sizeDelta.x - Screen.width, 0);
                */
            transform.position = finalPos;

            gameObject.SetActive(true);
            GetComponent<RectTransform>().sizeDelta = new Vector2(LayoutUtility.GetPreferredWidth(TooltipText.rectTransform) + 100 ,GetComponent<RectTransform>().sizeDelta.y);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        // Standard Singleton Access
        private static BoundTooltipItem instance;
        public static BoundTooltipItem Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<BoundTooltipItem>();
                return instance;
            }
        }
    }
}

 
