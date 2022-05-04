using System;
namespace Xamarin.Forms.GoogleMaps
{
    public sealed class MapRegion : IEquatable<MapRegion>
    {
        public Position NearLeft { get; }
        public Position NearRight { get; }
        public Position FarLeft { get; }
        public Position FarRight { get; }

        internal MapRegion(Position nearLeft, Position nearRight, Position farLeft, Position farRight)
        {
            NearLeft = nearLeft;
            NearRight = nearRight;
            FarLeft = farLeft;
            FarRight = farRight;
        }

        public bool Equals(MapRegion region)
        {
            if (region == null) 
            {
                return false;
            }

            return NearLeft.Equals(region.NearLeft)
                           && NearRight.Equals(region.NearRight)
                           && FarLeft.Equals(region.FarLeft)
                           && FarRight.Equals(region.FarRight);
        }

        public override int GetHashCode()
        {
            return NearLeft.GetHashCode() ^ NearRight.GetHashCode() 
                           ^ FarLeft.GetHashCode() ^ FarRight.GetHashCode();
        }
    }
}
