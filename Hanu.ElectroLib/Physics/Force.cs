using Hanu.ElectroLib.Exceptions;
using Hanu.ElectroLib.Objects;

using MathNet.Numerics.LinearAlgebra;

using System;

namespace Hanu.ElectroLib.Physics
{
    /// <summary>
    /// unit = m
    /// <para>Provide physical forces between two <see cref="PhysicalObject"/>s</para>
    /// </summary>
    public static class Force
    {
        public static Vector<double> Gravity(PhysicalObject on, PhysicalObject by)
        {
            if (!TryGravity(on, by, out Vector<double> force))
            {
                throw new PhysicalNonsenseException(PhysicalNonsenseType.DistanceZero);
            }
            return force;
        }

        public static bool TryGravity(PhysicalObject on, PhysicalObject by, out Vector<double> force)
        {
            Vector<double> d = by.Position - on.Position;
            if (d.L2Norm() == 0)
            {
                force = CreateVector.Dense<double>(2);
                return false;
            }
            force = (Constant.Gravity * on.Mass * by.Mass / (double)Math.Pow(d.L2Norm(), 3)) * d;
            return true;
        }

        public static Vector<double> Electrostatic(PhysicalObject on, PhysicalObject by)
        {
            if (!TryElectrostatic(on, by, out Vector<double> force))
            {
                throw new PhysicalNonsenseException(PhysicalNonsenseType.DistanceZero);
            }
            return force;
        }

        public static bool TryElectrostatic(PhysicalObject on, PhysicalObject by, out Vector<double> force)
        {
            Vector<double> d = by.Position - on.Position;
            if (d.L2Norm() == 0)
            {
                force = CreateVector.Dense<double>(2);
                return false;
            }
            force = Constant.Coulomb * on.Charge * by.Charge / (double)Math.Pow(d.L2Norm(), 3) * d;
            return true;
        }
    }
}