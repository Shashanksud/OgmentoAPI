﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OgmentoAPI.Domain.Authorization.Abstractions.Models;
using OgmentoAPI.Domain.Authorization.Abstractions.Services;
using OgmentoAPI.Domain.Common.Abstractions;

namespace OgmentoAPI.Domain.Authorization.Api
{
    public class IdentityController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<IdentityController> _logger;
        private UserModel _user;
		
		public IdentityController(IUserService userService, ILogger<IdentityController> logger)
        {
            _logger = logger;
            _userService = userService;
        }
        private int GetUserId()
        {
            int userId = 0;
            string? strUserId = User == null ? string.Empty : User.FindFirst(c => c.Type == CustomClaimTypes.UserId)?.Value;
			

			int.TryParse(strUserId, out userId);
            if(userId == 0)
            {
                throw new InvalidOperationException();
            }
            return userId;
        }
        protected UserModel Self
        {
            get
            {
                if (_user == null)
                {
                    try
                    {

                        _user = _userService.GetUserDetail(GetUserId());
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "User is null");
                        throw new InvalidOperationException();
                    }
                }

                return _user;
            }
        }
    }
}
