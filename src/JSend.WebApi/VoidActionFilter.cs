using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace JSend.WebApi
{
    /// <summary>
    /// An action filter that selects a <see cref="JSendVoidResultConverter"/> as the action result converter for actions
    /// that do not return a value.
    /// </summary>
    public class VoidActionFilter : IActionFilter
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        /// <summary>Initializes a new instance of <see cref="VoidActionFilter"/>.</summary>
        /// <param name="serializerSettings">The serializer settings to pass into the action result converter.</param>
        /// <param name="encoding">The encoding to pass into the action result converter.</param>
        public VoidActionFilter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        /// <summary>
        /// Gets a value indicating whether more than one instance of the filter can be specified for a single program element.
        /// </summary>
        /// <returns>Always returns <see langword="false"/>.</returns>
        public bool AllowMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Selects a <see cref="JSendVoidResultConverter"/> as the action result converter for actions
        /// that do not return a value.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="cancellationToken">The cancellation token assigned for this task.</param>
        /// <param name="continuation">The delegate function to continue after the action method is invoked.</param>
        /// <returns>The newly created task for this operation.</returns>
        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null) throw new ArgumentNullException("actionContext");
            if (continuation == null) throw new ArgumentNullException("continuation");

            var returnType = actionContext.ActionDescriptor.ReturnType;

            if (returnType == null)
            {
                actionContext.ActionDescriptor = new DelegatingActionDescriptor(
                    descriptor: actionContext.ActionDescriptor,
                    resultConverter: new JSendVoidResultConverter(_serializerSettings, _encoding));
            }

            return continuation();
        }
    }
}
