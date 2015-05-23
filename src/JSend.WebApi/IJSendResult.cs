using System.Net;
using System.Net.Http;
using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi
{
    /// <summary>
    /// Represents an action result that returns the specified JSend response with the specified status code.
    /// </summary>
    /// <typeparam name="TResponse">The type of the JSend response.</typeparam>
    public interface IJSendResult<out TResponse> : IHttpActionResult where TResponse : IJSendResponse
    {
        /// <summary>Gets the response to be formatted into the message's body.</summary>
        TResponse Response { get; }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>Gets the request message which led to this result.</summary>
        HttpRequestMessage Request { get; }
    }
}
