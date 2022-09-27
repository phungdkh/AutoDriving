using AutoDriving.Common;

namespace AutoDriving.Models
{
    public class Vehicle
    {
        public string Code { get; set; } = string.Empty;

        public Position CurrentPosition { get; set; } = new();

        public Direction CurrentDirection { get; set; }

        public string Commands { get; set; } = string.Empty;
    }
}
