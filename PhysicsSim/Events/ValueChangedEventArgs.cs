using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Events
{
    public class ValueChangedEventArgs<T> : EventArgs
        where T: struct
    {
        public T OldValue { get; private set; }

        public T NewValue { get; private set; }

        public string ValueName { get; private set; }

        public ValueChangedEventArgs(T oldVal, T newVal)
            : this(oldVal, newVal, String.Empty) { }

        public ValueChangedEventArgs(T oldVal, T newVal, string name)
            : base()
        {
            OldValue = oldVal;
            NewValue = newVal;
            ValueName = name;
        }
    }
}
