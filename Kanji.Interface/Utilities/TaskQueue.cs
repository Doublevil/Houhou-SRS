using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Utilities
{
    /// <summary>
    /// This component is designed to queue actions and execute them automatically in their queue order, one at a time.
    /// </summary>
    class TaskQueue
    {
        #region Fields

        Object _lock = new object();
        Queue<Action> _waiting;
        bool _isBusy;

        #endregion

        #region Properties



        #endregion

        #region Constructors

        public TaskQueue()
        {
            _waiting = new Queue<Action>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the given action to the queue.
        /// The action will be automatically launched when possible.
        /// </summary>
        /// <param name="action">Action to add to the queue.</param>
        public void Enqueue(Action action)
        {
            lock (_lock)
            {
                _waiting.Enqueue(action);
                if (!_isBusy)
                {
                    RunNext();
                }
            }
        }

        /// <summary>
        /// Runs the next task if any.
        /// Sets the busy state to the appropriate value.
        /// Should be enclosed in a lock.
        /// </summary>
        protected void RunNext()
        {
            if (_waiting.Any())
            {
                // Set busy and fire the next task.
                _isBusy = true;
                Task.Run(() =>
                {
                    _waiting.Dequeue().Invoke();
                    ActionCompleted();
                });
            }
            else
            {
                _isBusy = false;
            }
        }

        /// <summary>
        /// Method called after an action is completed.
        /// </summary>
        protected void ActionCompleted()
        {
            lock (_lock)
            {
                RunNext();
            }
        }

        #endregion
    }
}
