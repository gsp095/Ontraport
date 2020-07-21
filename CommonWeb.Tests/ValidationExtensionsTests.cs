using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Validation;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class ValidationExtensionsTests
    {
        private static Dictionary<string, string> CreateValidDictionary()
        {
            return new Dictionary<string, string>()
            {
                {"Email:MailFrom", "a@b.c"},
                {"Email:MailFromName", "a" },
                {"Email:MailServer", "smtp.server.com"}
            };
        }

        private static Dictionary<string, string> CreateInvalidDictionary()
        {
            return new Dictionary<string, string>()
            {
                {"Email:MailFrom", "a@b.c"},
                {"Email:MailServer", "" }
            };
        }

        private static EmailConfig CreateValid()
        {
            return new EmailConfig()
            {
                MailFrom = "a@b.c",
                MailFromName = "a",
                MailServer = "smtp.server.com"
            };
        }

        private static EmailConfig CreateInvalid()
        {
            return new EmailConfig();
        }

        [Fact]
        public void GetValid_Valid_ReturnsConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(CreateValidDictionary())
                .Build();

            var result = configuration.GetSection("Email").GetValid<EmailConfig>();

            Assert.NotNull(result);
            Assert.IsType<EmailConfig>(result);
            Assert.NotNull(result.MailServer);
        }

        [Fact]
        public void GetValid_Invalid_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(CreateInvalidDictionary())
                .Build();

            void Act() => configuration.GetSection("Email").GetValid<EmailConfig>();

            var exception = Assert.Throws<ValidationException>(Act);
        }

        [Fact]
        public void ValidateAndThrow_Valid_ReturnsSameObject()
        {
            var obj = CreateValid();

            var result = obj.ValidateAndThrow();

            Assert.Equal(obj, result);
        }

        [Fact]
        public void ValidateAndThrow_Invalid_ThrowsException()
        {
            var obj = CreateInvalid();

            Assert.Throws<ValidationException>(() => obj.ValidateAndThrow());
        }

        [Fact]
        public void Validate_Valid_ReturnsNull()
        {
            var obj = CreateValid();

            var result = obj.Validate();

            Assert.Null(result);
        }

        [Fact]
        public void Validate_Invalid_ReturnsList()
        {
            var obj = CreateInvalid();

            var result = obj.Validate();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
