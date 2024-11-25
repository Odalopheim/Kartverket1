using System.Data;
using System.Threading.Tasks;
using Kartverket.Controllers;
using Kartverket.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KartverketTest.Controllers
{
    public class AccountControllerTest
    {
        //Tester når informasjon er ugyldig
        [Fact]
        public async Task Login_ReturnsViewWithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<IdentityUser>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);
            var mockLogger = new Mock<ILogger<AccountController>>();
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockUserService = new Mock<UserService>(null, null);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockLogger.Object, mockContext.Object, mockUserService.Object);
            controller.ModelState.AddModelError("Email", "Required");

            var model = new LoginViewModel { Email = "", Password = "password" };

            // Act
            var result = await controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ViewName);
            Assert.Equal(model, result.Model);
        }
        //Tester når informasjon er gyldig
        [Fact]
        public async Task Login_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<IdentityUser>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);
            var mockLogger = new Mock<ILogger<AccountController>>();
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var mockUserService = new Mock<UserService>(null, null);

            var user = new IdentityUser { Email = "test@example.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockSignInManager.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockLogger.Object, mockContext.Object, mockUserService.Object);

            var model = new LoginViewModel { Email = "test@example.com", Password = "password" };

            // Act
            var result = await controller.Login(model);

            // Assert
            Assert.NotNull(result); // Check if result is not null
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("MinSide", redirectResult.ActionName); 
            Assert.Equal("Account", redirectResult.ControllerName); 
        }

        //Tester at saksbehandler logger inn til saksbehandler side
        [Fact]
        public async Task Login_ReturnsRedirectToSaksbehandler_WhenUserIsSaksbehandler()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<IdentityUser>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);
            var mockLogger = new Mock<ILogger<AccountController>>();
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockUserService = new Mock<UserService>(null, null);

            var user = new IdentityUser { Email = "user@kartverket.no" };
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockSignInManager.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockLogger.Object, mockContext.Object, mockUserService.Object);

            var model = new LoginViewModel { Email = "user@kartverket.no", Password = "password" };

            // Act
            var result = await controller.Login(model);

            // Assert
            Assert.NotNull(result); // Check if result is not null
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("Saksbehandler", redirectResult.ActionName); 
            Assert.Equal("Saksbehandler", redirectResult.ControllerName); 
        }
    }
}