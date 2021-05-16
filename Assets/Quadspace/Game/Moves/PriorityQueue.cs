using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Quadspace.Game.Moves {
    public class PriorityQueue<T> where T : struct, IComparable<T> {
        private readonly List<T> list;
        private readonly bool greater;

        public PriorityQueue(bool greater, int initLength) {
            list = new List<T>(initLength);
            this.greater = greater;
        }

        public PriorityQueue(bool greater) : this(greater, 0) { }

        public void Enqueue(T value) {
            list.Add(value);
            var i = list.Count - 1;
            while (i != 0) {
                var parent = (i - 1) / 2;
                if (Compare(list[i], list[parent]) > 0) {
                    Swap(i, parent);
                    i = parent;
                } else break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int i1, int i2) {
            var tmp = list[i1];
            list[i1] = list[i2];
            list[i2] = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Compare(T a, T b) {
            var c = a.CompareTo(b);
            if (!greater) {
                c = -c;
            }

            return c;
        }

        public bool TryDequeue(out T value) {
            if (list.Count == 0) {
                value = default;
                return false;
            }

            value = list[0];
            Swap(0, list.Count-1);
            list.RemoveAt(list.Count-1);
            var parent = 0;
            while (true) {
                var child = 2 * parent + 1;
                if (child > list.Count - 1) break;
                if (child < list.Count - 1 && Compare(list[child], list[child + 1]) < 0) {
                    child += 1;
                }

                if (Compare(list[parent], list[child]) < 0) {
                    Swap(parent, child);
                    parent = child;
                } else break;
            }

            return true;
        }

        public T Dequeue() {
            if (!TryDequeue(out var value)) throw new Exception();
            return value;
        }

        public T Peek() {
            return list[0];
        }

        public void Clear() {
            list.Clear();
        }
    }
}