namespace FMS.Pages.Booking
{
    public class Booking
    {
        public string? id { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? number { get; set; }
        public string? payment { get; set; }

        public Booking() { }

        public Booking(string? id, string? firstName, string? lastName, string? number, string? payment)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.number = number;
            this.payment = payment;
        }
    }
}
