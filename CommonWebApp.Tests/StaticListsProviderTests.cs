using System;
using Moq;
using Xunit;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public class StaticListsProviderTests
    {
        public StaticListsProvider Model => _model ??= new StaticListsProvider(
            Mock.Of<IDateTimeService>(x => x.UtcNow == new DateTime(2020, 01, 01))
        );
        private StaticListsProvider? _model;

        [Fact]
        public void ExpirationMonths_Get_ReturnsList()
        {
            var result = Model.ExpirationMonths;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ExpirationYears_Get_ReturnsList()
        {
            var result = Model.ExpirationYears;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void States_Get_ReturnsList()
        {
            var result = Model.States;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Couuntries_Get_ReturnsList()
        {
            var result = Model.Countries;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
