using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using PhysicsSim.VBOs;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Interactions
{
    public class RectangularButton : AButton
    {
        public static float DefaultEdgeWidth = 5f;

        private ROCollection _render;

        private bool _disposed;

        private int _program;

        private float _lineWidth;
        public float Line
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                LoadObject();
            }
        }

        private Color4 _lineColor;
        public Color4 LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                LoadObject();
            }
        }

        private Color4 _color;
        public Color4 Color
        {
            get => _color;
            set
            {
                _color = value;
                LoadObject();
            }
        }

        private float _width;
        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                LoadObject();
            }
        }

        private float _height;
        public float Height
        {
            get => _height;
            set
            {
                _height = value;
                LoadObject();
            }
        }

        public SizeF Size
        {
            get => new SizeF(_width, _height);
            set
            {
                _width = value.Width;
                _height = value.Height;
                LoadObject();
            }
        }

        public RectangleF Area
        {
            get => new RectangleF(_render.Position.X - Width / 2f, _render.Position.Y - Height / 2f, Width, Height);
        }

        public RectangularButton(float x, float y, float width, float height, Color4 color, Color4 lineColor, int program)
            : this(x, y, width, height, DefaultEdgeWidth, color, lineColor, program)
        {
        }

        public RectangularButton(float x, float y, float width, float height, float edgeWidth, Color4 color, Color4 lineColor, int program)
        {
            _disposed = false;
            _width = width;
            _height = height;
            _color = color;
            _lineColor = lineColor;
            _lineWidth = edgeWidth;
            _program = program;
            LoadObject();
        }

        public RectangularButton(RectangleF rectangle, Color4 color, Color4 lineColor,  int program)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, DefaultEdgeWidth, color, lineColor, program) { }

        public RectangularButton(RectangleF rectangle, float edgeWidth, Color4 color, Color4 lineColor, int program)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, edgeWidth, color, lineColor, program) { }

        public override bool IsInside(System.Numerics.Vector2 coord)
        {
            if (_disposed) throw new ObjectDisposedException("_render");
            PointF pt = new PointF(coord.X, coord.Y);
            return Area.Contains(pt);
        }

        public override void Render(ref Matrix4 projection, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            if (_disposed) throw new ObjectDisposedException("_render");
            _render.Render(ref projection, translation, rotation, scale);
        }

        private void LoadObject()
        {
            if (_disposed) throw new ObjectDisposedException("_render");
            _render?.Dispose();
            var renders = new RenderObject[]
            {
                new RenderObject(ObjectFactory.Rectangle(_width, _height, _color), _program),
                //new RenderObject(ObjectFactory.Rectangle(_width, _height, _lineColor), _program),
                new RenderObject(ObjectFactory.RectangleEdge(_width, _height, _lineWidth, _lineColor), _program)
            };
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
                _disposed = true;
            }
        }
    }
}
