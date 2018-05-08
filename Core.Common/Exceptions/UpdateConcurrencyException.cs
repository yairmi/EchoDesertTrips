using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Exceptions
{
    [Serializable]
    public class UpdateConcurrencyException : ApplicationException
    {
        public UpdateConcurrencyException(string message)
            : base(message)
        {
        }

        public UpdateConcurrencyException(string message, DbUpdateConcurrencyException exception)
            : base(message, exception)
        {
        }
        public IEnumerable<DbEntityEntry> Entries { get; }
    }
}
