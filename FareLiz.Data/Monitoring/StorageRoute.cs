namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;

    /// <summary>This class is a streamlined version for Route class, which is used for the sole purpose of storing live fare data metadata</summary>
    public sealed class StorageRoute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageRoute"/> class.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        public StorageRoute(string origin, string destination)
        {
            this.Departure = origin;
            this.Destination = destination;
        }

        /// <summary>Gets or sets the departure.</summary>
        public string Departure { get; set; }

        /// <summary>Gets or sets the destination.</summary>
        public string Destination { get; set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return this.Departure + " - " + this.Destination;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as StorageRoute;
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Departure, other.Departure, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(this.Destination, other.Destination, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>The <see cref="int" />.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

    /// <summary>Contains StorageRoute object with an ID</summary>
    public class RouteInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteInfo"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        public RouteInfo(int id, StorageRoute route)
        {
            this.Id = id;
            this.Route = route;
        }

        /// <summary>Gets or sets the id.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the route.</summary>
        public StorageRoute Route { get; set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            if (this.Route != null)
            {
                return this.Route.Departure + " - " + this.Route.Destination;
            }

            return base.ToString();
        }
    }
}