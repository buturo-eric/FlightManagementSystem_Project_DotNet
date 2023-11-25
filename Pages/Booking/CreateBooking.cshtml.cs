using FMS.Pages.Flight;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Booking
{
    public class CreateBookingModel : PageModel
    {
        String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";

        public Booking booking = new Booking();

        public List<Booking> bookingList = new List<Booking>();

        public string message = "";
        public void OnGet()
        {
            if (TempData.Count > 0)
                message = TempData["Message"] as string;

            loadBookingList();
        }
        public void loadBookingList()
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
        }

        public void OnPost()
        {
            try
            {
                booking.firstName = Request.Form["firstName"];
                booking.lastName = Request.Form["lastName"];
                booking.number = Request.Form["number"]; 
                booking.payment = Request.Form["payment"];

            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }

            // saving to DB
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO BOOKING VALUES (@firstName, @lastName, @number ,@payment)";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@firstName", booking.firstName);
                        cmd.Parameters.AddWithValue("@lastName", booking.lastName);
                        cmd.Parameters.AddWithValue("@number", booking.number);
                        cmd.Parameters.AddWithValue("@payment", booking.payment);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Booking Added Successfully";
                            booking = new Booking(); // empty the inputs
                            loadBookingList();
                        }
                        else
                        {
                            message = "Booking Not Added Successfully";
                            loadBookingList();
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY "))
                    {
                        message = "There's a problem: Booking already exists";
                        loadBookingList();
                    }
                    else
                    {
                        message = "There's a problem: " + ex.Message;
                        loadBookingList();
                    }

                }
            }
        }
    }
}