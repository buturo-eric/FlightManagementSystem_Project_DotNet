using FMS.Pages.Airline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Passenger
{
    public class CreatePassengerModel : PageModel
    {
        String conString = "Data Source=SQL5073.site4now.net;Initial Catalog=db_aa2c17_fmsdb;User Id=db_aa2c17_fmsdb_admin;Password=Hosting123!";

        public Passenger passenger = new Passenger();

        public List<Passenger> passengerList = new List<Passenger>();

        public string message = "";
        public void OnGet()
        {
            if (TempData.Count > 0)
                message = TempData["Message"] as string;

            loadPassengerList();
        }

        public void loadPassengerList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {

                    string qry = "SELECT * FROM USERVIEW";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Passenger passenger = new Passenger();

                                passenger.flightId = reader.GetString(0);
                                passenger.firstName = reader.GetString(1);
                                passenger.lastName = reader.GetString(2);
                                passenger.airline = reader.GetString(3);
                                passenger.origin = reader.GetString(4);
                                passenger.destination = reader.GetString(5);
                                passenger.seatType = reader.GetString(6);
                                passenger.seatNumber = reader.GetString(7);
                                passenger.bookingDate = reader.GetString(8);


                                passengerList.Add(passenger);
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

            // saving to DB
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO USERVIEW VALUES (@flightId, @firstName, @lastName, @airline, @origin, @destination, @seatType, @seatNumber, @bookingDate)";

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
                            message = "Passenger Added Successfully";
                            passenger = new Passenger(); // empty the inputs
                            loadPassengerList();
                        }
                        else
                        {
                            message = "Passenger Not Added Successfully";
                            loadPassengerList();
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY "))
                    {
                        message = "There's a problem: Passenger already exists";
                        loadPassengerList();
                    }
                    else
                    {
                        message = "There's a problem: " + ex.Message;
                        loadPassengerList();
                    }

                }
            }
        }
    }
}
