
namespace Maui.GoogleMaps
{
    public sealed class MapRegion : IEquatable<MapRegion>
    {
        public Position NearLeft { get; }
        public Position NearRight { get; }
        public Position FarLeft { get; }
        public Position FarRight { get; }

        internal MapRegion(Position nearLeft, Position nearRight, Position farLeft, Position farRight)
        {
            this.NearLeft = nearLeft;
            this.NearRight = nearRight;
            this.FarLeft = farLeft;
            this.FarRight = farRight;
        }

        public bool Equals(MapRegion other)
        {
            if (other == null) 
            {
                return false;
            }

            return NearLeft.Equals(other.NearLeft)
                           && NearRight.Equals(other.NearRight)
                           && FarLeft.Equals(other.FarLeft)
                           && FarRight.Equals(other.FarRight);
        }

        public override int GetHashCode()
        {
            return NearLeft.GetHashCode() ^ NearRight.GetHashCode() 
                           ^ FarLeft.GetHashCode() ^ FarRight.GetHashCode();
        }
    }
}
