using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class ScoreGetter : MonoBehaviour
    {
        [SerializeField] Text _text;

        Score _score;

        void Awake()
        {
            _text.text = string.Empty;
        }

        void Start()
        {
            PrintScore();
        }

        [Inject]
        void Construct(Score scoreHolder)
        {
            _score = scoreHolder;
        }

        void PrintScore()
        {
            if (_score == null) return;
            _text.text = _score.Value.ToString();
        }
    }
}