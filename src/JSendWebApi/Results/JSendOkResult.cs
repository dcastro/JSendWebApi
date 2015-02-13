﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using JSendWebApi.Responses;
using Newtonsoft.Json;

namespace JSendWebApi.Results
{
    public class JSendOkResult : IHttpActionResult
    {
        private readonly JsonResult<SuccessJSendResponse> _result;

        public JSendOkResult(JSendApiController controller)
        {
            _result = new JsonResult<SuccessJSendResponse>(
                new SuccessJSendResponse(),
                controller.JsonSerializerSettings,
                controller.Encoding,
                controller);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}