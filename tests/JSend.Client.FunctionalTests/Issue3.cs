using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.FunctionalTests.FixtureCustomizations;
using Nito.AsyncEx;
using Xunit;

namespace JSend.Client.FunctionalTests
{
    /// <summary>
    /// https://github.com/dcastro/JSendWebApi/issues/3
    /// </summary>
    public class Issue3
    {
        [Theory, JSendAutoData]
        public void BlockingDoesNotCauseDeadlocks(JSendClient client)
        {
            // Fixture setup
            // Use a synchronization context that schedules continuations on the initial thread to mimic an UI application.
            // This scenario is also somewhat equivalent to an ASP.NET environment, where the request context is not tied to a specific thread, 
            // but is limited to one thread at a time (http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html).
            Action action = () => AsyncContext.Run(() =>
            {
                client.GetAsync<string>("http://localhost/users/get").Wait();
            });

            // Exercise system and verify outcome
            action.ExecutionTime().ShouldNotExceed(3.Seconds());
        }
    }
}
