using HospitalLibrary.Core.Model;
using HospitalLibrary.Core.Repository;
using HospitalLibrary.Core.Service;
using HospitalLibrary.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HospitalAPIAppTest
{
    public class UserServiceTest : IDisposable
    {
        private readonly HospitalDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UserServiceTest()
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HospitalDbContext(options);
            _userRepository = new UserRepository(_context);
            _userService = new UserService(_userRepository);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.SaveChanges();
            _context.Dispose();
        }


        [Fact]
        public void GetAll_ReturnsAllUsers()
        {
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "User 1" },
                new User { Id = 2, FirstName = "User 2" },
                new User { Id = 3, FirstName = "User 3" }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();

            var result = _userService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count());
            foreach (var user in users)
            {
                Assert.Contains(user, result);
            }
        }

        [Fact]
        public void GetById_ExistingId()
        {
            var user = new User { Id = 1, FirstName = "John" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = _userService.GetById(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.FirstName, result.FirstName);
        }

        [Fact]
        public void GetById_NonExistingId()
        {
            var user = new User { Id = 1, FirstName = "John" };
            _context.Users.Add(user);
            _context.SaveChanges();
            var wrongId = 2;

            var exception = Record.Exception(() => _userService.GetById(wrongId));

            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
        }

        [Fact]
        public void Create_NewUser()
        {
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

            _userService.Create(newUser);
            var createdUser = _userService.GetById(newUser.Id);

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
        public void Create_InvalidEmailFormat()
        {
            var newUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Emails = "invalid_email",
                Password = "password",
                Role = UserRole.ROLE_ADMINISTRATOR,
                Address = "123 Main St",
                PhoneNumber = "123456789",
                Jmbg = 1234567890,
                Gender = Gender.MALE
            };

            var exception = Record.Exception(() => _userService.Create(newUser));

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
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var updatedUser = new User
            {
                Id = existingUser.Id,
                FirstName = "Updated John",
                LastName = "Updated Doe",
                Emails = "updated.john.doe@example.com",
                Password = "updatedpassword",
                Role = UserRole.ROLE_USER,
                Address = "456 New St",
                PhoneNumber = "987654321",
                Jmbg = 111,
                Gender = Gender.FEMALE
            };

            _context.Entry(existingUser).State = EntityState.Detached;

            _userRepository.Update(updatedUser);

            var result = _userService.GetById(existingUser.Id);
            Assert.NotNull(result);
            Assert.Equal(updatedUser.Id, result.Id);
            Assert.Equal(updatedUser.FirstName, result.FirstName);
            Assert.Equal(updatedUser.LastName, result.LastName);
            Assert.Equal(updatedUser.Emails, result.Emails);
            Assert.Equal(updatedUser.Password, result.Password);
            Assert.Equal(updatedUser.Role, result.Role);
            Assert.Equal(updatedUser.Address, result.Address);
            Assert.Equal(updatedUser.PhoneNumber, result.PhoneNumber);
            Assert.Equal(updatedUser.Jmbg, result.Jmbg);
            Assert.Equal(updatedUser.Gender, result.Gender);
        }

        [Fact]
        public void Update_InvalidLastName()
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
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var updatedUser = new User
            {
                Id = existingUser.Id,
                FirstName = "Updated John",
                LastName = "",
                Emails = "updated.john.doe@example.com",
                Password = "updatedpassword",
                Role = UserRole.ROLE_USER,
                Address = "456 New St",
                PhoneNumber = "987654321",
                Jmbg = 111,
                Gender = Gender.FEMALE
            };

            var exception = Record.Exception(() => _userService.Update(updatedUser));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
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
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            _userService.Delete(existingUser);

            var exception = Record.Exception(() => _userService.GetById(existingUser.Id));
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
        }



        [Fact]
        public void Delete_NonExistingUser()
        {
            var nonExistingId = 999;
            var nonExistingUser = new User { Id = nonExistingId };

            var exception = Record.Exception(() => _userService.Delete(nonExistingUser));

            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
        }
    }
}
