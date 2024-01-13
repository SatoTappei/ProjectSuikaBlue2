using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 試験的記録機能 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) SaveManager.Save(1, 1);
        else if (Input.GetKeyDown(KeyCode.L))
        {
            SaveData save = SaveManager.Load();
            if (save != null)
            {
                Debug.Log(save.Episode + " " + save.Index);
            }
        }
    }
}
