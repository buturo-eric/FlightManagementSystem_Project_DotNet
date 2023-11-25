namespace FMS.Pages.Airline
{
    public class Airline
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? contact { get; set; }        

        public Airline() { }

        public Airline(string? id, string? name, string? contact)
        {
            this.id = id;
            this.name = name;
            this.contact = contact;
        }
    }
}