using Hanu.ElectroLib.Exceptions;
using Hanu.ElectroLib.Objects;

using MathNet.Numerics.LinearAlgebra;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hanu.ElectroLib.Physics
{
    /// <summary>
    /// A physical system of <see cref="PhysicalObject"/>s
    /// </summary>
    public class PSystem : ICollection<PhysicalObject>
    {
        private readonly List<PhysicalObject> _pObjs;

        public int Count => _pObjs.Count;

        public bool IsReadOnly => false;

        public PSystem()
        {
            _pObjs = new List<PhysicalObject>();
        }

        public PSystem(int capacity)
        {
            _pObjs = new List<PhysicalObject>(capacity);
        }

        public PSystem(IEnumerable<PhysicalObject> objects)
        {
            PhysicalObject[] array = objects.ToArray();
            _pObjs = new List<PhysicalObject>(array.Length);
            Array.Sort(array, (a, b) => a.X.CompareTo(b.X));
            for (int i = 0; i < array.Length - 1; ++i)
            {
                if (array[i].Position == array[i+1].Position)
                {
                    throw new PhysicalNonsenseException(PhysicalNonsenseType.DistanceZero);
                }
            }
            _pObjs.AddRange(objects);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pObjs.GetEnumerator();
        }

        public IEnumerator<PhysicalObject> GetEnumerator()
        {
            return _pObjs.GetEnumerator();
        }

        public void Add(PhysicalObject item)
        {
            _pObjs.Add(item);
        }

        public void Clear() => _pObjs.Clear();

        public bool Contains(PhysicalObject item) => _pObjs.Contains(item);

        public void CopyTo(PhysicalObject[] array, int arrayIndex) => _pObjs.CopyTo(array, arrayIndex);

        public bool Remove(PhysicalObject item) => _pObjs.Remove(item);

        #region Physics Implementation

        /// <summary>
        /// Provide a function that returns electric field of the system
        /// </summary>
        /// <returns>Electric field(<see cref="Vector<double>"/>) with input of position(<see cref="Vector<double>"/>)</returns>
        public Func<Vector<double>, Vector<double>> GetElectricFieldFunc()
        {
            return (pos) => GetElectricFieldAt(pos);
        }

        /// <summary>
        /// Provide the electric field at a certain position in the system
        /// </summary>
        /// <returns>electric field at a certain position in the system</returns>
        public Vector<double> GetElectricFieldAt(Vector<double> pos)
        {
            Vector<double> res = default;
            foreach (PhysicalObject obj in _pObjs)
            {
                Vector<double> disp = pos - obj.Position;
                res += Constant.Coulomb * obj.Charge / Math.Pow(disp.L2Norm(), 3) * disp;
            }
            return res;
        }

        /// <summary>
        /// Provide a function that returns voltage of the system
        /// </summary>
        /// <returns>Voltage(<see cref="Single"/>) with input of position(<see cref="Vector<double>"/>)</returns>
        public Func<Vector<double>, double> GetVoltageFunc()
        {
            return (pos) => GetVoltageAt(pos);
        }

        /// <summary>
        /// Provide the voltage at a certain position in the system
        /// </summary>
        /// <returns>voltage at a certain position in the system</returns>
        public double GetVoltageAt(Vector<double> pos)
        {
            return (from obj in _pObjs
                    select Constant.Coulomb * obj.Charge / (pos - obj.Position).L2Norm()).Sum();
        }

        #endregion
    }
}