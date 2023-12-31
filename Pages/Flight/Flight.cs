﻿namespace FMS.Pages.Flight

{
    using Airline;

    public class Flight
    {
        public string? flightID { get; set; }
        public string? airlineId {  get; set; }
        public string? AirlineName { get; set; }
        public string? origin {  get; set; }
        public string? destination { get; set; }
        public DateTime? departureTime { get; set; }
        public DateTime? arrivalTime { get; set; }
        public string? availableSeats { get; set; }
        public string? ticketPrice { get; set; }

        public Flight() { }

        public Flight(string? flightID, string? airlineId, string? airlineName, string? origin, string? destination, DateTime? departureTime, DateTime? arrivalTime, string? availableSeats, string? ticketPrice)
        {
            this.flightID = flightID;
            this.airlineId = airlineId;
            AirlineName = airlineName;
            this.origin = origin;
            this.destination = destination;
            this.departureTime = departureTime;
            this.arrivalTime = arrivalTime;
            this.availableSeats = availableSeats;
            this.ticketPrice = ticketPrice;
        }
    }
}