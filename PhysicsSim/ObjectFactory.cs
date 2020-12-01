using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using PhysicsSim.Vertices;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PhysicsSim
{
    public static class ObjectFactory
    {
        public static (ColoredVertex[], PrimitiveType) FilledCircle(
            float radius,
            Color4 color,
            int precision = 30)
        {
            ColoredVertex[] result = new ColoredVertex[precision + 2];
            result[0] = new ColoredVertex(new Vector4(0, 0, 0, 1), color);
            for (int i = 0; i <= precision; ++i)
            {
                result[i+1] = new ColoredVertex(
                    new Vector4(
                        radius * (float)Math.Cos(2*i*Math.PI / precision),
                        radius * (float)Math.Sin(2*i*Math.PI / precision), 0, 1), color);
            }
            return (result, PrimitiveType.TriangleFan);
        }

        public enum BorderType
        {
            Inner, Middle, Outter
        }

        public static (ColoredVertex[], PrimitiveType) HollowCircle(
            float radius,
            float thickness,
            Color4 color,
            BorderType borderType = BorderType.Inner,
            int precision = 30)
        {
            float outrad = 0f, inrad = 0f;
            switch (borderType)
            {
                case BorderType.Inner:
                    outrad = radius;
                    inrad = radius - thickness;
                    break;
                case BorderType.Middle:
                    outrad = radius + thickness / 2;
                    inrad = radius - thickness / 2;
                    break;
                case BorderType.Outter:
                    outrad = radius + thickness;
                    inrad = radius;
                    break;
                default:
                    throw new ArgumentException("Invalid argument", "borderType");
            }
            ColoredVertex[] result = new ColoredVertex[2 * precision + 2];
            for (int i = 0; i <= precision; ++i)
            {
                result[2*i+0] = new ColoredVertex(
                    new Vector4(
                        outrad * (float)Math.Cos(2*i*Math.PI / precision),
                        outrad * (float)Math.Sin(2*i*Math.PI / precision), 0, 1), color);
                result[2*i+1] = new ColoredVertex(
                    new Vector4(
                        inrad * (float)Math.Cos(2*i*Math.PI / precision),
                        inrad * (float)Math.Sin(2*i*Math.PI / precision), 0, 1), color);
            }
            return (result, PrimitiveType.TriangleStrip);
        }

        public static (ColoredVertex[], PrimitiveType) Rectangle(
            float width,
            float height,
            Color4 color)
        {
            width /= 2; height /= 2;
            ColoredVertex[] result = new ColoredVertex[]
            {
                new ColoredVertex(new Vector4(+width, +height, 0, 1), color),
                new ColoredVertex(new Vector4(+width, -height, 0, 1), color),
                new ColoredVertex(new Vector4(-width, +height, 0, 1), color),
                new ColoredVertex(new Vector4(+width, -height, 0, 1), color),
                new ColoredVertex(new Vector4(-width, +height, 0, 1), color),
                new ColoredVertex(new Vector4(-width, -height, 0, 1), color)
            };
            return (result, PrimitiveType.Triangles);
        }

        public static (ColoredVertex[], PrimitiveType) RectangleEdge(
            float width,
            float height,
            float linewidth,
            Color4 color,
            BorderType borderType = BorderType.Middle)
        {
            float inwidth = -1f, outwidth = -1f, inheight = -1f, outheight = -1f;
            switch (borderType)
            {
                case BorderType.Inner:
                    inwidth = width - linewidth * 2; outwidth = width;
                    inheight = height - linewidth * 2; outheight = height;
                    break;
                case BorderType.Middle:
                    inwidth = width - linewidth; outwidth = width + linewidth;
                    inheight = height - linewidth; outheight = height + linewidth;
                    break;
                case BorderType.Outter:
                    inwidth = width; outwidth = width + linewidth * 2;
                    inheight = height; outheight = height + linewidth * 2;
                    break;
            }
            inwidth /= 2f; outwidth /= 2f; inheight /= 2f; outheight /= 2f;
            ColoredVertex[] vertices = new ColoredVertex[]
            {
                new ColoredVertex(new Vector4(+inwidth, +inheight, 0, 1), color),
                new ColoredVertex(new Vector4(+outwidth, +outheight, 0, 1), color),
                new ColoredVertex(new Vector4(-inwidth, +inheight, 0, 1), color),
                new ColoredVertex(new Vector4(-outwidth, +outheight, 0, 1), color),
                new ColoredVertex(new Vector4(-inwidth, -inheight, 0, 1), color),
                new ColoredVertex(new Vector4(-outwidth, -outheight, 0, 1), color),
                new ColoredVertex(new Vector4(+inwidth, -inheight, 0, 1), color),
                new ColoredVertex(new Vector4(+outwidth, -outheight, 0, 1), color),
                new ColoredVertex(new Vector4(+inwidth, +inheight, 0, 1), color),
                new ColoredVertex(new Vector4(+outwidth, +outheight, 0, 1), color),
            };
            return (vertices, PrimitiveType.TriangleStrip);
        }

        public static (ColoredVertex[], PrimitiveType) Curve(
            IEnumerable<System.Numerics.Vector2> curve,
            Color4 color)
        {
            System.Numerics.Vector2[] array = curve.ToArray();
            ColoredVertex[] result = new ColoredVertex[array.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new ColoredVertex(new Vector4(array[i].X, array[i].Y, 0, 1), color);
            }
            return (result, PrimitiveType.LineStrip);
        }

        public static (ColoredVertex[], PrimitiveType) Curve(
            Color4 color,
            params System.Numerics.Vector2[] points)
        {
            return Curve(points, color);
        }

        public static (ColoredVertex[], PrimitiveType) CheckMark(
            float width,
            float height,
            Color4 color)
        {
            ColoredVertex[] result = new ColoredVertex[]
            {
                new ColoredVertex(new Vector4(-0.0852f*width, -0.3510f*height, 0f, 1f), color),
                new ColoredVertex(new Vector4(+0.3520f*width, +0.2360f*height, 0f, 1f), color),
                new ColoredVertex(new Vector4(+0.2554f*width, +0.3080f*height, 0f, 1f), color),
                new ColoredVertex(new Vector4(-0.0820f*width, -0.1450f*height, 0f, 1f), color),
                new ColoredVertex(new Vector4(-0.0820f*width, -0.1450f*height, 0f, 1f), color),
                new ColoredVertex(new Vector4(-0.2464f*width, +0.0193f*height, 0f, 1f), color),
                new ColoredVertex(new Vector4(-0.3510f*width, -0.0854f*height, 0f, 1f), color),
            };
            return (result, PrimitiveType.TriangleFan);
        }

        public static (TexturedVertex[], PrimitiveType) TexRectangle(
            float width,
            float height)
        {
            width /= 2f; height /= 2f;
            TexturedVertex[] res = new TexturedVertex[]
            {
                new TexturedVertex(new Vector4(+width, +height, 0f, 1f), new Vector2(1f, 0f)),
                new TexturedVertex(new Vector4(-width, +height, 0f, 1f), new Vector2(0f, 0f)),
                new TexturedVertex(new Vector4(+width, -height, 0f, 1f), new Vector2(1f, 1f)),
                new TexturedVertex(new Vector4(-width, +height, 0f, 1f), new Vector2(0f, 0f)),
                new TexturedVertex(new Vector4(+width, -height, 0f, 1f), new Vector2(1f, 1f)),
                new TexturedVertex(new Vector4(-width, -height, 0f, 1f), new Vector2(0f, 1f))
            };
            return (res, PrimitiveType.Triangles);
        }

        public static (TexturedVertex[], PrimitiveType) TexRectangle(
            float width,
            float height,
            RectangleF region)
        {
            width /= 2f; height /= 2f;
            TexturedVertex[] res = new TexturedVertex[]
            {
                new TexturedVertex(new Vector4(+width, +height, 0f, 1f), new Vector2(region.Right, region.Top)),
                new TexturedVertex(new Vector4(-width, +height, 0f, 1f), new Vector2(region.Left, region.Top)),
                new TexturedVertex(new Vector4(+width, -height, 0f, 1f), new Vector2(region.Right, region.Bottom)),
                new TexturedVertex(new Vector4(-width, +height, 0f, 1f), new Vector2(region.Left, region.Top)),
                new TexturedVertex(new Vector4(+width, -height, 0f, 1f), new Vector2(region.Right, region.Bottom)),
                new TexturedVertex(new Vector4(-width, -height, 0f, 1f), new Vector2(region.Left, region.Bottom))
            };
            return (res, PrimitiveType.Triangles);
        }
    }
}