using Hanu.ElectroLib.Exceptions;
using Hanu.ElectroLib.Objects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hanu.ElectroLib.Physics
{
    /// <summary>
    /// unit = m
    /// <para>Provide physical forces between two <see cref="Objects"/></para>
    /// </summary>
    public static class Force
    {
        public static Vector2 Gravity(PhysicalObject on, PhysicalObject by)
        {
            if (!TryGravity(on, by, out Vector2 force))
                throw new PhysicalNonsenseException(PhysicalNonsenseType.DistanceZero);
            return force;
        }

        public static bool TryGravity(PhysicalObject on, PhysicalObject by, out Vector2 force)
        {
            Vector2 d = by.Position - on.Position;
            if (d == Vector2.Zero)
            {
                force = Vector2.Zero;
                return false;
            }
            force = (Constant.Gravity * on.Mass * by.Mass / (float)Math.Pow(d.Length(), 3)) * d;
            return true;
        }

        public static Vector2 Electrostatic(PhysicalObject on, PhysicalObject by)
        {
            if (!TryElectrostatic(on, by, out Vector2 force))
                throw new PhysicalNonsenseException(PhysicalNonsenseType.DistanceZero);
            return force;
        }

        public static bool TryElectrostatic(PhysicalObject on, PhysicalObject by, out Vector2 force)
        {
            Vector2 d = by.Position - on.Position;
            if (d == Vector2.Zero)
            {
                force = Vector2.Zero;
                return false;
            }
            force = Constant.Coulomb * on.Charge * by.Charge / (float)Math.Pow(d.Length(), 3) * d;
            return true;
        }
    }
}
