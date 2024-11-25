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

        //Tester stedsnavn når søk er gyldig
        [Fact]
        public async Task Stedsnavn_ReturnsViewWithResults_WhenSearchTermIsValid()
        {
            // Arrange
            var searchTerm = "Oslo";
            var mockService = new Mock<IStedsnavnService>();
            mockService.Setup(service => service.GetStedsnavnAsync(searchTerm))
                .ReturnsAsync(new StedsnavnResponse
                {
                    Navn = new List<Navn>
                    {
                new Navn { Skrivemåte = "Oslo", Navneobjekttype = "City", Språk = "Norwegian", Navnestatus = "Official"}
                    }
                });

            var controller = new HomeController(null, null, mockService.Object);

            // Act
            var result = await controller.Stedsnavn(searchTerm) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Stedsnavn", result.ViewName);
            var viewModel = Assert.IsType<List<StedsnavnViewModel>>(result.Model);
            Assert.Single(viewModel);
            Assert.Equal("Oslo", viewModel.First().Skrivemåte);
        }
        [Fact]
        public async Task Stedsnavn_ReturnsIndexWithError_WhenNoResultsFound()
        {
            // Arrange
            var searchTerm = "UnknownPlace";
            var mockService = new Mock<IStedsnavnService>();
            mockService.Setup(service => service.GetStedsnavnAsync(searchTerm))
                .ReturnsAsync(new StedsnavnResponse { Navn = new List<Navn>() });

            var controller = new HomeController(null, null, mockService.Object);

            // Act
            var result = await controller.Stedsnavn(searchTerm) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal($"No results found for '{searchTerm}'.", result.ViewData["Error"]);
        }

        //Tester kommuneinfo med ugyldig nummer
        [Fact]
        public async Task KommuneInfo_ReturnsIndexWithError_WhenNoResultsFound()
        {
            // Arrange
            var kommuneNr = "9999";
            var mockService = new Mock<IKommuneInfoService>();
            mockService.Setup(service => service.GetKommuneInfoAsync(kommuneNr))
                .ReturnsAsync((KommuneInfo)null);

            var controller = new HomeController(null, mockService.Object, null);

            // Act
            var result = await controller.KommuneInfo(kommuneNr) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal($"No result found for municipality number '{kommuneNr}'.", result.ViewData["Error"]);
        }

        //Tester kommuneinfo uten når nummer er tomt
        [Fact]
        public async Task KommuneInfo_ReturnsIndexWithError_WhenKommuneNrIsEmpty()
        {
            // Arrange
            var mockService = new Mock<IKommuneInfoService>();
            var controller = new HomeController(null, mockService.Object, null);

            // Act
            var result = await controller.KommuneInfo(string.Empty) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal("Please enter a valid municipality number.", result.ViewData["Error"]);
        }

        //Tester kommuneinfo når nummer er gyldig
        [Fact]
        public async Task KommuneInfo_ReturnsViewWithResults_WhenKommuneNrIsValid()
        {
            // Arrange
            var kommuneNr = "1234";
            var mockService = new Mock<IKommuneInfoService>();
            mockService.Setup(service => service.GetKommuneInfoAsync(kommuneNr))
                .ReturnsAsync(new KommuneInfo
                {
                    Kommunenavn = "Oslo",
                    Kommunenummer = "1234",
                    Fylkesnavn = "Viken",
                    SamiskForvaltningsomrade = false
                });

            var controller = new HomeController(null, mockService.Object, null);

            // Act
            var result = await controller.KommuneInfo(kommuneNr) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("KommuneInfo", result.ViewName);
            var viewModel = Assert.IsType<KommuneInfoViewModel>(result.Model);
            Assert.Equal("Oslo", viewModel.Kommunenavn);
            Assert.Equal("1234", viewModel.Kommunenummer);
        }

    }

}
