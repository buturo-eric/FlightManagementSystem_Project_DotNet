using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Shared.Passenger
{
    public class CreatePassengerModel : PageModel
    {
        String connectionString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";
        public PassengerInfo passengerInfo = new PassengerInfo();
        public string errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {

        }

        public void OnPost()
        {
            passengerInfo.flightId = Request.Form["flightId"];
            passengerInfo.firstName = Request.Form["firstName"];
            passengerInfo.lastName = Request.Form["lastName"];
            passengerInfo.airline = Request.Form["airline"];
            passengerInfo.origin = Request.Form["origin"];
            passengerInfo.destination = Request.Form["destination"];
            passengerInfo.seatType = Request.Form["seatType"];
            passengerInfo.seatNumber = Request.Form["seatNumber"];
            passengerInfo.paymentStatus = Request.Form["paymentStatus"];
            passengerInfo.bookingDate = Request.Form["bookingDate"];

            if (passengerInfo.flightId.Length == 0 || passengerInfo.firstName.Length == 0 || passengerInfo.lastName.Length == 0 || passengerInfo.airline.Length == 0 || passengerInfo.origin.Length == 0 || passengerInfo.destination.Length == 0 || passengerInfo.seatType.Length == 0 || passengerInfo.seatNumber.Length == 0 || passengerInfo.paymentStatus.Length == 0 || passengerInfo.bookingDate.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }

            //Save Data in DB
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    String sqlQuery = "INSERT INTO USERVIEW VALUES (@flightId, @firstName, @lastName, @airline, @origin, @destination, @seatType, @seatNumber, @paymentStatus, @bookingDate)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@flightId", passengerInfo.flightId);
                        cmd.Parameters.AddWithValue("@firstName", passengerInfo.firstName);
                        cmd.Parameters.AddWithValue("@lastName", passengerInfo.lastName);
                        cmd.Parameters.AddWithValue("@airline", passengerInfo.airline);
                        cmd.Parameters.AddWithValue("@origin", passengerInfo.origin);
                        cmd.Parameters.AddWithValue("@destination", passengerInfo.destination);
                        cmd.Parameters.AddWithValue("@seatType", passengerInfo.seatType);
                        cmd.Parameters.AddWithValue("@seatNumber", passengerInfo.seatNumber);
                        cmd.Parameters.AddWithValue("@paymentStatus", passengerInfo.paymentStatus);
                        cmd.Parameters.AddWithValue("@bookingDate", passengerInfo.bookingDate);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            passengerInfo.flightId = ""; passengerInfo.firstName = ""; passengerInfo.lastName = ""; passengerInfo.airline = ""; passengerInfo.origin = ""; passengerInfo.destination = ""; passengerInfo.seatType = ""; passengerInfo.seatNumber = ""; passengerInfo.paymentStatus = ""; passengerInfo.bookingDate = "";

            successMessage = "Added New Passenger Successfully";

            Response.Redirect("/Passenger/Index");

        }
    }
}