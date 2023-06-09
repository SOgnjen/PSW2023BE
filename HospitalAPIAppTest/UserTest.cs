using HospitalLibrary.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HospitalAPIAppTest
{
    public class UserTest
    {
        [Fact]
        public void User_FullModel_ShouldPassValidation()
        {
            var user = new User
            {
                Id = 1,
                Emails = "test@example.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "555-1234",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(user);
            var validationResult = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResult, true);

            Assert.True(isValid);
            Assert.Empty(validationResult);
        }

        [Fact]
        public void User_FullModel_ShouldntPassValidation()
        {
            var user = new User
            {
                Id = 1,
                Emails = "testexample.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "555-1234",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(user);
            var validationResult = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResult, true);

            Assert.False(isValid);
            Assert.NotEmpty(validationResult);
        }

        [Fact]
        public void User_MissingRequiredProperties()
        {
            var user = new User();

            var validationContext = new ValidationContext(user);
            var validationResult = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResult, true);

            Assert.False(isValid);
            Assert.NotEmpty(validationResult);
        }

        [Fact]
        public void User_MaximumStringLengthExceeded()
        {
            var user = new User
            {
                FirstName = "This is a very long first name that exceeds the maximum length",
                LastName = "This is a very long last name that exceeds the maximum length"
            };

            var validationContext = new ValidationContext(user);
            var validationResult = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResult, true);

            Assert.False(isValid);
            Assert.NotEmpty(validationResult);
        }

        [Fact]
        public void User_InvalidPhoneNumberFormat()
        {
            var user = new User
            {
                PhoneNumber = "12345"
            };

            var validationContext = new ValidationContext(user);
            var validationResult = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResult, true);

            Assert.False(isValid);
            Assert.NotEmpty(validationResult);
        }
    }
}
