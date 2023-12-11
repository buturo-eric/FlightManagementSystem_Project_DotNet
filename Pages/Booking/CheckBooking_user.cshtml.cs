using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;

namespace FMS.Pages.Booking
{
    public class CheckBookingUserModel : PageModel
    {
        string conString = "Data Source=SQL5073.site4now.net;Initial Catalog=db_aa2c17_fmsdb;User Id=db_aa2c17_fmsdb_admin;Password=Hosting123!";
        public List<CheckBooking> BookingList { get; set; } = new List<CheckBooking>();

        CheckBooking checkBooking = new CheckBooking();
        public void OnGet()
        {
            // Fetch booking data from the database
            BookingList = GetBookingList();
        }

        private List<CheckBooking> GetBookingList()
        {
            List<CheckBooking> bookings = new List<CheckBooking>();
            checkBooking.userId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to fetch booking data from the clientBookings table
                    string qry = "SELECT * FROM clientBookings WHERE userId=@od";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@od", checkBooking.userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there are rows returned
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string flightID = reader.GetString(0);
                                    string airlineId = reader.GetString(1);
                                    string origin = reader.GetString(2);
                                    string destination = reader.GetString(3);
                                    DateTime departureTime = reader.GetDateTime(4);
                                    DateTime arrivalTime = reader.GetDateTime(5);
                                    string ticketPrice = reader.GetString(6);

                                    // Replace the airline ID with the airline name
                                    string airlineName = GetAirlineName(airlineId);

                                    // Get the airlineId based on the airlineName
                                    int? airlineIdValue = GetAirlineId(airlineName);

                                    // Create a CheckBooking object and add it to the list
                                    CheckBooking booking = new CheckBooking(flightID, airlineIdValue, airlineName, origin, destination, departureTime, arrivalTime, ticketPrice, checkBooking.userId);
                                    bookings.Add(booking);
                                }
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine("There's a problem fetching booking data: " + ex.Message);
            }

            return bookings;
        }

        public int GetAirlineId(string airlineName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to get the ID of the airline based on the airlineName
                    string query = "SELECT id FROM AIRLINE WHERE name = @airlineName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@airlineName", airlineName); // Updated parameter name

                        // Execute the query
                        object result = cmd.ExecuteScalar();

                        // Check if the result is not null
                        if (result != null && int.TryParse(result.ToString(), out int airlineId))
                        {
                            return airlineId;
                        }
                        else
                        {
                            // Handle the case where the airlineName doesn't exist
                            return 0; // Or throw an exception or handle differently based on your requirements
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or throw as needed)
                Console.WriteLine("Error fetching airline ID: " + ex.Message);
                return 0; // Or throw an exception or handle differently based on your requirements
            }
        }

        /*private bool CancelBooking(string flightID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to delete the booking based on flightID
                    string qry = "DELETE FROM clientBookings WHERE flightID = @flightID";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightID", flightID);

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Check if the booking was successfully canceled
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine("There's a problem canceling the booking: " + ex.Message);
                return false;
            }
        }*/

        public string GetAirlineName(string airlineId)
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
