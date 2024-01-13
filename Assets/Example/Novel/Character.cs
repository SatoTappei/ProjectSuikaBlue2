using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace PSB.Novel
{
    public enum CharacterType
    {
        Boy,
        Girl,
        Oink,
    }

    [DefaultExecutionOrder(-1)]
    public class Character : MonoBehaviour
    {
        // 必要に応じてインスペクタから設定できるように変更可能
        static readonly Color OnBackColor = new Color(.5f, .5f, .5f, 1);
        const float JumpHeight = 3.0f;

        [System.Serializable]
        class SpriteData
        {
            [SerializeField] CharacterType _type;
            [SerializeField] Sprite _sprite;

            public CharacterType Type => _type;
            public Sprite Sprite => _sprite;
        }

        [SerializeField] SpriteData[] _spriteData;
        [SerializeField] Transform _transform;
        [SerializeField] Image _image1;
        [SerializeField] Image _image2;

        Dictionary<CharacterType, Sprite> _sprites;
        Image _foreImage;
        Image _backImage;
        Vector2 _defaultPosition;

        void Start()
        {
            Init();
        }

        /// <summary>
        /// シーン開始時、各種コマンドを実行する前に初期化する必要がある
        /// </summary>
        public void Init()
        {
            _sprites = _spriteData.ToDictionary(d => d.Type, d => d.Sprite);
            _image1.sprite = null;
            _image2.sprite = null;
            _image1.color = Color.clear;
            _image2.color = Color.clear;

            // インデックスが大きい = 下にあるので手前に表示される
            int sib1 = _image1.transform.GetSiblingIndex();
            int sib2 = _image2.transform.GetSiblingIndex();
            _foreImage = sib1 > sib2 ? _image1 : _image2;
            _backImage = sib1 > sib2 ? _image2 : _image1;

            _defaultPosition = _transform.position;
        }

        public void SetImage(CharacterType character)
        {
            _foreImage.sprite = _sprites.TryGetValue(character, out Sprite sprite) ? sprite : null;
            _foreImage.color = new Color(1, 1, 1, 0);
        }

        public async UniTask SetOnFrontColorAsync(CancellationToken token)
        {
            _foreImage.color = Color.white;
            await UniTask.Yield(token);
        }

        public async UniTask SetOnBackColorAsync(CancellationToken token)
        {
            _foreImage.color = OnBackColor;
            await UniTask.Yield(token);
        }

        public async UniTask FadeInAsync(float duration, CancellationToken token)
        {
            if (_foreImage == null) return;
            await _foreImage.FadeInAsync(duration, token);
        }

        public async UniTask FadeInAsync(CharacterType character, float duration, CancellationToken token)
        {
            SetImage(character);
            await FadeInAsync(duration, token);
        }

        public async UniTask FadeOutAsync(float duration, CancellationToken token)
        {
            if (_foreImage == null) return;
            await _foreImage.FadeOutAsync(duration, token);
        }

        public async UniTask MoveRelativeAsync(float duration, Vector2 to, CancellationToken token)
        {
            Vector3 skip = _transform.position + (Vector3)to;
            await _transform.MoveRelativeAsync(to, duration, skip, token);
        }

        public async UniTask JumpAsync(float duration, int count, CancellationToken token)
        {
            Vector3 skip = _transform.position;
            for (int i = 0; i < count; i++)
            {
                await _transform.MoveRelativeAsync(Vector2.up * JumpHeight, duration, skip, token);
                await _transform.MoveRelativeAsync(Vector2.down * JumpHeight, duration, skip, token);
            }
        }
    }
}
