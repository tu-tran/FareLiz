using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Root class for all travel data. Contains the route information and all fare data
    /// </summary>
    [DebuggerDisplay("{Id} - [{Departure}]-[{Destination}]")]
    [Serializable, ProtoContract]
    public class TravelRoute
    {
        /// <summary>
        /// Unique ID of the route
        /// </summary>
        [ProtoMember(1)]
        public long Id { get; set; }

        /// <summary>
        /// Departure airport
        /// </summary>
        [ProtoMember(2)]
        public Airport Departure { get; set; }

        /// <summary>
        /// Destination airport
        /// </summary>
        [ProtoMember(3)]
        public Airport Destination { get; set; }

        /// <summary>
        /// All journeys of the route (different travel dates)
        /// </summary>
        [ProtoMember(4)]
        public List<Journey> Journeys { get; private set; }

        protected TravelRoute()
        {
            Journeys = new List<Journey>();
        }

        public TravelRoute(long id, Airport departure, Airport destination)
            : this()
        {
            if (departure == null)
                throw new ArgumentException("Departure cannot be empty");
            if (destination == null)
                throw new ArgumentException("Destination cannot be empty");

            Id = id;
            Departure = departure;
            Destination = destination;
        }

        public TravelRoute(TravelRoute route)
            : this(route.Id, route.Departure, route.Destination)
        { }

        /// <summary>
        /// Add journey into the travel route
        /// </summary>
        public void AddJourney(Journey journey)
        {
            Journeys.Add(journey);
            journey.Route = this;
        }

        /// <summary>
        /// Add journeys into the travel route
        /// </summary>
        /// <param name="journeys">Journeys to be added</param>
        /// <param name="linkJourney">Set the link of the journeys to the current object</param>
        public void AddJourney(IEnumerable<Journey> journeys, bool linkJourney)
        {
            if (journeys == null)
                return;

            Journeys.AddRange(journeys);
            if (linkJourney)
                foreach (var j in journeys)
                    j.Route = this;
        }

        public override string ToString()
        {
            return Departure + " - " + Destination;
        }

        /// <summary>
        /// Check if 2 routes have the same departure and destination airport
        /// </summary>
        public bool IsSameRoute(TravelRoute other)
        {
            if (other == null)
                return false;

            return String.Equals(Departure.IATA, other.Departure.IATA, StringComparison.OrdinalIgnoreCase)
                && String.Equals(Destination.IATA, other.Destination.IATA, StringComparison.OrdinalIgnoreCase);
        }

        [ProtoAfterDeserialization]
        public void SetJourneyDataLinks()
        {
            if (Journeys.Count > 0)
                foreach (var j in Journeys)
                    j.Route = this;
        }

        /// <summary>
        /// Merge duplicated travel routes
        /// </summary>
        public static void Merge(IList<TravelRoute> data)
        {
            if (data == null || data.Count < 2)
                return;

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
                    data.Add(d);
            }
        }
    }
}
