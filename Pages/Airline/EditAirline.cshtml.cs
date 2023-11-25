using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace FMS.Pages.Airline
{
    public class EditAirlineModel : PageModel
    {
        String conString = "Data Source=BUTURO\\SQLEXPRESS;Initial Catalog=FMSDB;Integrated Security=True";

        public Airline airline = new Airline();

        public List<Airline> airlineList = new List<Airline>();

        public string message = "";
        

        public void OnGet()
        {
            string airlineId = Request.Query["id"];
            try
            {

                using (SqlConnection con = new SqlConnection(conString))
                {
                    string qry = "SELECT id, name, contact FROM AIRLINE WHERE id=@id";
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@id", airlineId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                airline.id = reader.GetString(0);
                                airline.name = reader.GetString(1);
                                airline.contact = reader.GetString(2);
                            }
                            else
                            {
                                message = "No information available for this Airline:";
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
                airline.id = Request.Form["id"];
                airline.name = Request.Form["name"];
                airline.contact = Request.Form["contact"];

            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }

            // updating info
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "UPDATE AIRLINE SET name=@name, contact=@contact WHERE id=@id";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@id", airline.id);
                        cmd.Parameters.AddWithValue("@name", airline.name);
                        cmd.Parameters.AddWithValue("@contact", airline.contact);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["Message"] = "Airline Updated Successfully";
                            Response.Redirect("/Airline/CreateAirline");
                        }
                        else
                        {
                            message = "Airline Not Updated, Id shouldn't be changed";
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    message = "Airline Not Updated " + ex.Message;
                }
            }
        }
    }
}