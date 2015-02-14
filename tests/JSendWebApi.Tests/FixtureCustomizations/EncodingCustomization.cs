using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    internal class EncodingCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Inject<Encoding>(Encoding.UTF8);
        }
    }
}
