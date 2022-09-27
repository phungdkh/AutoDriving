using AutoDriving.Models;

namespace AutoDriving.Common
{
    public enum Direction
    {
        S = 1,
        W = 2,
        N = 3,
        E = 4
    }

    public class Command
    {
        public const char R = 'R';
        public const char L = 'L';
        public const char F = 'F';
    }

    public static class DrivingHelper
    {
        private static readonly List<DirectionPoint> CircleDirections = new List<DirectionPoint>()
        {
            new()
            {
                LeftDirection = Direction.N,
                CurrentDirection = Direction.E,
                RightDirection = Direction.S,
            },
            new()
            {
                LeftDirection = Direction.E,
                CurrentDirection = Direction.S,
                RightDirection = Direction.W,
            },
            new()
            {
                LeftDirection = Direction.S,
                CurrentDirection = Direction.W,
                RightDirection = Direction.N,
            },
            new()
            {
                LeftDirection = Direction.W,
                CurrentDirection = Direction.N,
                RightDirection = Direction.E
            }
        };

        private static void SetNextDirection(char command, Vehicle vehicle)
        {
            var currentDirectionPoint =
                CircleDirections.Single(_ => _.CurrentDirection == vehicle.CurrentDirection);

            vehicle.CurrentDirection = command switch
            {
                Command.L => currentDirectionPoint.LeftDirection,
                Command.R => currentDirectionPoint.RightDirection,
                _ => throw new NotImplementedException()
            };
        }

        private static bool CheckAllowMoveToNextPosition(Vehicle vehicle, Field field)
        {
            var x = vehicle.CurrentPosition.X;
            var y = vehicle.CurrentPosition.Y;

            switch (vehicle.CurrentDirection)
            {
                case Direction.N:
                    y++;
                    return (y <= field.Height);
                case Direction.E:
                    x++;
                    return (x <= field.Width);
                case Direction.S:
                    y--;
                    return (y >= 0);
                case Direction.W:
                    x--;
                    return (x >= 0);
                default:
                    throw new NotImplementedException();
            }
        }

        private static void MoveToNextPosition(Vehicle vehicle)
        {
            switch (vehicle.CurrentDirection)
            {
                case Direction.N:
                    vehicle.CurrentPosition.Y++;
                    break;
                case Direction.E:
                    vehicle.CurrentPosition.X++;
                    break;
                case Direction.S:
                    vehicle.CurrentPosition.Y--;
                    break;
                case Direction.W:
                    vehicle.CurrentPosition.X--;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static Vehicle? CheckPossibleCollision(Vehicle vehicle, List<Vehicle> vehicles)
        {
            return vehicles.FirstOrDefault(_ => _.Code != vehicle.Code && vehicle.CurrentPosition.X == _.CurrentPosition.X 
                                                                       && vehicle.CurrentPosition.Y == _.CurrentPosition.Y);
        }

        public static (string Warning, string AllowCommands) SetDestinationPosition(Vehicle vehicle, Field field)
        {
            var warning = string.Empty;
            var idxWarningCommand = 0;
            var currentCommands = vehicle.Commands;
            var allowCommands = currentCommands;
            foreach (var command in currentCommands)
            {
                if (command != Command.F)
                {
                    SetNextDirection(command, vehicle);
                }
                else
                {
                    if (CheckAllowMoveToNextPosition(vehicle, field))
                        MoveToNextPosition(vehicle);
                    else
                    {
                        warning = "Outside of the field";
                        allowCommands = currentCommands.Remove(idxWarningCommand);
                        break;
                    }
                }
                idxWarningCommand++;
            }

            return (warning, allowCommands);
        }

        public static (Vehicle First, Vehicle Second, int Step) CheckCollision(List<Vehicle> vehicles, Field field)
        {
            var maxCommandsLength = vehicles.OrderByDescending(_ => _.Commands.Length).First().Commands.Length;
            for (var commandIdx = 0; commandIdx < maxCommandsLength; commandIdx++)
            {
                foreach (var vehicle in vehicles)
                {
                    if (vehicle.Commands.Length > commandIdx && vehicle.Commands[commandIdx] != Command.F)
                    {
                        SetNextDirection(vehicle.Commands[commandIdx], vehicle);
                    }
                    else
                    {
                        if (CheckAllowMoveToNextPosition(vehicle, field))
                            MoveToNextPosition(vehicle);
                    }

                    var collisionVehicle = CheckPossibleCollision(vehicle, vehicles);
                    if (collisionVehicle != null)
                        return (vehicle, collisionVehicle, commandIdx + 1);
                }
            }

            return (null, null, 0)!;
        }
    }
}
