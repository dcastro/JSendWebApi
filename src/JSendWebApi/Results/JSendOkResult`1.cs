﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace JSendWebApi.Results
{
    public class JSendOkResult<T> : IHttpActionResult
    {
        private readonly JsonResult<SuccessJSendResponse> _result;

        public JSendOkResult(ApiController controller, T content)
        {
            _result = new JsonResult<SuccessJSendResponse>(
                new SuccessJSendResponse<T>(content),
                new JsonSerializerSettings(),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
                controller);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
