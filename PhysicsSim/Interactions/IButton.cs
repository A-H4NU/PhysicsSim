
using System;
using System.Numerics;

namespace PhysicsSim.Interactions
{
    /// <summary>Abstract class for buttons</summary>
    public interface IButton : IArea
    {
        event EventHandler ButtonPressEvent;

        /// <summary>
        /// Press the button if coord is on the button
        /// </summary>
        /// <returns>If successed to press</returns>
        bool PressIfInside(Vector2 coord);

        /// <summary>
        /// Press the button forcibly
        /// </summary>
        void Press();
    }
}
