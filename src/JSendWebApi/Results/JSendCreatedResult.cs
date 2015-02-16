﻿using System;
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
        private readonly JSendResult<SuccessJSendResponse> _result;
        private readonly Uri _location;

        public JSendCreatedResult(JSendApiController controller, Uri location, T content)
        {
            if (location == null) throw new ArgumentNullException("location");

            _result = new JSendResult<SuccessJSendResponse>(
                controller, new SuccessJSendResponse(content), HttpStatusCode.Created);

            _location = location;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = _location;
            return message;
        }
    }
}