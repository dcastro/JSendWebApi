﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions()
            : base(
                new JSendApiControllerCustomization(),
                new EncodingCustomization(),
                new JsonSerializerSettingsCustomization())
        {

        }
    }
}