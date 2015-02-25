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
    public class JSendCreatedResult<T> : IHttpActionResult
    {
        private readonly JSendResult<SuccessResponse> _result;
        private readonly Uri _location;

        public JSendCreatedResult(Uri location, T content, JSendApiController controller)
        {
            if (location == null) throw new ArgumentNullException("location");

            _result = new JSendResult<SuccessResponse>(HttpStatusCode.Created, new SuccessResponse(content), controller);

            _location = location;
        }

        public SuccessResponse Response
        {
            get { return _result.Response; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        public Uri Location
        {
            get { return _location; }
        }

        public T Content
        {
            get { return (T) _result.Response.Data; }
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = _location;
            return message;
        }
    }
}
