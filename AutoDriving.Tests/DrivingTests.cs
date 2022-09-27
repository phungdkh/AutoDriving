using AutoDriving.Common;
using AutoDriving.Models;

namespace AutoDriving.Tests
{
    public class DrivingTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("FFRFFFRRLF")]
        public void Should_Moved_To_Destination_Position(string commands)
        {
            var vehicle = new Vehicle()
            {
                Code = "A",
                CurrentDirection = Direction.N,
                CurrentPosition = new Position(1, 2),
                Commands = commands
            };

            var field = new Field(10, 10);

            var result = DrivingHelper.SetDestinationPosition(vehicle, field);

            Assert.That(result.Warning, Is.EqualTo(string.Empty));
            Assert.That(vehicle.CurrentDirection, Is.EqualTo(Direction.S));
            Assert.That(vehicle.CurrentPosition.X, Is.EqualTo(4));
            Assert.That(vehicle.CurrentPosition.Y, Is.EqualTo(3));
        }

        [Test]
        [TestCase("FFRFFFRRLFFFFF")]
        public void Should_Throw_Warning_And_Ignore_Wrong_Command(string commands)
        {
            var vehicle = new Vehicle()
            {
                Code = "A",
                CurrentDirection = Direction.N,
                CurrentPosition = new Position(1, 2),
                Commands = commands
            };

            var field = new Field(10, 10);

            var result = DrivingHelper.SetDestinationPosition(vehicle, field);

            Assert.That(result.Warning, Is.EqualTo("Outside of the field"));
            Assert.That(result.AllowCommands, Is.EqualTo("FFRFFFRRLFFFF"));
            Assert.That(vehicle.CurrentDirection, Is.EqualTo(Direction.S));
            Assert.That(vehicle.CurrentPosition.X, Is.EqualTo(4));
            Assert.That(vehicle.CurrentPosition.Y, Is.EqualTo(0));//stopped at safe position
        }

        [Test]
        public void Should_Show_Collision()
        {
            var vehicleA = new Vehicle()
            {
                Code = "A",
                CurrentDirection = Direction.N,
                CurrentPosition = new Position(1, 2),
                Commands = "FFRFFFFRRL"
            };

            var vehicleB = new Vehicle()
            {
                Code = "B",
                CurrentDirection = Direction.W,
                CurrentPosition = new Position(7, 8),
                Commands = "FFLFFFFFFF"
            };

            var vehicles = new List<Vehicle>()
            {
                vehicleA,
                vehicleB
            };

            var field = new Field(10, 10);

            var result = DrivingHelper.CheckCollision(vehicles, field);

            Assert.NotNull(result.First);
            Assert.NotNull(result.Second);

            Assert.That(result.First.Code, Is.EqualTo("B"));
            Assert.That(result.First.CurrentPosition.X, Is.EqualTo(5));
            Assert.That(result.First.CurrentPosition.Y, Is.EqualTo(4));

            Assert.That(result.Second.Code, Is.EqualTo("A"));
            Assert.That(result.Second.CurrentPosition.X, Is.EqualTo(5));
            Assert.That(result.Second.CurrentPosition.Y, Is.EqualTo(4));

            Assert.That(result.Step, Is.EqualTo(7));
        }

        [Test]
        public void Should_Show_No_Collision()
        {
            var vehicleA = new Vehicle()
            {
                Code = "A",
                CurrentDirection = Direction.N,
                CurrentPosition = new Position(1, 2),
                Commands = "FFLFF"
            };

            var vehicleB = new Vehicle()
            {
                Code = "B",
                CurrentDirection = Direction.W,
                CurrentPosition = new Position(7, 8),
                Commands = "FFLFRFFF"
            };

            var vehicles = new List<Vehicle>()
            {
                vehicleA,
                vehicleB
            };

            var field = new Field(10, 10);

            var result = DrivingHelper.CheckCollision(vehicles, field);

            Assert.Null(result.First);
            Assert.Null(result.Second);
        }
    }
}