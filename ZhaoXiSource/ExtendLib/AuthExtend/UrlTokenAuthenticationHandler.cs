using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
namespace ExtendLib.AuthExtend
{
    /// <summary>
    /// 通过URL参数做鉴权--
    /// 需要在Startup注册映射
    /// </summary>
    public class UrlTokenAuthenticationHandler : IAuthenticationHandler, IAuthenticationSignInHandler, IAuthenticationSignOutHandler
    {
        /// <summary>
        /// 核心鉴权处理方法
        /// 解析用户信息
        /// </summary>
        /// <returns></returns>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.AuthenticateAsync");
            string userInfo = this._HttpContext.Request.Query["UrlToken"];//信息从哪里读
            Console.WriteLine($"获取token：{userInfo}");
            if (string.IsNullOrWhiteSpace(userInfo))
            {
                return Task.FromResult<AuthenticateResult>(AuthenticateResult.NoResult());
            }
            else if ("eleven-123456".Equals(userInfo))//信息是否可靠？  校验规则可以传递到Option的
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, userInfo));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xuyang@ZhaoxiEdu.Net"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Country, "China"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, "1986"));
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimIdentity);//信息拼装和传递

                return Task.FromResult<AuthenticateResult>(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, null, _AuthenticationScheme.Name)));
            }
            else
            {
                return Task.FromResult<AuthenticateResult>(AuthenticateResult.Fail($"UrlToken is wrong: {userInfo}"));
            }

        }
        /// <summary>
        /// 未登录
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.ChallengeAsync");
            string redirectUri = "/Home/Index";
            this._HttpContext.Response.Redirect(redirectUri);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 未授权，无权限
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.ForbidAsync");
            this._HttpContext.Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        private AuthenticationScheme _AuthenticationScheme = null;//"UrltokenScheme"
        private HttpContext _HttpContext = null;

        /// <summary>
        /// 初始化，Provider传递进来的
        /// 像方法注入
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Console.WriteLine($"This is {nameof(UrlTokenAuthenticationHandler)}.InitializeAsync");
            this._AuthenticationScheme = scheme;
            this._HttpContext = context;
            return Task.CompletedTask;
        }

        /// <summary>
        /// SignInAsync和SignOutAsync使用了独立的定义接口，
        /// 因为在现代架构中，通常会提供一个统一的认证中心，负责证书的颁发及销毁（登入和登出），
        /// 而其它服务只用来验证证书，并用不到SingIn/SingOut。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var ticket = new AuthenticationTicket(user, properties, this._AuthenticationScheme.Name);
            this._HttpContext.Response.Cookies.Append("UrlTokenCookie", Newtonsoft.Json.JsonConvert.SerializeObject(ticket.Principal.Claims));
            //把一些信息再写入到前端cookie，客户端请求时，从coookie读取UrlTokenCookie信息，放到url上
            return Task.CompletedTask;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignOutAsync(AuthenticationProperties properties)
        {
            this._HttpContext.Response.Cookies.Delete("UrlTokenCookie");
            return Task.CompletedTask;
        }



        /**
         * public class JwtBearerHandler : AuthenticationHandler<JwtBearerOptions>
{
            protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
            {
              JwtBearerHandler jwtBearerHandler = this;
              string token = (string) null;
              object obj;
              AuthenticationFailedContext authenticationFailedContext;
              int num;
              try
              {
                MessageReceivedContext messageReceivedContext = new MessageReceivedContext(jwtBearerHandler.Context, jwtBearerHandler.Scheme, jwtBearerHandler.Options);
                await jwtBearerHandler.Events.MessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                  return messageReceivedContext.Result;
                token = messageReceivedContext.Token;
                if (string.IsNullOrEmpty(token))
                {
                  string header = (string) jwtBearerHandler.Request.Headers["Authorization"];
                  if (string.IsNullOrEmpty(header))
                    return AuthenticateResult.NoResult();
                  if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = header.Substring("Bearer ".Length).Trim();
                  if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.NoResult();
                }
                if (jwtBearerHandler._configuration == null && jwtBearerHandler.Options.ConfigurationManager != null)
                {
                  OpenIdConnectConfiguration configurationAsync = await jwtBearerHandler.Options.ConfigurationManager.GetConfigurationAsync(jwtBearerHandler.Context.RequestAborted);
                  jwtBearerHandler._configuration = configurationAsync;
                }
                TokenValidationParameters validationParameters1 = jwtBearerHandler.Options.TokenValidationParameters.Clone();
                if (jwtBearerHandler._configuration != null)
                {
                  string[] strArray = new string[1]
                  {
                    jwtBearerHandler._configuration.Issuer
                  };
                  TokenValidationParameters validationParameters2 = validationParameters1;
                  IEnumerable<string> validIssuers = validationParameters1.get_ValidIssuers();
                  object obj1 = (validIssuers != null ? (object) validIssuers.Concat<string>((IEnumerable<string>) strArray) : (object) null) ?? (object) strArray;
                  validationParameters2.set_ValidIssuers((IEnumerable<string>) obj1);
                  TokenValidationParameters validationParameters3 = validationParameters1;
                  IEnumerable<SecurityKey> issuerSigningKeys = validationParameters1.get_IssuerSigningKeys();
                  IEnumerable<SecurityKey> securityKeys = (issuerSigningKeys != null ? issuerSigningKeys.Concat<SecurityKey>((IEnumerable<SecurityKey>) jwtBearerHandler._configuration.get_SigningKeys()) : (IEnumerable<SecurityKey>) null) ?? (IEnumerable<SecurityKey>) jwtBearerHandler._configuration.get_SigningKeys();
                  validationParameters3.set_IssuerSigningKeys(securityKeys);
                }
                List<Exception> exceptionList = (List<Exception>) null;
                foreach (ISecurityTokenValidator securityTokenValidator in (IEnumerable<ISecurityTokenValidator>) jwtBearerHandler.Options.SecurityTokenValidators)
                {
                  if (securityTokenValidator.CanReadToken(token))
                  {
                    SecurityToken securityToken;
                    ClaimsPrincipal claimsPrincipal;
                    try
                    {
                      claimsPrincipal = securityTokenValidator.ValidateToken(token, validationParameters1, ref securityToken);
                    }
                    catch (Exception ex)
                    {
                      jwtBearerHandler.Logger.TokenValidationFailed(ex);
                      if (jwtBearerHandler.Options.RefreshOnIssuerKeyNotFound && jwtBearerHandler.Options.ConfigurationManager != null && ex is SecurityTokenSignatureKeyNotFoundException)
                        jwtBearerHandler.Options.ConfigurationManager.RequestRefresh();
                      if (exceptionList == null)
                        exceptionList = new List<Exception>(1);
                      exceptionList.Add(ex);
                      continue;
                    }
                    jwtBearerHandler.Logger.TokenValidationSucceeded();
                    TokenValidatedContext validatedContext = new TokenValidatedContext(jwtBearerHandler.Context, jwtBearerHandler.Scheme, jwtBearerHandler.Options);
                    validatedContext.Principal = claimsPrincipal;
                    validatedContext.SecurityToken = securityToken;
                    TokenValidatedContext tokenValidatedContext = validatedContext;
                    await jwtBearerHandler.Events.TokenValidated(tokenValidatedContext);
                    if (tokenValidatedContext.Result != null)
                      return tokenValidatedContext.Result;
                    if (jwtBearerHandler.Options.SaveToken)
                      tokenValidatedContext.Properties.StoreTokens((IEnumerable<AuthenticationToken>) new AuthenticationToken[1]
                      {
                        new AuthenticationToken()
                        {
                          Name = "access_token",
                          Value = token
                        }
                      });
                    tokenValidatedContext.Success();
                    return tokenValidatedContext.Result;
                  }
                }
                if (exceptionList == null)
                  return AuthenticateResult.Fail("No SecurityTokenValidator available for token: " + token ?? "[null]");
                authenticationFailedContext = new AuthenticationFailedContext(jwtBearerHandler.Context, jwtBearerHandler.Scheme, jwtBearerHandler.Options)
                {
                  Exception = exceptionList.Count == 1 ? exceptionList[0] : (Exception) new AggregateException((IEnumerable<Exception>) exceptionList)
                };
                await jwtBearerHandler.Events.AuthenticationFailed(authenticationFailedContext);
                return authenticationFailedContext.Result == null ? AuthenticateResult.Fail(authenticationFailedContext.Exception) : authenticationFailedContext.Result;
              }
              catch (Exception ex)
              {
                obj = (object) ex;
                num = 1;
              }
              if (num == 1)
              {
                Exception ex = (Exception) obj;
                jwtBearerHandler.Logger.ErrorProcessingMessage(ex);
                authenticationFailedContext = new AuthenticationFailedContext(jwtBearerHandler.Context, jwtBearerHandler.Scheme, jwtBearerHandler.Options)
                {
                  Exception = ex
                };
                await jwtBearerHandler.Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                  return authenticationFailedContext.Result;
                Exception source = obj as Exception;
                if (source == null)
                  throw obj;
                ExceptionDispatchInfo.Capture(source).Throw();
                authenticationFailedContext = (AuthenticationFailedContext) null;
              }
              obj = (object) null;
              token = (string) null;
              AuthenticateResult authenticateResult;
              return authenticateResult;
            }
}
         * **/
    }

    /// <summary>
    /// 提供个固定值
    /// </summary>
    public class UrlTokenAuthenticationDefaults
    {
        /// <summary>
        /// 提供固定名称
        /// </summary>
        public const string AuthenticationScheme = "UrlTokenScheme";
    }


    //public class CookieAuthenticationHandler : SignInAuthenticationHandler<CookieAuthenticationOptions>
    //{
    //    private const string HeaderValueNoCache = "no-cache";
    //    private const string HeaderValueNoCacheNoStore = "no-cache,no-store";
    //    private const string HeaderValueEpocDate = "Thu, 01 Jan 1970 00:00:00 GMT";
    //    private const string SessionIdClaim = "Microsoft.AspNetCore.Authentication.Cookies-SessionId";

    //    private bool _shouldRefresh;
    //    private bool _signInCalled;
    //    private bool _signOutCalled;

    //    private DateTimeOffset? _refreshIssuedUtc;
    //    private DateTimeOffset? _refreshExpiresUtc;
    //    private string? _sessionKey;
    //    private Task<AuthenticateResult>? _readCookieTask;
    //    private AuthenticationTicket? _refreshTicket;

    //    /// <summary>
    //    /// Initializes a new instance of <see cref="CookieAuthenticationHandler"/>.
    //    /// </summary>
    //    /// <param name="options">Accessor to <see cref="CookieAuthenticationOptions"/>.</param>
    //    /// <param name="logger">The <see cref="ILoggerFactory"/>.</param>
    //    /// <param name="encoder">The <see cref="UrlEncoder"/>.</param>
    //    /// <param name="clock">The <see cref="ISystemClock"/>.</param>
    //    public CookieAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
    //        : base(options, logger, encoder, clock)
    //    { }

    //    /// <summary>
    //    /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
    //    /// If it is not provided a default instance is supplied which does nothing when the methods are called.
    //    /// </summary>
    //    protected new CookieAuthenticationEvents Events
    //    {
    //        get { return (CookieAuthenticationEvents)base.Events!; }
    //        set { base.Events = value; }
    //    }

    //    /// <inheritdoc />
    //    protected override Task InitializeHandlerAsync()
    //    {
    //        // Cookies needs to finish the response
    //        Context.Response.OnStarting(FinishResponseAsync);
    //        return Task.CompletedTask;
    //    }

    //    /// <summary>
    //    /// Creates a new instance of the events instance.
    //    /// </summary>
    //    /// <returns>A new instance of the events instance.</returns>
    //    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new CookieAuthenticationEvents());

    //    private Task<AuthenticateResult> EnsureCookieTicket()
    //    {
    //        // We only need to read the ticket once
    //        if (_readCookieTask == null)
    //        {
    //            _readCookieTask = ReadCookieTicket();
    //        }
    //        return _readCookieTask;
    //    }

    //    private void CheckForRefresh(AuthenticationTicket ticket)
    //    {
    //        var currentUtc = Clock.UtcNow;
    //        var issuedUtc = ticket.Properties.IssuedUtc;
    //        var expiresUtc = ticket.Properties.ExpiresUtc;
    //        var allowRefresh = ticket.Properties.AllowRefresh ?? true;
    //        if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
    //        {
    //            var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
    //            var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

    //            if (timeRemaining < timeElapsed)
    //            {
    //                RequestRefresh(ticket);
    //            }
    //        }
    //    }

    //    private void RequestRefresh(AuthenticationTicket ticket, ClaimsPrincipal? replacedPrincipal = null)
    //    {
    //        var issuedUtc = ticket.Properties.IssuedUtc;
    //        var expiresUtc = ticket.Properties.ExpiresUtc;

    //        if (issuedUtc != null && expiresUtc != null)
    //        {
    //            _shouldRefresh = true;
    //            var currentUtc = Clock.UtcNow;
    //            _refreshIssuedUtc = currentUtc;
    //            var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
    //            _refreshExpiresUtc = currentUtc.Add(timeSpan);
    //            _refreshTicket = CloneTicket(ticket, replacedPrincipal);
    //        }
    //    }

    //    private AuthenticationTicket CloneTicket(AuthenticationTicket ticket, ClaimsPrincipal? replacedPrincipal)
    //    {
    //        var principal = replacedPrincipal ?? ticket.Principal;
    //        var newPrincipal = new ClaimsPrincipal();
    //        foreach (var identity in principal.Identities)
    //        {
    //            newPrincipal.AddIdentity(identity.Clone());
    //        }

    //        var newProperties = new AuthenticationProperties();
    //        foreach (var item in ticket.Properties.Items)
    //        {
    //            newProperties.Items[item.Key] = item.Value;
    //        }

    //        return new AuthenticationTicket(newPrincipal, newProperties, ticket.AuthenticationScheme);
    //    }

    //    private async Task<AuthenticateResult> ReadCookieTicket()
    //    {
    //        var cookie = Options.CookieManager.GetRequestCookie(Context, Options.Cookie.Name!);
    //        if (string.IsNullOrEmpty(cookie))
    //        {
    //            return AuthenticateResult.NoResult();
    //        }

    //        var ticket = Options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
    //        if (ticket == null)
    //        {
    //            return AuthenticateResult.Fail("Unprotect ticket failed");
    //        }

    //        if (Options.SessionStore != null)
    //        {
    //            var claim = ticket.Principal.Claims.FirstOrDefault(c => c.Type.Equals(SessionIdClaim));
    //            if (claim == null)
    //            {
    //                return AuthenticateResult.Fail("SessionId missing");
    //            }
    //            // Only store _sessionKey if it matches an existing session. Otherwise we'll create a new one.
    //            ticket = await Options.SessionStore.RetrieveAsync(claim.Value, Context.RequestAborted);
    //            if (ticket == null)
    //            {
    //                return AuthenticateResult.Fail("Identity missing in session store");
    //            }
    //            _sessionKey = claim.Value;
    //        }

    //        var currentUtc = Clock.UtcNow;
    //        var expiresUtc = ticket.Properties.ExpiresUtc;

    //        if (expiresUtc != null && expiresUtc.Value < currentUtc)
    //        {
    //            if (Options.SessionStore != null)
    //            {
    //                await Options.SessionStore.RemoveAsync(_sessionKey!, Context.RequestAborted);
    //            }
    //            return AuthenticateResult.Fail("Ticket expired");
    //        }

    //        CheckForRefresh(ticket);

    //        // Finally we have a valid ticket
    //        return AuthenticateResult.Success(ticket);
    //    }

    //    /// <inheritdoc />
    //    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    //    {
    //        var result = await EnsureCookieTicket();
    //        if (!result.Succeeded)
    //        {
    //            return result;
    //        }

    //        Debug.Assert(result.Ticket != null);
    //        var context = new CookieValidatePrincipalContext(Context, Scheme, Options, result.Ticket);
    //        await Events.ValidatePrincipal(context);

    //        if (context.Principal == null)
    //        {
    //            return AuthenticateResult.Fail("No principal.");
    //        }

    //        if (context.ShouldRenew)
    //        {
    //            RequestRefresh(result.Ticket, context.Principal);
    //        }

    //        return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
    //    }

    //    private CookieOptions BuildCookieOptions()
    //    {
    //        var cookieOptions = Options.Cookie.Build(Context);
    //        // ignore the 'Expires' value as this will be computed elsewhere
    //        cookieOptions.Expires = null;

    //        return cookieOptions;
    //    }

    //    /// <inheritdoc />
    //    protected virtual async Task FinishResponseAsync()
    //    {
    //        // Only renew if requested, and neither sign in or sign out was called
    //        if (!_shouldRefresh || _signInCalled || _signOutCalled)
    //        {
    //            return;
    //        }

    //        var ticket = _refreshTicket;
    //        if (ticket != null)
    //        {
    //            var properties = ticket.Properties;

    //            if (_refreshIssuedUtc.HasValue)
    //            {
    //                properties.IssuedUtc = _refreshIssuedUtc;
    //            }

    //            if (_refreshExpiresUtc.HasValue)
    //            {
    //                properties.ExpiresUtc = _refreshExpiresUtc;
    //            }

    //            if (Options.SessionStore != null && _sessionKey != null)
    //            {
    //                await Options.SessionStore.RenewAsync(_sessionKey, ticket, Context.RequestAborted);
    //                var principal = new ClaimsPrincipal(
    //                    new ClaimsIdentity(
    //                        new[] { new Claim(SessionIdClaim, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
    //                        Scheme.Name));
    //                ticket = new AuthenticationTicket(principal, null, Scheme.Name);
    //            }

    //            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

    //            var cookieOptions = BuildCookieOptions();
    //            if (properties.IsPersistent && _refreshExpiresUtc.HasValue)
    //            {
    //                cookieOptions.Expires = _refreshExpiresUtc.Value.ToUniversalTime();
    //            }

    //            Options.CookieManager.AppendResponseCookie(
    //                Context,
    //                Options.Cookie.Name!,
    //                cookieValue,
    //                cookieOptions);

    //            await ApplyHeaders(shouldRedirectToReturnUrl: false, properties: properties);
    //        }
    //    }

    //    /// <inheritdoc />
    //    protected async override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    //    {
    //        if (user == null)
    //        {
    //            throw new ArgumentNullException(nameof(user));
    //        }

    //        properties = properties ?? new AuthenticationProperties();

    //        _signInCalled = true;

    //        // Process the request cookie to initialize members like _sessionKey.
    //        await EnsureCookieTicket();
    //        var cookieOptions = BuildCookieOptions();

    //        var signInContext = new CookieSigningInContext(
    //            Context,
    //            Scheme,
    //            Options,
    //            user,
    //            properties,
    //            cookieOptions);

    //        DateTimeOffset issuedUtc;
    //        if (signInContext.Properties.IssuedUtc.HasValue)
    //        {
    //            issuedUtc = signInContext.Properties.IssuedUtc.Value;
    //        }
    //        else
    //        {
    //            issuedUtc = Clock.UtcNow;
    //            signInContext.Properties.IssuedUtc = issuedUtc;
    //        }

    //        if (!signInContext.Properties.ExpiresUtc.HasValue)
    //        {
    //            signInContext.Properties.ExpiresUtc = issuedUtc.Add(Options.ExpireTimeSpan);
    //        }

    //        await Events.SigningIn(signInContext);

    //        if (signInContext.Properties.IsPersistent)
    //        {
    //            var expiresUtc = signInContext.Properties.ExpiresUtc ?? issuedUtc.Add(Options.ExpireTimeSpan);
    //            signInContext.CookieOptions.Expires = expiresUtc.ToUniversalTime();
    //        }

    //        var ticket = new AuthenticationTicket(signInContext.Principal!, signInContext.Properties, signInContext.Scheme.Name);

    //        if (Options.SessionStore != null)
    //        {
    //            if (_sessionKey != null)
    //            {
    //                // Renew the ticket in cases of multiple requests see: https://github.com/dotnet/aspnetcore/issues/22135
    //                await Options.SessionStore.RenewAsync(_sessionKey, ticket, Context.RequestAborted);
    //            }
    //            else
    //            {
    //                _sessionKey = await Options.SessionStore.StoreAsync(ticket, Context.RequestAborted);
    //            }

    //            var principal = new ClaimsPrincipal(
    //                new ClaimsIdentity(
    //                    new[] { new Claim(SessionIdClaim, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
    //                    Options.ClaimsIssuer));
    //            ticket = new AuthenticationTicket(principal, null, Scheme.Name);
    //        }

    //        var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

    //        Options.CookieManager.AppendResponseCookie(
    //            Context,
    //            Options.Cookie.Name!,
    //            cookieValue,
    //            signInContext.CookieOptions);

    //        var signedInContext = new CookieSignedInContext(
    //            Context,
    //            Scheme,
    //            signInContext.Principal!,
    //            signInContext.Properties,
    //            Options);

    //        await Events.SignedIn(signedInContext);

    //        // Only redirect on the login path
    //        var shouldRedirect = Options.LoginPath.HasValue && OriginalPath == Options.LoginPath;
    //        await ApplyHeaders(shouldRedirect, signedInContext.Properties);

    //        Logger.AuthenticationSchemeSignedIn(Scheme.Name);
    //    }

    //    /// <inheritdoc />
    //    protected async override Task HandleSignOutAsync(AuthenticationProperties? properties)
    //    {
    //        properties = properties ?? new AuthenticationProperties();

    //        _signOutCalled = true;

    //        // Process the request cookie to initialize members like _sessionKey.
    //        await EnsureCookieTicket();
    //        var cookieOptions = BuildCookieOptions();
    //        if (Options.SessionStore != null && _sessionKey != null)
    //        {
    //            await Options.SessionStore.RemoveAsync(_sessionKey, Context.RequestAborted);
    //        }

    //        var context = new CookieSigningOutContext(
    //            Context,
    //            Scheme,
    //            Options,
    //            properties,
    //            cookieOptions);

    //        await Events.SigningOut(context);

    //        Options.CookieManager.DeleteCookie(
    //            Context,
    //            Options.Cookie.Name!,
    //            context.CookieOptions);

    //        // Only redirect on the logout path
    //        var shouldRedirect = Options.LogoutPath.HasValue && OriginalPath == Options.LogoutPath;
    //        await ApplyHeaders(shouldRedirect, context.Properties);

    //        Logger.AuthenticationSchemeSignedOut(Scheme.Name);
    //    }

    //    private async Task ApplyHeaders(bool shouldRedirectToReturnUrl, AuthenticationProperties properties)
    //    {
    //        Response.Headers.CacheControl = HeaderValueNoCacheNoStore;
    //        Response.Headers.Pragma = HeaderValueNoCache;
    //        Response.Headers.Expires = HeaderValueEpocDate;

    //        if (shouldRedirectToReturnUrl && Response.StatusCode == 200)
    //        {
    //            // set redirect uri in order:
    //            // 1. properties.RedirectUri
    //            // 2. query parameter ReturnUrlParameter
    //            //
    //            // Absolute uri is not allowed if it is from query string as query string is not
    //            // a trusted source.
    //            var redirectUri = properties.RedirectUri;
    //            if (string.IsNullOrEmpty(redirectUri))
    //            {
    //                redirectUri = Request.Query[Options.ReturnUrlParameter];
    //                if (string.IsNullOrEmpty(redirectUri) || !IsHostRelative(redirectUri))
    //                {
    //                    redirectUri = null;
    //                }
    //            }

    //            if (redirectUri != null)
    //            {
    //                await Events.RedirectToReturnUrl(
    //                    new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, redirectUri));
    //            }
    //        }
    //    }

    //    private static bool IsHostRelative(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //        {
    //            return false;
    //        }
    //        if (path.Length == 1)
    //        {
    //            return path[0] == '/';
    //        }
    //        return path[0] == '/' && path[1] != '/' && path[1] != '\\';
    //    }

    //    /// <inheritdoc />
    //    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    //    {
    //        var returnUrl = properties.RedirectUri;
    //        if (string.IsNullOrEmpty(returnUrl))
    //        {
    //            returnUrl = OriginalPathBase + OriginalPath + Request.QueryString;
    //        }
    //        var accessDeniedUri = Options.AccessDeniedPath + QueryString.Create(Options.ReturnUrlParameter, returnUrl);
    //        var redirectContext = new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(accessDeniedUri));
    //        await Events.RedirectToAccessDenied(redirectContext);
    //    }

    //    /// <inheritdoc />
    //    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    //    {
    //        var redirectUri = properties.RedirectUri;
    //        if (string.IsNullOrEmpty(redirectUri))
    //        {
    //            redirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
    //        }

    //        var loginUri = Options.LoginPath + QueryString.Create(Options.ReturnUrlParameter, redirectUri);
    //        var redirectContext = new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(loginUri));
    //        await Events.RedirectToLogin(redirectContext);
    //    }

    //    private string? GetTlsTokenBinding()
    //    {
    //        var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
    //        return binding == null ? null : Convert.ToBase64String(binding);
    //    }
    //}
}
