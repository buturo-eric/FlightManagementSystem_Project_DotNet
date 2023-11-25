using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Shared.Passenger
{
    public class IndexModel : PageModel
    {
        public List<PassengerInfo> listPassengers = new List<PassengerInfo>();  
        public void OnGet()
        {
            listPassengers.Clear();
            try
            {
                String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    String sqlQuery = "SELECT * FROM USERVIEW";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PassengerInfo info = new PassengerInfo();
                                info.flightId = "" + reader.GetString(0);
                                info.firstName = "" + reader.GetString(1);
                                info.lastName = "" + reader.GetString(2);
                                info.email = "" + reader.GetString(3);
                                info.airline = "" + reader.GetString(4);
                                info.origin = "" + reader.GetString(5);
                                info.destination = "" + reader.GetString(6);
                                info.seatType = "" + reader.GetString(7);
                                info.seatNumber = "" + reader.GetString(8);
                                info.bookingDate = "" + reader.GetString(9);

                                listPassengers.Add(info);
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
        }
    }

    public class PassengerInfo
    {
        public string flightId;
        public string firstName;
        public string lastName;
        public string email;
        public string airline;
        public string origin;
        public string destination;
        public string seatType;
        public string seatNumber;
        public string bookingDate;
    }
}