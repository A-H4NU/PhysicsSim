using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Interactions
{
    /// <summary>Abstract class for buttons</summary>
    public abstract class AButton : ARenderable
    {
        /// <summary>True if and only if the coord is inside the button area</summary>
        public abstract bool IsInside(System.Numerics.Vector2 coord);
    }
}
