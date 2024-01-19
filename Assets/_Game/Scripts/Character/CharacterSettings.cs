using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings_")]
public class CharacterSettings : ScriptableObject
{
    public readonly int MaxMental = 100;
    public readonly int MinMental = 0;

    [SerializeField] string _characterName = "�߂���";
    [SerializeField] int _defaultMental = 100;

    /// <summary>
    /// ��b����p�̃w�b�_�[
    /// </summary>
    public string LogHeader => $"{_characterName}: ";
    /// <summary>
    /// �S��̏����l
    /// </summary>
    public int DefaultMental => _defaultMental;
}
