using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Interactions
{
    public abstract class ATexturedInteraction : ARenderable
    {
        public static float DefaultLineWidth = 5f;

        protected bool _disposed;

        protected ROCollection _render;

        protected int _colorProgram, _textureProgram;

        protected float _width;
        public float Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    LoadObject();
                }
            }
        }

        protected float _height;
        public float Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    LoadObject();
                }
            }
        }

        protected float _lineWidth;
        public float LineWidth
        {
            get => _lineWidth;
            set
            {
                if (_lineWidth != value)
                {
                    _lineWidth = value;
                    LoadObject();
                }
            }
        }

        private readonly string _filePath;

        protected Color4? _lineColor;
        public Color4? LineColor
        {
            get => _lineColor;
            set
            {
                if (_lineColor != value)
                {
                    _lineColor = value;
                    LoadObject();
                }
            }
        }
        public RectangleF Area
        {
            get => new RectangleF(Position.X - _width / 2f, Position.Y - _height / 2f, _width, _height);
            set
            {
                if (Area.Location != value.Location)
                {
                    Position = new Vector3(value.X + 0.5f * value.Width, value.Y + 0.5f * value.Height, 0);
                    _render.Position = Position;
                }
                if (Area.Size != value.Size)
                {
                    _width = value.Width;
                    _height = value.Height;
                    LoadObject();
                }
            }
        }

        public ATexturedInteraction(float width, float height, float lineWidth, Color4 lineColor, string filepath, int colorProgram, int textureProgram)
            : this(width, height, filepath, colorProgram, textureProgram)
        {
            _lineWidth = lineWidth;
            _lineColor = lineColor;
        }

        public ATexturedInteraction(float width, float height, string filepath, int colorProgram, int textureProgram)
        {
            _width = width;
            _height = height;
            _filePath = filepath;
            _colorProgram = colorProgram;
            _textureProgram = textureProgram;
            _lineWidth = DefaultLineWidth;
            _lineColor = null;
        }

        public ATexturedInteraction(RectangleF rec, float lineWidth, Color4 lineColor, string filePath, int colorProgram, int textureProgram)
            : this(rec.Width, rec.Height, lineWidth, lineColor, filePath, colorProgram, textureProgram)
        {
            Position = new Vector3(rec.X + rec.Width / 2f, rec.Y + rec.Height / 2f, 0f);
        }

        public ATexturedInteraction(RectangleF rec, string filePath, int colorProgram, int textureProgram)
            : this(rec.Width, rec.Height, filePath, colorProgram, textureProgram)
        {
            Position = new Vector3(rec.X + rec.Width / 2f, rec.Y + rec.Height / 2f, 0f);
        }

        protected virtual void LoadObject()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("_render");
            }
            _render?.Dispose();
            List<ARenderable> renders = new List<ARenderable>()
            {
                new TexturedRenderObject(
                    ObjectFactory.TexRectangle(_width, _height),
                    _filePath,
                    _textureProgram)
            };
            if (_lineColor != null)
            {
                renders.Add(new RenderObject(
                    ObjectFactory.RectangleEdge(_width, _height, _lineWidth, _lineColor.Value),
                    _colorProgram));
            }
            _render = new ROCollection(renders)
            {
                Position = new Vector3(Position.X, Position.Y, 0)
            };
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _render.Dispose();
                _render = null;
            }
        }
    }
}