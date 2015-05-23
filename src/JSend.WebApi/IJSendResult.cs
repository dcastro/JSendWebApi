using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi
{
    public interface IJSendResult<out TResponse> : IHttpActionResult where TResponse : IJSendResponse
    {
        /// <summary>Gets the response to be formatted into the message's body.</summary>
        TResponse Response { get; }
    }
}
