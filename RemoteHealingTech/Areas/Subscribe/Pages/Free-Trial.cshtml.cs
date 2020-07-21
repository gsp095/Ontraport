using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Identity;
using HanumanInstitute.RemoteHealingTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages
{
    [Authorize(Roles = AppRoles.User)]
    public class FreeTrialModel : PageModel
    {
        public IList<DisplayTrialAvailable> Products { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        private readonly RemoteHealingTechDbContext _db;
        private readonly IDateTimeService _dateTimeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogoutManager _logoutManager;

        public FreeTrialModel(RemoteHealingTechDbContext dbContext, IDateTimeService dateTimeService, UserManager<ApplicationUser> userManager, ILogoutManager logoutManager)
        {
            _db = dbContext;
            _dateTimeService = dateTimeService;
            _userManager = userManager;
            _logoutManager = logoutManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return await _logoutManager.LogOutAsync(this);
            }

            var cart = GetCartForCustomer(user.CustomerId);
            await LoadProducts(user.CustomerId, cart);
            return Page();
        }

        private async Task LoadProducts(Guid customerId, Cart cart)
        {
            var query =
                from p in _db.Products
                where p.TrialDays.HasValue
                orderby p.DisplayOrder
                let existingTrial = p.Trials.FirstOrDefault(x => x.CustomerId == customerId)
                select new DisplayTrialAvailable(p)
                {
                    TrialAvailable = existingTrial == null,
                    Selected = existingTrial == null && cart != null && p.CartTrials.Any(x => x.CartId == cart.Id),
                    Message = existingTrial == null ?
                        (p.TrialDescription ?? $"Free trial for {p.TrialDays} days") :
                        $"Had trial on {existingTrial.StartDate:d}"
                };
            Products = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(IList<DisplayTrialAvailable> products)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return await _logoutManager.LogOutAsync(this);
            }

            // Load products and availability.
            var cart = GetCartForCustomer(user.CustomerId);
            await LoadProducts(user.CustomerId, cart);

            // Get list of valid selected trials.
            var selection = new List<Guid>();
            if (products != null)
            {
                foreach (var item in products.Where(x => x.Selected))
                {
                    if (Products.Any(x => x.Id == item.Id && x.TrialAvailable))
                    {
                        selection.Add(item.Id);
                    }
                }
            }

            // Store in cart, create if doesn't exist.
            if (cart != null)
            {
                cart.ModifiedDate = _dateTimeService.UtcNow;
            }
            else
            {
                cart = new Cart()
                {
                    CustomerId = user.CustomerId,
                    ModifiedDate = _dateTimeService.UtcNow
                };
                _db.Carts.Add(cart);
            }

            // Store cart trials, flush old data.
            var trials = _db.CartTrials.Where(x => x.CartId == cart.Id);
            _db.CartTrials.RemoveRange(trials);
            cart.CartTrials = new List<CartTrial>(
                selection.Select(x => new CartTrial()
                {
                    CartId = cart.Id,
                    ProductId = x
                }));
            await _db.SaveChangesAsync();

            // Redirect.
            if (selection.Any())
            {
                return RedirectToPage("Pictures");
            }
            else
            {
                StatusMessage = "You must select at least one service.";
                return Page();
            }
        }

        private Cart GetCartForCustomer(Guid customerId)
        {
            return _db.Carts.FirstOrDefault(x => x.CustomerId == customerId);
        }
    }
}
