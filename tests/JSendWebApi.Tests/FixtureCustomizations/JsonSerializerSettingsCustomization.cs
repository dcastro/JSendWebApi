using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    public class JsonSerializerSettingsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JsonSerializerSettings>(
                c => c.OmitAutoProperties());
        }
    }
}
