namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;

    /// <summary>
    /// This class is a streamlined version for Route class, which is used for the sole purpose of storing live fare data metadata
    /// </summary>
    public sealed class StorageRoute
    {
        public string Departure { get; set; }
        public string Destination { get; set; }

        public StorageRoute(string origin, string destination)
        {
            this.Departure = origin;
            this.Destination = destination;
        }

        public override string ToString()
        {
            return this.Departure + " - " + this.Destination;
        }

        public override bool Equals(object obj)
        {
            var other = obj as StorageRoute;
            if (other == null)
                return false;

            return String.Equals(this.Departure, other.Departure, StringComparison.OrdinalIgnoreCase)
                && String.Equals(this.Destination, other.Destination, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

    /// <summary>
    /// Contains StorageRoute object with an ID
    /// </summary>
    public class RouteInfo
    {
        public int Id { get; set; }
        public StorageRoute Route { get; set; }

        public RouteInfo(int id, StorageRoute route)
        {
            this.Id = id;
            this.Route = route;
        }

        public override string ToString()
        {
            if (this.Route != null)
                return this.Route.Departure + " - " + this.Route.Destination;
            return base.ToString();
        }
    }
}
