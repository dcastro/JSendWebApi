using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendCreatedAtRouteResult<T> : IHttpActionResult
    {
        private readonly JSendResult<SuccessJSendResponse<T>> _result;
        private readonly Uri _location;

        public JSendCreatedAtRouteResult(JSendApiController controller, string routeName,
            IDictionary<string, object> routeValues, T content)
        {
            _result = new JSendResult<SuccessJSendResponse<T>>(
                controller, new SuccessJSendResponse<T>(content), HttpStatusCode.Created);

            UrlHelper urlFactory = controller.Url ?? new UrlHelper(controller.Request);

            string link = urlFactory.Link(routeName, routeValues);

            _location = new Uri(link);
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = _location;
            return message;
        }
    }
}
