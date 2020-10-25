using PhysicsSim.Vertices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;

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
            Inner, Middle , Outter
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

        public static (ColoredVertex[], PrimitiveType) Curve(
            List<System.Numerics.Vector2> curve,
            Color4 color)
        {
            ColoredVertex[] result = new ColoredVertex[curve.Count];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new ColoredVertex(new Vector4(curve[i].X, curve[i].Y, 0, 1), color);
            }
            return (result, PrimitiveType.LineStrip);
        }

        public static (TexturedVertex[], PrimitiveType) TexRectangle(
            float width,
            float height)
        {
            width /= 2f; height /= 2f;
            TexturedVertex[] res = new TexturedVertex[]
            {
                new TexturedVertex(new Vector4(+width, +height, 0f, 1f), new Vector2(1f, 1f)),
                new TexturedVertex(new Vector4(-width, +height, 0f, 1f), new Vector2(0f, 1f)),
                new TexturedVertex(new Vector4(+width, -height, 0f, 1f), new Vector2(1f, 0f)),
                new TexturedVertex(new Vector4(-width, +height, 0f, 1f), new Vector2(0f, 1f)),
                new TexturedVertex(new Vector4(+width, -height, 0f, 1f), new Vector2(1f, 0f)),
                new TexturedVertex(new Vector4(-width, -height, 0f, 1f), new Vector2(0f, 0f))
            };
            return (res, PrimitiveType.Triangles);
        }
    }
}
