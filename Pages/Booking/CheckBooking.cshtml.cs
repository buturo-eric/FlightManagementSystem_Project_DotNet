using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FMS.Pages.Booking
{
    public class CheckBookingModel : PageModel
    {
        string conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";
        public List<CheckBookingAdm> BookingList { get; set; } = new List<CheckBookingAdm>();

        public void OnGet()
        {
            // Fetch booking data from the database
            BookingList = GetBookingList();
        }

        private List<CheckBookingAdm> GetBookingList()
        {
            List<CheckBookingAdm> bookings = new List<CheckBookingAdm>();

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // Query to fetch booking data from the clientBookings table
                    string qry = "SELECT * FROM clientBookings";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string flightID = reader.GetString(0);
                                string airlineId = reader.GetString(1);
                                string origin = reader.GetString(2);
                                string destination = reader.GetString(3);
                                DateTime departureTime = reader.GetDateTime(4);
                                DateTime arrivalTime = reader.GetDateTime(5);
                                /*string availableSeats = reader.GetString(6);*/
                                string ticketPrice = reader.GetString(6);

                                // Replace the airline ID with the airline name
                                string airlineName = GetAirlineName(airlineId);

                                // Create a CheckBookingAdm object and add it to the list
                                CheckBookingAdm booking = new CheckBookingAdm(flightID, airlineName, origin, destination, departureTime, arrivalTime, ticketPrice);
                                bookings.Add(booking);
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
                Console.WriteLine("Error fetching booking data: " + ex.ToString());
                throw; // Rethrow the exception to ensure it's propagated up the call stack

            }

            return bookings;
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
