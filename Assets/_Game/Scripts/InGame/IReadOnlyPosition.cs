using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// Vector3�ō��W��ǂݎ��p�r�ŃN���X��\���̂𗘗p����ۂɎg�p
    /// </summary>
    public interface IReadOnlyPosition
    {
        Vector3 Position { get; }
    }
}
