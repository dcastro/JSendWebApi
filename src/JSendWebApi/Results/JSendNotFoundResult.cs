using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendNotFoundResult : IHttpActionResult
    {
        private readonly JSendResult<FailJSendResponse> _result;

        public JSendNotFoundResult(JSendApiController controller, string reason)
        {
            if (reason == null)
                reason = "The requested resource could not be found.";

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason cannot be an empty string.", "reason");

            _result = new JSendResult<FailJSendResponse>(
                controller, new FailJSendResponse(reason), HttpStatusCode.NotFound);
        }

        public FailJSendResponse Response
        {
            get { return _result.Response; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
