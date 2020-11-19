using OpenTK;

using PhysicsSim.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.ComponentModel
{
    public interface INotifyRenderPropertyChanged
    {
        Vector3 Position { get; }
        Vector3 Rotation { get; }
        Vector3 Scale { get; }

        event EventHandler<RenderChangedEventArgs> PositionChangedEvent;

        event EventHandler<RenderChangedEventArgs> RotationChangedEvent;

        event EventHandler<RenderChangedEventArgs> ScaleChangedEvent;
    }
}
