using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Tests.TestClasses
{
    public class Model
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
