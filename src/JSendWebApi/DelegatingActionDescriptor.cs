using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace JSendWebApi
{
    public class DelegatingActionDescriptor : HttpActionDescriptor
    {
        private readonly HttpActionDescriptor _descriptor;
        private readonly IActionResultConverter _resultConverter;

        public DelegatingActionDescriptor(HttpActionDescriptor descriptor, IActionResultConverter resultConverter)
        {
            if (descriptor == null) throw new ArgumentNullException("descriptor");
            if (resultConverter == null) throw new ArgumentNullException("resultConverter");

            _descriptor = descriptor;
            _resultConverter = resultConverter;
        }

        public HttpActionDescriptor InnerActionDescriptor
        {
            get { return _descriptor; }
        }

        public override IActionResultConverter ResultConverter
        {
            get { return _resultConverter; }
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return _descriptor.GetParameters();
        }

        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            return _descriptor.ExecuteAsync(controllerContext, arguments, cancellationToken);
        }

        public override string ActionName
        {
            get { return _descriptor.ActionName; }
        }

        public override Type ReturnType
        {
            get { return _descriptor.ReturnType; }
        }
    }
}
