using System;
using Satrimono.Pages.Shared;
using Xunit;

namespace Satrimono.UnitTests.Shared
{
    public class HeaderModelTests
    {
        [Fact]
        public void Constructor_NoParam_ActiveIndexNull()
        {
            var model = new _HeaderModel();

            Assert.Null(model.ActiveIndex);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void Constructor_WithParam_ReturnsSameActiveIndex(int? activeIndex)
        {
            var model = new _HeaderModel(activeIndex);

            Assert.Equal(activeIndex, model.ActiveIndex);
        }
    }
}
