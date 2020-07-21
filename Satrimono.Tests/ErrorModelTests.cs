using System;
using System.Collections.Generic;
using System.Text;
using Satrimono.Pages;
using Xunit;

namespace Satrimono.UnitTests
{
    public class ErrorModelTests
    {
        private const string DefaultCodeName = "Error";

        [Fact]
        public void OnGet_Null_CodeNameError()
        {
            var model = new ErrorModel();

            model.OnGet(null);

            Assert.Null(model.Code);
            Assert.Equal(DefaultCodeName, model.CodeName);
        }

        [Theory]
        [InlineData(404)]
        public void OnGet_ValidCode_CodeNameDescription(int code)
        {
            var model = new ErrorModel();

            model.OnGet(code);

            Assert.NotNull(model.Code);
            Assert.NotNull(model.CodeName);
            Assert.NotEqual(DefaultCodeName, model.CodeName);
        }
    }
}
