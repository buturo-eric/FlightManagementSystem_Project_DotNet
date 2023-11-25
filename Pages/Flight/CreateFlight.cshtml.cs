using FMS.Pages.Passenger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Flight
{
    public class CreateFlightModel : PageModel
    {
        String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";

        public Flight flight = new Flight();

        public List<Flight> flightList = new List<Flight>();

        public string message = "";
        public void OnGet()
        {
            if (TempData.Count > 0)
                message = TempData["Message"] as string;

            loadFlightList();
        }

        public void loadFlightList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {

                    string qry = "SELECT * FROM FLIGHTS";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Flight flight = new Flight();

                                flight.flightID = reader.GetString(0);
                                flight.airlineId = reader.GetString(1);
                                flight.origin = reader.GetString(2);
                                flight.destination = reader.GetString(3);
                                flight.departureTime = reader.GetDateTime(4);
                                flight.arrivalTime = reader.GetDateTime(5);
                                flight.availableSeats = reader.GetString(6);
                                flight.ticketPrice = reader.GetString(7);


                                flightList.Add(flight);
                            }
                        }
                    }
                    con.Close(); ;
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

            // saving to DB
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO FLIGHTS VALUES (@flightId, @airlineId, @origin, @destination ,@departureTime, @arrivalTime, @availableSeats, @ticketPrice)";

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
                            message = "Flight Added Successfully";
                            flight = new Flight(); // empty the inputs
                            loadFlightList();
                        }
                        else
                        {
                            message = "Flight Not Added Successfully";
                            loadFlightList();
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY "))
                    {
                        message = "There's a problem: Flight already exists";
                        loadFlightList();
                    }
                    else
                    {
                        message = "There's a problem: " + ex.Message;
                        loadFlightList();
                    }

                }
            }
        }
    }
}
