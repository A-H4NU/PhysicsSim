
using System.ComponentModel;
using System.Numerics;

namespace Hanu.ElectroLib.Objects
{
    public class MovableObject : PhysicalObject
    {
        public MovableObject(Vector2 position, float charge = 0, float mass = 0)
            : base(position, charge, mass)
        {
            return;
        }

        /// <summary>
        /// unit = m
        /// <para> To modify this, use <see cref="Position"/> </para>
        /// </summary>
        public override Vector2 Position
        {
            get => _position;
        }

        /// <summary>
        /// unit = m
        /// <para> Modifiable property of <see cref="Position"/> </para>
        /// </summary>
        public Vector2 PositionM
        {
            get => _position;
            set
            {
                _position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
            }
        }

        private float _charge;
        public override float Charge
        {
            get => _charge;
            set
            {
                _charge = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Charge"));
            }
        }

        private float _mass;

        public override event PropertyChangedEventHandler PropertyChanged;

        public override float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Mass"));
            }
        }

        // TODO: implement velocity property
    }
}