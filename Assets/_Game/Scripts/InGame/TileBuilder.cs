using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Tile = PSB.Game.WFC.Cell.Tile;
using PSB.Game.WFC; // WFCアルゴリズム周りは専用の名前空間にまとめてある

namespace PSB.Game
{
    public class TileBuilder : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
#if UNITY_EDITOR
            [SerializeField] string _name; // インスペクタが見やすくなるだけ
#endif
            [Header("x軸とz軸を正とした角度0のPrefab")]
            public GameObject Prefab;
            [Header("角度毎にPrefabを変える場合")]
            public GameObject RightPrefab;
            public GameObject DownPrefab;
            public GameObject LeftPrefab;
            [Header("角度毎の列挙型の値")]
            public Tile Up;
            public Tile Right;
            public Tile Down;
            public Tile Left;
        }

        [SerializeField] Data[] _data;
        [Header("1タイル当たりの大きさ")]
        [SerializeField] float _tileSize = 1;

        Dictionary<Tile, Data> _tiles = new();
        List<GameObject> _tileObjects = new();
        Transform _parent;

        void Awake()
        {
            // タイルがどの角度のものと一致するかで角度が変わるのでクラス自体を値にする
            for (int i = 0; i < _data.Length; i++)
            {
                _tiles.TryAdd(_data[i].Up, _data[i]);
                _tiles.TryAdd(_data[i].Right, _data[i]);
                _tiles.TryAdd(_data[i].Down, _data[i]);
                _tiles.TryAdd(_data[i].Left, _data[i]);
            }

            // 生成したオブジェクトを登録しておく親
            _parent = new GameObject("Dungeon").transform;
        }

        /// <summary>
        /// 各セルの情報からオブジェクトを生成
        /// </summary>
        public void Draw(Cell[,] map)
        {
            DestroyTileObjectsAll();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int k = 0; k < map.GetLength(1); k++)
                {
                    CreateTile(map[i, k], i, k);
                }
            }
        }

        // タイルを生成
        void CreateTile(Cell cell, int y, int x)
        {
            // 崩壊済みかつ、念のため選択可能なタイルの長さが1かを調べる
            if (!cell.IsCollapsed || cell.SelectableTiles.Length > 1) return;
            
            if (_tiles.TryGetValue(cell.SelectedTile, out Data data))
            {
                (GameObject prefab, float angle) args = PrefabAndAngle(cell.SelectedTile, data);
                Vector3 pos = new Vector3(y * _tileSize, 0, x * _tileSize);
                GameObject g = Instantiate(args.prefab, pos, Quaternion.Euler(0, args.angle, 0), _parent);

                _tileObjects.Add(g);
            }
            else
            {
                throw new KeyNotFoundException("添え字に対応するタイルが辞書内に無い: " + cell.SelectedTile);
            }
        }

        // 生成するタイルのPrefabと角度
        (GameObject, float) PrefabAndAngle(Tile tile, Data data)
        {
            // 角度用のPrefabがある場合はそれを使用
            // どの方向を向いているタイルかによって時計回りに90度刻みで回転させる
            if (tile == data.Up) return (data.Prefab, 0);
            if (tile == data.Right) return (data.RightPrefab == null ? data.Prefab : data.RightPrefab, 90.0f);
            if (tile == data.Down) return (data.DownPrefab == null ? data.Prefab : data.DownPrefab, 180.0f);
            if (tile == data.Left) return (data.LeftPrefab == null ? data.Prefab : data.LeftPrefab, 270.0f);

            throw new KeyNotFoundException("生成するタイルに登録されていない: " + tile);
        }

        // 全部削除
        // アルゴリズムが正しく動作しているかを確認する用途
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