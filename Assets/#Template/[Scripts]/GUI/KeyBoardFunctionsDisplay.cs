using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DLMTP_GAME
{
    
    public class KeyBoardFunctionsDisplay : MonoBehaviour
    {
        private void Start()
        {
            KeyBoardManager.instance.onKeyFunctionsChanged += RefreshKeyBoardFunctionsDisplay;
        }

        public void RefreshKeyBoardFunctionsDisplay()
        {
            GetComponent<Text>().text = "";
            for (int i = 0; i < KeyBoardManager.instance.keyFunctions.Count; i++)
            {
                GetComponent<Text>().text += $"{KeyBoardManager.instance.keyFunctions[i].keyCode} : {KeyBoardManager.instance.keyFunctions[i].comment}    ";
            }
        }

        private void Update()
        {
            KeyBoardManager.instance.AddKeyFunction(KeyCode.V, "显示/隐藏键位显示", () =>
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                else gameObject.SetActive(true);
            });
        }
    }

}