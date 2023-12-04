using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

namespace FMS.Pages.Booking
{
    public class CheckFlightsModel : PageModel
    {
        string conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";
        public List<CheckFlight> FlightList { get; set; }

        public void OnGet()
        {
            // Fetch the flight data from the database
            FlightList = GetFlightList();
        }

        /*public IActionResult OnPostBookNow(string flightID)
        {
            // Fetch additional details about the flight based on the flightID
            CheckFlight flight = GetFlightDetails(flightID);

            if (flight != null)
            {
                // Perform the actions to save the flight details to the clientBookings table
                SaveBookingDetails(flight);

                // Show a message indicating successful booking
                TempData["BookingMessage"] = "Booked Successfully";

                // Redirect to the CreateBooking page
                return RedirectToPage("/Booking/CreateBooking");
            }
            else
            {
                TempData["BookingMessage"] = "Error: Flight details not found.";

                // Redirect back to the CheckFlights page
                return RedirectToPage("/Booking/CheckFlights");
            }
        }
        */
        private List<CheckFlight> GetFlightList()
        {
            List<CheckFlight> flightList = new List<CheckFlight>();

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to fetch flight data from the FLIGHTS table
                    string qry = "SELECT * FROM FLIGHTS";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Assuming column indices, update them based on your actual database schema
                                string flightID = reader.GetString(0);
                                string airlineId = reader.GetString(1); // Assuming this is the airline ID
                                string origin = reader.GetString(2);
                                string destination = reader.GetString(3);
                                DateTime departureTime = reader.GetDateTime(4);
                                DateTime arrivalTime = reader.GetDateTime(5);
                                string availableSeats = reader.GetString(6);
                                string ticketPrice = reader.GetString(7);

                                // Replace the airline ID with the airline name
                                string airlineName = GetAirlineName(airlineId);

                                // Create a CheckFlight object and add it to the list
                                CheckFlight flight = new CheckFlight(flightID, airlineName, origin, destination, departureTime, arrivalTime, availableSeats, ticketPrice);
                                flightList.Add(flight);
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine("There's a problem fetching flight data: " + ex.Message);
            }

            return flightList;
        }

        private CheckFlight GetFlightDetails(string flightID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to fetch flight details from the FLIGHTS table based on flightID
                    string qry = "SELECT * FROM FLIGHTS WHERE flightID = @flightID";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightID", flightID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming column indices, update them based on your actual database schema
                                CheckFlight flight = new CheckFlight
                                {
                                    flightID = reader.GetString(0),
                                    airlineId = reader.GetString(1),
                                    origin = reader.GetString(2),
                                    destination = reader.GetString(3),
                                    departureTime = reader.GetDateTime(4),
                                    arrivalTime = reader.GetDateTime(5),
                                    ticketPrice = reader.GetString(6)
                                };

                                return flight;
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine("There's a problem fetching flight details: " + ex.Message);
            }

            return null;
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

        /*private void SaveBookingDetails(CheckFlight flight)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    string qry = "INSERT INTO clientBookings (flightID, airlineId, origin, destination, departureTime, arrivalTime, availableSeats, ticketPrice) " +
                                 "VALUES (@flightID, @airlineId, @origin, @destination, @departureTime, @arrivalTime, @availableSeats, @ticketPrice)";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightID", flight.flightID);
                        cmd.Parameters.AddWithValue("@airlineId", flight.airlineId);
                        cmd.Parameters.AddWithValue("@origin", flight.origin);
                        cmd.Parameters.AddWithValue("@destination", flight.destination);
                        cmd.Parameters.AddWithValue("@departureTime", flight.departureTime);
                        cmd.Parameters.AddWithValue("@arrivalTime", flight.arrivalTime);
                        cmd.Parameters.AddWithValue("@availableSeats", flight.availableSeats);
                        cmd.Parameters.AddWithValue("@ticketPrice", flight.ticketPrice);

                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There's a problem saving booking details: " + ex.Message);
            }
        }*/
    }
}
