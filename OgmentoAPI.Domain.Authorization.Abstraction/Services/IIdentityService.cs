using OgmentoAPI.Domain.Authorization.Abstractions.Models;
using System.Security.Claims;

namespace OgmentoAPI.Domain.Authorization.Abstractions.Services
{
    public interface IIdentityService
    {
        Task<TokenModel> LoginAsync(LoginModel login);
		Task LogoutAsync(ClaimsPrincipal User);
    }
}
