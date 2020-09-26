using Hanu.ElectroLib.ComponentModel;
using Hanu.ElectroLib.Objects;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Hanu.ElectroLib.Physics
{
    /// <summary>
    /// A physical system of <see cref="PhysicalObject"/>s
    /// </summary>
    public class PSystem : INotifyDeepCollectionChanged, ICollection<PhysicalObject>
    {
        private readonly List<PhysicalObject> _pObjs;

        private readonly List<PropertyChangedEventHandler> _handlers;

        public int Count => _pObjs.Count;

        public bool IsReadOnly => false;

        public event NotifyDeepCollectionChangedEventHandler DeepCollectionChanged;

        public PSystem(IEnumerable<PhysicalObject> objects)
        {
            PhysicalObject[] objList = objects.ToArray();
            _pObjs = new List<PhysicalObject>(objList.Length);
            _handlers = new List<PropertyChangedEventHandler>(objList.Length);
            foreach (PhysicalObject obj in objList)
            {
                Add(obj);
            }
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
            void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
                       => DeepCollectionChanged?.Invoke(sender,
                           new NotifyDeepCollectionChangedEventArgs(
                               NotifyDeepCollectionChangedAction.Modified,
                               item,
                               _pObjs.Count));
            _pObjs.Add(item);
            _handlers.Add(Item_PropertyChanged);
            DeepCollectionChanged?.Invoke(this,
                new NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction.Add, item, _pObjs.Count));
        }

        public void Clear()
        {
            _pObjs.Clear();
            _handlers.Clear();
            DeepCollectionChanged?.Invoke(this,
                new NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction.Reset));
        }

        public bool Contains(PhysicalObject item) => _pObjs.Contains(item);

        public void CopyTo(PhysicalObject[] array, int arrayIndex) => _pObjs.CopyTo(array, arrayIndex);

        public bool Remove(PhysicalObject item)
        {
            int index = _pObjs.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            PhysicalObject obj = _pObjs[index];
            _pObjs.RemoveAt(index);
            _handlers.RemoveAt(index);
            DeepCollectionChanged?.Invoke(this,
                new NotifyDeepCollectionChangedEventArgs(NotifyDeepCollectionChangedAction.Remove, obj, index));
            return true;
        }

        #region Physics Implementation

        /// <summary>
        /// Provide a function that returns electric field of the system
        /// </summary>
        /// <returns>Electric field(<see cref="Vector2"/>) with input of position(<see cref="Vector2"/>)</returns>
        public Func<Vector2, Vector2> GetElectricFieldFunc()
        {
            return (pos) => GetElectricFieldAt(pos);
        }

        /// <summary>
        /// Provide the electric field at a certain position in the system
        /// </summary>
        /// <returns>electric field at a certain position in the system</returns>
        public Vector2 GetElectricFieldAt(Vector2 pos)
        {
            Vector2 res = default;
            foreach (PhysicalObject obj in _pObjs)
            {
                Vector2 disp = obj.Position - pos;
                res += Constant.Coulomb * obj.Charge / (float)Math.Pow(disp.Length(), 3) * disp;
            }
            return res;
        }

        /// <summary>
        /// Provide a function that returns voltage of the system
        /// </summary>
        /// <returns>Voltage(<see cref="Single"/>) with input of position(<see cref="Vector2"/>)</returns>
        public Func<Vector2, float> GetVoltageFunc()
        {
            return (pos) => GetVoltageAt(pos);
        }

        /// <summary>
        /// Provide the voltage at a certain position in the system
        /// </summary>
        /// <returns>voltage at a certain position in the system</returns>
        public float GetVoltageAt(Vector2 pos)
        {
            return (from obj in _pObjs
                    select Constant.Coulomb * obj.Charge / (pos - obj.Position).Length()).Sum();
        }

        #endregion
    }
}