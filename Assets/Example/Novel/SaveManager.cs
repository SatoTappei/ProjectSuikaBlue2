using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int Episode;
    public int Index;
}

public class SaveManager
{
    const string FileName = "NovelGameSaveData";
    const string FilePath = "/Example/Novel";

    /// <summary>
    /// Json�`���ŃZ�[�u
    /// </summary>
    public static void Save(int episode, int index)
    {
        Debug.Log("������" + episode + " " + index);
        using (StreamWriter writer = new(Path(), append: false))
        {
            SaveData save = new() { Episode = episode, Index = index };
            string json = JsonUtility.ToJson(save);
            writer.Write(json);
            writer.Flush();
            writer.Close();
        }
    }

    /// <summary>
    /// Json�t�@�C�������[�h
    /// </summary>
    public static SaveData Load()
    {
        SaveData save;
        using (StreamReader reader = new(Path()))
        {
            string str = reader.ReadLine();
            reader.Close();
            save = JsonUtility.FromJson<SaveData>(str);
        }
        Debug.Log("���" + save.Episode + " " + save.Index);
        return save;
    }

    static string Path()
    {
        string root = Application.persistentDataPath;
#if UNITY_EDITOR
        root = Application.dataPath;
#endif
        return $"{root}{FilePath}/{FileName}.json";
    }
}