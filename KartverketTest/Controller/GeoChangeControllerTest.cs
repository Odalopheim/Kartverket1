using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Kartverket.Controllers;
using Kartverket.Services;
using Kartverket.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Kartverket.Data;
using Microsoft.Extensions.Logging;
using System.Data;

public class GeoChangeControllerTest
{
    //Tester når data er ugyldig
    [Fact]
    public async Task RegisterAreaChange_Post_ReturnsViewWithModel_WhenDataIsInvalid()
    {
        // Arrange
        var geoJson = ""; // Invalid GeoJson
        var description = ""; // Invalid Description
        var category = GeoChangeCategory.Bilvei;

        var mockGeoChangeServiceLogger = new Mock<ILogger<GeoChangeService>>();
        var mockControllerLogger = new Mock<ILogger<GeoChangeController>>();
        var mockDbConnection = new Mock<IDbConnection>();

        // Opprett tjenesten med mocked avhengigheter
        var geoChangeService = new GeoChangeService(mockDbConnection.Object, mockGeoChangeServiceLogger.Object);
        var mockGeoChangeService = new Mock<GeoChangeService>(mockDbConnection.Object, mockGeoChangeServiceLogger.Object);

        // Mock UserManager
        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

        var user = new IdentityUser { Id = "user123" };
        mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                       .ReturnsAsync(user);

        var controller = new GeoChangeController(mockControllerLogger.Object, mockGeoChangeService.Object, mockUserManager.Object, null);

        // Add model state error to simulate invalid data
        controller.ModelState.AddModelError("GeoJson", "Required");
        controller.ModelState.AddModelError("Description", "Required");

        // Act
        var result = await controller.RegisterAreaChange(geoJson, description, category);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.False(controller.ModelState.IsValid);
    }
}
