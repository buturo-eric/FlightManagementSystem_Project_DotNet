using FMS.Pages.Shared.Passenger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Passenger
{
    public class EditModel : PageModel
    {
        String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";

        public Passenger passenger = new Passenger();

        public List<Passenger> passengerList = new List<Passenger>();

        public string message = "";


        public void OnGet()
        {
            string flightId = Request.Query["flightId"];
            try
            {

                using (SqlConnection con = new SqlConnection(conString))
                {
                    string qry = "SELECT flightId, firstName, lastName, airline, origin, destination, seatType, seatNumber, bookingDate FROM USERVIEW WHERE flightId=@flightId";
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightId", flightId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                passenger.flightId = reader.GetString(0);
                                passenger.firstName = reader.GetString(1);
                                passenger.lastName = reader.GetString(2);
                                passenger.airline = reader.GetString(3);
                                passenger.origin = reader.GetString(4);
                                passenger.destination = reader.GetString(5);
                                passenger.seatType = reader.GetString(6);
                                passenger.seatNumber = reader.GetString(7);
                                passenger.bookingDate = reader.GetString(8);
                            }
                            else
                            {
                                message = "No information available for this Passenger:";
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
                passenger.flightId = Request.Form["flightid"];
                passenger.firstName = Request.Form["firstName"];
                passenger.lastName = Request.Form["lastName"];
                passenger.airline = Request.Form["airline"];
                passenger.origin = Request.Form["origin"];
                passenger.destination = Request.Form["destination"];
                passenger.seatType = Request.Form["seatType"];
                passenger.seatNumber = Request.Form["seatNumber"];
                passenger.bookingDate = Request.Form["bookingDate"];

            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }

            // updating info
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "UPDATE USERVIEW SET firstName=@firstName, lastName=@lastName, airline=@airline, origin=@origin, destination=@destination, seatType=@seatType, seatNumber=@seatNumber, bookingDate=@bookingDate WHERE flightid=@flightid";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@flightId", passenger.flightId);
                        cmd.Parameters.AddWithValue("@firstName", passenger.firstName);
                        cmd.Parameters.AddWithValue("@lastName", passenger.lastName);
                        cmd.Parameters.AddWithValue("@airline", passenger.airline);
                        cmd.Parameters.AddWithValue("@origin", passenger.origin);
                        cmd.Parameters.AddWithValue("@destination", passenger.destination);
                        cmd.Parameters.AddWithValue("@seatType", passenger.seatType);
                        cmd.Parameters.AddWithValue("@seatNumber", passenger.seatNumber);
                        cmd.Parameters.AddWithValue("@bookingDate", passenger.bookingDate);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["Message"] = "Passenger Updated Successfully";
                            Response.Redirect("/Passenger/CreatePassenger");
                        }
                        else
                        {
                            message = "Passenger Not Updated, Id shouldn't be changed";
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    message = "Passenger Not Updated Successfully" + ex.Message;
                }
            }
        }
    }
}