using System;
using System.Collections;

namespace Hanu.ElectroLib.ComponentModel
{
    public enum NotifyDeepCollectionChangedAction
    {
        Add = 0,
        Remove = 1,
        Replace = 2,
        Move = 3,
        Reset = 4,
        Modified = 5
    }

    public class NotifyDeepCollectionChangedEventArgs : EventArgs
    {
        public NotifyDeepCollectionChangedAction Action { get; private set; }

        public IList NewItems { get; private set; }

        public int NewStartingIndex { get; private set; } = -1;

        public IList OldItems { get; private set; }

        public int OldStartingIndex { get; private set; } = -1;

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Reset"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action)
        {
            if (action != NotifyDeepCollectionChangedAction.Reset)
            {
                throw new ArgumentException("action");
            }
            InitializeAdd(action, null, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Add"/>, <see cref="NotifyDeepCollectionChangedAction.Remove"/>, <see cref="NotifyDeepCollectionChangedAction.Reset"/>, and <see cref="NotifyDeepCollectionChangedAction.Modified"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, object changedItem)
        {
            if (action != NotifyDeepCollectionChangedAction.Add && action != NotifyDeepCollectionChangedAction.Remove && action != NotifyDeepCollectionChangedAction.Reset && action != NotifyDeepCollectionChangedAction.Modified)
            {
                throw new ArgumentException("action");
            }
            if (action != NotifyDeepCollectionChangedAction.Reset)
            {
                InitializeAddOrRemove(action, new object[] { changedItem }, -1);
                return;
            }
            if (changedItem != null)
            {
                throw new ArgumentException("action");
            }
            InitializeAdd(action, null, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Add"/>, <see cref="NotifyDeepCollectionChangedAction.Remove"/>, <see cref="NotifyDeepCollectionChangedAction.Reset"/>, and <see cref="NotifyDeepCollectionChangedAction.Modified"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, object changedItem, int index)
        {
            if (action != NotifyDeepCollectionChangedAction.Add && action != NotifyDeepCollectionChangedAction.Remove && action != NotifyDeepCollectionChangedAction.Reset && action != NotifyDeepCollectionChangedAction.Modified)
            {
                throw new ArgumentException("action");
            }
            if (action != NotifyDeepCollectionChangedAction.Reset)
            {
                InitializeAddOrRemove(action, new object[] { changedItem }, index);
                return;
            }
            if (changedItem != null)
            {
                throw new ArgumentException("action");
            }
            if (index != -1)
            {
                throw new ArgumentException("action");
            }
            InitializeAdd(action, null, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Add"/>, <see cref="NotifyDeepCollectionChangedAction.Remove"/>, <see cref="NotifyDeepCollectionChangedAction.Reset"/>, and <see cref="NotifyDeepCollectionChangedAction.Modified"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, IList changedItems)
        {
            if (action != NotifyDeepCollectionChangedAction.Add && action != NotifyDeepCollectionChangedAction.Remove && action != NotifyDeepCollectionChangedAction.Reset && action != NotifyDeepCollectionChangedAction.Modified)
            {
                throw new ArgumentException("action");
            }
            if (action != NotifyDeepCollectionChangedAction.Reset)
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                InitializeAddOrRemove(action, changedItems, -1);
                return;
            }
            if (changedItems != null)
            {
                throw new ArgumentException("action");
            }
            InitializeAdd(action, null, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Add"/>, <see cref="NotifyDeepCollectionChangedAction.Remove"/>, <see cref="NotifyDeepCollectionChangedAction.Reset"/>, and <see cref="NotifyDeepCollectionChangedAction.Modified"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            if (action != NotifyDeepCollectionChangedAction.Add && action != NotifyDeepCollectionChangedAction.Remove && action != NotifyDeepCollectionChangedAction.Reset && action != NotifyDeepCollectionChangedAction.Modified)
            {
                throw new ArgumentException("action");
            }
            if (action != NotifyDeepCollectionChangedAction.Reset)
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                if (startingIndex < -1)
                {
                    throw new ArgumentException("startingIndex");
                }
                InitializeAddOrRemove(action, changedItems, startingIndex);
                return;
            }
            if (changedItems != null)
            {
                throw new ArgumentException("action");
            }
            if (startingIndex != -1)
            {
                throw new ArgumentException("action");
            }
            InitializeAdd(action, null, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Replace"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, object newItem, object oldItem)
        {
            if (action != NotifyDeepCollectionChangedAction.Replace)
            {
                throw new ArgumentException("action");
            }
            object[] objArray = new object[] { newItem };
            object[] objArray1 = new object[] { oldItem };
            InitializeMoveOrReplace(action, objArray, objArray1, -1, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Replace"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            if (action != NotifyDeepCollectionChangedAction.Replace)
            {
                throw new ArgumentException("action");
            }
            int num = index;
            object[] objArray = new object[] { newItem };
            object[] objArray1 = new object[] { oldItem };
            InitializeMoveOrReplace(action, objArray, objArray1, index, num);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Replace"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, IList newItems, IList oldItems)
        {
            if (action != NotifyDeepCollectionChangedAction.Replace)
            {
                throw new ArgumentException("action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Replace"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            if (action != NotifyDeepCollectionChangedAction.Replace)
            {
                throw new ArgumentException("action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Move"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, object changedItem, int index, int oldIndex)
        {
            if (action != NotifyDeepCollectionChangedAction.Move)
            {
                throw new ArgumentException("action");
            }
            if (index < 0)
            {
                throw new ArgumentException("index");
            }
            object[] objArray = new object[] { changedItem };
            InitializeMoveOrReplace(action, objArray, objArray, index, oldIndex);
        }

        /// <summary> Only for <see cref="NotifyDeepCollectionChangedAction.Move"/> </summary>
        public NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            if (action != NotifyDeepCollectionChangedAction.Move)
            {
                throw new ArgumentException("action");
            }
            if (index < 0)
            {
                throw new ArgumentException("index");
            }
            InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        internal NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction action, IList newItems, IList oldItems, int newIndex, int oldIndex)
        {
            IList lists;
            IList lists1;
            Action = action;
            if (newItems == null)
            {
                lists = null;
            }
            else
            {
                lists = ArrayList.ReadOnly(newItems);
            }
            NewItems = lists;
            if (oldItems == null)
            {
                lists1 = null;
            }
            else
            {
                lists1 = ArrayList.ReadOnly(oldItems);
            }
            OldItems = lists1;
            NewStartingIndex = newIndex;
            OldStartingIndex = oldIndex;
        }

        private void InitializeAdd(NotifyDeepCollectionChangedAction action, IList newItems, int newStartingIndex)
        {
            IList lists;
            Action = action;
            if (newItems == null)
            {
                lists = null;
            }
            else
            {
                lists = ArrayList.ReadOnly(newItems);
            }
            NewItems = lists;
            NewStartingIndex = newStartingIndex;
        }

        private void InitializeAddOrRemove(NotifyDeepCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            if (action == NotifyDeepCollectionChangedAction.Add)
            {
                InitializeAdd(action, changedItems, startingIndex);
                return;
            }
            if (action == NotifyDeepCollectionChangedAction.Remove)
            {
                InitializeRemove(action, changedItems, startingIndex);
            }
        }

        private void InitializeMoveOrReplace(NotifyDeepCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
        {
            InitializeAdd(action, newItems, startingIndex);
            InitializeRemove(action, oldItems, oldStartingIndex);
        }

        private void InitializeRemove(NotifyDeepCollectionChangedAction action, IList oldItems, int oldStartingIndex)
        {
            IList lists;
            Action = action;
            if (oldItems == null)
            {
                lists = null;
            }
            else
            {
                lists = ArrayList.ReadOnly(oldItems);
            }
            OldItems = lists;
            OldStartingIndex = oldStartingIndex;
        }
    }
}