
using Hanu.ElectroLib.Objects;

using MathNet.Numerics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Hanu.ElectroLib.Physics
{
    public static class PLine
    {
        public async static Task<List<Vector2>> ElectricFieldLineAsync(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, bool startFromNegative, float delta = 1e-3f)
            => ElectricFieldLine(system, initPos, endFunc, startFromNegative, delta);

        public static List<Vector2> ElectricFieldLine(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, bool startFromNegative, float delta = 1e-3f)
        {
            Func<Vector2, Vector2> electric_field = PSystem.GetElectricFieldFunc(system);
            Vector2 f(float t, Vector2 v)
            {
                var e = electric_field(v);
                return e / e.Length() * 100f;
            }
            List<Vector2> vectorArrays = new List<Vector2>();
            float num1 = 0f;
            vectorArrays.Add(initPos);
            while (!endFunc(num1, vectorArrays.Last()) &&
                f(num1, vectorArrays.Last()).LengthSquared() >= delta * delta)
            {
                Vector2 nums = f(num1, initPos);
                Vector2 nums1 = f(num1, initPos + (nums * delta));
                vectorArrays.Add(initPos + (delta * 0.5f * (nums + nums1)));
                if (TryFindMatch(
                    system,
                    (p) => p.Charge != 0 && DistFromPointToSegment(vectorArrays[vectorArrays.Count-2], vectorArrays.Last(), p.Position) < 0.1f,
                    out PhysicalObject obj))
                {
                    vectorArrays.RemoveAt(vectorArrays.Count - 1);
                    vectorArrays.Add(obj.Position);
                    break;
                }
                num1 += delta;
                initPos = vectorArrays.Last();
            }
            return vectorArrays;
        }

        private static bool TryFindMatch(IEnumerable<PhysicalObject> e, Func<PhysicalObject, bool> predicate, out PhysicalObject physicalObject)
        {
            foreach (PhysicalObject obj in e)
            {
                if (predicate(obj))
                {
                    physicalObject = obj;
                    return true;
                }
            }
            physicalObject = null;
            return false;
        }

        private static float DistFromPointToSegment(Vector2 p1, Vector2 p2, Vector2 point)
        {
            Vector2 pp1 = point - p1, pp2 = point - p2, d = p2 - p1;
            if (Vector2.Dot(pp1, d) * Vector2.Dot(pp2, d) > 0)
            {
                return Math.Min(pp1.Length(), pp2.Length());
            }
            d /= d.Length();
            return (pp1 - Vector2.Dot(pp1, d) * d).Length();
            
        }

        public static List<Vector2> SecondOrder(Vector2 initPos, float start, Func<float, Vector2, Vector2> f, Func<float, Vector2, bool> endFunc, float delta = 1e-5f)
        {
            List<Vector2> vectorArrays = new List<Vector2>();
            float num1 = start;
            vectorArrays.Add(initPos);
            while (!endFunc(num1, vectorArrays.Last()) && vectorArrays.Last().X != Single.NaN && vectorArrays.Last().Y != Single.NaN)
            {
                Vector2 nums = f(num1, initPos);
                Vector2 nums1 = f(num1, initPos + (nums * delta));
                vectorArrays.Add(initPos + (delta * 0.5f * (nums + nums1)));
                num1 += delta;
                initPos = vectorArrays.Last();
            }
            return vectorArrays;
        }
    }
}
