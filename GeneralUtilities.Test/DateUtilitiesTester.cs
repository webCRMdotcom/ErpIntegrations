using FluentAssertions;
using System;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.GeneralUtilities.Test
{
    public class DateUtilitiesTester : BaseTester
    {
        public DateUtilitiesTester(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCestInMay()
        {
            var dateToTest = new DateTime(2018, 5, 17, 22, 3, 43);
            var result = dateToTest.FromUtcToSwedish();

            result.Should().Be(new DateTime(2018, 5, 18, 00, 3, 43));
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCestInOct()
        {
            var dateToTest = new DateTime(2018, 10, 27, 7, 3, 43);
            var result = dateToTest.FromUtcToSwedish();
            result.Should().Be(new DateTime(2018, 10, 27, 9, 3, 43));
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCetInFeb()
        {
            var dateToTest = new DateTime(2018, 2, 1, 2, 1, 27);
            var result = dateToTest.FromUtcToSwedish();
            result.Should().Be(new DateTime(2018, 2, 1, 3, 1, 27));
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCetInOct()
        {
            var dateToTest = new DateTime(2018, 10, 30, 2, 1, 27);
            var result = dateToTest.FromUtcToSwedish();
            result.Should().Be(new DateTime(2018, 10, 30, 3, 1, 27));
        }
    }
}