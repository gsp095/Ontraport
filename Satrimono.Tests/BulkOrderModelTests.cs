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
    public class BulkOrderModelTests
    {
        private MockEmailSender _emailService;

        private BulkOrderModel SetupModelInvalid()
        {
            _emailService = new MockEmailSender();
            var model = new BulkOrderModel(_emailService);
            model.SetModelStateInvalid();
            return model;
        }

        private BulkOrderModel SetupModelValid()
        {
            _emailService = new MockEmailSender();
            return new BulkOrderModel(_emailService)
            {
                Name = "name",
                Email = "name@email.com",
                City = "city",
                Country = "country",
                BookName = "book",
                Quantity = 10,
                Purpose = "purpose",
                Reference = "reference",
                AdditionalInformation = "additional information"
            };
        }

        [Fact]
        public void Constructor_NullParam_ThrowsException()
        {
            void act() => new BulkOrderModel(null);

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

            Assert.Equal("Bulk Order", _emailService.Instances[0].Mail.Subject);
            _emailService.Instances[0].Mail.Body.Contains(model.AdditionalInformation);
        }
    }
}
