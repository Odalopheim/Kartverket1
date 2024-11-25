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

namespace KartverketTest.Models
{
    public class LoginViewModelTest
    {
        [Fact]
        public void ValidModel_ShouldPassValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid); 
            Assert.Empty(results); 
        }

        [Fact]
        public void InvalidModel_MissingEmail_ShouldReturnValidationError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "", 
                Password = "Password123"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid); 
            Assert.Contains(results, r => r.ErrorMessage.Contains("The Email field is required."));
        }

        [Fact]
        public void InvalidModel_MissingPassword_ShouldReturnValidationError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "" 
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid); 
            Assert.Contains(results, r => r.ErrorMessage.Contains("The Password field is required."));
        }
    }
}
    

