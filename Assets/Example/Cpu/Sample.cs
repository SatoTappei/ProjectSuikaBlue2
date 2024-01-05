using System.Threading.Tasks;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private async void Start()
    {
        var cubes = await CubeCreateAsync();
        await RotateCubeAsync(cubes);
    }

    private async Task RotateCubeAsync(GameObject[] cubes)
    {
        while (Application.isPlaying)
        {
            foreach (var cube in cubes)
            {
                cube.transform.Rotate(0, 1, 0);
            }
            await Task.Yield(); // ���ꂪ Unity �X���b�h�Ȃ�1�t���[���҂͂�
        }
    }

    /// <summary>
    /// ���Ԃ������� Cube �𐶐��B
    /// </summary>
    /// <returns>�������� Cube �̔z��B</returns>
    private async Task<GameObject[]> CubeCreateAsync()
    {
        var result = new GameObject[5];
        for (var i = 0; i < result.Length; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new(-5 + i * 2, 0, 0);
            result[i] = cube;
            await Task.Delay(1000);
        }
        return result;
    }
}