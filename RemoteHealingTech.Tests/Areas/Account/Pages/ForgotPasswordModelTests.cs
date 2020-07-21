using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Tests;
using HanumanInstitute.RemoteHealingTech.Areas.Account.Pages;
using HanumanInstitute.RemoteHealingTech.Models;
using HanumanInstitute.RemoteHealingTech.UnitTests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages.UnitTests
{
    public class ForgotPasswordModelTests
    {
        private readonly SetupContext _context = new SetupContext();
        private MockEmailSender _fakeEmailSender;

        private async Task<ForgotPasswordModel> SetupModelAsync()
        {
            _fakeEmailSender = new MockEmailSender();
            await SeedData.Initialize(_context.Database, _context.RoleManager, _context.UserManager);

            return new ForgotPasswordModel(_context.UserManager, _fakeEmailSender)
            {
                PageContext = _context.PageContext,
                Url = _context.UrlHelper
            };
        }

        [Fact]
        public async Task OnGet_NoException()
        {
            var model = await SetupModelAsync();

            model.OnGet();
        }

        [Fact]
        public async Task OnPost_ModelStateInvalid_ReturnsPage()
        {
            var model = await SetupModelAsync();
            model.ModelState.AddModelError("", "error");

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPost_UserNotFound_ReturnsRedirectToPage()
        {
            var model = await SetupModelAsync();
            model.Input = new ForgotPasswordModel.InputModel()
            {
                Email = "a@b.c"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPost_UserFound_SendEmailToUser()
        {
            var model = await SetupModelAsync();
            await _context.UserManager.SeedTestUserAsync();
            model.Input = new ForgotPasswordModel.InputModel()
            {
                Email = SeedTestData.Email
            };

            var result = await model.OnPostAsync();

            Assert.Single(_fakeEmailSender.Instances);
            var emailSender = Mock.Get<IEmailMessage>(_fakeEmailSender.Instances[0]);
            emailSender.Verify(x => x.SendAsync(), Times.Once);
            Assert.Equal(SeedTestData.Email, emailSender.Object.Mail.To[0].Address);
        }
    }
}
