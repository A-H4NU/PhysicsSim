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

        protected Color4 _fillColor;
        public Color4 FillColor
        {
            get => _fillColor;
            set
            {
                if (_fillColor != value)
                {
                    _fillColor = value;
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

        public ARectangularInteraction(float width, float height, float lineWidth, Color4 fillColor, Color4 lineColor, int program)
        {
            _width = width;
            _height = height;
            _lineWidth = lineWidth;
            _fillColor = fillColor;
            _lineColor = lineColor;
            _program = program;
        }

        public ARectangularInteraction(float width, float height, Color4 fillColor, int program)
        {
            _width = width;
            _height = height;
            _lineWidth = DefaultLineWidth;
            _fillColor = fillColor;
            _lineColor = null;
            _program = program;
        }

        public ARectangularInteraction(RectangleF rec, float lineWidth, Color4 fillColor, Color4 lineColor, int program)
            : this(rec.Width, rec.Height, lineWidth, fillColor, lineColor, program)
        {
            Position = new Vector3(rec.X + rec.Width / 2f, rec.Y + rec.Height / 2f, 0f);
        }

        public ARectangularInteraction(RectangleF rec, Color4 fillColor, int program)
            : this(rec.Width, rec.Height, fillColor, program)
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
            RenderObject[] renders = new RenderObject[_lineColor == null ? 1 : 2];
            renders[0] = new RenderObject(ObjectFactory.Rectangle(_width, _height, _fillColor), _program);
            if (_lineColor != null)
            {
                renders[1] = new RenderObject(
                    ObjectFactory.RectangleEdge(_width, _height, _lineWidth, _lineColor.Value, ObjectFactory.BorderType.Middle),
                    _program);
            }
            _render = new ROCollection(renders)
            {
                Position = new Vector3(Position.X, Position.Y, 0)
            };
        }
    }
}
