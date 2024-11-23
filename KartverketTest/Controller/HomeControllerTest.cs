using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kartverket.API_Models;
using Kartverket.Controllers;
using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace KartverketTest.Controllers
{
    public class HomeControllerTest
    {

        //Tester Stedsnavn API når det ikke er noe skrevet inn 
        [Fact]
        public async Task Stedsnavn_ReturnsIndexWithError_WhenSearchTermIsEmpty()
        {
            // Arrange
            var mockService = new Mock<IStedsnavnService>();
            var controller = new HomeController(null, null, mockService.Object);

            // Act
            var result = await controller.Stedsnavn(string.Empty) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal("Please enter a valid place name.", result.ViewData["Error"]);
        }

    }
}
