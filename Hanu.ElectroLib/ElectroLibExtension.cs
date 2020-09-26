using System.Numerics;

namespace Hanu.ElectroLib
{
    public static class ElectroLibExtension
    {
        public static void Normalize(this ref Vector2 vector) => vector /= vector.Length();

        public static Vector2 Normalized(this Vector2 vector) => vector / vector.Length();
    }
}