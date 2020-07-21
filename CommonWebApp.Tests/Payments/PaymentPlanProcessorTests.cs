using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.OntraportApi.Models;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public partial class PaymentPlanProcessorTests : IDisposable
    {
        private const int CampaignId = 1;
        private readonly DateTimeOffset _now = new DateTimeOffset(2020, 01, 10, 8, 0, 0, TimeSpan.Zero);
        private const decimal ProductPrice = 100;
        private const string ProductNameA = "Product A", ProductNameB = "Product B";

        public Mock<IOntraportContacts> MockContacts => _mockContacts ??= new Mock<IOntraportContacts>();
        private Mock<IOntraportContacts>? _mockContacts;

        public Mock<IOntraportPaymentPlans> MockPlans => _mockPlans ??= new Mock<IOntraportPaymentPlans>();
        private Mock<IOntraportPaymentPlans>? _mockPlans;

        public Mock<IPaymentProcessor> MockProcessor => _mockProcessor ??= new Mock<IPaymentProcessor>();
        private Mock<IPaymentProcessor>? _mockProcessor;

        public IOptions<PaymentProcessorConfig> Config => _config ??= Mock.Of<IOptions<PaymentProcessorConfig>>(x => x.Value == new PaymentProcessorConfig() { PaymentPlanCampaignId = CampaignId });
        private IOptions<PaymentProcessorConfig>? _config;

        public PaymentPlanProcessor Model => _model ??= new PaymentPlanProcessor(MockProcessor.Object, MockContacts.Object, MockPlans.Object, new RandomGenerator(), Config);
        private PaymentPlanProcessor? _model;

        [NotNull]
        public List<int>? ProcessCalled { get; private set; }

        public Mock<PaymentPlanProcessor> CreateMockModelForQueue()
        {
            ProcessCalled = new List<int>();
            var mockModel = new Mock<PaymentPlanProcessor>(MockProcessor.Object, MockContacts.Object, MockPlans.Object, new RandomGenerator(), Config) { CallBase = true };
            mockModel.Setup(x => x.ProcessAsync(It.IsAny<ApiCustomContact>(), It.IsAny<IEnumerable<ApiPaymentPlan>>(), It.IsAny<DateTimeOffset>()))
                .Returns<ApiCustomContact, IEnumerable<ApiPaymentPlan>, DateTimeOffset>((contact, planList, currentDate) =>
                {
                    ProcessCalled.Add(contact.Id!.Value);
                    return Task.FromResult(new PaymentResult());
                });
            MockContacts.Setup(x => x.SelectAsync(It.IsAny<int>()))
                .Returns<int>((id) => Task.FromResult(new ApiCustomContact() { Id = id }));
            MockPlans.Setup(x => x.SelectAsync(It.IsAny<ApiSearchOptions>()))
                .Returns(Task.FromResult<IList<ApiPaymentPlan>>(new List<ApiPaymentPlan>()));
            return mockModel;
        }

        public static ApiCustomContact CreateContact(int? collectionPlanId = null, bool paid = true, decimal credits = 0, PaymentMethod paymentMethod = PaymentMethod.CreditCard, Currency paymentCurrency = Currency.Usd, int? lastMasterId = null)
        {
            var result = new ApiCustomContact()
            {
                Id = 1,
                CollectionPlanId = collectionPlanId,
                HasPaidHealingTechnologies = paid,
                AccountCredits = credits,
                RecurringPaymentMethod = paymentMethod,
                RecurringPaymentCurrency = paymentCurrency,
                RecurringLastMasterId = lastMasterId
            };
            result.ClearChanges();
            return result;
        }

        public static List<ApiPaymentPlan> CreatePlansWithCampaign()
        {
            var result = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan() { Id = 1 },
                new ApiPaymentPlan() { Id = 2 }
            };
            foreach (var item in result)
            {
                item.ListCampaigns.Add(CampaignId);
                item.ListCampaignsField.Changed();
            }
            return result;
        }

        private DateTimeOffset NowMonth => new DateTimeOffset(_now.Year, _now.Month, 1, 0, 0, 0, TimeSpan.Zero);

        public static IEnumerable<object[]> DiffHours => new[] {
            new object[] { -12 },
            new object[] { 0 },
            new object[] { 12 },
            new object[] { 48 }
        };

        [Fact]
        public async Task ResetCollection_ContactNotInCollection_ReturnsTrue()
        {
            var contact = CreateContact();
            var plans = CreatePlansWithCampaign();

            var result = await Model.ResetCollectionAsync(contact, plans, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task ResetCollection_ContactNotInCollection_DoNotSetCollectionPlan()
        {
            var contact = CreateContact();
            var plans = CreatePlansWithCampaign();

            await Model.ResetCollectionAsync(contact, plans, 1);

            Assert.Null(contact.CollectionPlanId);
        }

        [Fact]
        public async Task ResetCollection_ContactNotInCollection_DoNotDisableOtherPlans()
        {
            var contact = CreateContact();
            var plans = CreatePlansWithCampaign();

            await Model.ResetCollectionAsync(contact, plans, 1);

            MockPlans.Verify(x => x.RemoveFromCampaignAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ResetCollection_InCollectionByThisPlan_DisableOtherPlans()
        {
            var contact = CreateContact(1);
            var plans = CreatePlansWithCampaign();

            await Model.ResetCollectionAsync(contact, plans, 1);

            MockPlans.Verify(x => x.RemoveFromCampaignAsync(1, It.IsAny<int>()), Times.Never);
            MockPlans.Verify(x => x.RemoveFromCampaignAsync(2, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ResetCollection_InCollectionByOtherPlan_DisableThisPlan()
        {
            var contact = CreateContact(2);
            var plans = CreatePlansWithCampaign();

            await Model.ResetCollectionAsync(contact, plans, 1);

            MockPlans.Verify(x => x.RemoveFromCampaignAsync(1, It.IsAny<int>()), Times.Once);
            MockPlans.Verify(x => x.RemoveFromCampaignAsync(2, It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ResetCollection_InCollectionByOtherPlan_ReturnFalse()
        {
            var contact = CreateContact(2);
            var plans = CreatePlansWithCampaign();

            var result = await Model.ResetCollectionAsync(contact, plans, 1);

            Assert.False(result);
        }

        [Fact]
        public async Task ResetCollection_InCollectionAlreadyDisabled_DoesNotRemoveFromCampaign()
        {
            var contact = CreateContact(2);
            var plans = CreatePlansWithCampaign();
            plans[0].ListCampaigns.Clear();

            await Model.ResetCollectionAsync(contact, plans, 1);

            MockPlans.Verify(x => x.RemoveFromCampaignAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ProcessInQueueAsync_Once_CallsProcess()
        {
            var mockModel = CreateMockModelForQueue();

            await mockModel.Object.ProcessInQueueAsync(0, 0, _now);

            Assert.Single(ProcessCalled);
        }

        [Fact]
        public async Task ProcessInQueueAsync_FiftyTimes_CallsFiftyTimesInOrder()
        {
            var mockModel = CreateMockModelForQueue();

            var tasks = Enumerable.Range(0, 50).Select(x => mockModel.Object.ProcessInQueueAsync(x, 0, _now)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            for (var i = 0; i < 50; i++)
            {
                Assert.Equal(i, ProcessCalled[i]);
            }
        }

        [Fact]
        public async Task ProcessInQueueAsync_LongTask_WaitUntilFinish()
        {
            var mockModel = CreateMockModelForQueue();
            MockPlans.Setup(x => x.SelectAsync(It.IsAny<ApiSearchOptions>()))
                .Returns(async () =>
                {
                    await Task.Delay(100);
                    return new List<ApiPaymentPlan>();
                });

            var task1 = mockModel.Object.ProcessInQueueAsync(1, 0, _now).ConfigureAwait(false);
            var task2 = mockModel.Object.ProcessInQueueAsync(2, 0, _now).ConfigureAwait(false);
            var task3 = mockModel.Object.ProcessInQueueAsync(3, 0, _now).ConfigureAwait(false);

            var result1 = await task1;
            Assert.Single(ProcessCalled);
            var result2 = await task2;
            Assert.Equal(2, ProcessCalled.Count);
            await task3;
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _model?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
