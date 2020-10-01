using ElectroSim.Vertices;

using MathNet.Numerics.LinearAlgebra;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroSim
{
    public static class ObjectFactory
    {
        public static (ColoredVertex[], PrimitiveType) FilledCircle(
            float radius,
            Color4 color,
            int precision = 100)
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
            int precision = 100)
        {
            float outrad = 0f, inrad = 0f;
            if (borderType == BorderType.Inner)
            {
                outrad = radius;
                inrad = radius - thickness;
            }
            else if (borderType == BorderType.Middle)
            {
                outrad = radius + thickness / 2;
                inrad = radius - thickness / 2;
            }
            else if (borderType == BorderType.Outter)
            {
                outrad = radius + thickness;
                inrad = radius;
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
            List<Vector2> curve,
            Color4 color)
        {
            ColoredVertex[] result = new ColoredVertex[curve.Count];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new ColoredVertex(new Vector4((float)curve[i][0], (float)curve[i][1], 0, 1), color);
            }
            return (result, PrimitiveType.LineStrip);
        }

        private static ColoredVertex[] FromTriangleFan(ColoredVertex[] vertices)
        {
            List<ColoredVertex> result = new List<ColoredVertex>(3*vertices.Length - 6)
            {
                vertices[0], vertices[1], vertices[2]
            };
            for (int i = 3; i < vertices.Length; ++i)
            {
                result.Add(vertices[0]);
                result.Add(vertices[i-1]);
                result.Add(vertices[i]);
            }
            return result.ToArray();
        }
    }
}
