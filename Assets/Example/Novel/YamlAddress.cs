using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YamlAddress
{
    const string FilePath = "/Example/Novel";
    const string Episode1 = "Scene_Sample_1.yaml";
    const string Episode2 = "Scene_Sample_2.yaml";
    const string Episode3 = "Scene_Sample_3.yaml";

    public static string Get(int number)
    {
        string episode = string.Empty;
        if (number == 1) episode = Episode1;
        else if (number == 2) episode = Episode2;
        else if (number == 3) episode = Episode3;
        else throw new System.ArgumentOutOfRangeException("‘Î‰ž‚·‚é˜b‚ÌYAML‚ª–³‚¢: " + number);

        string root = Application.persistentDataPath;
#if UNITY_EDITOR
        root = Application.dataPath;
#endif
        return $"{root}{FilePath}/{episode}";
    }

    public static string GetWithIndex(int index)
    {
        return Get(index + 1);
    }
}
