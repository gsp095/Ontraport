using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using HanumanInstitute.RemoteHealingTech.Areas.Account.Pages;
using HanumanInstitute.RemoteHealingTech.Models;
using HanumanInstitute.RemoteHealingTech.UnitTests.Utilities;
using Xunit;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages.UnitTests
{
    public class LoginModelTests
    {
        private readonly SetupContext _context = new SetupContext();
        private Mock<ILogger<LoginModel>> _mockLogger;

        private const string TestEmail = SeedTestData.Email;
        private const string TestPassword = SeedTestData.Password;

        private async Task<LoginModel> SetupModelAsync()
        {
            _mockLogger = new Mock<ILogger<LoginModel>>();
            await SeedData.Initialize(_context.Database, _context.RoleManager, _context.UserManager);
            return new LoginModel(_context.SignInManager, _mockLogger.Object)
            {
                PageContext = _context.PageContext,
                Url = _context.UrlHelper
            };
        }

        [Theory]
        [InlineData("Error Message")]
        public async Task OnGetAsync_HasErrorMessage_ErrorInModelState(string errorMessage)
        {
            var model = await SetupModelAsync();
            model.ErrorMessage = errorMessage;

            await model.OnGetAsync();

            Assert.Single(model.ModelState);
        }

        [Theory]
        [InlineData("returnurl.html")]
        public async Task OnGetAsync_HasReturnUrl_ReturnUrlSetOnModel(string returnUrl)
        {
            var model = await SetupModelAsync();

            await model.OnGetAsync(returnUrl);

            Assert.Equal(returnUrl, model.ReturnUrl);
        }

        [Fact]
        public async Task OnGetAsync_HasNoReturnUrl_DefaultReturnUrlSetOnModel()
        {
            var model = await SetupModelAsync();

            await model.OnGetAsync();

            Assert.NotEmpty(model.ReturnUrl);
        }

        [Fact]
        public async Task OnPostAsync_ValidCredentials_ReturnsLocalRedirectResult()
        {
            var model = await SetupModelAsync();
            await _context.UserManager.SeedTestUserAsync();
            model.Input = new LoginModel.InputModel()
            {
                Email = TestEmail,
                Password = TestPassword,
                RememberMe = false
            };

            var result = await model.OnPostAsync();

            Assert.IsType<LocalRedirectResult>(result);
        }

        [Theory]
        [InlineData("a@b.c", "b")]
        [InlineData("a", "test")]
        [InlineData("", "b")]
        [InlineData("a@b.c", "")]
        [InlineData(null, null)]
        public async Task OnPostAsync_InvalidCredentials_ReturnsPage(string email, string password)
        {
            var model = await SetupModelAsync();
            model.Input = new LoginModel.InputModel()
            {
                Email = email,
                Password = password,
                RememberMe = false
            };
            model.ValidateToModelState();

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
        }
    }
}
