using System;
using System.Collections;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HanumanInstitute.CommonWeb.Tests
{
    /// <summary>
    /// Configures the database, .Net Core Identity Framework and page context for unit testing.
    /// </summary>
    /// <typeparam name="T">The type of IdentityUser used for identity framework.</typeparam>
    /// <typeparam name="TRole">The type of IdentityRole used for identity framework.</typeparam>
    /// <typeparam name="TContext">The DbContext database type.</typeparam>
    /// <typeparam name="TKey">The data type of identity framework primary keys.</typeparam>
    public class SetupContext<T, TRole, TContext, TKey> : IDisposable
        where T : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>
        where TContext : DbContext
        where TKey : IEquatable<TKey>
    {
        public SetupContext() { }

        public TContext Database => _database ??= (TContext)Activator.CreateInstance(typeof(TContext), new[] { InMemoryDbContext.GetTestDbOptions<TContext>() })!;
        private TContext? _database;

        public IUserRoleStore<T> UserStore => _userStore ??= new UserStore<T, TRole, TContext, TKey>(Database);
        private IUserRoleStore<T>? _userStore;

        public IRoleStore<TRole> RoleStore => _roleStore ??= new RoleStore<TRole, TContext, TKey>(Database);
        private IRoleStore<TRole>? _roleStore;

        public RoleManager<TRole> RoleManager => _roleManager ??= new RoleManager<TRole>(RoleStore, null, null, null, null);
        private RoleManager<TRole>? _roleManager;

        public PageContext PageContext => _pageContext ??= new PageContext(ActionContext);
        private PageContext? _pageContext;

        public ActionContext ActionContext => _actionContext ??= new ActionContext(HttpContext, new RouteData(), new PageActionDescriptor(), ModelState);
        private ActionContext? _actionContext;

        public ModelStateDictionary ModelState => _modelState ??= new ModelStateDictionary();
        private ModelStateDictionary? _modelState;

        public IModelMetadataProvider ModelMetadataProvider => _modelMetadataProvider ??= new EmptyModelMetadataProvider();
        private IModelMetadataProvider? _modelMetadataProvider;

        public ViewDataDictionary ViewData => _viewData ??= new ViewDataDictionary(ModelMetadataProvider, ModelState);
        private ViewDataDictionary? _viewData;

        public TempDataDictionary TempData => _tempData ??= new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
        private TempDataDictionary? _tempData;

        public Mock<IServiceProvider> ServiceProviderMock => _serviceProviderMock ??= new Mock<IServiceProvider>();
        private Mock<IServiceProvider>? _serviceProviderMock;

        public HttpContext HttpContext => _httpContext ??=
            new DefaultHttpContext()
            {
                RequestServices = ServiceProviderMock.Object
            };
        private HttpContext? _httpContext;

        public IUrlHelper UrlHelper
        {
            get
            {
                if (_urlHelper == null)
                {
                    var urlHelperMock = new Mock<UrlHelper>(ActionContext);
                    urlHelperMock.Setup(x => x.Content(It.IsAny<string>())).Returns<string>((content) => content);
                    urlHelperMock.Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>())).Returns<UrlRouteContext>((route) =>
                    {
                        var result = new StringBuilder();
                        foreach (var item in (IEnumerable)route.Values)
                        {
                            result.Append(item);
                        }
                        return result.Length > 0 ? result.ToString() : "/";
                    });
                    _urlHelper = urlHelperMock.Object;
                }
                return _urlHelper;
            }
        }
        private IUrlHelper? _urlHelper;

        public void SetUser(TKey userId) => HttpContext.User = CreateClaim(userId);

        public void SetViewData() => PageContext.ViewData = ViewData;

        public UserManager<T> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var fakeLogger = Mock.Of<ILogger<UserManager<T>>>();
                    _userManager = new UserManager<T>(UserStore, null, new PasswordHasher<T>(), null, null, null, new IdentityErrorDescriber(), null, fakeLogger);
                    _userManager.RegisterTokenProvider("Default", new EmailTokenProvider<T>());
                }
                return _userManager;
            }
        }
        private UserManager<T>? _userManager;

        public SignInManager<T> SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    var authServiceMock = new Mock<IAuthenticationService>();
                    authServiceMock
                        .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                        .Returns(Task.FromResult((object?)null));
                    ServiceProviderMock
                        .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                        .Returns(authServiceMock.Object);
                    var mockAccessor = Mock.Of<IHttpContextAccessor>(x => x.HttpContext == HttpContext);
                    var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<T>>();
                    mockClaimsFactory.Setup(x => x.CreateAsync(It.IsAny<T>())).Returns<T>(t => Task.FromResult(CreateClaim(t.Id)));
                    var fakeLogger = Mock.Of<ILogger<SignInManager<T>>>();
                    _signInManager = new SignInManager<T>(UserManager, mockAccessor, mockClaimsFactory.Object, null, fakeLogger, null, null);
                }
                return _signInManager;
            }
        }
        private SignInManager<T>? _signInManager;

        private static ClaimsPrincipal CreateClaim(TKey userId)
        {
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(), null)
            });
            return new ClaimsPrincipal(identity);
        }


        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _roleManager?.Dispose();
                    _roleStore?.Dispose();
                    _userManager?.Dispose();
                    _userStore?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
