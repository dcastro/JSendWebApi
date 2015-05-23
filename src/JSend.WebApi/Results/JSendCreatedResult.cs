using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="SuccessResponse"/> containing the created content
    /// with status code <see cref="HttpStatusCode.Created"/>.
    /// </summary>
    /// <typeparam name="T">The type of the created content.</typeparam>
    public class JSendCreatedResult<T> : IJSendResult<SuccessResponse>
    {
        private readonly JSendResult<SuccessResponse> _result;
        private readonly Uri _location;

        /// <summary>Initializes a new instance of <see cref="JSendCreatedResult{T}"/>.</summary>
        /// <param name="location">The location at which the content has been created.</param>
        /// <param name="content">The content value to negotiate and format in the entity body.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendCreatedResult(Uri location, T content, ApiController controller)
        {
            if (location == null) throw new ArgumentNullException("location");

            _result = new JSendResult<SuccessResponse>(HttpStatusCode.Created, new SuccessResponse(content), controller);

            _location = location;
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

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request
        {
            get { return _result.Request; }
        }

        /// <summary>Gets the location at which the content has been created.</summary>
        public Uri Location
        {
            get { return _location; }
        }

        /// <summary>Gets the content value to format in the entity body.</summary>
        public T Content
        {
            get { return (T) _result.Response.Data; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = _location;
            return message;
        }
    }
}
