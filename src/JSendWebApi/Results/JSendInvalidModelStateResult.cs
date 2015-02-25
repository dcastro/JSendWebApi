using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendInvalidModelStateResult : IHttpActionResult
    {
        private readonly JSendResult<FailResponse> _result;

        public JSendInvalidModelStateResult(JSendApiController controller, ModelStateDictionary modelState)
        {
            if (controller == null) throw new ArgumentNullException("controller");

            IDictionary<string, IEnumerable<string>> validationErrorsDictionary =
                new HttpError(modelState, controller.RequestContext.IncludeErrorDetail)
                    .ModelState
                    .ToDictionary(pair => pair.Key, pair => (IEnumerable<string>) pair.Value);

            var readOnlyValidationErrors =
                new ReadOnlyDictionary<string, IEnumerable<string>>(validationErrorsDictionary);

            _result = new JSendResult<FailResponse>(HttpStatusCode.BadRequest, new FailResponse(readOnlyValidationErrors), controller);
        }

        public FailResponse Response
        {
            get { return _result.Response; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        public IReadOnlyDictionary<string, IEnumerable<string>> ModelState
        {
            get { return (IReadOnlyDictionary<string, IEnumerable<string>>) _result.Response.Data; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
