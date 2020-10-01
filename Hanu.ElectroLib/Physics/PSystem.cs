using Hanu.ElectroLib.Exceptions;
using Hanu.ElectroLib.Objects;

using MathNet.Numerics.LinearAlgebra;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Hanu.ElectroLib.Physics
{
    /// <summary>
    /// A physical system of <see cref="PhysicalObject"/>s
    /// </summary>
    public static class PSystem
    {
        /// <summary>
        /// Provide a function that returns electric field of the system
        /// </summary>
        /// <returns>Electric field(<see cref="Vector2"/>) with input of position(<see cref="Vector2"/>)</returns>
        public static Func<Vector2, Vector2> GetElectricFieldFunc(IEnumerable<PhysicalObject> system)
        {
            return (pos) => GetElectricFieldAt(system, pos);
        }

        /// <summary>
        /// Provide the electric field at a certain position in the system
        /// </summary>
        /// <returns>electric field at a certain position in the system</returns>
        public static Vector2 GetElectricFieldAt(IEnumerable<PhysicalObject> system, Vector2 pos)
        {
            Vector2 res = default;
            foreach (PhysicalObject obj in system)
            {
                Vector2 disp = pos - obj.Position;
                res += Constant.Coulomb * obj.Charge / (float)Math.Pow(disp.Length(), 3) * disp;
            }
            return res;
        }

        /// <summary>
        /// Provide a function that returns voltage of the system
        /// </summary>
        /// <returns>Voltage(<see cref="Single"/>) with input of position(<see cref="Vector2"/>)</returns>
        public static Func<Vector2, double> GetVoltageFunc(IEnumerable<PhysicalObject> system)
        {
            return (pos) => GetVoltageAt(system, pos);
        }

        /// <summary>
        /// Provide the voltage at a certain position in the system
        /// </summary>
        /// <returns>voltage at a certain position in the system</returns>
        public static double GetVoltageAt(IEnumerable<PhysicalObject> system, Vector2 pos)
        {
            return (from obj in system
                    select Constant.Coulomb * obj.Charge / (pos - obj.Position).Length()).Sum();
        }
    }
}