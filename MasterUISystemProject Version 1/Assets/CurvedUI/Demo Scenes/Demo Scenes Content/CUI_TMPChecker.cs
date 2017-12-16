using UnityEngine;
using System.Collections;

namespace CurvedUI
{
    public class CUI_TMPChecker : MonoBehaviour
    {

        [SerializeField]
        GameObject testMsg;

        [SerializeField]
        GameObject enabledMsg;

        [SerializeField]
        GameObject disabledMsg;

        // Use this for initialization
        void Start()
        {
            testMsg.gameObject.SetActive(false);

#if CURVEDUI_TMP
            enabledMsg.gameObject.SetActive(true);
            disabledMsg.gameObject.SetActive(false);
#else
            enabledMsg.gameObject.SetActive(false);
            disabledMsg.gameObject.SetActive(true);
#endif


        }


    }
}
