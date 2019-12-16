using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Repositories.Exceptions
{
    public class TransactionException : ApplicationException
    {
        public TransactionException(string message) : base(message)
        {

        }
    }
}
