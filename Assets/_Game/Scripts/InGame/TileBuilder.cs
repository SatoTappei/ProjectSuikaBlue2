using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tile = PSB.Game.WFC2.Cell.Tile;
using PSB.Game.WFC2; // WFCアルゴリズム周りは専用の名前空間にまとめてある

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
        [Header("1タイル当たりの大きさ")]
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
            // 崩壊済みかつ、念のため選択可能なタイルの長さが1かを調べる
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
                // 回転はプレハブと同じ値を使用する
                return Instantiate(prefab, pos, prefab.transform.rotation, _parent);
            }
            else
            {
                throw new KeyNotFoundException("添え字に対応するタイルが辞書内に無い: " + tile);
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
