using OpenTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Events
{
    public class RenderChangedEventArgs : EventArgs
    {
        public Vector3 OldValue { get; private set; }
        public Vector3 NewValue { get; private set; }

        public RenderChangedEventArgs(Vector3 oldVal, Vector3 newVal)
            : base()
        {
            OldValue = oldVal;
            NewValue = newVal;
        }
    }
}
