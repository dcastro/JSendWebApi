using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using JSendWebApi.Results;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public abstract class JSendApiController : ApiController
    {
        protected internal virtual JSendOkResult JSendOk()
        {
            return new JSendOkResult(this);
        }
    }
}
