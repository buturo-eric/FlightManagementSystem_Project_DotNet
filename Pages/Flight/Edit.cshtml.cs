using FMS.Pages.Passenger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Flight
{
    public class EditFlightModel : PageModel
    {
        String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";

        public Flight flight = new Flight();

        public List<Flight> flightList = new List<Flight>();

        public string message = "";

        public void OnGet()
        {
            string flightId = Request.Query["flightID"];
            try
            {

                using (SqlConnection con = new SqlConnection(conString))
                {
                    string qry = "SELECT flightID, departureTime, arrivalTime, availableSeats, ticketPrice, status FROM FLIGHTS WHERE flightID=@flightID";
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightID", flightId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                flight.flightID = reader.GetString(0);
                                flight.airlineId = reader.GetString(1);
                                flight.origin = reader.GetString(2);
                                flight.destination = reader.GetString(3);
                                flight.departureTime = reader.GetDateTime(4);
                                flight.arrivalTime = reader.GetDateTime(5);
                                flight.availableSeats = reader.GetString(6);
                                flight.ticketPrice = reader.GetString(7);
                            }
                            else
                            {
                                message = "No information available for this Flight:";
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }
        }

        public void OnPost()
        {
            try
            {
                flight.flightID = Request.Form["flightid"];
                flight.airlineId = Request.Form["airlineId"];
                flight.origin = Request.Form["origin"];
                flight.destination = Request.Form["destination"];
                flight.departureTime = DateTime.Parse(Request.Form["departureTime"]);
                flight.arrivalTime = DateTime.Parse(Request.Form["arrivalTime"]);
                flight.availableSeats = Request.Form["availableSeats"];
                flight.ticketPrice = Request.Form["ticketPrice"];

            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }

            // updating info
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "UPDATE FLIGHTS SET flightID=@flightID, departureTime=@departureTime, arrivalTime=@arrivalTime, availableSeats=@availableSeats, ticketPrice=@ticketPrice, status=@status WHERE flightID=@flightID";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightId", flight.flightID);
                        cmd.Parameters.AddWithValue("@airlineId", flight.airlineId);
                        cmd.Parameters.AddWithValue("@origin", flight.origin);
                        cmd.Parameters.AddWithValue("@destination", flight.destination);
                        cmd.Parameters.AddWithValue("@departureTime", flight.departureTime);
                        cmd.Parameters.AddWithValue("@arrivalTime", flight.arrivalTime);
                        cmd.Parameters.AddWithValue("@availableSeats", flight.availableSeats);
                        cmd.Parameters.AddWithValue("@ticketPrice", flight.ticketPrice);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["Message"] = "Flight Updated Successfully";
                            Response.Redirect("/Flight/CreateFlight");
                        }
                        else
                        {
                            message = "Flight Not Updated, Id shouldn't be changed";
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    message = "Flight Not Updated Successfully" + ex.Message;
                }
            }
        }
    }
}
