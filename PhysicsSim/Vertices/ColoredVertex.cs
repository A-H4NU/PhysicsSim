using OpenTK;
using OpenTK.Graphics;

namespace PhysicsSim.Vertices
{
    public struct ColoredVertex
    {
        public const int SIZE = 32;

        private readonly Vector4 _position;
        private readonly Color4 _color;

        public ColoredVertex(Vector4 position, Color4 color)
        {
            _position = position;
            _color = color;
        }
    }
}
