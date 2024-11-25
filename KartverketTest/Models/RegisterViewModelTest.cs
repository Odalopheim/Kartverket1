using System.ComponentModel.DataAnnotations;
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



namespace Kartverket.Tests
{
    public class RegistrerViewModelTests
    {
        [Fact]
        public void ValidModel_ShouldPassValidation()
        {
            // Arrange
            var model = new RegistrerViewModel
            {
                Name = "Valid Name",
                Address = "Valid Address",
                PostNumber = "1234",
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

    }
}

