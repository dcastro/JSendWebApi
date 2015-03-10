using System;
using System.Net.Http;

namespace JSend.Client
{
    /// <summary>Represents the result of a request to a JSend API.</summary>
    /// <typeparam name="T">The type of data expected to be returned by the API.</typeparam>
    public class JSendResult<T>
    {
        private readonly IJSendResponse _jsendResponse;
        private readonly HttpResponseMessage _responseMessage;

        /// <summary>Initializes a new instance of <see cref="JSendResult{T}"/>.</summary>
        /// <param name="jsendResponse">The JSend response retrieved from the entity body.</param>
        /// <param name="responseMessage">The HTTP response message.</param>
        public JSendResult(SuccessResponse<T> jsendResponse, HttpResponseMessage responseMessage)
            : this(jsendResponse as IJSendResponse, responseMessage)
        {
        }

        /// <summary>Initializes a new instance of <see cref="JSendResult{T}"/>.</summary>
        /// <param name="jsendResponse">The JSend response retrieved from the entity body.</param>
        /// <param name="responseMessage">The HTTP response message.</param>
        public JSendResult(FailResponse jsendResponse, HttpResponseMessage responseMessage)
            : this(jsendResponse as IJSendResponse, responseMessage)
        {
        }

        private JSendResult(IJSendResponse jsendResponse, HttpResponseMessage responseMessage)
        {
            if (jsendResponse == null) throw new ArgumentNullException("jsendResponse");
            if (responseMessage == null) throw new ArgumentNullException("responseMessage");

            _responseMessage = responseMessage;
            _jsendResponse = jsendResponse;
        }

        /// <summary>Gets the JSend response retrieved from the entity body.</summary>
        public IJSendResponse JsendResponse
        {
            get { return _jsendResponse; }
        }

        /// <summary>Gets the HTTP response message.</summary>
        public HttpResponseMessage ResponseMessage
        {
            get { return _responseMessage; }
        }
    }
}
