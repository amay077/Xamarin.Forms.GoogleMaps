
namespace Maui.GoogleMaps
{
    public class Bounds
    {
        public Position SouthWest { get; }
        public Position NorthEast { get; }
        public Position SouthEast
        {
            get
            {
                return new Position(SouthWest.Latitude, NorthEast.Longitude);
            }
        }

        public Position NorthWest {
            get
            {
                return new Position(NorthEast.Latitude, SouthWest.Longitude);
            }
        }

        public Position Center
        {
            get
            {
                return new Position((SouthWest.Latitude + NorthEast.Latitude) / 2d,
                              (SouthWest.Longitude + NorthEast.Longitude) / 2d);
            }
        }

        public double WidthDegrees
        {
            get
            {
                return Math.Abs(NorthEast.Longitude - SouthWest.Longitude);
            }
        }

        public double HeightDegrees
        {
            get
            {
                return Math.Abs(NorthEast.Latitude - SouthWest.Latitude);
            }
        }

        public static Bounds FromPositions(IEnumerable<Position> positions)
        {
            if (positions == null)
            {
                throw new ArgumentNullException(nameof(positions));
            }

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            var isEmpty = true;

            foreach(var p in positions)
            {
                isEmpty = false;
                minX = Math.Min(minX, p.Longitude);
                minY = Math.Min(minY, p.Latitude);
                maxX = Math.Max(maxX, p.Longitude);
                maxY = Math.Max(maxY, p.Latitude);
            }

            if (isEmpty)
            {
                throw new ArgumentException(@"{nameof(positions)} is empty");
            }

            return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
        }

        public Bounds(Position southWest, Position northEast)
        {
            SouthWest = southWest;
            NorthEast = northEast;
        }

        public Bounds Including(Position position)
        {
            var minX = Math.Min(SouthWest.Longitude, position.Longitude);
            var minY = Math.Min(SouthWest.Latitude, position.Latitude);
            var maxX = Math.Max(NorthEast.Longitude, position.Longitude);
            var maxY = Math.Max(NorthEast.Latitude, position.Latitude);

            return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
        }

        public Bounds Including(Bounds other)
        {
            var minX = Math.Min(SouthWest.Longitude, other.SouthEast.Longitude);
            var minY = Math.Min(SouthWest.Latitude, other.SouthWest.Latitude);
            var maxX = Math.Max(NorthEast.Longitude, other.NorthEast.Longitude);
            var maxY = Math.Max(NorthEast.Latitude, other.NorthEast.Latitude);

            return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
        }

        public bool Contains(Position position)
        {
            return SouthWest.Longitude <= position.Longitude && position.Longitude <= NorthEast.Longitude
                    && SouthWest.Latitude <= position.Latitude && position.Latitude <= NorthEast.Latitude;
        }
    }
}


