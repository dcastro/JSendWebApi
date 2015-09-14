using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace JSend.WebApi
{
    /// <summary>
    /// Wraps around an instance of <see cref="HttpActionDescriptor"/>, delegating all member invocations
    /// to that instance, and exposes a custom <see cref="IActionResultConverter"/>.
    /// </summary>
    public class DelegatingActionDescriptor : HttpActionDescriptor
    {
        /// <summary>Initializes a new instance of <see cref="DelegatingActionDescriptor"/>.</summary>
        /// <param name="descriptor">The descriptor to which to delegate member invocations.</param>
        /// <param name="resultConverter">The custom converter.</param>
        public DelegatingActionDescriptor(HttpActionDescriptor descriptor, IActionResultConverter resultConverter)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (resultConverter == null) throw new ArgumentNullException(nameof(resultConverter));

            InnerActionDescriptor = descriptor;
            ResultConverter = resultConverter;
        }

        /// <summary>Gets the inner <see cref="HttpActionDescriptor"/>.</summary>
        public HttpActionDescriptor InnerActionDescriptor { get; }

        /// <summary>
        /// Gets the converter for correctly transforming the result of calling
        /// <see cref="ExecuteAsync(HttpControllerContext, IDictionary{string,object}, CancellationToken)"/> into an instance of
        /// <see cref="HttpResponseMessage"/>. 
        /// </summary>
        public override IActionResultConverter ResultConverter { get; }

        /// <summary>Retrieves the parameters for the action descriptor.</summary>
        /// <returns>The parameters for the action descriptor.</returns>
        public override Collection<HttpParameterDescriptor> GetParameters() => InnerActionDescriptor.GetParameters();

        /// <summary>
        /// Executes the described action and returns a <see cref="Task{T}"/> that once completed will
        /// contain the return value of the action.
        /// </summary>
        /// <param name="controllerContext">The context.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{T}"/> that once completed will contain the return value of the action.</returns>
        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext,
            IDictionary<string, object> arguments, CancellationToken cancellationToken)
            => InnerActionDescriptor.ExecuteAsync(controllerContext, arguments, cancellationToken);

        /// <summary>Gets the name of the action.</summary>
        /// <returns>The name of the action.</returns>
        public override string ActionName => InnerActionDescriptor.ActionName;

        /// <summary>Gets the return type of the descriptor.</summary>
        /// <returns>The return type of the descriptor.</returns>
        public override Type ReturnType => InnerActionDescriptor.ReturnType;
    }
}
