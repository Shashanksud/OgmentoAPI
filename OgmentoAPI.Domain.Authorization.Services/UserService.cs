﻿using OgmentoAPI.Domain.Authorization.Abstractions.DataContext;
using OgmentoAPI.Domain.Authorization.Abstractions.Enums;
using OgmentoAPI.Domain.Authorization.Abstractions.Models;
using OgmentoAPI.Domain.Authorization.Abstractions.Repository;
using OgmentoAPI.Domain.Authorization.Abstractions.Services;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Service;

namespace OgmentoAPI.Domain.Authorization.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthorizationRepository _context;
        private readonly ISalesCenterService _salesCenterService;
		private readonly IKioskService _kioskService;

		public UserService(IAuthorizationRepository context, ISalesCenterService salesCenterService, IKioskService kioskService)
        {
            _context = context;
            _salesCenterService = salesCenterService;
			_kioskService = kioskService;
        }

        public UserModel GetUserDetail(int userId)
        {
            UserModel user = new UserModel();

            try
            {
                user = _context.GetUserByID(userId);
                UserRoles userRole = _context.GetUserRole(userId).RoleId;
                List<SalesCenter> salesCenterNames = _salesCenterService.GetSalesCenterForUser(userId).ToList();

                Dictionary<Guid, string> salesCenterDictionary = salesCenterNames.ToDictionary(sc => sc.SalesCenterUid, sc => sc.SalesCenterName);

                if (user != null)
                {
                    user.RoleId = userRole;
                    user.SalesCenters = salesCenterDictionary;
                    return user;
                }
                else
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserModel> GetUserDetails()
        {
            try
            {
                List<UserModel> userModels = _context.GetUserDetails();
                userModels.ForEach(userModel =>
                {
                    UserRoles userRole = _context.GetUserRole(userModel.UserId).RoleId;
					List<SalesCenter> salesCenterList = _salesCenterService.GetSalesCenterForUser(userModel.UserId).ToList();
					List<int> saleCenterIds = salesCenterList.Select(s => s.ID).ToList();
					List<KioskModel> kioskDetails = _kioskService.GetKioskDetails(saleCenterIds);
					string kioskNames = string.Join(", ", kioskDetails.Select(kiosk => kiosk.KioskName));
					userModel.KioskName = kioskNames;
					Dictionary<Guid, string> salesCenterDictionary = salesCenterList.ToDictionary(sc => sc.SalesCenterUid, sc => sc.SalesCenterName);

                    userModel.RoleId = userRole;
					userModel.SalesCenters = salesCenterDictionary;
				});
                return userModels;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int? UpdateUser(UserModel user)
        {
            try
            {
                List<Guid> guidList = new List<Guid>(user.SalesCenters.Keys);

				int? userId = _context.GetUserId(user.UserUid);
				_salesCenterService.UpdateSalesCenters(userId.Value, guidList);
                return _context.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddUser(UserModel user)
        {
            try
            {
                int userId = _context.AddUser(user);
                List<Guid> guidList = new List<Guid>(user.SalesCenters.Keys);
				_salesCenterService.UpdateSalesCenters(userId, guidList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteUser(Guid userUId)
		{
			int? userId = _context.GetUserId(userUId);
			_salesCenterService.DeleteSalesCenterUserMapping(userId.Value);
			return _context.DeleteUserDetails(userUId);
        }

	}
}

