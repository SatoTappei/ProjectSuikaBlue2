using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Token
{
    public bool Cancel;
}

public class Sample : MonoBehaviour
{
    [SerializeField] 
    int _count = 5;

    [SerializeField]
    private float _duration = 1F; // �A�j���[�V�������ԁi�b�P�ʁj

    [SerializeField]
    private Color _from = Color.clear; // ���̐F����t�F�[�h��������

    [SerializeField]
    private Color _to = Color.white; // ���̐F�Ɍ������ăt�F�[�h��������

    IEnumerator Start()
    {
        Image[] images = new Image[_count];
        for (int i = 0; i < _count; i++)
        {
            // Image �R���|�[�l���g�����I�u�W�F�N�g�𐶐�
            var obj = new GameObject("Image");
            obj.transform.parent = transform;
            var image = obj.AddComponent<Image>();
            image.color = _from;

            images[i] = image;
        }

        foreach (Image image in images)
        {
            // �������ςȂ�(Fire and Forget)
            // �������o�ɕێ������ϐ��ł������Ƃ��ēn���Ă��ƁA���\�b�h���ł͈����ȊO�̒l�����炷���Ƃ��o����B
            var c1 = StartCoroutine(FadeAsync(image, _duration));
            var c2 = StartCoroutine(ShakeAsync(image.transform, _duration));

            // StartCoroutine ���Ԃ��� Coroutine �I�u�W�F�N�g��ҋ@�ł���
            // �����̏I����҂������Ȃ�A���ꂼ�� yield return ����΂悢
            yield return c1;
            yield return c2;
        }
    }

    IEnumerator FadeAsync(Image image, float duration)
    {
        // ��while��for�ɕύX�����̂Ŋȗ�������Ă���B
        for (float p = 0; p < 1; p += Time.deltaTime / duration)
        {
            image.color = Color.Lerp(_from, _to, p);
            yield return null;
        }

        image.color = Color.Lerp(_from, _to, 1);
    }

    IEnumerator AnimationAsync(Transform transform, float duration)
    {
        Vector3 from = transform.eulerAngles;
        Vector3 left = new Vector3(0, 0, 45);
        Vector3 right = new Vector3(0, 0, -45);

        yield return RotationAsync(transform, from, left, duration);
        yield return RotationAsync(transform, left, right, duration);
        yield return RotationAsync(transform, right, from, duration);
    }

    IEnumerator RotationAsync(Transform transform, Vector3 a, Vector3 b, float duration)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            transform.eulerAngles = Vector3.Lerp(a, b, progress);
            yield return null;
        }
    }

    // �ԍ�u�t��
    IEnumerator ShakeAsync(Transform target, float duration)
    {
        for (var t = 0F; t < duration; t += Time.deltaTime)
        {
            var p = t / duration;
            var r = 20 * Mathf.Sin(Mathf.PI * 2 * p);
            target.eulerAngles = new Vector3(0, 0, r);
            yield return null;
        }
    }

    // �ԍ�u�t��(���񂵂��R���[�`���̎��s)
    IEnumerator WhenAll(params IEnumerator[] enumerators)
    {
        // �R���[�`����S�ċN��
        var awaiters = new Coroutine[enumerators.Length];
        for (var i = 0; i < awaiters.Length; i++)
        {
            var e = enumerators[i];
            awaiters[i] = StartCoroutine(e);
        }

        // �N�������R���[�`����S�đ҂�
        foreach (var awaiter in awaiters)
        {
            yield return awaiter;
        }
    }
}