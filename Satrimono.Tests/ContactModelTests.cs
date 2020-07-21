using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using EmergenceGuardian.WebCommon.Email;
using EmergenceGuardian.WebCommon.UnitTests;
using EmergenceGuardian.WebTools.UnitTests;
using Satrimono.Pages;
using Xunit;
using Moq;

namespace Satrimono.UnitTests
{
    public class ContactModelTests
    {
        private MockEmailSender _emailService;

        private ContactModel SetupModelInvalid()
        {
            _emailService = new MockEmailSender();
            var model = new ContactModel(_emailService);
            model.SetModelStateInvalid();
            return model;
        }

        private ContactModel SetupModelValid()
        {
            _emailService = new MockEmailSender();
            return new ContactModel(_emailService)
            {
                Input = new ContactModel.InputModel()
                {
                    Name = "name",
                    Email = "name@email.com",
                    Message = "message",
                    Challenge = "four"
                }
            };
        }

        [Fact]
        public void Constructor_NullParam_ThrowsException()
        {
            void act() => new ContactModel(null);

            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Constructor_Valid_NoException()
        {
            SetupModelValid();
        }

        [Fact]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            var model = SetupModelInvalid();

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ValidModel_SendEmailAndRedirect()
        {
            var model = SetupModelValid();

            var result = await model.OnPostAsync();

            Assert.Single(_emailService.Instances);
            Mock.Get<IEmailMessage>(_emailService.Instances[0]).Verify(x => x.SendAsync(), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ValidModel_EmailHasRightContent()
        {
            var model = SetupModelValid();

            await model.OnPostAsync();

            Assert.Equal("Contact Us", _emailService.Instances[0].Mail.Subject);
            _emailService.Instances[0].Mail.Body.Contains(model.Input.Message);
        }
    }
}
