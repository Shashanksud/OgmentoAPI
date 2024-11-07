﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OgmentoAPI.Domain.Authorization.Abstractions.DataContext;
using OgmentoAPI.Domain.Authorization.Abstractions.Models;
using OgmentoAPI.Domain.Authorization.Abstractions.Repository;
using OgmentoAPI.Domain.Authorization.Abstractions.Services;
using OgmentoAPI.Domain.Common.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OgmentoAPI.Domain.Authorization.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ServiceConfiguration _appSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IAuthorizationRepository _contextRepository;
        private readonly ICookieService _cookieService;

        public IdentityService(IOptions<ServiceConfiguration> settings,
            TokenValidationParameters tokenValidationParameters, IAuthorizationRepository contextRepository, ICookieService cookieService)
        {
            _appSettings = settings.Value;
            _tokenValidationParameters = tokenValidationParameters;
            _contextRepository = contextRepository;
            _cookieService = cookieService;
        }
        public async Task<TokenModel> LoginAsync(LoginModel login)
        {
            TokenModel response = new TokenModel();
            try
            {
                UsersMaster loginUser = _contextRepository.GetUserDetail(login);
                if (loginUser == null)
                {
                    return response;
                }
                AuthenticationResult authenticationResult = await AuthenticateAsync(loginUser);

                if (authenticationResult != null && authenticationResult.Success)
                {
                    response = new TokenModel() { Token = authenticationResult.Token };//, RefreshToken = authenticationResult.RefreshToken };
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
		public async Task LogoutAsync(ClaimsPrincipal User)
		{
			Guid securityStamp = Guid.NewGuid();
			Claim userIdClaim = User.FindFirst(CustomClaimTypes.UserId);
			int userId = int.Parse(userIdClaim.Value);
			await _contextRepository.UpdateSecurityStamp(securityStamp,userId);
		}

		private RolesMaster GetUserRole(int userId)
        {
            try
            {
                RolesMaster rolesMaster = _contextRepository.GetUserRole(userId);
                return rolesMaster;
            }
            catch (Exception)
            {
                return new RolesMaster();
            }
        }
		private async Task<string> GetSecurityStamp(int userId)
		{
			Guid securityStamp = Guid.NewGuid();
			await _contextRepository.UpdateSecurityStamp(securityStamp, userId);
			return securityStamp.ToString();
		}
        public async Task<AuthenticationResult> AuthenticateAsync(UsersMaster user)
        {
            AuthenticationResult authenticationResult = new AuthenticationResult();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var key = Encoding.ASCII.GetBytes(_appSettings.JwtSettings.Secret);

                ClaimsIdentity Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(CustomClaimTypes.UserId, user.UserId.ToString()),
                    new Claim(CustomClaimTypes.UserUid,user.UserUid.ToString()),
                    new Claim(CustomClaimTypes.EmailId,user.Email),
                    new Claim(CustomClaimTypes.UserName,user.UserName ?? ""),
                    new Claim(CustomClaimTypes.Jti, Guid.NewGuid().ToString()),
                    new Claim(CustomClaimTypes.Role, GetUserRole(user.UserId).RoleName),
					new Claim(CustomClaimTypes.SecurityStamp,await GetSecurityStamp(user.UserId))
                    });

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = Subject,
                    Expires = DateTime.UtcNow.Add(_appSettings.JwtSettings.TokenLifetime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                authenticationResult.Token = tokenHandler.WriteToken(token);
                _cookieService.SetAuthToken(authenticationResult.Token);

                var refreshToken = new RefreshToken
                {
                    Token = Guid.NewGuid().ToString(),
                    JwtId = token.Id,
                    UserId = user.UserId,
                    CreationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6)
                };
                //Commenting it as refresh token functionality is not is use as of now but can be considered in future
                // await _context.RefreshToken.AddAsync(refreshToken);
                //    await _context.SaveChangesAsync();
                //     authenticationResult.RefreshToken = refreshToken.Token;
                authenticationResult.Success = true;
                return authenticationResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        //public async Task<ResponseModel<TokenModel>> RefreshTokenAsync(TokenModel request)
        //{

        //    ResponseModel<TokenModel> response = new ResponseModel<TokenModel>();
        //    try
        //    {
        //        var authResponse = await GetRefreshTokenAsync(request.Token, request.RefreshToken);
        //        if (!authResponse.Success)
        //        {

        //            response.IsSuccess = false;
        //            response.Message = string.Join(",", authResponse.Errors);
        //            return response;
        //        }
        //        TokenModel refreshTokenModel = new TokenModel();
        //        refreshTokenModel.Token = authResponse.Token;
        //        refreshTokenModel.RefreshToken = authResponse.RefreshToken;
        //        response.Data = refreshTokenModel;
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {


        //        response.IsSuccess = false;
        //        response.Message = "Something went wrong!";
        //        return response;
        //    }
        //}

        //private async Task<AuthenticationResult> GetRefreshTokenAsync(string token, string refreshToken)
        //{
        //    var validatedToken = GetPrincipalFromToken(token);

        //    if (validatedToken == null)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
        //    }

        //    var expiryDateUnix =
        //        long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        //    var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        //        .AddSeconds(expiryDateUnix);

        //    if (expiryDateTimeUtc > DateTime.UtcNow)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
        //    }

        //    var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        //    var storedRefreshToken = _context.RefreshToken.FirstOrDefault(x => x.Token == refreshToken);

        //    if (storedRefreshToken == null)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
        //    }

        //    if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
        //    }

        //    if (storedRefreshToken.Used.HasValue && storedRefreshToken.Used == true)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
        //    }

        //    if (storedRefreshToken.JwtId != jti)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
        //    }

        //    storedRefreshToken.Used = true;
        //    _context.RefreshToken.Update(storedRefreshToken);
        //    await _context.SaveChangesAsync();
        //    string strUserId = validatedToken.Claims.Single(x => x.Type == "UserId").Value;
        //    long userId = 0;
        //    long.TryParse(strUserId, out userId);
        //    var user = _context.UsersMaster.FirstOrDefault(c => c.UserId == userId);
        //    if (user == null)
        //    {
        //        return new AuthenticationResult { Errors = new[] { "User Not Found" } };
        //    }

        //    return await AuthenticateAsync(user);
        //}
    }
}
