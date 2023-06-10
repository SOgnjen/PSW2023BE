using HospitalAPI.Controllers;
using HospitalLibrary.Core.Model;
using HospitalLibrary.Core.Repository;
using HospitalLibrary.Core.Service;
using HospitalLibrary.Settings;
using Microsoft.AspNetCore.Mvc;
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
    public class UsersControllerTests : IDisposable
    {
        private readonly UsersController _usersController;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly HospitalDbContext _dbContext;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new HospitalDbContext(options);

            _userServiceMock = new Mock<IUserService>();
            _usersController = new UsersController(_userServiceMock.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.SaveChanges();
            _dbContext.Dispose();
        }

        [Fact]
        public void GetAll()
        {
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "User 1" },
                new User { Id = 2, FirstName = "User 2" },
                new User { Id = 3, FirstName = "User 3" }
            };
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();

            _userServiceMock.Setup(service => service.GetAll()).Returns(users);

            var result = _usersController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count());
        }

        [Fact]
        public void GetById()
        {
            var userId = 1;
            var testUser = new User { Id = userId, FirstName = "Test User" };
            var users = new List<User> { testUser };
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();

            _userServiceMock.Setup(service => service.GetById(userId)).Returns(testUser);

            var result = _usersController.GetById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsAssignableFrom<User>(okResult.Value);
            Assert.Equal(testUser, returnedUser);
        }

        [Fact]
        public void Create_ValidUser_ReturnsCreatedUser()
        {
            var userToCreate = new User { FirstName = "New User" };

            _userServiceMock
                .Setup(service => service.Create(It.IsAny<User>()))
                .Callback<User>(user =>
                {
                });

            var result = _usersController.Create(userToCreate);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsAssignableFrom<User>(createdAtActionResult.Value);
            Assert.Equal(userToCreate.FirstName, returnedUser.FirstName);

            _userServiceMock.Verify(service => service.Create(It.IsAny<User>()), Times.Once);
        }





        [Fact]
        public void Delete_ExistingUser()
        {
            var userId = 1;
            var existingUser = new User { Id = userId, FirstName = "John" };

            _userServiceMock.Setup(service => service.GetById(userId)).Returns(existingUser);

            var result = _usersController.Delete(userId);

            var noContentResult = Assert.IsType<NoContentResult>(result);

            _userServiceMock.Verify(service => service.Delete(existingUser), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingUser()
        {
            var userId = 1;
            _userServiceMock.Setup(service => service.GetById(userId)).Returns((User)null);

            var result = _usersController.Delete(userId);

            var notFoundResult = Assert.IsType<NotFoundResult>(result);

            _userServiceMock.Verify(service => service.Delete(It.IsAny<User>()), Times.Never);
        }


    }
}
