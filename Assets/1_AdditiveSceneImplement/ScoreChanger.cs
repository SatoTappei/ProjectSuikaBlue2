using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class ScoreChanger : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] Text _text;

        Score _score;

        void Awake()
        {
            _button.onClick.AddListener(AddScore);
            _text.text = string.Empty;
        }

        [Inject]
        void Construct(Score scoreHolder)
        {
            _score = scoreHolder;
        }

        public void AddScore()
        {
            if (_score == null) return;
            _score.Value++;
            _text.text = _score.Value.ToString();
        }
    }
}