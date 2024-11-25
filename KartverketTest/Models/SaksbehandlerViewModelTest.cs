using Kartverket.Models;
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kartverket.Models;
using Xunit;

namespace Kartverket.Tests
{
    public class CreateSaksbehandlerViewModelTests
    {
        [Fact]
        public void ValidModel_ShouldPassValidation()
        {
            // Arrange
            var model = new CreateSaksbehandlerViewModel
            {
                Email = "user@Kartverket.no",
                Password = "StrongPassword123",
                ConfirmPassword = "StrongPassword123"
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
            var model = new CreateSaksbehandlerViewModel
            {
                Email = "", 
                Password = "StrongPassword123",
                ConfirmPassword = "StrongPassword123"
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
        public void InvalidModel_InvalidEmailFormat_ShouldReturnValidationError()
        {
            // Arrange
            var model = new CreateSaksbehandlerViewModel
            {
                Email = "invalid-email",
                Password = "StrongPassword123",
                ConfirmPassword = "StrongPassword123"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid); 
            Assert.Contains(results, r => r.ErrorMessage.Contains("The Email field is not a valid e-mail address."));
        }

        [Fact]
        public void InvalidModel_NonKartverketDomain_ShouldReturnValidationError()
        {
            // Arrange
            var model = new CreateSaksbehandlerViewModel
            {
                Email = "user@example.com", 
                Password = "StrongPassword123",
                ConfirmPassword = "StrongPassword123"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);
            var customValidationResults = model.Validate(context);

            // Assert
            Assert.False(isValid); // Data annotation validation should fail
            Assert.Contains(customValidationResults, r => r.ErrorMessage.Contains("Email must be from the domain @Kartverket.no"));
        }

        [Fact]
        public void InvalidModel_PasswordsDoNotMatch_ShouldReturnValidationError()
        {
            // Arrange
            var model = new CreateSaksbehandlerViewModel
            {
                Email = "user@Kartverket.no",
                Password = "Password123",
                ConfirmPassword = "DifferentPassword" 
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid); 
            Assert.Contains(results, r => r.ErrorMessage.Contains("The password and confirmation password do not match."));
        }
    }
}
    
