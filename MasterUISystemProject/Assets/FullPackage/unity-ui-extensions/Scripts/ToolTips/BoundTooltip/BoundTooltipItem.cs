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
        public bool moveTooltip = false;

        private Vector3 finalPos = Vector3.zero;
        private RectTransform tooltipRect;

        void Awake()
        {
            tooltipRect = GetComponent<RectTransform>();
            if(instance == null)
                instance = this;

            if (!TooltipText)
                TooltipText = GetComponentInChildren<Text>();
            HideTooltip();
        }

        void Update()
        {
            // here we will need to update the tooltips position based on the mouse
            // Show and Hide tooltip will just enable and disable it and update the text
            if (moveTooltip)
            {
                PlaceTooltip();
            }
        }


        public void PlaceTooltip()
        {
            finalPos = Input.mousePosition;

            // need to check to keep the tootip on screen
            // open in a quadrant around the mouse to keep it on screen.
            if (Input.mousePosition.x + tooltipRect.sizeDelta.x > Screen.width)
                finalPos -= new Vector3(tooltipRect.sizeDelta.x * .6f, 0, 0);
            else
                finalPos += new Vector3(tooltipRect.sizeDelta.x * .6f, 0, 0);

            if (Input.mousePosition.y + tooltipRect.sizeDelta.y * .6f > Screen.height)
                finalPos -= new Vector3(0, tooltipRect.sizeDelta.y * .6f, 0);
            else
                finalPos += new Vector3(0, tooltipRect.sizeDelta.y * .6f, 0);

            transform.position = finalPos;
        }

        /// <summary>
        /// this function is called from the TooltipTrigger component
        /// this function displays the tooltip and sizes it to fit the text inside
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        public void ShowTooltip(string text)
        {

            if (TooltipText.text != text)
                TooltipText.text = text;

            moveTooltip = true;

            

            gameObject.SetActive(true);
            GetComponent<RectTransform>().sizeDelta = new Vector2(LayoutUtility.GetPreferredWidth(TooltipText.rectTransform) + 100, GetComponent<RectTransform>().sizeDelta.y);
            PlaceTooltip();

        }

        public void HideTooltip()
        {
            moveTooltip = false;
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

 
