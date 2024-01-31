using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tile = PSB.Game.WFC2.Cell.Tile;
using PSB.Game.WFC2; // WFC�A���S���Y������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���

namespace PSB.Game
{
    public class TileBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public Tile Tile;
            public GameObject Prefab;
        }

        [SerializeField] Data[] _data;
        [Header("1�^�C��������̑傫��")]
        [SerializeField] float _tileSize = 1;

        Dictionary<Tile, GameObject> _tiles = new();
        List<GameObject> _tileObjects = new();
        Transform _parent;

        void Awake()
        {
            _tiles = _data.ToDictionary(v => v.Tile, v => v.Prefab);
            _parent = new GameObject("Dungeon").transform;
        }

        public void Draw(Cell[,] map)
        {
            DestroyTileObjectsAll();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int k = 0; k < map.GetLength(1); k++)
                {
                    CheckCell(map[i, k], i, k);
                }
            }
        }

        void CheckCell(Cell cell, int indexY, int indexX)
        {
            // ����ς݂��A�O�̂��ߑI���\�ȃ^�C���̒�����1���𒲂ׂ�
            if (cell.IsCollapsed && cell.SelectableTiles.Length == 1)
            {
                Vector3 pos = new Vector3(indexY * _tileSize, 0, indexX * _tileSize);
                GameObject tile = CreateTile(cell.SelectedTile, pos);
                _tileObjects.Add(tile);
            }
        }

        GameObject CreateTile(Tile tile, Vector3 pos)
        {
            if (_tiles.TryGetValue(tile, out GameObject prefab))
            {
                // ��]�̓v���n�u�Ɠ����l���g�p����
                return Instantiate(prefab, pos, prefab.transform.rotation, _parent);
            }
            else
            {
                throw new KeyNotFoundException("�Y�����ɑΉ�����^�C�����������ɖ���: " + tile);
            }
        }

        void DestroyTileObjectsAll()
        {
            for (int i = _tileObjects.Count - 1; i >= 0; i--)
            {
                Destroy(_tileObjects[i]);
            }

            _tileObjects.Clear();
        }
    }
}
