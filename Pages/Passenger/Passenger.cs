namespace FMS.Pages.Passenger
{
    public class Passenger
    {
        public string? flightId { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? airline { get; set; }
        public string? origin { get; set; }
        public string? destination { get; set; }
        public string? seatType { get; set; }
        public string? seatNumber { get; set; }
        public string? paymentStatus { get; set; }
        public string? bookingDate { get; set; }

        public Passenger() { }

        public Passenger(string? flightId, string? firstName, string? lastName, string? airline, string? origin, string? destination, string? seatType, string? seatNumber, string? paymentStatus, string? bookingDate)
        {
            this.flightId = flightId;
            this.firstName = firstName;
            this.lastName = lastName;
            this.airline = airline;
            this.origin = origin;
            this.destination = destination;
            this.seatType = seatType;
            this.seatNumber = seatNumber;
            this.paymentStatus = paymentStatus;
            this.bookingDate = bookingDate;
        }
    }
}
