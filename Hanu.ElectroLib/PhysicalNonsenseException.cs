using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanu.ElectroLib.Exceptions
{
    class PhysicalNonsenseException : Exception
    {
        public PhysicalNonsenseType PhysicalNonsenseType;

        public PhysicalNonsenseException(PhysicalNonsenseType type)
        {
            PhysicalNonsenseType = type;
        }
    }
}
