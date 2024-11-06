using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OgmentoAPI.Domain.Authorization.Abstractions.Repository;
using OgmentoAPI.Domain.Common.Abstractions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OgmentoAPI.Middlewares
{
	public class SecurityStampValidationHandler
	{
		private readonly RequestDelegate _next;
		private readonly IServiceProvider _serviceProvider;
		public SecurityStampValidationHandler(RequestDelegate next, IServiceProvider serviceProvider)
		{
			_next = next;
			_serviceProvider = serviceProvider;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			if (context.User.Identity.IsAuthenticated)
			{
				using(IServiceScope scope = _serviceProvider.CreateScope())
				{
					IAuthorizationRepository _authorizationRepository = scope.ServiceProvider.GetRequiredService<IAuthorizationRepository>();
					Claim userIdClaim = context.User.FindFirst(CustomClaimTypes.UserId);
					Claim securityStampClaim = context.User.FindFirst(CustomClaimTypes.SecurityStamp);
					if (userIdClaim != null && securityStampClaim != null)
					{
						int userId = int.Parse(userIdClaim.Value);
						String securityStampFromClaims = securityStampClaim.Value;
						Guid? securityStamp = await _authorizationRepository.GetSecurityStamp(userId);
						if (securityStamp != null)
						{
							if (securityStampFromClaims != securityStamp.ToString())
							{
								throw new UnauthorizedAccessException("You have been logged out.");
							}
						}
					}
				}
			}
			await _next(context);
		}
	}
}
