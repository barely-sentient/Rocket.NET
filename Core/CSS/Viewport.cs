namespace Rocket.Core
{
    /// <summary>
    /// Describes a viewport used for responsive CSS generation.
    /// </summary>
    public class Viewport
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Orientation Orientation { get; set; }

        public Viewport() { }

        public Viewport(int width, int height, Orientation orientation)
        {
            Width = width;
            Height = height;
            Orientation = orientation;
        }

        public Viewport(int width, int height, DeviceOrientation orientation)
        {
            Width = width;
            Height = height;
            Orientation = (Orientation)orientation;
        }
    }
}