using FMS.Pages.Passenger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using FMS.Pages.Airline;

namespace FMS.Pages.Flight
{
    using FMS.Pages.Airline;
    public class CreateFlightModel : PageModel
    {
        String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";

        public SelectList AirlineSelectList { get; set; }



        public Flight flight = new Flight();

        public List<Flight> flightList = new List<Flight>();

        public string message = "";
        public void OnGet()
        {
            // Fetch the dictionary of airline IDs and names
            Dictionary<string, string> airlineDictionary = GetAirlineList();

            // Create a SelectList for the dropdown using the names
            AirlineSelectList = new SelectList(airlineDictionary.Values);

            if (TempData.Count > 0)
                message = TempData["Message"] as string;

            loadFlightList();
        }


        private Dictionary<string, string> GetAirlineList()
        {
            Dictionary<string, string> airlineDictionary = new Dictionary<string, string>();

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string qry = "SELECT id, name FROM AIRLINE";
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string id = reader.GetString(0);
                                string name = reader.GetString(1);

                                airlineDictionary[id] = name;
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                message = "There's a problem: " + ex.Message;
            }

            return airlineDictionary;
        }



        public void loadFlightList()
        {
            Dictionary<string, string> airlineDictionary = GetAirlineList();

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
                                flight.airlineId = reader.GetString(1); // Assuming this is the airline ID

                                // Replace the airline ID with the airline name
                                if (airlineDictionary.ContainsKey(flight.airlineId))
                                {
                                    flight.airlineId = airlineDictionary[flight.airlineId];
                                }

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
                flight.airlineId = GetAirlineIdByName(Request.Form["airlineId"]);
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

            if (flight.flightID == null || flight.airlineId == null || flight.origin == null
                || flight.destination == null || flight.departureTime == null
                || flight.arrivalTime == null || flight.availableSeats == null
                || flight.ticketPrice == null)
            {
                message = "There's a problem: One or more required fields are null.";
                loadFlightList();
                return;
            }

            // saving to DB
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO FLIGHTS (flightId, airlineId, origin, destination, departureTime, arrivalTime, availableSeats, ticketPrice) " +
                             "VALUES (@flightId, @airlineId, @origin, @destination, @departureTime, @arrivalTime, @availableSeats, @ticketPrice)";

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

        private string? GetAirlineIdByName(string airlineName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to get the ID of the airline based on the name
                    string query = "SELECT id FROM AIRLINE WHERE name = @airlineName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@airlineName", airlineName);

                        // Execute the query
                        object result = cmd.ExecuteScalar();

                        // Check if the result is not null
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            // Handle the case where the airlineName doesn't exist
                            return null; // or throw an exception, depending on your logic
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or throw as needed)
                Console.WriteLine("Error fetching airline ID: " + ex.Message);
                return null; // or throw an exception, depending on your logic
            }
        }



        private string GetAirlineName(string airlineId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to get the name of the airline based on the airlineId
                    string query = "SELECT name FROM AIRLINE WHERE id = @airlineId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@airlineId", airlineId);

                        // Execute the query
                        object result = cmd.ExecuteScalar();

                        // Check if the result is not null
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            // Handle the case where the airlineId doesn't exist
                            return "Unknown Airline";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or throw as needed)
                Console.WriteLine("Error fetching airline name: " + ex.Message);
                return "Unknown Airline";
            }
        }

    }
}
