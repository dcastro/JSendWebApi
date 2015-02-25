using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Properties;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendNotFoundResult : IHttpActionResult
    {
        private readonly JSendResult<FailResponse> _result;

        public JSendNotFoundResult(JSendApiController controller, string reason)
        {
            if (reason == null)
                reason = StringResources.NotFound_DefaultMessage;

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(StringResources.NotFound_WhiteSpaceReason, "reason");

            _result = new JSendResult<FailResponse>(HttpStatusCode.NotFound, new FailResponse(reason), controller);
        }

        public FailResponse Response
        {
            get { return _result.Response; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        public string Reason
        {
            get { return (string) _result.Response.Data; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
