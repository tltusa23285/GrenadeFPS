using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CircularBuffer<T>
    {
        private T[] Buffer;
        private int BufferSize;

        public CircularBuffer(int bufferSize)
        {
            this.BufferSize = bufferSize;
            Buffer = new T[bufferSize];
        }

        public void Add(T item, int i) => Buffer[i % BufferSize] = item;
        public T Get(int index) => Buffer[index % BufferSize];
        public void Clear() => Buffer = new T[BufferSize];
    }
}
