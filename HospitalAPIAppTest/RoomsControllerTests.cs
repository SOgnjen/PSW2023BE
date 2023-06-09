using HospitalAPI.Controllers;
using HospitalLibrary.Core.Model;
using HospitalLibrary.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HospitalAPIAppTest
{
    public class RoomsControllerTests
    {
        private readonly RoomsController _controller;
        private readonly Mock<IRoomService> _roomServiceMock;

        public RoomsControllerTests()
        {
            _roomServiceMock = new Mock<IRoomService>();
            _controller = new RoomsController(_roomServiceMock.Object);
        }

        [Fact]
        public void GetAll()
        {
            var rooms = new List<Room> { new Room { Id = 1, Number = "101A", Floor = 1 } };
            _roomServiceMock.Setup(service => service.GetAll()).Returns(rooms);

            var result = _controller.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            var returnedRooms = Assert.IsAssignableFrom<IEnumerable<Room>>(result.Value);
            Assert.Equal(rooms, returnedRooms);
        }

        [Fact]
        public void GetById_ExistingId()
        {
            var roomId = 1;
            var room = new Room { Id = roomId, Number = "101A", Floor = 1 };
            _roomServiceMock.Setup(service => service.GetById(roomId)).Returns(room);

            var result = _controller.GetById(roomId) as OkObjectResult;

            Assert.NotNull(result);
            var returnedRoom = Assert.IsAssignableFrom<Room>(result.Value);
            Assert.Equal(room, returnedRoom);
        }

        [Fact]
        public void GetById_NonExistingId()
        {
            var existingRoomId = 1;
            var nonExistingRoomId = 2;

            var existingRoom = new Room { Id = existingRoomId, Number = "101A", Floor = 1 };
            _roomServiceMock.Setup(service => service.GetById(existingRoomId)).Returns(existingRoom);
            _roomServiceMock.Setup(service => service.GetById(nonExistingRoomId)).Returns((Room)null);

            var existingResult = _controller.GetById(existingRoomId);
            var nonExistingResult = _controller.GetById(nonExistingRoomId);

            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult);
            var nonExistingNotFoundResult = Assert.IsType<NotFoundResult>(nonExistingResult);

            var returnedRoom = Assert.IsAssignableFrom<Room>(existingOkResult.Value);
            Assert.Equal(existingRoom, returnedRoom);
        }

        [Fact]
        public void Create_ValidRoom()
        {
            var room = new Room { Id = 1, Number = "101A", Floor = 1 };

            var result = _controller.Create(room);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", createdAtActionResult.ActionName);
            Assert.Equal(room.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(room, createdAtActionResult.Value);
        }

        [Fact]
        public void Create_InvalidRoom()
        {
            var room = new Room { Id = 1, Number = "A", Floor = 15 };

            _controller.ModelState.AddModelError("Number", "The Number field must be a minimum length of 3 characters.");
            _controller.ModelState.AddModelError("Floor", "The Floor field must be between 1 and 10.");

            var result = _controller.Create(room);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_ValidId()
        {
            var roomId = 1;
            var updatedRoom = new Room { Id = roomId, Number = "101A", Floor = 2 };
            var roomServiceMock = new Mock<IRoomService>();
            roomServiceMock.Setup(service => service.Update(updatedRoom));

            var controller = new RoomsController(roomServiceMock.Object);

            var result = controller.Update(roomId, updatedRoom);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRoom = Assert.IsAssignableFrom<Room>(okResult.Value);
            Assert.Equal(updatedRoom, returnedRoom);
        }

        [Fact]
        public void Update_InvalidId()
        {
            var roomId = 1;
            var updatedRoom = new Room { Id = roomId, Number = "101A", Floor = 2 };
            var roomServiceMock = new Mock<IRoomService>();
            var controller = new RoomsController(roomServiceMock.Object);

            var result = controller.Update(roomId + 1, updatedRoom);

            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }


        [Fact]
        public void Delete_ExistingId()
        {
            var roomId = 1;
            var room = new Room { Id = roomId, Number = "101A", Floor = 2 };
            var roomServiceMock = new Mock<IRoomService>();
            roomServiceMock.Setup(service => service.GetById(roomId)).Returns(room);

            var controller = new RoomsController(roomServiceMock.Object);

            var result = controller.Delete(roomId);

            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_NonExistingId()
        {
            var roomId = 1;
            var roomServiceMock = new Mock<IRoomService>();
            roomServiceMock.Setup(service => service.GetById(roomId)).Returns((Room)null);

            var controller = new RoomsController(roomServiceMock.Object);

            var result = controller.Delete(roomId);

            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}
