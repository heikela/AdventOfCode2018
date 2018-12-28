using System;

namespace Common
{
    class CircularQueue<TElem>
    {
        private TElem[] arr;
        private int head;
        private int end;
        public int Count
        {
            get
            {
                return (head - end) % arr.Length;
            }
        }
        public CircularQueue(int size)
        {
            arr = new TElem[size];
            head = 0;
            end = 0;
        }
        public void SkipForward()
        {
            if (head != end)
            {
                arr[end] = arr[head];
                end = IncCursor(end);
                head = IncCursor(head);
            }
        }
        public void SkipBackward()
        {
            if (head != end)
            {
                end = DecCursor(end);
                head = DecCursor(head);
                arr[head] = arr[end];
            }
        }
        public void Enqueue(TElem item)
        {
            arr[end] = item;
            end = IncCursor(end);
            if (Count == 0)
            {
                throw new InvalidOperationException("Buffer out of space");
            }
        }
        public TElem Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("No item to dequeue");
            }
            end = DecCursor(end);
            return arr[end];
        }
        private int IncCursor(int cursor)
        {
            ++cursor;
            if (cursor == arr.Length)
            {
                cursor = 0;
            }
            return cursor;
        }
        private int DecCursor(int cursor)
        {
            --cursor;
            if (cursor < 0)
            {
                cursor = arr.Length - 1;
            }
            return cursor;
        }
    }
}
