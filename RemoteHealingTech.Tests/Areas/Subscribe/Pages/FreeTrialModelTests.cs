using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Identity;
using HanumanInstitute.RemoteHealingTech.Models;
using HanumanInstitute.RemoteHealingTech.UnitTests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages.UnitTests
{
    public class FreeTrialModelTests
    {
        private readonly SetupContext _context = new SetupContext();
        private Mock<IDateTimeService> _mockDateTimeService;
        private Mock<ILogoutManager> _mockLogoutManager;
        private RemoteHealingTechDbContext _db;
        private Customer _customer;

        private async Task<FreeTrialModel> SetupModelAsync()
        {
            _mockDateTimeService = new Mock<IDateTimeService>();
            _mockLogoutManager = new Mock<ILogoutManager>();
            await SeedData.Initialize(_context.Database, _context.RoleManager, _context.UserManager);

            _db = _context.Database;
            var user = _context.UserManager.Users.First();
            _context.SetUser(user.Id);
            _customer = _db.Customers.Where(c => c.Id == user.CustomerId).FirstOrDefault();
            return new FreeTrialModel(_context.Database, _mockDateTimeService.Object, _context.UserManager, _mockLogoutManager.Object)
            {
                PageContext = _context.PageContext
            };
        }

        private async Task<Customer> SeedCustomerAsync()
        {
            var customer = new Customer();
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        private async Task<List<Guid>> SeedCartAsync(int[] productIndexes)
        {
            var productIds = _db.Products.Select(p => p.Id).ToList();
            var products = new List<Guid>(
                productIndexes.Select(i => productIds[i]));
            var cart = new Cart() { CustomerId = _customer.Id };
            cart.CartTrials = products.Select(p => new CartTrial()
            {
                ProductId = p
            }).ToList();
            await _db.Carts.AddAsync(cart);
            await _db.SaveChangesAsync();
            return products;
        }

        private async Task<List<Guid>> SeedTrialsAsync(int[] productIndexes)
        {
            var productIds = _db.Products.Select(p => p.Id).ToList();
            var products = new List<Guid>(
                productIndexes.Select(i => productIds[i]));
            await _db.Trials.AddRangeAsync(new List<Trial>(
                products.Select(p => new Trial()
                {
                    CustomerId = _customer.Id,
                    ProductId = p
                })));
            await _db.SaveChangesAsync();
            return products;
        }

        [Fact]
        public async Task OnGetAsync_CustomerCompletedNoTrial_AllTrialsAvailable()
        {
            var model = await SetupModelAsync();
            var productCount = _db.Products.Count();

            await model.OnGetAsync();

            Assert.NotNull(model.Products);
            Assert.Equal(productCount, model.Products.Count());
            foreach (var item in model.Products)
            {
                Assert.True(item.TrialAvailable);
            }
        }

        [Theory]
        [InlineData(new int[] { 0 })]
        [InlineData(new int[] { 1, 2 })]
        [InlineData(new int[] { 0, 2, 3, 4, 5 })]
        public async Task OnGetAsync_CustomerCompletedTrials_ThoseTrialsPresentButUnavailable(int[] productIndexes)
        {
            var model = await SetupModelAsync();

            var productIds = _db.Products.Select(p => p.Id).ToList();
            var products = new List<Guid>(
                productIndexes.Select(i => productIds[i]));
            _customer.Trials = new List<Trial>(
                products.Select(p => new Trial()
                {
                    ProductId = p
                }));
            await _db.SaveChangesAsync();
            var productCount = _db.Products.Count();

            await model.OnGetAsync();

            Assert.Equal(productCount, model.Products.Count());
            foreach (var item in model.Products)
            {
                Assert.Equal(!products.Contains(item.Id), item.TrialAvailable);
            }
        }

        [Theory]
        [InlineData(new int[] { 0 })]
        [InlineData(new int[] { 1, 2 })]
        [InlineData(new int[] { 0, 2, 3, 4, 5 })]
        public async Task OnGetAsync_TrialsInCart_ThoseTrialsSelected(int[] productIndexes)
        {
            var model = await SetupModelAsync();
            var trialProductIds = await SeedCartAsync(productIndexes);

            await model.OnGetAsync();

            foreach (var item in model.Products)
            {
                Assert.Equal(trialProductIds.Contains(item.Id), item.Selected);
            }
        }

        [Fact]
        public async Task OnPostAsync_Null_ShowsErrorMessage()
        {
            var model = await SetupModelAsync();

            await model.OnPostAsync(null);

            Assert.NotEmpty(model.StatusMessage);
        }

        [Fact]
        public async Task OnPostAsync_EmptyList_ShowsErrorMessage()
        {
            var model = await SetupModelAsync();

            await model.OnPostAsync(new List<DisplayTrialAvailable>());

            Assert.NotEmpty(model.StatusMessage);
        }

        [Fact]
        public async Task OnPostAsync_InvalidProductsSelected_ShowsErrorMessage()
        {
            var model = await SetupModelAsync();
            var products = new List<DisplayTrialAvailable>()
            {
                new DisplayTrialAvailable()
                {
                    Id = Guid.NewGuid(),
                    Selected = true
                }
            };

            await model.OnPostAsync(products);

            Assert.NotEmpty(model.StatusMessage);
        }

        [Fact]
        public async Task OnPostAsync_NoProductSelected_ShowErrorMessage()
        {
            var model = await SetupModelAsync();
            var products = _db.Products.Select(p => new DisplayTrialAvailable(p)).ToList();

            var result = await model.OnPostAsync(products);

            Assert.NotEmpty(model.StatusMessage);
        }

        [Fact]
        public async Task OnPostAsync_ValidProductSelected_Redirect()
        {
            var model = await SetupModelAsync();
            var products = _db.Products.Select(p => new DisplayTrialAvailable(p)).ToList();
            products[0].Selected = true;

            var result = await model.OnPostAsync(products);

            Assert.IsType<RedirectToPageResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task OnPostAsync_ValidProductSelected_CartCreated(int productIndex)
        {
            var model = await SetupModelAsync();
            var products = _db.Products.Select(p => new DisplayTrialAvailable(p)).ToList();
            var selection = products[productIndex];
            selection.Selected = true;
            var cartUpdateDate = new DateTime(2020, 01, 01);
            _mockDateTimeService.Setup(x => x.UtcNow).Returns(cartUpdateDate);

            var result = await model.OnPostAsync(products);

            var cart = _db.Carts.Where(x => x.CustomerId == _customer.Id).ToList();
            Assert.Single(cart);
            Assert.Equal(cartUpdateDate, cart.First().ModifiedDate);
            var cartTrials = _db.CartTrials.Where(x => x.CartId == cart.First().Id).ToList();
            Assert.Single(cartTrials);
            Assert.Equal(selection.Id, cartTrials.First().ProductId);
        }

        [Fact]
        public async Task OnPostAsync_AllProductsSelected_AllProductsInCart()
        {
            var model = await SetupModelAsync();
            var products = _db.Products.Select(p => new DisplayTrialAvailable(p) { Selected = true }).ToList();

            var result = await model.OnPostAsync(products);

            var cart = _db.Carts.Where(x => x.CustomerId == _customer.Id).FirstOrDefault();
            var cartProducts = _db.CartTrials.Where(x => x.CartId == cart.Id).Select(x => x.ProductId).ToList();
            foreach (var item in products)
            {
                Assert.True(cartProducts.Contains(item.Id), "Product should be in cart.");
            }
        }

        [Fact]
        public async Task OnPostAsync_CartExists_CartUpdated()
        {
            var model = await SetupModelAsync();
            await SeedCartAsync(new int[] { 0 });
            var products = _db.Products.Select(p => new DisplayTrialAvailable(p)).ToList();
            products[1].Selected = true;
            products[2].Selected = true;
            var cartUpdateDate = new DateTime(2020, 01, 01);
            _mockDateTimeService.Setup(x => x.UtcNow).Returns(cartUpdateDate);

            var result = await model.OnPostAsync(products);

            var cart = _db.Carts.Where(x => x.CustomerId == _customer.Id).ToList();
            Assert.Single(cart);
            Assert.Equal(cartUpdateDate, cart.First().ModifiedDate);
            var cartTrials = _db.CartTrials.Where(x => x.CartId == cart.First().Id).Select(x => x.ProductId).ToList();
            Assert.Contains(products[1].Id, cartTrials);
            Assert.Contains(products[2].Id, cartTrials);
            Assert.Equal(2, cartTrials.Count);
        }

        [Fact]
        public async Task OnPostAsync_HasExistingTrial_ThatTrialNotInCart()
        {
            var model = await SetupModelAsync();
            var trialProductIds = await SeedTrialsAsync(new int[] { 0 });
            var products = _db.Products.Select(p => new DisplayTrialAvailable(p) { Selected = true }).ToList();
            var existingTrial = products[0].Id;

            var result = await model.OnPostAsync(products);

            var cart = _db.Carts.Where(x => x.CustomerId == _customer.Id).FirstOrDefault();
            var cartProducts = _db.CartTrials.Where(x => x.CartId == cart.Id).Select(x => x.ProductId).ToList();
            foreach (var item in products)
            {
                Assert.Equal(!trialProductIds.Contains(item.Id), cartProducts.Contains(item.Id));
            }
        }
    }
}
