using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoProject.CustomAuth
{
    /// <summary>
    /// 鉴权源码（用于调试）
    /// </summary>
    #region Middleware
    public static class CustomAuthAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="CustomAuthenticationMiddleware"/> to the specified <see cref="IApplicationBuilder"/>, which enables authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder CustomUseAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CustomAuthenticationMiddleware>();
        }
    }

    /// <summary>
    /// Middleware that performs authentication.
    /// </summary>
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of <see cref="CustomAuthenticationMiddleware"/>.
        /// </summary>
        /// <param name="next">The next item in the middleware pipeline.</param>
        /// <param name="schemes">The <see cref="IAuthenticationSchemeProvider"/>.</param>
        public CustomAuthenticationMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (schemes == null)
            {
                throw new ArgumentNullException(nameof(schemes));
            }

            _next = next;
            Schemes = schemes;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAuthenticationSchemeProvider"/>.
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        /// <summary>
        /// Invokes the middleware performing authentication.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        public async Task Invoke(HttpContext context)
        {
            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = context.Request.Path,
                OriginalPathBase = context.Request.PathBase
            });

            // Give any IAuthenticationRequestHandler schemes a chance to handle the request
            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())//多了个Request,是直接处理请求，终止流程的，专门留着扩展
            {
                var handler = await handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;//之前没有Add
                if (handler != null && await handler.HandleRequestAsync())//HandleRequestAsync?不是AuthorticateAsync? 直接处理请求，不走后续流程
                {
                    return;
                }
            }//不是常规的鉴权，而是处理请求

            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();//获取默认鉴权方案，只走默认鉴权
            if (defaultAuthenticate != null)
            {
                var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                if (result?.Principal != null)
                {
                    context.User = result.Principal;
                }
            }

            await _next(context);
        }
    }
    #endregion

    #region IOC
    public static class CustomAuthenticationCoreServiceCollectionExtensions
    {
        /// <summary>
        /// Add core authentication services needed for <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection CustomAddAuthenticationCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //services.TryAddScoped<IAuthenticationService, AuthenticationService>();
            services.Replace(ServiceDescriptor.Scoped<IAuthenticationService, CustomAuthenticationService>());

            //services.TryAddSingleton<IClaimsTransformation, NoopClaimsTransformation>(); // Can be replaced with scoped ones that use DbContext
            services.Replace(ServiceDescriptor.Singleton<IClaimsTransformation, CustomNoopClaimsTransformation>());

            //services.TryAddScoped<IAuthenticationHandlerProvider, AuthenticationHandlerProvider>();
            services.Replace(ServiceDescriptor.Scoped<IAuthenticationHandlerProvider, CustomAuthenticationHandlerProvider>());

            //services.TryAddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
            services.Replace(ServiceDescriptor.Singleton<IAuthenticationSchemeProvider, CustomAuthenticationSchemeProvider>());
            return services;
        }
    }

    public class CustomAuthenticationService : IAuthenticationService
    {
        private HashSet<ClaimsPrincipal>? _transformCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IAuthenticationSchemeProvider"/>.</param>
        /// <param name="handlers">The <see cref="IAuthenticationHandlerProvider"/>.</param>
        /// <param name="transform">The <see cref="IClaimsTransformation"/>.</param>
        /// <param name="options">The <see cref="AuthenticationOptions"/>.</param>
        public CustomAuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers, IClaimsTransformation transform, IOptions<AuthenticationOptions> options)
        {
            Schemes = schemes;
            Handlers = handlers;
            Transform = transform;
            Options = options.Value;
        }

        /// <summary>
        /// Used to lookup AuthenticationSchemes.
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; }

        /// <summary>
        /// Used to resolve IAuthenticationHandler instances.
        /// </summary>
        public IAuthenticationHandlerProvider Handlers { get; }

        /// <summary>
        /// Used for claims transformation.
        /// </summary>
        public IClaimsTransformation Transform { get; }

        /// <summary>
        /// The <see cref="AuthenticationOptions"/>.
        /// </summary>
        public AuthenticationOptions Options { get; }

        /// <summary>
        /// Authenticate for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        public virtual async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultAuthenticateSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            // Handlers should not return null, but we'll be tolerant of null values for legacy reasons.
            var result = (await handler.AuthenticateAsync()) ?? AuthenticateResult.NoResult();

            if (result.Succeeded)
            {
                var principal = result.Principal!;
                var doTransform = true;
                _transformCache ??= new HashSet<ClaimsPrincipal>();
                if (_transformCache.Contains(principal))
                {
                    doTransform = false;
                }

                if (doTransform)
                {
                    principal = await Transform.TransformAsync(principal);
                    _transformCache.Add(principal);
                }
                return AuthenticateResult.Success(new AuthenticationTicket(principal, result.Properties, result.Ticket!.AuthenticationScheme));
            }
            return result;
        }

        /// <summary>
        /// Challenge the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        public virtual async Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            if (scheme == null)
            {
                var defaultChallengeScheme = await Schemes.GetDefaultChallengeSchemeAsync();
                scheme = defaultChallengeScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultChallengeScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            await handler.ChallengeAsync(properties);
        }

        /// <summary>
        /// Forbid the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        public virtual async Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            if (scheme == null)
            {
                var defaultForbidScheme = await Schemes.GetDefaultForbidSchemeAsync();
                scheme = defaultForbidScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultForbidScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            await handler.ForbidAsync(properties);
        }

        /// <summary>
        /// Sign a principal in for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> to sign in.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        public virtual async Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            if (Options.RequireAuthenticatedSignIn)
            {
                if (principal.Identity == null)
                {
                    throw new InvalidOperationException("SignInAsync when principal.Identity == null is not allowed when AuthenticationOptions.RequireAuthenticatedSignIn is true.");
                }
                if (!principal.Identity.IsAuthenticated)
                {
                    throw new InvalidOperationException("SignInAsync when principal.Identity.IsAuthenticated is false is not allowed when AuthenticationOptions.RequireAuthenticatedSignIn is true.");
                }
            }

            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultSignInSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultSignInScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingSignInHandlerException(scheme);
            }

            var signInHandler = handler as IAuthenticationSignInHandler;
            if (signInHandler == null)
            {
                throw await CreateMismatchedSignInHandlerException(scheme, handler);
            }

            await signInHandler.SignInAsync(principal, properties);
        }

        /// <summary>
        /// Sign out the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        public virtual async Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultSignOutSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultSignOutScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingSignOutHandlerException(scheme);
            }

            var signOutHandler = handler as IAuthenticationSignOutHandler;
            if (signOutHandler == null)
            {
                throw await CreateMismatchedSignOutHandlerException(scheme, handler);
            }

            await signOutHandler.SignOutAsync(properties);
        }

        private async Task<Exception> CreateMissingHandlerException(string scheme)
        {
            var schemes = string.Join(", ", (await Schemes.GetAllSchemesAsync()).Select(sch => sch.Name));

            var footer = $" Did you forget to call AddAuthentication().Add[SomeAuthHandler](\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                return new InvalidOperationException(
                    $"No authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No authentication handler is registered for the scheme '{scheme}'. The registered schemes are: {schemes}." + footer);
        }

        private async Task<string> GetAllSignInSchemeNames()
        {
            return string.Join(", ", (await Schemes.GetAllSchemesAsync())
                .Where(sch => typeof(IAuthenticationSignInHandler).IsAssignableFrom(sch.HandlerType))
                .Select(sch => sch.Name));
        }

        private async Task<Exception> CreateMissingSignInHandlerException(string scheme)
        {
            var schemes = await GetAllSignInSchemeNames();

            // CookieAuth is the only implementation of sign-in.
            var footer = $" Did you forget to call AddAuthentication().AddCookie(\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                return new InvalidOperationException(
                    $"No sign-in authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No sign-in authentication handler is registered for the scheme '{scheme}'. The registered sign-in schemes are: {schemes}." + footer);
        }

        private async Task<Exception> CreateMismatchedSignInHandlerException(string scheme, IAuthenticationHandler handler)
        {
            var schemes = await GetAllSignInSchemeNames();

            var mismatchError = $"The authentication handler registered for scheme '{scheme}' is '{handler.GetType().Name}' which cannot be used for SignInAsync. ";

            if (string.IsNullOrEmpty(schemes))
            {
                // CookieAuth is the only implementation of sign-in.
                return new InvalidOperationException(mismatchError
                    + $"Did you forget to call AddAuthentication().AddCookie(\"Cookies\") and SignInAsync(\"Cookies\",...)?");
            }

            return new InvalidOperationException(mismatchError + $"The registered sign-in schemes are: {schemes}.");
        }

        private async Task<string> GetAllSignOutSchemeNames()
        {
            return string.Join(", ", (await Schemes.GetAllSchemesAsync())
                .Where(sch => typeof(IAuthenticationSignOutHandler).IsAssignableFrom(sch.HandlerType))
                .Select(sch => sch.Name));
        }

        private async Task<Exception> CreateMissingSignOutHandlerException(string scheme)
        {
            var schemes = await GetAllSignOutSchemeNames();

            var footer = $" Did you forget to call AddAuthentication().AddCookie(\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                // CookieAuth is the most common implementation of sign-out, but OpenIdConnect and WsFederation also support it.
                return new InvalidOperationException($"No sign-out authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No sign-out authentication handler is registered for the scheme '{scheme}'. The registered sign-out schemes are: {schemes}." + footer);
        }

        private async Task<Exception> CreateMismatchedSignOutHandlerException(string scheme, IAuthenticationHandler handler)
        {
            var schemes = await GetAllSignOutSchemeNames();

            var mismatchError = $"The authentication handler registered for scheme '{scheme}' is '{handler.GetType().Name}' which cannot be used for {nameof(SignOutAsync)}. ";

            if (string.IsNullOrEmpty(schemes))
            {
                // CookieAuth is the most common implementation of sign-out, but OpenIdConnect and WsFederation also support it.
                return new InvalidOperationException(mismatchError
                    + $"Did you forget to call AddAuthentication().AddCookie(\"Cookies\") and {nameof(SignOutAsync)}(\"Cookies\",...)?");
            }

            return new InvalidOperationException(mismatchError + $"The registered sign-out schemes are: {schemes}.");
        }
    }

    public class CustomNoopClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// Returns the principal unchanged.
        /// </summary>
        /// <param name="principal">The user.</param>
        /// <returns>The principal unchanged.</returns>
        public virtual Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            Console.WriteLine(principal.Claims.FirstOrDefault().Value);
            return Task.FromResult(principal);
        }
    }

    public class CustomAuthenticationHandlerProvider : IAuthenticationHandlerProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IAuthenticationHandlerProvider"/>.</param>
        public CustomAuthenticationHandlerProvider(IAuthenticationSchemeProvider schemes)
        {
            Schemes = schemes;
        }

        /// <summary>
        /// The <see cref="IAuthenticationHandlerProvider"/>.
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; }

        // handler instance cache, need to initialize once per request
        private readonly Dictionary<string, IAuthenticationHandler> _handlerMap = new Dictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);

        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationScheme">The name of the authentication scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        public async Task<IAuthenticationHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
        {
            if (_handlerMap.TryGetValue(authenticationScheme, out var value))
            {
                return value;
            }

            var scheme = await Schemes.GetSchemeAsync(authenticationScheme);
            if (scheme == null)
            {
                return null;
            }
            var handler = (context.RequestServices.GetService(scheme.HandlerType) ??
                ActivatorUtilities.CreateInstance(context.RequestServices, scheme.HandlerType))
                as IAuthenticationHandler;
            if (handler != null)
            {
                await handler.InitializeAsync(scheme, context);
                _handlerMap[authenticationScheme] = handler;
            }
            return handler;
        }
    }

    public class CustomAuthenticationSchemeProvider : IAuthenticationSchemeProvider
    {
        /// <summary>
        /// Creates an instance of <see cref="AuthenticationSchemeProvider"/>
        /// using the specified <paramref name="options"/>,
        /// </summary>
        /// <param name="options">The <see cref="AuthenticationOptions"/> options.</param>
        public CustomAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options)
            : this(options, new Dictionary<string, AuthenticationScheme>(StringComparer.Ordinal))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AuthenticationSchemeProvider"/>
        /// using the specified <paramref name="options"/> and <paramref name="schemes"/>.
        /// </summary>
        /// <param name="options">The <see cref="AuthenticationOptions"/> options.</param>
        /// <param name="schemes">The dictionary used to store authentication schemes.</param>
        protected CustomAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options, IDictionary<string, AuthenticationScheme> schemes)
        {
            _options = options.Value;

            _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _requestHandlers = new List<AuthenticationScheme>();

            foreach (var builder in _options.Schemes)
            {
                var scheme = builder.Build();
                AddScheme(scheme);
            }
        }

        private readonly AuthenticationOptions _options;
        private readonly object _lock = new object();

        private readonly IDictionary<string, AuthenticationScheme> _schemes;
        private readonly List<AuthenticationScheme> _requestHandlers;
        // Used as a safe return value for enumeration apis
        private IEnumerable<AuthenticationScheme> _schemesCopy = Array.Empty<AuthenticationScheme>();
        private IEnumerable<AuthenticationScheme> _requestHandlersCopy = Array.Empty<AuthenticationScheme>();

        private Task<AuthenticationScheme?> GetDefaultSchemeAsync()
            => _options.DefaultScheme != null
            ? GetSchemeAsync(_options.DefaultScheme)
            : Task.FromResult<AuthenticationScheme?>(null);

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.AuthenticateAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultAuthenticateScheme"/>.
        /// Otherwise, this will fallback to <see cref="AuthenticationOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.AuthenticateAsync(HttpContext, string)"/>.</returns>
        public virtual Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync()
            => _options.DefaultAuthenticateScheme != null
            ? GetSchemeAsync(_options.DefaultAuthenticateScheme)
            : GetDefaultSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.ChallengeAsync(HttpContext, string, AuthenticationProperties)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultChallengeScheme"/>.
        /// Otherwise, this will fallback to <see cref="AuthenticationOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.ChallengeAsync(HttpContext, string, AuthenticationProperties)"/>.</returns>
        public virtual Task<AuthenticationScheme?> GetDefaultChallengeSchemeAsync()
            => _options.DefaultChallengeScheme != null
            ? GetSchemeAsync(_options.DefaultChallengeScheme)
            : GetDefaultSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.ForbidAsync(HttpContext, string, AuthenticationProperties)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultForbidScheme"/>.
        /// Otherwise, this will fallback to <see cref="GetDefaultChallengeSchemeAsync"/> .
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.ForbidAsync(HttpContext, string, AuthenticationProperties)"/>.</returns>
        public virtual Task<AuthenticationScheme?> GetDefaultForbidSchemeAsync()
            => _options.DefaultForbidScheme != null
            ? GetSchemeAsync(_options.DefaultForbidScheme)
            : GetDefaultChallengeSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, AuthenticationProperties)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultSignInScheme"/>.
        /// Otherwise, this will fallback to <see cref="AuthenticationOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, AuthenticationProperties)"/>.</returns>
        public virtual Task<AuthenticationScheme?> GetDefaultSignInSchemeAsync()
            => _options.DefaultSignInScheme != null
            ? GetSchemeAsync(_options.DefaultSignInScheme)
            : GetDefaultSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.SignOutAsync(HttpContext, string, AuthenticationProperties)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultSignOutScheme"/>.
        /// Otherwise this will fallback to <see cref="GetDefaultSignInSchemeAsync"/> if that supports sign out.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.SignOutAsync(HttpContext, string, AuthenticationProperties)"/>.</returns>
        public virtual Task<AuthenticationScheme?> GetDefaultSignOutSchemeAsync()
            => _options.DefaultSignOutScheme != null
            ? GetSchemeAsync(_options.DefaultSignOutScheme)
            : GetDefaultSignInSchemeAsync();

        /// <summary>
        /// Returns the <see cref="AuthenticationScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        public virtual Task<AuthenticationScheme?> GetSchemeAsync(string name)
            => Task.FromResult(_schemes.ContainsKey(name) ? _schemes[name] : null);

        /// <summary>
        /// Returns the schemes in priority order for request handling.
        /// </summary>
        /// <returns>The schemes in priority order for request handling</returns>
        public virtual Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
            => Task.FromResult(_requestHandlersCopy);

        /// <summary>
        /// Registers a scheme for use by <see cref="IAuthenticationService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns>true if the scheme was added successfully.</returns>
        public virtual bool TryAddScheme(AuthenticationScheme scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                return false;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(scheme.Name))
                {
                    return false;
                }
                if (typeof(IAuthenticationRequestHandler).IsAssignableFrom(scheme.HandlerType))
                {
                    _requestHandlers.Add(scheme);
                    _requestHandlersCopy = _requestHandlers.ToArray();
                }
                _schemes[scheme.Name] = scheme;
                _schemesCopy = _schemes.Values.ToArray();
                return true;
            }
        }

        /// <summary>
        /// Registers a scheme for use by <see cref="IAuthenticationService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public virtual void AddScheme(AuthenticationScheme scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
            }
            lock (_lock)
            {
                if (!TryAddScheme(scheme))
                {
                    throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
                }
            }
        }

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        public virtual void RemoveScheme(string name)
        {
            if (!_schemes.ContainsKey(name))
            {
                return;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(name))
                {
                    var scheme = _schemes[name];
                    if (_requestHandlers.Remove(scheme))
                    {
                        _requestHandlersCopy = _requestHandlers.ToArray();
                    }
                    _schemes.Remove(name);
                    _schemesCopy = _schemes.Values.ToArray();
                }
            }
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
            => Task.FromResult(_schemesCopy);
    }
    #endregion

}
