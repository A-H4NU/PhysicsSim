using OpenTK;
using OpenTK.Graphics;

namespace PhysicsSim.Vertices
{
    public struct ColoredVertex
    {
        public const int SIZE = 32;

        public readonly Vector4 Position;
        public readonly Color4 Color;

        public ColoredVertex(Vector4 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }
}
