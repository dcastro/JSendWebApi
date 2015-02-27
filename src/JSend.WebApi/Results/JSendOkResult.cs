using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Responses;
using Newtonsoft.Json;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="SuccessResponse"/> with status code <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    public class JSendOkResult : IHttpActionResult
    {
        private readonly JSendResult<SuccessResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendOkResult(JSendApiController controller)
        {
            if (controller == null) throw new ArgumentNullException("controller");

            _result = InitializeResult(controller.JsonSerializerSettings, controller.Encoding, controller.Request);
        }

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="settings">The serializer settings.</param>
        /// <param name="encoding">The content encoding.</param>
        /// <param name="request">The request message which led to this result.</param>
        public JSendOkResult(JsonSerializerSettings settings, Encoding encoding, HttpRequestMessage request)
        {
            _result = InitializeResult(settings, encoding, request);
        }

        private static JSendResult<SuccessResponse> InitializeResult(JsonSerializerSettings settings, Encoding encoding,
            HttpRequestMessage request)
        {
            return new JSendResult<SuccessResponse>(HttpStatusCode.OK, new SuccessResponse(), settings, encoding, request);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public SuccessResponse Response
        {
            get { return _result.Response; }
        }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
