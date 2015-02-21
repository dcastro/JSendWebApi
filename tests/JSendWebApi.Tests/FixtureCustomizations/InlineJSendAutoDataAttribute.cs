using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture.Xunit;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    public class InlineJSendAutoDataAttribute : InlineAutoDataAttribute
    {
        public InlineJSendAutoDataAttribute(params object[] values)
            : base(new JSendAutoDataAttribute(), values)
        {

        }
    }
}
