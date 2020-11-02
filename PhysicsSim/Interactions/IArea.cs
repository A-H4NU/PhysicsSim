using System.Numerics;

namespace PhysicsSim.Interactions
{
    public interface IArea
    {
        /// <summary>
        /// Whether or not coord is inside the area
        /// </summary>
        bool IsInside(Vector2 coord);
    }
}
