using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Domains.Interfaces
{
    public interface IDateTracking
    {
        //DatetimeOffset is better than Date && Time convert from UTC to Local
        DateTimeOffset CreatedDate { get; set; }

        DateTimeOffset? LastModifiedDate { get; set; }
    }
}
