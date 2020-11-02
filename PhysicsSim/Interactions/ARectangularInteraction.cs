using OpenTK;
using OpenTK.Graphics;

using PhysicsSim.VBOs;

using System;
using System.Drawing;

namespace PhysicsSim.Interactions
{
    public abstract class ARectangularInteraction : ARenderable
    {
        public static float DefaultLineWidth = 5f;

        protected ROCollection _render;

        protected bool _disposed;

        protected int _program;

        protected float _width;
        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                LoadObject();
            }
        }

        protected float _height;
        public float Height
        {
            get => _height;
            set
            {
                _height = value;
                LoadObject();
            }
        }

        protected float _lineWidth;
        public float Line
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                LoadObject();
            }
        }

        protected Color4? _lineColor;
        public Color4? LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                LoadObject();
            }
        }

        protected Color4 _fillColor;
        public Color4 FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                LoadObject();
            }
        }

        public RectangleF Area => new RectangleF(Position.X - _width / 2f, Position.Y - _height / 2f, _width, _height);

        public ARectangularInteraction(float x, float y, float width, float height, float lineWidth, Color4 fillColor, Color4 lineColor, int program)
        {
            Position = new Vector3(x, y, 0);
            _width = width;
            _height = height;
            _lineWidth = lineWidth;
            _fillColor = fillColor;
            _lineColor = lineColor;
            _program = program;
        }

        public ARectangularInteraction(float x, float y, float width, float height, Color4 fillColor, int program)
        {
            Position = new Vector3(x, y, 0);
            _width = width;
            _height = height;
            _lineWidth = DefaultLineWidth;
            _fillColor = fillColor;
            _lineColor = null;
            _program = program;
        }

        public ARectangularInteraction(RectangleF rectangle, float lineWidth, Color4 fillColor, Color4 lineColor, int program)
            : this(rectangle.X - rectangle.Width / 2f, rectangle.Y - rectangle.Height / 2f, rectangle.Width, rectangle.Height, lineWidth, fillColor, lineColor, program) { }

        public ARectangularInteraction(RectangleF rectangle, Color4 fillColor, int program)
            : this(rectangle.X - rectangle.Width / 2f, rectangle.Y - rectangle.Height / 2f, rectangle.Width, rectangle.Height, fillColor, program) { }

        protected virtual void LoadObject()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("_render");
            }
            _render?.Dispose();
            RenderObject[] renders = new RenderObject[_lineColor == null ? 1 : 2];
            renders[0] = new RenderObject(ObjectFactory.Rectangle(_width, _height, _fillColor), _program);
            if (_lineColor != null)
            {
                renders[1] = new RenderObject(ObjectFactory.RectangleEdge(_width, _height, _lineWidth, _lineColor.Value), _program);
            }
            _render = new ROCollection(renders)
            {
                Position = new Vector3(Position.X, Position.Y, 0)
            };
        }
    }
}
