namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>Helper object for storing the fare filter condition</summary>
    public class FilterCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCondition"/> class.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="minDuration">
        /// The min duration.
        /// </param>
        /// <param name="maxDuration">
        /// The max duration.
        /// </param>
        /// <param name="maxPrice">
        /// The max price.
        /// </param>
        /// <param name="departureDate">
        /// The departure date.
        /// </param>
        /// <param name="returnDate">
        /// The return date.
        /// </param>
        /// <param name="operators">
        /// The operators.
        /// </param>
        public FilterCondition(
            Airport origin, 
            Airport destination, 
            int minDuration, 
            int maxDuration, 
            int maxPrice, 
            DateTime departureDate, 
            DateTime returnDate, 
            Dictionary<string, bool> operators)
        {
            this.Departure = origin;
            this.Destination = destination;
            this.MinDuration = minDuration;
            this.MaxDuration = maxDuration;
            this.MaxPrice = maxPrice;
            this.DepartureDate = departureDate;
            this.ReturnDate = returnDate;
            this.Operators = operators;
        }

        /// <summary>Departure airport</summary>
        public Airport Departure { get; set; }

        /// <summary>Destination airport</summary>
        public Airport Destination { get; set; }

        /// <summary>Departure date</summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>Return date</summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>Minimum stay duration</summary>
        public int MinDuration { get; set; }

        /// <summary>Maximum stay duration</summary>
        public int MaxDuration { get; set; }

        /// <summary>Price limit</summary>
        public int MaxPrice { get; set; }

        /// <summary>List of operators and their visibility</summary>
        public Dictionary<string, bool> Operators { get; set; }
    }
}