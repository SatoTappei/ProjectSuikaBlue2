using System;

namespace PSB.Game
{
    /// <summary>
    /// BinaryHeap<T>クラスで値を管理するために必要なインターフェース
    /// </summary>
    public interface IBinaryHeapCollectable<T> : IComparable<T>
    {
        int BinaryHeapIndex { get; set; }
    }

    /// <summary>
    /// 二分ヒープを実装するクラス
    /// </summary>
    public class BinaryHeap<T> where T : IBinaryHeapCollectable<T>
    {
        T[] _values;

        public BinaryHeap(int size)
        {
            _values = new T[size];
        }

        public int Count { get; private set; }

        /// <summary>
        /// 追加時に適切な場所に移動させるので、値がその場所に存在すればヒープ内に存在する
        /// </summary>
        public bool Contains(T value) => Equals(_values[value.BinaryHeapIndex], value);

        /// <summary>
        /// 一番後ろに追加して適切な場所まで移動させる
        /// </summary>
        public void Push(T value)
        {
            value.BinaryHeapIndex = Count;
            _values[Count++] = value;
            SortUp(value);
        }

        /// <summary>
        /// 先頭を返し、一番後ろの値を先頭に持ってきた後、適切な場所まで移動させる
        /// </summary>
        public T Pop()
        {
            // 要素が無い時に取得しようとした場合は規定値を返す
            if (--Count < 0) return default;

            T value = _values[0];
            _values[0] = _values[Count];
            _values[0].BinaryHeapIndex = 0;
            SortDown(_values[0]);

            return value;
        }

        /// <summary>
        /// Pushした際に木の下から上に向けてソートしていく
        /// </summary>
        void SortUp(T value)
        {
            // インデックスが0の場合、そのままの計算結果だと-0.5だが、int型にすると0になる。
            int parentIndex = (value.BinaryHeapIndex - 1) / 2;

            // 比較して親と交換する
            T parent = _values[parentIndex];
            if (value.CompareTo(parent) < 0)
            {
                Swap(value, parent);
                SortUp(value);
            }
        }

        /// <summary>
        /// 引数の値をPop()した際に木の上から下に向けてソートしていく
        /// </summary>
        void SortDown(T value)
        {
            int leftChildIndex = value.BinaryHeapIndex * 2 + 1;
            int rightChildIndex = value.BinaryHeapIndex * 2 + 2;

            // 左の子ノードを指す添え字が木の要素数以上なら処理をしない
            if (leftChildIndex >= Count) return;

            int swapIndex = leftChildIndex;
            // 右の子が左の子より大きい場合はこちらと交換する
            if (rightChildIndex < Count && _values[leftChildIndex].CompareTo(_values[rightChildIndex]) > 0)
            {
                swapIndex = rightChildIndex;
            }

            // 子より大きければ交換する
            if (value.CompareTo(_values[swapIndex]) > 0)
            {
                Swap(value, _values[swapIndex]);
                SortDown(value);
            }
        }

        /// <summary>
        /// 現在の添え字の個所の値を交換した後、添え字を交換する
        /// </summary>
        void Swap(T value1, T value2)
        {
            _values[value1.BinaryHeapIndex] = value2;
            _values[value2.BinaryHeapIndex] = value1;

            int temp = value1.BinaryHeapIndex;
            value1.BinaryHeapIndex = value2.BinaryHeapIndex;
            value2.BinaryHeapIndex = temp;
        }
    }
}
