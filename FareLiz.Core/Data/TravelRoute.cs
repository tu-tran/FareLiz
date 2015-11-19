namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using ProtoBuf;

    /// <summary>Root class for all travel data. Contains the route information and all fare data</summary>
    [DebuggerDisplay("{Id} - [{Departure}]-[{Destination}]")]
    [Serializable]
    [ProtoContract]
    public class TravelRoute
    {
        /// <summary>Initializes a new instance of the <see cref="TravelRoute" /> class.</summary>
        protected TravelRoute()
        {
            this.Journeys = new List<Journey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelRoute"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="departure">
        /// The departure.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public TravelRoute(long id, Airport departure, Airport destination)
            : this()
        {
            if (departure == null)
            {
                throw new ArgumentException("Departure cannot be empty");
            }

            if (destination == null)
            {
                throw new ArgumentException("Destination cannot be empty");
            }

            this.Id = id;
            this.Departure = departure;
            this.Destination = destination;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelRoute"/> class.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        public TravelRoute(TravelRoute route)
            : this(route.Id, route.Departure, route.Destination)
        {
        }

        /// <summary>Unique ID of the route</summary>
        [ProtoMember(1)]
        public long Id { get; set; }

        /// <summary>Departure airport</summary>
        [ProtoMember(2)]
        public Airport Departure { get; set; }

        /// <summary>Destination airport</summary>
        [ProtoMember(3)]
        public Airport Destination { get; set; }

        /// <summary>All journeys of the route (different travel dates)</summary>
        [ProtoMember(4)]
        public List<Journey> Journeys { get; private set; }

        /// <summary>
        /// Add journey into the travel route
        /// </summary>
        /// <param name="journey">
        /// The journey.
        /// </param>
        public void AddJourney(Journey journey)
        {
            this.Journeys.Add(journey);
            journey.Route = this;
        }

        /// <summary>
        /// Add journeys into the travel route
        /// </summary>
        /// <param name="journeys">
        /// Journeys to be added
        /// </param>
        /// <param name="linkJourney">
        /// Set the link of the journeys to the current object
        /// </param>
        public void AddJourney(IEnumerable<Journey> journeys, bool linkJourney)
        {
            if (journeys == null)
            {
                return;
            }

            this.Journeys.AddRange(journeys);
            if (linkJourney)
            {
                foreach (var j in journeys)
                {
                    j.Route = this;
                }
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return this.Departure + " - " + this.Destination;
        }

        /// <summary>
        /// Check if 2 routes have the same departure and destination airport
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSameRoute(TravelRoute other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Departure.IATA, other.Departure.IATA, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(this.Destination.IATA, other.Destination.IATA, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>The set journey data links.</summary>
        [ProtoAfterDeserialization]
        public void SetJourneyDataLinks()
        {
            if (this.Journeys.Count > 0)
            {
                foreach (var j in this.Journeys)
                {
                    j.Route = this;
                }
            }
        }

        /// <summary>
        /// Merge duplicated travel routes
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public static void Merge(IList<TravelRoute> data)
        {
            if (data == null || data.Count < 2)
            {
                return;
            }

            var oldData = new List<TravelRoute>(data);
            data.Clear();
            foreach (var d in oldData)
            {
                bool added = false;
                foreach (var r in data)
                {
                    if (d.IsSameRoute(r))
                    {
                        added = true;
                        r.AddJourney(d.Journeys, true);
                        break;
                    }
                }

                if (!added)
                {
                    data.Add(d);
                }
            }
        }
    }
}