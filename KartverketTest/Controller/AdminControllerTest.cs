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
using NuGet.Protocol;
using Microsoft.AspNetCore.Routing;

namespace KartverketTest.Controller;

public class AdminControllerTest
{
    
    [Fact]
    public async Task AdminHjemmeside_ShouldReturnSaksbehandlere()
    {
        // Arrange
        var mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        mockUserManager.Setup(um => um.GetUsersInRoleAsync("Saksbehandler"))
                       .ReturnsAsync(new List<IdentityUser> { new IdentityUser { Email = "test@saks.no" } });

        var controller = new AdminController(mockUserManager.Object);

        // Act
        var result = await controller.AdminHjemmeside();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<IdentityUser>>(viewResult.Model);
        Assert.Single(model); // Forventer én saksbehandler i testen
        Assert.Equal("test@saks.no", model.First().Email);
    }
}
