using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Utilities
{
    class FixedSizeStack<T>
    {
        #region Fields

        private List<T> _list;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximal stack size.
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// Gets the current stack count.
        /// </summary>
        public int Count { get { return _list.Count; } }

        #endregion

        #region Constructor

        public FixedSizeStack(int maxSize)
        {
            MaxSize = maxSize;
            _list = new List<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Pushes the given item on the stack.
        /// If the maximal size is attained, removes
        /// the excedent from the bottom of the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            lock (this)
            {
                _list.Add(item);

                while (Count > MaxSize && Count > 0)
                {
                    T overflow = _list.FirstOrDefault();
                    if (overflow != null)
                    {
                        if (overflow is IDisposable)
                        {
                            ((IDisposable)overflow).Dispose();
                        }

                        _list.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the last item from the stack and returns it.
        /// </summary>
        public T Pop()
        {
            lock (this)
            {
                if (Count > 0)
                {
                    T result = _list.LastOrDefault();
                    _list.RemoveAt(_list.Count - 1);
                    return result;
                }
                else
                {
                    throw new InvalidOperationException("The stack is empty.");
                }
            }
        }

        #endregion
    }
}
