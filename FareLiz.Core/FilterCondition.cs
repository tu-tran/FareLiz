using SkyDean.FareLiz.Core.Data;
using System;
using System.Collections.Generic;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Helper object for storing the fare filter condition
    /// </summary>
    public class FilterCondition
    {
        /// <summary>
        /// Departure airport
        /// </summary>
        public Airport Departure { get; set; }

        /// <summary>
        /// Destination airport
        /// </summary>
        public Airport Destination { get; set; }

        /// <summary>
        /// Departure date
        /// </summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Return date
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Minimum stay duration
        /// </summary>
        public int MinDuration { get; set; }

        /// <summary>
        /// Maximum stay duration
        /// </summary>
        public int MaxDuration { get; set; }

        /// <summary>
        /// Price limit
        /// </summary>
        public int MaxPrice { get; set; }

        /// <summary>
        /// List of operators and their visibility
        /// </summary>
        public Dictionary<string, bool> Operators { get; set; }

        public FilterCondition(Airport origin, Airport destination, int minDuration, int maxDuration, int maxPrice, DateTime departureDate,
                               DateTime returnDate, Dictionary<string, bool> operators)
        {
            Departure = origin;
            Destination = destination;
            MinDuration = minDuration;
            MaxDuration = maxDuration;
            MaxPrice = maxPrice;
            DepartureDate = departureDate;
            ReturnDate = returnDate;
            Operators = operators;
        }
    }
}