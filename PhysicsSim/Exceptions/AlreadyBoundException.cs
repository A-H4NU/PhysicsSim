using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsSim.Exceptions
{
    public class AlreadyBoundException : Exception
    {
        public AlreadyBoundException()
            : base() { }

        public AlreadyBoundException(string message)
            : base(message) { }

        public AlreadyBoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
