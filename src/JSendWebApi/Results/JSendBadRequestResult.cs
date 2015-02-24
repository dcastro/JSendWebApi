using System;
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
        private readonly JSendResult<FailResponse> _result;

        public JSendBadRequestResult(JSendApiController controller, string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason cannot be an empty string.", "reason");

            _result = new JSendResult<FailResponse>(controller, new FailResponse(reason),
                HttpStatusCode.BadRequest);
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
