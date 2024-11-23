using Kartverket.Controllers;
using Kartverket.Data;
using Kartverket.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace Kartverket.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task Login_InvalidLoginAttempt_ReturnsViewWithModel() 
        {
            // Arrange
            var userManager = Substitute.For<UserManager<IdentityUser>>( //mocking av dependencies
                Substitute.For<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null);
            var signInManager = Substitute.For<SignInManager<IdentityUser>>(
                userManager,
                Substitute.For<IHttpContextAccessor>(),
                Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>(),
                null, null, null, null);
            var logger = Substitute.For<ILogger<AccountController>>();
            var context = Substitute.For<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new AccountController(userManager, signInManager, logger, context); //instanse av controller med mocked dependencies
            var model = new LoginViewModel //instanse med email og falsk passord
            {
                Email = "test@example.com",
                Password = "InvalidPassword"
            };

            userManager.FindByEmailAsync(model.Email).Returns(Task.FromResult<IdentityUser>(null)); //settes som null, simulerer at bruker ikke er funnet

            // Act
            var result = await controller.Login(model); //logg inn metode

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedModel = Assert.IsType<LoginViewModel>(viewResult.Model); 
            Assert.Equal(model.Email, returnedModel.Email); //verifiserer email i returnert model matcher email i input model
        }
    }
}
