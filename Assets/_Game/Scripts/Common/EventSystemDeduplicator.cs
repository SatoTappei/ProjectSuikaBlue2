using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 加算シーンで再生する際に重複したEventSystemを検知し削除する。
/// </summary>
public class EventSystemDeduplicator : MonoBehaviour
{
    [Header("複数シーンで再生した際に無効化")]
    [SerializeField] EventSystem _eventSystem;

    // シングルトン的な使い方するためのフラグ
    static bool _first = true;

    void Awake()
    {
        // シングルトン風なチェック
        if (_first) _first = false;
        else
        {
            // 警告が出るのでListenerもついでに削除しておく
            if (_eventSystem != null) Destroy(_eventSystem.gameObject);
        }
    }

    void OnDestroy()
    {
        _first = true;
    }
}
