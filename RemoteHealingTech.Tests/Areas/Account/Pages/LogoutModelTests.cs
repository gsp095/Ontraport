using System;
using System.Threading.Tasks;
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
    public class LogoutModelTests
    {
        private readonly SetupContext _context = new SetupContext();
        private Mock<ILogger<LogoutModel>> _mockLogger;

        private async Task<LogoutModel> SetupModelAsync()
        {
            _mockLogger = new Mock<ILogger<LogoutModel>>();
            await SeedData.Initialize(_context.Database, _context.RoleManager, _context.UserManager);
            return new LogoutModel(_context.SignInManager, _mockLogger.Object);
        }

        [Fact]
        public async Task OnGet_NoException()
        {
            var model = await SetupModelAsync();

            model.OnGet();
        }

        [Fact]
        public async Task OnPost_NoReturnUrl_ReturnsPage()
        {
            var model = await SetupModelAsync();

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPost_WithReturnUrl_ReturnsLocalRedirect()
        {
            var model = await SetupModelAsync();

            var result = await model.OnPostAsync("/");

            Assert.IsType<LocalRedirectResult>(result);
        }
    }
}
