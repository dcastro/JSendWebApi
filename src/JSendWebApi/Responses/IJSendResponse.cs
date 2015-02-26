using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSendWebApi.Responses
{
    /// <summary>A JSend response.</summary>
    public interface IJSendResponse
    {
        /// <summary>Gets the status of this response.</summary>
        string Status { get; }
    }
}
