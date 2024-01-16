using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [Header("�\�����[�h�̐؂�ւ�")]
    [SerializeField] CanvasGroup _full;
    [SerializeField] CanvasGroup _simple;
    [SerializeField] Button _changeButton;
    [Header("�V���v���\���̒ʒm")]
    [SerializeField] GameObject _notice;
    [SerializeField] Transform _noticeParent;
    [SerializeField] int _noticeMax = 5;
    [SerializeField] Vector3 _defaultNoticePosition;

    List<GameObject> _notices = new();
    int _usedNotice;
    bool _isFull = true;

    void Start()
    {
        _changeButton.onClick.AddListener(Switch);
        StateChange(_isFull);
        CreateNoticeInstances();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Notice("�ق��ق������҂�");
        }
    }

    // ���炩���ߐ����������̂��L���[�ɂ��Ďg���܂킷
    void CreateNoticeInstances()
    {
        for (int i = 0; i < _noticeMax; i++)
        {
            GameObject g = Instantiate(_notice);            
            g.transform.SetParent(_noticeParent); 
            g.transform.localPosition = _defaultNoticePosition;
            g.transform.localScale = Vector3.one;
            _notices.Add(g);
        }
    }

    // �V���v���ƃt����؂�ւ���
    void Switch()
    {
        _isFull = !_isFull;
        StateChange(_isFull);
    }

    void StateChange(bool isFull)
    {
        _full.alpha = isFull ? 1 : 0;
        _simple.alpha = isFull ? 0 : 1;
    }

    //
    void Notice(string line)
    {
        GameObject g = _notices[_usedNotice++];
        // �ǉ�
        // ������
    }
}
