using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Tile = PSB.Game.WFC.Cell.Tile;
using PSB.Game.WFC; // WFC�A���S���Y������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���

namespace PSB.Game
{
    public class TileBuilder : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
#if UNITY_EDITOR
            [SerializeField] string _name; // �C���X�y�N�^�����₷���Ȃ邾��
#endif
            [Header("x����z���𐳂Ƃ����p�x0��Prefab")]
            public GameObject Prefab;
            [Header("�p�x����Prefab��ς���ꍇ")]
            public GameObject RightPrefab;
            public GameObject DownPrefab;
            public GameObject LeftPrefab;
            [Header("�p�x���̗񋓌^�̒l")]
            public Tile Up;
            public Tile Right;
            public Tile Down;
            public Tile Left;
        }

        [SerializeField] Data[] _data;
        [Header("1�^�C��������̑傫��")]
        [SerializeField] float _tileSize = 1;

        Dictionary<Tile, Data> _tiles = new();
        List<GameObject> _tileObjects = new();
        Transform _parent;

        void Awake()
        {
            // �^�C�����ǂ̊p�x�̂��̂ƈ�v���邩�Ŋp�x���ς��̂ŃN���X���̂�l�ɂ���
            for (int i = 0; i < _data.Length; i++)
            {
                _tiles.TryAdd(_data[i].Up, _data[i]);
                _tiles.TryAdd(_data[i].Right, _data[i]);
                _tiles.TryAdd(_data[i].Down, _data[i]);
                _tiles.TryAdd(_data[i].Left, _data[i]);
            }

            // ���������I�u�W�F�N�g��o�^���Ă����e
            _parent = new GameObject("Dungeon").transform;
        }

        /// <summary>
        /// �e�Z���̏�񂩂�I�u�W�F�N�g�𐶐�
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

        // �^�C���𐶐�
        void CreateTile(Cell cell, int y, int x)
        {
            // ����ς݂��A�O�̂��ߑI���\�ȃ^�C���̒�����1���𒲂ׂ�
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
                throw new KeyNotFoundException("�Y�����ɑΉ�����^�C�����������ɖ���: " + cell.SelectedTile);
            }
        }

        // ��������^�C����Prefab�Ɗp�x
        (GameObject, float) PrefabAndAngle(Tile tile, Data data)
        {
            // �p�x�p��Prefab������ꍇ�͂�����g�p
            // �ǂ̕����������Ă���^�C�����ɂ���Ď��v����90�x���݂ŉ�]������
            if (tile == data.Up) return (data.Prefab, 0);
            if (tile == data.Right) return (data.RightPrefab == null ? data.Prefab : data.RightPrefab, 90.0f);
            if (tile == data.Down) return (data.DownPrefab == null ? data.Prefab : data.DownPrefab, 180.0f);
            if (tile == data.Left) return (data.LeftPrefab == null ? data.Prefab : data.LeftPrefab, 270.0f);

            throw new KeyNotFoundException("��������^�C���ɓo�^����Ă��Ȃ�: " + tile);
        }

        // �S���폜
        // �A���S���Y�������������삵�Ă��邩���m�F����p�r
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