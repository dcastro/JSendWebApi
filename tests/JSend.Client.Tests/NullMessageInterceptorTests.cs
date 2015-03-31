using FluentAssertions;
using Xunit;

namespace JSend.Client.Tests
{
    public class NullMessageInterceptorTests
    {
        [Fact]
        public void InstanceIsSingleton()
        {
            // Exercise system
            var first = NullMessageInterceptor.Instance;
            var second = NullMessageInterceptor.Instance;
            // Verify outcome
            first.Should().BeSameAs(second);
        }
    }
}
