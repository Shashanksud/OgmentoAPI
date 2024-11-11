using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Moq;
using OgmentoAPI.Domain.Client.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Service;
using OgmentoAPI.Domain.Client.Api.Kiosk;

public class KioskControllerTests
{

	private Mock<IKioskService> kioskServiceMock;
	private KioskController _kioskController; 
	[SetUp]
	public void Setup()
	{
		kioskServiceMock = new Mock<IKioskService>();
	
		
		_kioskController = new KioskController(kioskServiceMock.Object);
	}

	[Test]
	public void Constructor_HappyPath_Success()
	{
		Assert.IsNotNull(_kioskController);
	}

	[Test]
	public void GetKioskDetails_HappyPath_SuccessAsync()
	{
		var kioskList = new List<KioskModel>();
		kioskList.Add(GetKioskData());

		kioskServiceMock.Setup(x => x.GetKioskDetails()).Returns(kioskList);
		var result =  _kioskController.GetKioskDetails();
		var okResult = result as OkObjectResult;
		List<KioskDto> kioskDetailsDto = okResult?.Value as List<KioskDto>;
		Assert.IsTrue(kioskList[0].KioskName.Equals(kioskDetailsDto[0].KioskName));
		Assert.That(kioskList[0].SalesCenter.Item1,Is.EqualTo(kioskDetailsDto[0].SalesCenter.Item1));
		Assert.That(kioskList[0].SalesCenter.Item2,Is.EqualTo(kioskDetailsDto[0].SalesCenter.Item2));
		//Assert.IsTrue(result.IsCompletedSuccessfully);
	}
	[Test]
	public async Task DeleteKiosk_HappyPath_SuccessAsync()
	{
		string kioskName = "KioskToDelete";
		kioskServiceMock.Setup(x => x.DeleteKioskByName(kioskName)).Returns(Task.CompletedTask);

		var result = await _kioskController.DeleteKiosk(kioskName);

		Assert.That(result, Is.TypeOf<OkResult>());
		kioskServiceMock.Verify(x => x.DeleteKioskByName(kioskName), Times.Once);
	}

	[Test]
	public async Task UpdateKiosk_HappyPath_SuccessAsync()
	{
		string kioskName = "TestKiosk";
		Guid salesCenterUid = Guid.NewGuid();
		kioskServiceMock.Setup(x => x.UpdateKioskDetails(kioskName, salesCenterUid)).Returns(Task.CompletedTask);

		var result = await _kioskController.UpdateKioskDetails(kioskName, salesCenterUid);

		Assert.That(result, Is.TypeOf<OkResult>());
		kioskServiceMock.Verify(x => x.UpdateKioskDetails(kioskName, salesCenterUid),Times.Once);
	}

	[Test]
	public void Addkiosk_HappyPath_SuccessAsync()
	{
		var kioskDto = new KioskDto();
		kioskServiceMock.Setup(x => x.AddKiosk(It.IsAny<KioskModel>())).Returns(Task.CompletedTask);
		var result = _kioskController.AddKiosk(kioskDto).Result;
		Assert.That(result, Is.TypeOf<OkResult>());
		kioskServiceMock.Verify(x => x.AddKiosk(It.IsAny<KioskModel>()),Times.Once);
	}
	private KioskModel GetKioskData()
	{
		return new KioskModel()
		{
			ID = 1,
			IsActive = true,
			IsDeleted = false,
			KioskName = "kiosk1",
			SalesCenter = new Tuple<Guid, string>(Guid.NewGuid(), "TestSalesCenter"),
			SalesCenterId = 1,
		};
	}
}
