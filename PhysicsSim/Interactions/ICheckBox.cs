
using System;

namespace PhysicsSim.Interactions
{
    public interface ICheckBox : IArea
    {
        event EventHandler CheckBoxToggleEvent;

        /// <summary>
        /// Whether or not the checkbox is marked "checked"
        /// </summary>
        bool IsChecked { get; }

        /// <summary>
        /// Press the button if coord is on the checkbox
        /// </summary>
        /// <returns>If successed to toggle</returns>
        bool ToggleIfInside(System.Numerics.Vector2 coord);

        /// <summary>
        /// Toggle the checkbox forcibly
        /// </summary>
        void Toggle();
    }
}
