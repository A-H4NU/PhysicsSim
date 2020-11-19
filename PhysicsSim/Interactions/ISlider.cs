using PhysicsSim.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Interactions
{
    public interface ISlider : IArea
    {
        bool Selected { get; }

        float Value { get; set; }

        float MaxValue { get; }

        float MinValue { get; }

        bool SelectIfInside(Vector2 pos);

        void Unselect();

        bool SlideIfSelected(Vector2 pos);

        event EventHandler<ValueChangedEventArgs<float>> ValueChangedEvent;
    }
}
