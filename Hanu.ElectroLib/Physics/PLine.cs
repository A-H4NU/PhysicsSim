using MathNet.Numerics.OdeSolvers;
using MathNet.Numerics.LinearAlgebra;

using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Hanu.ElectroLib.Objects;
using System.Numerics;

namespace Hanu.ElectroLib.Physics
{
    public static class PLine
    {
        public static Task<List<Vector2>> ElectricFieldLineAsync(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, float delta = 1e-4f)
        {
            var electric_field = PSystem.GetElectricFieldFunc(system);
            Vector2 f(float t, Vector2 v) => electric_field(v);
            var y0 = electric_field(initPos);
            return new Task<List<Vector2>>(() => SecondOrder(y0, 0, f, endFunc, delta));
        }

        private static List<Vector2> SecondOrder(Vector2 y0, float start, Func<float, Vector2, Vector2> f, Func<float, Vector2, bool> endFunc, float delta = 1e-4f)
        {
            List<Vector2> vectorArrays = new List<Vector2>();
            float num1 = start;
            vectorArrays.Add(y0);
            while (endFunc(num1, vectorArrays.Last()) == false)
            {
                Vector2 nums = f(num1, y0);
                Vector2 nums1 = f(num1, y0 + (nums * delta));
                vectorArrays.Add(y0 + (delta * 0.5f * (nums + nums1)));
                num1 += delta;
                y0 = vectorArrays.Last();
            }
            return vectorArrays;
        }
    }
}
