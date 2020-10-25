using OpenTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Vertices
{
    public struct TexturedVertex
    {
        public const int SIZE = 24;

        private readonly Vector4 _position;
        private readonly Vector2 _textureCoord;

        public TexturedVertex(Vector4 position, Vector2 textureCoord)
        {
            _position = position;
            _textureCoord = textureCoord;
        }
    }
}
