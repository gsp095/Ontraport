using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Validation;
using HanumanInstitute.RemoteHealingTech.Areas.Account.Pages;
using HanumanInstitute.RemoteHealingTech.Models;
using HanumanInstitute.RemoteHealingTech.UnitTests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages.UnitTests
{
    public class ResetPasswordModelTests
    {
        private readonly SetupContext _context = new SetupContext();

        private async Task<ResetPasswordModel> SetupModelAsync()
        {
            await SeedData.Initialize(_context.Database, _context.RoleManager, _context.UserManager);

            return new ResetPasswordModel(_context.UserManager)
            {
                PageContext = _context.PageContext,
                Url = _context.UrlHelper
            };
        }

        [Fact]
        public async Task OnGet_NoCode_ReturnsBadRequest()
        {
            var model = await SetupModelAsync();

            var result = model.OnGet();

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task OnGet_WithCode_ReturnsPageWithCode()
        {
            var model = await SetupModelAsync();
            var code = "aaa";

            var result = model.OnGet(code);

            Assert.IsType<PageResult>(result);
            Assert.Equal(code, model.Input.Code);
        }

        [Fact]
        public async Task OnPostAsync_InvalidState_ReturnsPage()
        {
            var model = await SetupModelAsync();
            model.ModelState.AddModelError("", "error");

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_UserNotFound_RedirectToPage()
        {
            var model = await SetupModelAsync();
            model.Input = new ResetPasswordModel.InputModel()
            {
                Code = "111111",
                Email = SeedTestData.Email,
                Password = SeedTestData.Password,
                ConfirmPassword = SeedTestData.Password
            };

            var result = await model.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_InvalidToken_ModelStateContainsError()
        {
            var model = await SetupModelAsync();
            await _context.UserManager.SeedTestUserAsync();
            model.Input = new ResetPasswordModel.InputModel()
            {
                Code = "111111",
                Email = SeedTestData.Email,
                Password = SeedTestData.Password,
                ConfirmPassword = SeedTestData.Password
            };

            var result = await model.OnPostAsync();

            Assert.Single(model.ModelState);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ValidToken_RedirectToPage()
        {
            var model = await SetupModelAsync();
            var user = await _context.UserManager.SeedTestUserAsync();
            var code = await _context.UserManager.GeneratePasswordResetTokenAsync(user);
            model.Input = new ResetPasswordModel.InputModel()
            {
                Code = code,
                Email = SeedTestData.Email,
                Password = SeedTestData.Password,
                ConfirmPassword = SeedTestData.Password
            };

            var result = await model.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WrongPasswordConfirm_ReturnsPageWithModelStateError()
        {
            var model = await SetupModelAsync();
            var user = await _context.UserManager.SeedTestUserAsync();
            var code = await _context.UserManager.GeneratePasswordResetTokenAsync(user);
            model.Input = new ResetPasswordModel.InputModel()
            {
                Code = code,
                Email = SeedTestData.Email,
                Password = SeedTestData.Password,
                ConfirmPassword = "wrong"
            };
            model.ValidateToModelState();

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Single(model.ModelState);
        }
    }
}
