using System;
using System.Collections;
using System.Collections.Generic;

namespace MacroTalk
{
    public class FixedStack<T>(int capacity)
    {
        Stack<T> _stack = new(capacity);

        int _capacity = capacity;

        public void Push (T item)
        {
            if (_stack.Count == _capacity)
            {
                Stack<T> temp = new(_capacity);
                temp.Push(item);
                for (int i = 0; i < _stack.Count - 1; i++)
                {
                    temp.Push(_stack.Pop());
                }
                for (int i = 0; i < temp.Count; i++)
                {
                    _stack.Push(temp.Pop());
                }
                return;
            }
            _stack.Push(item);
        }

        public T Pop()
        {
            if (_stack.Count == 0)
                return default!;
            return _stack.Pop();
        }

        public void Clear()
        {
            _stack.Clear();
        }

        public int Count => _stack.Count;
    }
}