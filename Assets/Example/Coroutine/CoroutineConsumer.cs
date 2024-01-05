using System.Threading;
using UnityEngine;

public class CoroutineConsumer : MonoBehaviour
{
    [SerializeField]
    private CoroutineProvider _provider;

    private CancellationTokenSource _ctsA = new();
    private CancellationTokenSource _ctsB = new();

    void Start()
    {
        _provider.Run("èàóù1", _ctsA.Token);
        _provider.Run("èàóù2", _ctsB.Token);
    }

    private void OnDestroy()
    {
        _ctsA.Dispose();
        _ctsB.Dispose();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _ctsA.Cancel();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            _ctsB.Cancel();
        }
    }
}