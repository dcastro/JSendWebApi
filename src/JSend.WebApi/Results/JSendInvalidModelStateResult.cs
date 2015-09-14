using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="FailResponse"/> containing the specified model state errors
    /// with status code <see cref="HttpStatusCode.BadRequest"/>.
    /// </summary>
    public class JSendInvalidModelStateResult : IJSendResult<FailResponse>
    {
        private readonly JSendResult<FailResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendInvalidModelStateResult"/>.</summary>
        /// <param name="modelState">The invalid model state to include in the response's body as key-value pairs.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendInvalidModelStateResult(ModelStateDictionary modelState, ApiController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            IDictionary<string, IEnumerable<string>> validationErrorsDictionary =
                new HttpError(modelState, controller.RequestContext.IncludeErrorDetail)
                    .ModelState
                    .ToDictionary(pair => pair.Key, pair => (IEnumerable<string>) pair.Value);

            var readOnlyValidationErrors =
                new ReadOnlyDictionary<string, IEnumerable<string>>(validationErrorsDictionary);

            var response = new FailResponse(readOnlyValidationErrors);

            _result = new JSendResult<FailResponse>(HttpStatusCode.BadRequest, response, controller);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public FailResponse Response => _result.Response;

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode => _result.StatusCode;

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request => _result.Request;

        /// <summary>Gets the model state errors to include in the response.</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Since the nested generic type is the member's return type, and not a parameter type, it places little burden on the client. Additionally, the burden can be mitigated through type inference.")]
        public IReadOnlyDictionary<string, IEnumerable<string>> ModelState
            => (IReadOnlyDictionary<string, IEnumerable<string>>) _result.Response.Data;

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            => _result.ExecuteAsync(cancellationToken);
    }
}
