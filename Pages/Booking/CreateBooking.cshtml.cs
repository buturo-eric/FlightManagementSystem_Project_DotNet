using FMS.Pages.Flight;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Transactions;

namespace FMS.Pages.Booking
{
    public class CreateBookingModel : PageModel
    {
        String conString = "Data Source=SQL5073.site4now.net;Initial Catalog=db_aa2c17_fmsdb;User Id=db_aa2c17_fmsdb_admin;Password=Hosting123!";

        CheckBooking checkBooking = new CheckBooking();

        public List<Booking> bookingList = new List<Booking>();

        public string message = "";
        public void OnGet(string? flightID, string? airlineId, string? origin, string? destination,
            DateTime? departureTime, DateTime? arrivalTime, 
            string? ticketPrice, string? availableSeats)
        {
            if (TempData.Count > 0)
                message = TempData["BookingMessage"] as string;

            /*loadBookingList();*/
            ViewData["flightID"] = flightID;
            ViewData["airlineId"] = airlineId;
            ViewData["origin"] = origin;
            ViewData["destination"] = destination;
            ViewData["departureTime"] = departureTime?.ToString("yyyy-MM-ddTHH:mm");
            ViewData["arrivalTime"] = arrivalTime?.ToString("yyyy-MM-ddTHH:mm");
            ViewData["ticketPrice"] = ticketPrice;
            ViewData["availableSeats"] = availableSeats;
        }
        /*public void loadBookingList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {

                    string qry = "SELECT * FROM BOOKING";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Booking booking = new Booking();

                                booking.firstName = reader.GetString(1);
                                booking.lastName = reader.GetString(2);
                                booking.number = reader.GetString(3);
                                booking.payment = reader.GetString(3);

                                bookingList.Add(booking);
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
        }*/

        public IActionResult OnPost(string? flightID, string? airlineId, string? origin, string? destination, DateTime? departureTime, DateTime? arrivalTime, string? ticketPrice, string? availableSeats)
        {
            try
            {
                checkBooking.flightID = flightID;
                checkBooking.airlineId = GetAirlineId(airlineId);

                checkBooking.origin = origin;
                checkBooking.destination = destination;
                checkBooking.departureTime = departureTime;
                checkBooking.arrivalTime = arrivalTime;
                checkBooking.userId = HttpContext.Session.GetInt32("UserId");
                int passengers = int.Parse(Request.Form["number"]);
                int payment = int.Parse(ticketPrice) * passengers;
                checkBooking.ticketPrice = payment.ToString();
            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
                return Page();
            }

            // saving to DB
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO clientBookings (flightId, airlineId, origin, destination, departureTime, arrivalTime, ticketPrice, userId)" +
                              " VALUES (@flightId, @airlineId, @origin, @destination, @departureTime, @arrivalTime, @ticketPrice, @od)";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightId", checkBooking.flightID);
                        cmd.Parameters.AddWithValue("@airlineId", checkBooking.airlineId);
                        cmd.Parameters.AddWithValue("@origin", checkBooking.origin);
                        cmd.Parameters.AddWithValue("@destination", checkBooking.destination);
                        cmd.Parameters.AddWithValue("@departureTime", checkBooking.departureTime);
                        cmd.Parameters.AddWithValue("@arrivalTime", checkBooking.arrivalTime);
                        cmd.Parameters.AddWithValue("@ticketPrice", checkBooking.ticketPrice);
                        cmd.Parameters.AddWithValue("@od", checkBooking.userId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Booking Added Successfully";
                            checkBooking = new CheckBooking(); // empty the inputs
                            return RedirectToPage("/Booking/CheckBooking_user");
                        }
                        else
                        {
                            message = "Booking Not Added Successfully";
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY "))
                    {
                        message = "There's a problem: Booking already exists";
                    }
                    else
                    {
                        message = "There's a problem: " + ex.Message;
                    }
                }
            }
            return Page();
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


    }
}