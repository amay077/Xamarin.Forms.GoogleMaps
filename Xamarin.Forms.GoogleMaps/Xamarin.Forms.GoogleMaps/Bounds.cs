
namespace Xamarin.Forms.GoogleMaps
{
    public class Bounds
    {
        public Position SouthWest { get; }
        public Position NorthEast { get; }

        public Bounds(Position southWest, Position northEast)
        {
            SouthWest = southWest;
            NorthEast = northEast;
        }
    }
}


