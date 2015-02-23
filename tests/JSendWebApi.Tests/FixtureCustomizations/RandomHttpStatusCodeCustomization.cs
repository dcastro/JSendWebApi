using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    public class RandomHttpStatusCodeCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var rnd = new Random();
            fixture.Register<HttpStatusCode>(() => Generate(rnd));
        }

        private static HttpStatusCode Generate(Random rnd)
        {
            var values = Enum.GetValues(typeof (HttpStatusCode));
            return (HttpStatusCode) values.GetValue(rnd.Next(values.Length));
        }
    }
}
