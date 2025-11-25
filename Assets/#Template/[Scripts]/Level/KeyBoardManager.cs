using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DLMTP_GAME
{
    
    public class KeyBoardManager : MonoBehaviour
    {
        public static KeyBoardManager instance;
        private void Awake()
        {
            instance = this;
        }
    
        public List<KeyFunction> keyFunctions;

        public UnityAction onKeyFunctionsChanged;
    
        void Update()
        {
            // 创建副本以避免在遍历过程中修改集合
            List<KeyFunction> keyFunctionsCopy = new List<KeyFunction>(keyFunctions);
            foreach (var keyFunction in keyFunctionsCopy)
            {
                if (Input.GetKeyDown(keyFunction.keyCode))
                {
                    keyFunction.onKeyPress?.Invoke();
                }
            }
        }
    
        public bool AddKeyFunction(KeyCode keyCode, string comment, UnityAction action)
        {
            if (keyFunctions.Exists(x => x.keyCode == keyCode)) return false;
        
            keyFunctions.Add(new KeyFunction{keyCode = keyCode, comment = comment, onKeyPress = action});
            onKeyFunctionsChanged?.Invoke();
            return true;
        }
    
        public void RemoveKeyFunction(KeyCode keyCode)
        {
            keyFunctions.RemoveAll(x => x.keyCode == keyCode);
            onKeyFunctionsChanged?.Invoke();
        }
        
        public void ClearKeyFunctions()
        {
            keyFunctions.Clear();
            onKeyFunctionsChanged?.Invoke();
        }

        [Serializable]
        public class KeyFunction
        {
            public KeyCode keyCode;
            public string comment;
            public UnityAction onKeyPress;
        }
    }


}