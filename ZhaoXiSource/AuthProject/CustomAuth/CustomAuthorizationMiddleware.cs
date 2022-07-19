using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthProject.CustomAuth
{
    #region Middleware
    public static class CustomAuthorizationAppBuilderExtensions
    {
        public static IApplicationBuilder CustomUseAuthorization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            //VerifyServicesRegistered(app);

            return app.UseMiddleware<CustomAuthorizationMiddleware>();
        }

        //private static void VerifyServicesRegistered(IApplicationBuilder app)
        //{
        //    // Verify that AddAuthorizationPolicy was called before calling UseAuthorization
        //    // We use the AuthorizationPolicyMarkerService to ensure all the services were added.
        //    if (app.ApplicationServices.GetService(typeof(AuthorizationPolicyMarkerService)) == null)
        //    {
        //        throw new InvalidOperationException(Resources.FormatException_UnableToFindServices(
        //            nameof(IServiceCollection),
        //            nameof(PolicyServiceCollectionExtensions.AddAuthorization),
        //            "ConfigureServices(...)"));
        //    }
        //}
    }
    public class CustomAuthorizationMiddleware
    {
        // AppContext switch used to control whether HttpContext or endpoint is passed as a resource to AuthZ
        private const string SuppressUseHttpContextAsAuthorizationResource = "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

        // Property key is used by Endpoint routing to determine if Authorization has run
        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

        private readonly RequestDelegate _next;
        private readonly IAuthorizationPolicyProvider _policyProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthorizationMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the application middleware pipeline.</param>
        /// <param name="policyProvider">The <see cref="IAuthorizationPolicyProvider"/>.</param>
        public CustomAuthorizationMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
        }

        /// <summary>
        /// Invokes the middleware performing authorization.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
                // The Authorization middleware can only make this claim if it observes an actual endpoint.
                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
            }

            // IMPORTANT: Changes to authorization logic should be mirrored in MVC's AuthorizeFilter
            var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
            //var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
            var policy = await CustomAuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);//替换的
            if (policy == null)
            {
                await _next(context);
                return;
            }

            // Policy evaluator has transient lifetime so it fetched from request services instead of injecting in constructor
            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

            // Allow Anonymous skips all authorization
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            object? resource;
            if (AppContext.TryGetSwitch(SuppressUseHttpContextAsAuthorizationResource, out var useEndpointAsResource) && useEndpointAsResource)
            {
                resource = endpoint;
            }
            else
            {
                resource = context;
            }

            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource);
            var authorizationMiddlewareResultHandler = context.RequestServices.GetRequiredService<IAuthorizationMiddlewareResultHandler>();
            await authorizationMiddlewareResultHandler.HandleAsync(_next, context, policy, authorizeResult);
        }
    }


    public class CustomAuthorizationPolicy
    {
        public CustomAuthorizationPolicy(IEnumerable<IAuthorizationRequirement> requirements, IEnumerable<string> authenticationSchemes)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            if (authenticationSchemes == null)
            {
                throw new ArgumentNullException(nameof(authenticationSchemes));
            }

            if (!requirements.Any())
            {
                throw new InvalidOperationException();
            }
            Requirements = new List<IAuthorizationRequirement>(requirements).AsReadOnly();
            AuthenticationSchemes = new List<string>(authenticationSchemes).AsReadOnly();
        }

        /// <summary>
        /// Gets a readonly list of <see cref="IAuthorizationRequirement"/>s which must succeed for
        /// this policy to be successful.
        /// </summary>
        public IReadOnlyList<IAuthorizationRequirement> Requirements { get; }

        /// <summary>
        /// Gets a readonly list of the authentication schemes the <see cref="AuthorizationPolicy.Requirements"/> 
        /// are evaluated against.
        /// </summary>
        public IReadOnlyList<string> AuthenticationSchemes { get; }

        /// <summary>
        /// Combines the specified <see cref="AuthorizationPolicy"/> into a single policy.
        /// </summary>
        /// <param name="policies">The authorization policies to combine.</param>
        /// <returns>
        /// A new <see cref="AuthorizationPolicy"/> which represents the combination of the
        /// specified <paramref name="policies"/>.
        /// </returns>
        public static AuthorizationPolicy Combine(params AuthorizationPolicy[] policies)
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            return Combine((IEnumerable<AuthorizationPolicy>)policies);
        }

        /// <summary>
        /// Combines the specified <see cref="AuthorizationPolicy"/> into a single policy.
        /// </summary>
        /// <param name="policies">The authorization policies to combine.</param>
        /// <returns>
        /// A new <see cref="AuthorizationPolicy"/> which represents the combination of the
        /// specified <paramref name="policies"/>.
        /// </returns>
        public static AuthorizationPolicy Combine(IEnumerable<AuthorizationPolicy> policies)
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            var builder = new AuthorizationPolicyBuilder();
            foreach (var policy in policies)
            {
                builder.Combine(policy);
            }
            return builder.Build();
        }

        /// <summary>
        /// Combines the <see cref="AuthorizationPolicy"/> provided by the specified
        /// <paramref name="policyProvider"/>.
        /// </summary>
        /// <param name="policyProvider">A <see cref="IAuthorizationPolicyProvider"/> which provides the policies to combine.</param>
        /// <param name="authorizeData">A collection of authorization data used to apply authorization to a resource.</param>
        /// <returns>
        /// A new <see cref="AuthorizationPolicy"/> which represents the combination of the
        /// authorization policies provided by the specified <paramref name="policyProvider"/>.
        /// </returns>
        public static async Task<AuthorizationPolicy?> CombineAsync(IAuthorizationPolicyProvider policyProvider, IEnumerable<IAuthorizeData> authorizeData)
        {
            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            if (authorizeData == null)
            {
                throw new ArgumentNullException(nameof(authorizeData));
            }

            // Avoid allocating enumerator if the data is known to be empty
            var skipEnumeratingData = false;
            if (authorizeData is IList<IAuthorizeData> dataList)
            {
                skipEnumeratingData = dataList.Count == 0;
            }

            AuthorizationPolicyBuilder? policyBuilder = null;
            if (!skipEnumeratingData)
            {
                foreach (var authorizeDatum in authorizeData)
                {
                    if (policyBuilder == null)
                    {
                        policyBuilder = new AuthorizationPolicyBuilder();
                    }

                    var useDefaultPolicy = true;
                    if (!string.IsNullOrWhiteSpace(authorizeDatum.Policy))
                    {
                        var policy = await policyProvider.GetPolicyAsync(authorizeDatum.Policy);
                        if (policy == null)
                        {
                            throw new InvalidOperationException();
                        }
                        policyBuilder.Combine(policy);
                        useDefaultPolicy = false;
                    }

                    var rolesSplit = authorizeDatum.Roles?.Split(',');
                    if (rolesSplit?.Length > 0)
                    {
                        var trimmedRolesSplit = rolesSplit.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim());
                        policyBuilder.RequireRole(trimmedRolesSplit);
                        useDefaultPolicy = false;
                    }

                    var authTypesSplit = authorizeDatum.AuthenticationSchemes?.Split(',');
                    if (authTypesSplit?.Length > 0)
                    {
                        foreach (var authType in authTypesSplit)
                        {
                            if (!string.IsNullOrWhiteSpace(authType))
                            {
                                policyBuilder.AuthenticationSchemes.Add(authType.Trim());
                            }
                        }
                    }

                    if (useDefaultPolicy)
                    {
                        policyBuilder.Combine(await policyProvider.GetDefaultPolicyAsync());
                    }
                }
            }

            // If we have no policy by now, use the fallback policy if we have one
            if (policyBuilder == null)
            {
                var fallbackPolicy = await policyProvider.GetFallbackPolicyAsync();
                if (fallbackPolicy != null)
                {
                    return fallbackPolicy;
                }
            }

            return policyBuilder?.Build();
        }
    }
    #endregion

    #region IOC
    public static class CustomPolicyServiceCollectionExtensions
    {
        public static IServiceCollection CustomAddAuthorization(this IServiceCollection services)
        {
            //services.TryAddTransient<IPolicyEvaluator, PolicyEvaluator>();
            //services.TryAddTransient<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();
            //services.TryAdd(ServiceDescriptor.Transient<IAuthorizationService, DefaultAuthorizationService>());
            //services.TryAdd(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>());
            //services.TryAdd(ServiceDescriptor.Transient<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>());
            //services.TryAdd(ServiceDescriptor.Transient<IAuthorizationEvaluator, DefaultAuthorizationEvaluator>());
            //services.TryAdd(ServiceDescriptor.Transient<IAuthorizationHandlerContextFactory, DefaultAuthorizationHandlerContextFactory>());
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, PassThroughAuthorizationHandler>());

            services.Replace(ServiceDescriptor.Transient<IPolicyEvaluator, CustomPolicyEvaluator>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationService, CustomDefaultAuthorizationService>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, CustomDefaultAuthorizationPolicyProvider>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationHandlerProvider, CustomDefaultAuthorizationHandlerProvider>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationEvaluator, CustomDefaultAuthorizationEvaluator>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationHandlerContextFactory, CustomDefaultAuthorizationHandlerContextFactory>());
            services.Replace(ServiceDescriptor.Transient<IAuthorizationHandler, CustomPassThroughAuthorizationHandler>());

            return services;
        }
    }

    public class CustomDefaultAuthorizationService : IAuthorizationService
    {
        private readonly AuthorizationOptions _options;
        private readonly IAuthorizationHandlerContextFactory _contextFactory;
        private readonly IAuthorizationHandlerProvider _handlers;
        private readonly IAuthorizationEvaluator _evaluator;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultAuthorizationService"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IAuthorizationPolicyProvider"/> used to provide policies.</param>
        /// <param name="handlers">The handlers used to fulfill <see cref="IAuthorizationRequirement"/>s.</param>
        /// <param name="logger">The logger used to log messages, warnings and errors.</param>  
        /// <param name="contextFactory">The <see cref="IAuthorizationHandlerContextFactory"/> used to create the context to handle the authorization.</param>  
        /// <param name="evaluator">The <see cref="IAuthorizationEvaluator"/> used to determine if authorization was successful.</param>  
        /// <param name="options">The <see cref="AuthorizationOptions"/> used.</param>  
        public CustomDefaultAuthorizationService(IAuthorizationPolicyProvider policyProvider, IAuthorizationHandlerProvider handlers, ILogger<DefaultAuthorizationService> logger, IAuthorizationHandlerContextFactory contextFactory, IAuthorizationEvaluator evaluator, IOptions<AuthorizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (contextFactory == null)
            {
                throw new ArgumentNullException(nameof(contextFactory));
            }
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            _options = options.Value;
            _handlers = handlers;
            _policyProvider = policyProvider;
            _logger = logger;
            _evaluator = evaluator;
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Checks if a user meets a specific set of requirements for the specified resource.
        /// </summary>
        /// <param name="user">The user to evaluate the requirements against.</param>
        /// <param name="resource">The resource to evaluate the requirements against.</param>
        /// <param name="requirements">The requirements to evaluate.</param>
        /// <returns>
        /// A flag indicating whether authorization has succeeded.
        /// This value is <value>true</value> when the user fulfills the policy otherwise <value>false</value>.
        /// </returns>
        public virtual async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            var authContext = _contextFactory.CreateContext(requirements, user, resource);
            var handlers = await _handlers.GetHandlersAsync(authContext);
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(authContext);
                if (!_options.InvokeHandlersAfterFailure && authContext.HasFailed)
                {
                    break;
                }
            }

            var result = _evaluator.Evaluate(authContext);
            if (result.Succeeded)
            {
                //_logger.UserAuthorizationSucceeded();
            }
            else
            {
                //_logger.UserAuthorizationFailed(result.Failure!);
            }
            return result;
        }

        /// <summary>
        /// Checks if a user meets a specific authorization policy.
        /// </summary>
        /// <param name="user">The user to check the policy against.</param>
        /// <param name="resource">The resource the policy should be checked with.</param>
        /// <param name="policyName">The name of the policy to check against a specific context.</param>
        /// <returns>
        /// A flag indicating whether authorization has succeeded.
        /// This value is <value>true</value> when the user fulfills the policy otherwise <value>false</value>.
        /// </returns>
        public virtual async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            var policy = await _policyProvider.GetPolicyAsync(policyName);
            if (policy == null)
            {
                throw new InvalidOperationException($"No policy found: {policyName}.");
            }
            return await this.AuthorizeAsync(user, resource, policy);
        }
    }

    public class CustomDefaultAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        private Task<AuthorizationPolicy>? _cachedDefaultPolicy;
        private Task<AuthorizationPolicy?>? _cachedFallbackPolicy;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultAuthorizationPolicyProvider"/>.
        /// </summary>
        /// <param name="options">The options used to configure this instance.</param>
        public CustomDefaultAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        /// <summary>
        /// Gets the default authorization policy.
        /// </summary>
        /// <returns>The default authorization policy.</returns>
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            if (_cachedDefaultPolicy == null || _cachedDefaultPolicy.Result != _options.DefaultPolicy)
            {
                _cachedDefaultPolicy = Task.FromResult(_options.DefaultPolicy);
            }

            return _cachedDefaultPolicy;
        }

        /// <summary>
        /// Gets the fallback authorization policy.
        /// </summary>
        /// <returns>The fallback authorization policy.</returns>
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            if (_cachedFallbackPolicy == null || _cachedFallbackPolicy.Result != _options.FallbackPolicy)
            {
                _cachedFallbackPolicy = Task.FromResult(_options.FallbackPolicy);
            }

            return _cachedFallbackPolicy;
        }

        /// <summary>
        /// Gets a <see cref="AuthorizationPolicy"/> from the given <paramref name="policyName"/>
        /// </summary>
        /// <param name="policyName">The policy name to retrieve.</param>
        /// <returns>The named <see cref="AuthorizationPolicy"/>.</returns>
        public virtual Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // MVC caches policies specifically for this class, so this method MUST return the same policy per
            // policyName for every request or it could allow undesired access. It also must return synchronously.
            // A change to either of these behaviors would require shipping a patch of MVC as well.
            return Task.FromResult(_options.GetPolicy(policyName));
        }
    }

    public class CustomDefaultAuthorizationHandlerProvider : IAuthorizationHandlerProvider
    {
        private readonly IEnumerable<IAuthorizationHandler> _handlers;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultAuthorizationHandlerProvider"/>.
        /// </summary>
        /// <param name="handlers">The <see cref="IAuthorizationHandler"/>s.</param>
        public CustomDefaultAuthorizationHandlerProvider(IEnumerable<IAuthorizationHandler> handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            _handlers = handlers;
        }

        /// <inheritdoc />
        public Task<IEnumerable<IAuthorizationHandler>> GetHandlersAsync(AuthorizationHandlerContext context)
            => Task.FromResult(_handlers);
    }

    public class CustomDefaultAuthorizationEvaluator : IAuthorizationEvaluator
    {
        /// <summary>
        /// Determines whether the authorization result was successful or not.
        /// </summary>
        /// <param name="context">The authorization information.</param>
        /// <returns>The <see cref="AuthorizationResult"/>.</returns>
        public AuthorizationResult Evaluate(AuthorizationHandlerContext context)
            => context.HasSucceeded
                ? AuthorizationResult.Success()
                : AuthorizationResult.Failed(context.HasFailed
                    ? AuthorizationFailure.ExplicitFail()
                    : AuthorizationFailure.Failed(context.PendingRequirements));
    }

    /// <summary>
    /// A type used to provide a <see cref="AuthorizationHandlerContext"/> used for authorization.
    /// </summary>
    public class CustomDefaultAuthorizationHandlerContextFactory : IAuthorizationHandlerContextFactory
    {
        /// <summary>
        /// Creates a <see cref="AuthorizationHandlerContext"/> used for authorization.
        /// </summary>
        /// <param name="requirements">The requirements to evaluate.</param>
        /// <param name="user">The user to evaluate the requirements against.</param>
        /// <param name="resource">
        /// An optional resource the policy should be checked with.
        /// If a resource is not required for policy evaluation you may pass null as the value.
        /// </param>
        /// <returns>The <see cref="AuthorizationHandlerContext"/>.</returns>
        public virtual AuthorizationHandlerContext CreateContext(IEnumerable<IAuthorizationRequirement> requirements, ClaimsPrincipal user, object? resource)
        {
            return new AuthorizationHandlerContext(requirements, user, resource);
        }
    }

    public class CustomPassThroughAuthorizationHandler : IAuthorizationHandler
    {
        /// <summary>
        /// Makes a decision if authorization is allowed.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var handler in context.Requirements.OfType<IAuthorizationHandler>())
            {
                await handler.HandleAsync(context);
            }
        }
    }

    public class CustomPolicyEvaluator : IPolicyEvaluator
    {
        private readonly IAuthorizationService _authorization;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authorization">The authorization service.</param>
        public CustomPolicyEvaluator(IAuthorizationService authorization)
        {
            _authorization = authorization;
        }

        /// <summary>
        /// Does authentication for <see cref="AuthorizationPolicy.AuthenticationSchemes"/> and sets the resulting
        /// <see cref="ClaimsPrincipal"/> to <see cref="HttpContext.User"/>.  If no schemes are set, this is a no-op.
        /// </summary>
        /// <param name="policy">The <see cref="AuthorizationPolicy"/>.</param>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns><see cref="AuthenticateResult.Success"/> unless all schemes specified by <see cref="AuthorizationPolicy.AuthenticationSchemes"/> failed to authenticate.  </returns>
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            if (policy.AuthenticationSchemes != null && policy.AuthenticationSchemes.Count > 0)
            {
                ClaimsPrincipal? newPrincipal = null;
                foreach (var scheme in policy.AuthenticationSchemes)
                {
                    var result = await context.AuthenticateAsync(scheme);
                    if (result != null && result.Succeeded)
                    {
                        newPrincipal = SecurityHelper.MergeUserPrincipal(newPrincipal, result.Principal);
                    }
                }

                if (newPrincipal != null)
                {
                    context.User = newPrincipal;
                    return AuthenticateResult.Success(new AuthenticationTicket(newPrincipal, string.Join(";", policy.AuthenticationSchemes)));
                }
                else
                {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity());
                    return AuthenticateResult.NoResult();
                }
            }

            return (context.User?.Identity?.IsAuthenticated ?? false)
                ? AuthenticateResult.Success(new AuthenticationTicket(context.User, "context.User"))
                : AuthenticateResult.NoResult();
        }

        internal static class SecurityHelper
        {
            /// <summary>
            /// Add all ClaimsIdentities from an additional ClaimPrincipal to the ClaimsPrincipal
            /// Merges a new claims principal, placing all new identities first, and eliminating
            /// any empty unauthenticated identities from context.User
            /// </summary>
            /// <param name="existingPrincipal">The <see cref="ClaimsPrincipal"/> containing existing <see cref="ClaimsIdentity"/>.</param>
            /// <param name="additionalPrincipal">The <see cref="ClaimsPrincipal"/> containing <see cref="ClaimsIdentity"/> to be added.</param>
            public static ClaimsPrincipal MergeUserPrincipal(ClaimsPrincipal? existingPrincipal, ClaimsPrincipal? additionalPrincipal)
            {
                var newPrincipal = new ClaimsPrincipal();

                // New principal identities go first
                if (additionalPrincipal != null)
                {
                    newPrincipal.AddIdentities(additionalPrincipal.Identities);
                }

                // Then add any existing non empty or authenticated identities
                if (existingPrincipal != null)
                {
                    newPrincipal.AddIdentities(existingPrincipal.Identities.Where(i => i.IsAuthenticated || i.Claims.Any()));
                }
                return newPrincipal;
            }
        }

        /// <summary>
        /// Attempts authorization for a policy using <see cref="IAuthorizationService"/>.
        /// </summary>
        /// <param name="policy">The <see cref="AuthorizationPolicy"/>.</param>
        /// <param name="authenticationResult">The result of a call to <see cref="AuthenticateAsync(AuthorizationPolicy, HttpContext)"/>.</param>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="resource">
        /// An optional resource the policy should be checked with.
        /// If a resource is not required for policy evaluation you may pass null as the value.
        /// </param>
        /// <returns>Returns <see cref="PolicyAuthorizationResult.Success"/> if authorization succeeds.
        /// Otherwise returns <see cref="PolicyAuthorizationResult.Forbid(AuthorizationFailure)"/> if <see cref="AuthenticateResult.Succeeded"/>, otherwise
        /// returns  <see cref="PolicyAuthorizationResult.Challenge"/></returns>
        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            var result = await _authorization.AuthorizeAsync(context.User, resource, policy);
            if (result.Succeeded)
            {
                return PolicyAuthorizationResult.Success();
            }

            // If authentication was successful, return forbidden, otherwise challenge
            return (authenticationResult.Succeeded)
                ? PolicyAuthorizationResult.Forbid(result.Failure)
                : PolicyAuthorizationResult.Challenge();
        }
    }

    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        /// <inheritdoc />
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                if (policy.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ChallengeAsync(scheme);
                    }
                }
                else
                {
                    await context.ChallengeAsync();
                }

                return;
            }
            else if (authorizeResult.Forbidden)
            {
                if (policy.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ForbidAsync(scheme);
                    }
                }
                else
                {
                    await context.ForbidAsync();
                }

                return;
            }

            await next(context);
        }
    }
    #endregion
}
