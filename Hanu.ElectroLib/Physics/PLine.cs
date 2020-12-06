
using Hanu.ElectroLib.Objects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Hanu.ElectroLib.Physics
{
    public static class PLine
    {
        /// <summary>
        /// Calculate the electric field line asynchronously
        /// </summary>
        /// <param name="system">List of physical objects in the system</param>
        /// <param name="initPos">Starting position of the electric field line</param>
        /// <param name="endFunc">stop calculating the line if true</param>
        /// <param name="startFromNegative">whether or not starting from negative charged object</param>
        /// <param name="delta">small step of calculation, the smaller, the better</param>
        /// <returns>the task that returns the list of points that represent the electric field line</returns>
        public static Task<List<Vector2>> ElectricFieldLineAsync(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, bool startFromNegative, float delta = 1e-3f)
        {
            return Task.Factory.StartNew(() => ElectricFieldLine(system, initPos, endFunc, startFromNegative, delta));
        }

        /// <summary>
        /// Calculate the electric field line
        /// </summary>
        /// <param name="system">List of physical objects in the system</param>
        /// <param name="initPos">Starting position of the electric field line</param>
        /// <param name="endFunc">stop calculating the line if true</param>
        /// <param name="startFromNegative">whether or not starting from negative charged object</param>
        /// <param name="delta">small step of calculation, the smaller, the better</param>
        /// <returns>the list of points that represent the electric field line</returns>
        public static List<Vector2> ElectricFieldLine(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, bool startFromNegative, float delta = 1e-3f)
        {
            Func<Vector2, Vector2> electric_field = PSystem.GetElectricFieldFunc(system);
            Vector2 f(float t, Vector2 v)
            {
                // get electriv field at v
                var e = electric_field(v);
                // fix the length with 100
                return e / e.Length() * 100f;
            }
            List<Vector2> vectorArrays = new List<Vector2>();
            float num1 = 0f;
            vectorArrays.Add(initPos);
            while (!endFunc(num1, vectorArrays.Last()))
            {
                Vector2 nums = f(num1, initPos);
                Vector2 nums1 = f(num1, initPos + (nums * delta));
                vectorArrays.Add(initPos + (delta * 0.5f * (nums + nums1)));
                // if the line reached an charge very closely
                if (TryFindMatch(
                    system,
                    (p) => p.Charge != 0 && DistFromPointToSegment(vectorArrays[vectorArrays.Count-2], vectorArrays.Last(), p.Position) < 0.1f,
                    out PhysicalObject obj))
                {
                    // remove the last element
                    vectorArrays.RemoveAt(vectorArrays.Count - 1);
                    // add the position of the object that this line is approaching
                    vectorArrays.Add(obj.Position);
                    // break this loop
                    break;
                }
                num1 += delta;
                initPos = vectorArrays.Last();
            }
            return vectorArrays;
        }

        public static Task<List<Vector2>> ElectricFieldLineFastAsync(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, bool startFromNegative, float delta = 1e-3f)
        {
            return Task.Factory.StartNew(() => ElectricFieldLineFast(system, initPos, endFunc, startFromNegative, delta));
        }

        public static List<Vector2> ElectricFieldLineFast(IEnumerable<PhysicalObject> system, Vector2 initPos, Func<float, Vector2, bool> endFunc, bool startFromNegative, float delta = 1e-3f)
        {
            Vector2 f(float t, Vector2 v)
            {
                var e = PSystem.GetElectricFieldAt(system, v);
                return e / e.Length() * 100f;
            }
            var result = new List<Vector2>();
            float num1 = 0f;
            result.Add(initPos);
            while (!endFunc(num1, result.Last()) || Single.IsNaN(result.Last().X))
            {
                var vec = f(num1, initPos);
                result.Add(initPos + delta * vec);
                if (TryFindMatch(
                    system,
                    (p) => p.Charge != 0 && DistFromPointToSegment(result[result.Count-2], result.Last(), p.Position) < 0.1f,
                    out PhysicalObject obj))
                {
                    result.RemoveAt(result.Count - 1);
                    result.Add(obj.Position);
                    break;
                }
                num1 += delta;
                initPos = result.Last();
            }
            return result;
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

        /// <summary>
        /// Get the minimum distance between P on the segment p1 p2 and the point
        /// </summary>
        /// <returns>minimum distance between P on the segment p1 p2 and the point</returns>
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
    }
}
