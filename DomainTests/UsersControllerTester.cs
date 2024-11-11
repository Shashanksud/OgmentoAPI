using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OgmentoAPI.Domain.Authorization.Abstractions.Dto;
using OgmentoAPI.Domain.Authorization.Abstractions.Models;
using OgmentoAPI.Domain.Authorization.Abstractions.Services;
using OgmentoAPI.Domain.Authorization.Api;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using OgmentoAPI.Domain.Common.Abstractions;
using static OgmentoAPI.Domain.Common.Abstractions.Helpers.Enums;
using OgmentoAPI.Domain.Authorization.Abstractions.Enums;

public class UsersControllerTests
{
	private Mock<IUserService> userServiceMock;
	private Mock<ILogger<UsersController>> loggerMock;
	private UsersController usersController;


	public Mock<HttpContext> SetupMockUser()
	{ 
		var mockClaims = new List<Claim>
	    {
		  new Claim(CustomClaimTypes.UserId, "1")

	    };

		var mockUser = new Mock<ClaimsPrincipal>();
		mockUser.Setup(m => m.Claims).Returns(mockClaims);
		mockUser.Setup(m => m.Identity.Name).Returns("TestUser");

		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(m => m.User).Returns(mockUser.Object);

		return mockHttpContext;
	}

	[SetUp]
	public void Setup()
	{
		UserMapsterConfig.RegisterUserMappings();
		userServiceMock = new Mock<IUserService>();
		loggerMock = new Mock<ILogger<UsersController>>();
		var controllerContext = new ControllerContext
		{
			HttpContext = SetupMockUser().Object
		};

		usersController = new UsersController(userServiceMock.Object, loggerMock.Object);
		usersController.ControllerContext = controllerContext;
	}

	[Test]
	public void AddUser_HappyPath_Success()
	{

		var addUserDto = new AddUserDto { UserName = "newUser" };

		var result = usersController.AddUser(addUserDto);
		var okResult = result as OkResult;

		Assert.IsNotNull(okResult);
		userServiceMock.Verify(x => x.AddUser(It.IsAny<UserModel>()), Times.Once);
	}


	/*[Test]
	public void GetCurrentUser_HappyPath_Success()
	{
		var expectedModel = new UserModel()
		{
			UserName = "yeshkumar",
			Email = "yesh@gmail.com",
			RoleId = UserRoles.Admin,
			ValidityDays = 365,
			UserUid = Guid.NewGuid(),
			City = "delhi",
			Password = "password",
			PhoneNumber = "1234567890",
		};

		var expectedDto = expectedModel.Adapt<UserDetailsDto>();

		//userServiceMock.Setup(x => x.ToDto()).Returns(expectedDto);
		userServiceMock.Setup(x => x.GetUserDetail(It.IsAny<int>())).Returns(expectedModel);
		// userServiceMock.Setup(x => x.GetUserDetail(1)).Returns(expectedModel);

		var mockClaims = new List<Claim>
		{
		   new Claim(ClaimTypes.NameIdentifier, "1"),
		   new Claim(CustomClaimTypes.UserId, "1")
		};

		var mockUser = new Mock<ClaimsPrincipal>();
		mockUser.Setup(m => m.Claims).Returns(mockClaims);
		mockUser.Setup(m => m.FindFirst(It.IsAny<String>())).Returns(mockClaims[1]);
		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(m => m.User).Returns(mockUser.Object);

		usersController.ControllerContext = new ControllerContext
		{
			HttpContext = mockHttpContext.Object
		};

		var result = usersController.GetCurrentUser();
		var okResult = result as OkObjectResult;
		var actualDto = okResult?.Value as UserDetailsDto;

		Assert.IsNotNull(okResult);
		Assert.IsNotNull(actualDto);
		Assert.AreEqual(expectedModel.UserName, actualDto.UserName);
		Assert.AreEqual(expectedDto.EmailId, actualDto.EmailId);
		Assert.AreEqual(expectedDto.RoleId, actualDto.RoleId);
		Assert.AreEqual(expectedDto.ValidityDays, actualDto.ValidityDays);
		Assert.AreEqual(expectedDto.City, actualDto.City);
		Assert.AreEqual(expectedDto.UserUid, actualDto.UserUid);
		Assert.AreEqual(expectedDto.PhoneNumber, actualDto.PhoneNumber);

	}*/

	[Test]
	public void GetUserDetails_HappyPath_Success()
	{
		var userModels = new List<UserModel> { new UserModel()
		{
			UserName = "testUser",
			KioskName = "kiosk2",
			PhoneNumber = "1234567890",
			Email = "raj@gmail.com",
			City = "Noida",
			SalesCenters = new Dictionary<Guid, string>
			{
				{ Guid.NewGuid(), "DLF" } 
            },
			ValidityDays = 365,
		    UserUid = Guid.NewGuid()

		} };
		userServiceMock.Setup(x => x.GetUserDetails()).Returns(userModels);
		var result = usersController.GetUserDetails();
		var okResult = result as OkObjectResult;
		var userDetailsDtos = okResult?.Value as List<UserDetailsDto>;

		Assert.IsNotNull(okResult);
		Assert.IsNotNull(userDetailsDtos);
		Assert.AreEqual(userModels[0].UserName, userDetailsDtos[0].UserName);
		Assert.AreEqual(userModels[0].KioskName, userDetailsDtos[0].KioskName);
		Assert.AreEqual(userModels[0].PhoneNumber, userDetailsDtos[0].PhoneNumber);
		Assert.AreEqual(userModels[0].Email, userDetailsDtos[0].EmailId);
		Assert.AreEqual(userModels[0].City, userDetailsDtos[0].City);
		Assert.AreEqual(userModels[0].SalesCenters, userDetailsDtos[0].SalesCenters);
		Assert.AreEqual(userModels[0].ValidityDays, userDetailsDtos[0].ValidityDays);
		Assert.AreEqual(userModels[0].UserUid, userDetailsDtos[0].UserUid);
	}

	[Test]
	public void DeleteUserDetails_HappyPath_Success()
	{
		var userUId = Guid.NewGuid();
		userServiceMock.Setup(x => x.DeleteUser(userUId)).Returns(true);

		var result = usersController.DeleteUserDetails(userUId);
		var okResult = result as OkObjectResult;

		Assert.IsNotNull(okResult);
		Assert.AreEqual(true, okResult.Value);
		userServiceMock.Verify(x => x.DeleteUser(userUId), Times.Once);
	}

	/*[Test]
	public void UpdateUserDetails_HappyPath_Success()
	{
		var userDetailsDto = new UserDetailsDto { UserName = "updateUser" };
		userServiceMock.Setup(x => x.UpdateUser(It.IsAny<UserModel>())).Returns(true);
		
		var result = usersController.UpdateUserDetails(userDetailsDto);
		var okResult = result as OkObjectResult;
		
		Assert.IsNotNull(okResult);
		Assert.AreEqual(true, okResult.Value);
		userServiceMock.Verify(x => x.UpdateUser(It.IsAny<UserModel>()), Times.Once);
	}*/
}