using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackLog : MonoBehaviour
{
    const int DefaultElementCount = 2;

    [SerializeField] Button _openButton;
    [SerializeField] Button _closeButton;
    [SerializeField] GameObject _elementPrefab;
    [SerializeField] GameObject _main;
    [SerializeField] Transform _content;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] ContentSizeFitter _fitter;

    List<GameObject> _elements = new(DefaultElementCount);
    int _leadIndex;

    void Awake()
    {
        // �ŏ��Ɉ�萔�������Ă���
        for (int i = 0; i < DefaultElementCount; i++) CreateElement();

        _openButton.onClick.AddListener(Show);
        _closeButton.onClick.AddListener(Hide);
    }

    void Start()
    {
        Hide();
    }

    public void Show()
    {
        _closeButton.gameObject.SetActive(true);
        _main.SetActive(true);

        // �X�N���[���o�[����ԉ��Ɏ����čs������
        _scrollRect.verticalNormalizedPosition = 0;
        _fitter.SetLayoutVertical();
    }

    public void Hide()
    {
        _closeButton.gameObject.SetActive(false);
        _main.SetActive(false);
    }

    public void Add(string name, string line)
    {
        // ����Ȃ��Ȃ����琶������
        if (_leadIndex >= _elements.Count) CreateElement();

        GameObject g = _elements[_leadIndex++];
        g.SetActive(true);
        SetText(g, name, line);
    }

    public void Release()
    {
        for(int i = 0; i < _leadIndex; i++)
        {
            SetText(_elements[i], string.Empty, string.Empty);
            _elements[i].SetActive(false);
        }

        _leadIndex = 0;
    }

    void CreateElement()
    {
        GameObject g = Instantiate(_elementPrefab, _content);
        _elements.Add(g);
        g.SetActive(false);
    }

    // �v���n�u�ɖ��O�Ƒ䎌�p��2�t���Ă�
    void SetText(GameObject g, string name, string line)
    {
        Text[] t = g.GetComponentsInChildren<Text>();
        t[0].text = name;
        t[1].text = line;
    }
}
