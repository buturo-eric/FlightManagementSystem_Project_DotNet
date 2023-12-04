namespace FMS.Pages.Booking
{
    public class CheckBooking
    {
        public string? flightID { get; set; }
        public int? airlineId { get; set; }
        public string airlineName { get; set; }
        public String? origin { get; set; }
        public String? destination { get; set; }
        public DateTime? departureTime { get; set; }
        public DateTime? arrivalTime { get; set; }
        public string? ticketPrice { get; set; }
        public int? userId { get; set; }

        public CheckBooking() { }

        public CheckBooking(string? flightID, int? airlineId, string airlineName, string? origin, string? destination, DateTime? departureTime, DateTime? arrivalTime, string? ticketPrice, int? userId)
        {
            this.flightID = flightID;
            this.airlineId = airlineId;
            this.airlineName = airlineName;
            this.origin = origin;
            this.destination = destination;
            this.departureTime = departureTime;
            this.arrivalTime = arrivalTime;
            this.ticketPrice = ticketPrice;
            this.userId = userId;
        }

    }
}
