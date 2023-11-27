namespace FMS.Pages.Booking
{
    public class CheckFlight
    {
        public string? flightID { get; set; }
        public String? airlineId { get; set; }
        public String? origin { get; set; }
        public String? destination { get; set; }
        public DateTime? departureTime { get; set; }
        public DateTime? arrivalTime { get; set; }
        public string? availableSeats { get; set; }
        public string? ticketPrice { get; set; }

        public CheckFlight() { }

        public CheckFlight(string? flightID, string? airlineId, string? origin, string? destination, DateTime? departureTime, DateTime? arrivalTime, string? availableSeats, string? ticketPrice)
        {
            this.flightID = flightID;
            this.airlineId = airlineId;
            this.origin = origin;
            this.destination = destination;
            this.departureTime = departureTime;
            this.arrivalTime = arrivalTime;
            this.availableSeats = availableSeats;
            this.ticketPrice = ticketPrice;
        }
    }


}
