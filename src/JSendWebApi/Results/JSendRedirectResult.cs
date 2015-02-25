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
    public class JSendRedirectResult : IHttpActionResult
    {
        private readonly Uri _location;
        private readonly JSendResult<SuccessResponse> _result;

        public JSendRedirectResult(JSendApiController controller, Uri location)
        {
            if (location == null) throw new ArgumentNullException("location");

            _location = location;
            _result = new JSendResult<SuccessResponse>(HttpStatusCode.Redirect, new SuccessResponse(), controller);
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

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = _location;
            return message;
        }
    }
}
