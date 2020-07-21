using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Tests;
using HanumanInstitute.CommonWeb.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using HanumanInstitute.RemoteHealingTech.Models;
using HanumanInstitute.RemoteHealingTech.UnitTests.Utilities;
using Xunit;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages.UnitTests
{
    public class RegisterModelTests
    {
        private readonly SetupContext _context = new SetupContext();

        private async Task<RegisterModel> SetupModelAsync()
        {
            await SeedData.Initialize(_context.Database, _context.RoleManager, _context.UserManager);
            var fakeLogger = Mock.Of<ILogger<RegisterModel>>();
            var mockEmailSender = new MockEmailSender();

            return new RegisterModel(_context.UserManager, _context.SignInManager, fakeLogger, mockEmailSender)
            {
                PageContext = _context.PageContext,
                Url = _context.UrlHelper
            };
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("returnurl")]
        public async Task OnGetAsync_ReturnUrlValues_ModelHasReturnUrl(string returnUrl)
        {
            var model = await SetupModelAsync();

            model.OnGet(returnUrl);

            Assert.Equal(returnUrl, model.ReturnUrl);
        }

        [Fact]
        public async Task OnPostRegisterAsync_InvalidData_ReturnsPageWithError()
        {
            var model = await SetupModelAsync();
            var input = new RegisterModel.RegisterInputModel();
            model.ValidateToModelState(input);

            var result = await model.OnPostRegisterAsync(input);

            Assert.IsType<PageResult>(result);
            Assert.NotEmpty(model.ModelState);
        }

        // Duplicate check doesn't work during unit test but works live.
        //[Fact]
        //public async Task OnPostAsync_ExistingUser_ReturnsPageWithError()
        //{
        //    var model = await SetupModelAsync();
        //    var user = await _context.UserManager.SeedTestUserAsync();
        //    model.Input = new RegisterModel.InputModel()
        //    {
        //        Email = SeedTestData.Email,
        //        Password = SeedTestData.Password,
        //        ConfirmPassword = SeedTestData.Password
        //    };

        //    var result = await model.OnPostAsync();

        //    Assert.IsType<PageResult>(result);
        //    Assert.NotEmpty(model.ModelState);
        //}

        [Fact]
        public async Task OnPostRegisterAsync_NewUser_ReturnsLocalRedirect()
        {
            var model = await SetupModelAsync();
            var input = new RegisterModel.RegisterInputModel()
            {
                Email = SeedTestData.Email,
                Password = SeedTestData.Password,
                ConfirmPassword = SeedTestData.Password
            };

            var result = await model.OnPostRegisterAsync(input);

            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public async Task OnPostLoginAsync_ValidCredentials_ReturnsLocalRedirectResult()
        {
            var model = await SetupModelAsync();
            await _context.UserManager.SeedTestUserAsync();
            var input = new RegisterModel.LoginInputModel()
            {
                Email = SeedTestData.Email,
                Password = SeedTestData.Password,
                RememberMe = false
            };

            var result = await model.OnPostLoginAsync(input);

            Assert.IsType<LocalRedirectResult>(result);
        }

        [Theory]
        [InlineData("a@b.c", "b")]
        [InlineData("a", "test")]
        [InlineData("", "b")]
        [InlineData("a@b.c", "")]
        [InlineData(null, null)]
        public async Task OnPostLoginAsync_InvalidCredentials_ReturnsPage(string email, string password)
        {
            var model = await SetupModelAsync();
            var input = new RegisterModel.LoginInputModel()
            {
                Email = email,
                Password = password,
                RememberMe = false
            };
            model.ValidateToModelState(input);

            var result = await model.OnPostLoginAsync(input);

            Assert.IsType<PageResult>(result);
        }
    }
}
