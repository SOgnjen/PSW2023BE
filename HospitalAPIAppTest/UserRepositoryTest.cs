using HospitalLibrary.Core.Model;
using HospitalLibrary.Core.Repository;
using HospitalLibrary.Settings;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HospitalAPIAppTest
{
    public class UserRepositoryTest
    {
        private readonly HospitalDbContext _context;
        private readonly IUserRepository _userRepository;


        public UserRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HospitalDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public void GetAll()
        {
            var users = new[]
            {
                new User { Id = 1, FirstName = "User 1" },
                new User { Id = 2, FirstName = "User 2" },
                new User { Id = 3, FirstName = "User 3" }
            };
            _context.Users.AddRange(users);
            _context.SaveChanges();

            var result = _userRepository.GetAll();

            Assert.Equal(users.Length, result.Count());
            foreach (var user in users)
            {
                Assert.Contains(user, result);
            }
        }

        [Fact]
        public void GetById()
        {
            var users = new List<User>
    {
        new User { Id = 1, FirstName = "User 1" },
        new User { Id = 2, FirstName = "User 2" }
    };

            int userId = 1;
            var expectedUser = users.FirstOrDefault(u => u.Id == userId);

            foreach (var user in users)
            {
                _context.Add(user);
            }
            _context.SaveChanges();

            var result = _userRepository.GetById(userId);

            Assert.NotNull(result);
            var returnedUser = Assert.IsType<User>(result);
            Assert.Equal(expectedUser, returnedUser);
        }

        [Fact]
        public void GetById_UsingInvalidId()
        {
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "User 1" },
                new User { Id = 2, FirstName = "User 2" }
            };

            int invalidUserId = 3;

            var result = _userRepository.GetById(invalidUserId);

            Assert.Null(result);
        }

        [Fact]
        public void Create_NewUser()
        {
            // Arrange
            var newUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Emails = "john.doe@example.com",
                Password = "password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "123456789",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            _userRepository.Create(newUser);
            var createdUser = _userRepository.GetById(newUser.Id);
            // Assert
            Assert.NotNull(createdUser);
            Assert.Equal(newUser.Id, createdUser.Id);
            Assert.Equal(newUser.FirstName, createdUser.FirstName);
            Assert.Equal(newUser.LastName, createdUser.LastName);
            Assert.Equal(newUser.Emails, createdUser.Emails);
            Assert.Equal(newUser.Password, createdUser.Password);
            Assert.Equal(newUser.Role, createdUser.Role);
            Assert.Equal(newUser.Address, createdUser.Address);
            Assert.Equal(newUser.PhoneNumber, createdUser.PhoneNumber);
            Assert.Equal(newUser.Jmbg, createdUser.Jmbg);
            Assert.Equal(newUser.Gender, createdUser.Gender);
        }

        [Fact]
        public void Create_InvalidUser()
        {
            var invalidUser = new User
            {
                FirstName = "", 
                LastName = "Doe",
                Emails = "john.doe@example.com",
                Password = "password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "123456789",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

                        var exception = Record.Exception(() => _userRepository.Create(invalidUser));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void Update_ExistingUser()
        {
            var existingUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Emails = "john.doe@example.com",
                Password = "password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "123456789",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            _userRepository.Create(existingUser);

            existingUser.FirstName = "Updated First Name";
            existingUser.LastName = "Updated Last Name";
            existingUser.Emails = "updated.email@example.com";

            _userRepository.Update(existingUser);

            var updatedUser = _userRepository.GetById(existingUser.Id);

            Assert.NotNull(updatedUser);
            Assert.Equal(existingUser.Id, updatedUser.Id);
            Assert.Equal(existingUser.FirstName, updatedUser.FirstName);
            Assert.Equal(existingUser.LastName, updatedUser.LastName);
            Assert.Equal(existingUser.Emails, updatedUser.Emails);
        }

        [Fact]
        public void Update_InvalidUser()
        {
            var existingUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Emails = "john.doe@example.com",
                Password = "password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "123456789",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            _userRepository.Create(existingUser);

            var invalidUser = new User
            {
                Id = existingUser.Id,
                FirstName = "",
                LastName = "Updated Last Name",
                Emails = "updated.email@example.com",
                Password = "updated_password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "456 New St",
                PhoneNumber = "987654321",
                Jmbg = 11111111,
                Gender = Gender.FEMALE
            };

            var exception = Record.Exception(() => _userRepository.Update(invalidUser));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void Delete_ExistingUser()
        {
            var existingUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Emails = "john.doe@example.com",
                Password = "password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "123456789",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            _userRepository.Create(existingUser);

            _userRepository.Delete(existingUser);

            var deletedUser = _userRepository.GetById(existingUser.Id);

            Assert.Null(deletedUser);
        }

        [Fact]
        public void Delete_InvalidUser()
        {
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "User 1" },
                new User { Id = 2, FirstName = "User 2" }
            };

            int invalidUserId = 3;

            foreach (var user in users)
            {
                _context.Add(user);
            }
            _context.SaveChanges();

            var invalidUser = new User { Id = invalidUserId };

            var exception = Record.Exception(() => _userRepository.Delete(invalidUser));

            Assert.NotNull(exception);
            Assert.IsType<DbUpdateConcurrencyException>(exception);
        }

    }
}
