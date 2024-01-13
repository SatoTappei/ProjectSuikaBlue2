using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PSB.Novel
{
    public class SaveButton : MonoBehaviour
    {
        [SerializeField] EntryPoint _entryPoint;
        [SerializeField] Button _button;

        void Awake()
        {
            if (_entryPoint == null || _button == null) return;

            _button.onClick.AddListener(() => 
            {
                SaveManager.Save(_entryPoint.Episode, _entryPoint.Index);
            });
        }
    }
}
