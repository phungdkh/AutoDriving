namespace AutoDriving.Models
{
    public class Field
    {
        public Field()
        {

        }

        public Field(long width, long height) : this()
        {
            Width = width;
            Height = height;
        }

        public long Width { get; set; }
        public long Height { get; set; }
    }
}
