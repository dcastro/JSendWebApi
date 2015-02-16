using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendBadRequestResult : IHttpActionResult
    {
        private readonly JSendResult<FailJSendResponse> _result;

        public JSendBadRequestResult(JSendApiController controller, IEnumerable<string> reasons)
        {
            if (reasons == null) throw new ArgumentNullException("reasons");

            var reasonsList = reasons.ToList();

            if (!reasonsList.Any())
                throw new ArgumentException("List of reasons must not be empty.", "reasons");

            if (reasonsList.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Reasons must not be null or white-space.", "reasons");

            _result = new JSendResult<FailJSendResponse>(controller, new FailJSendResponse(reasonsList),
                HttpStatusCode.BadRequest);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
