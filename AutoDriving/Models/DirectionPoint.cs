using AutoDriving.Common;

namespace AutoDriving.Models
{
    public class DirectionPoint
    {
        public Direction LeftDirection { get; set; }
        public Direction CurrentDirection { get; set; }
        public Direction RightDirection { get; set; }
    }
}
