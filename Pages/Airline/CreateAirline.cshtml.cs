using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace FMS.Pages.Airline
{
    public class CreateAirlineModel : PageModel
    {
        String conString = "Data Source=SQL5073.site4now.net;Initial Catalog=db_aa2c17_fmsdb;User Id=db_aa2c17_fmsdb_admin;Password=Hosting123!";

        public Airline airline = new Airline();

        public List<Airline> airlineList = new List<Airline>();

        public string message = "";
        public void OnGet()
        {
            if (TempData.Count > 0)
                message = TempData["Message"] as string;

            loadAirlineList();
        }

        public void loadAirlineList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {

                    string qry = "SELECT * FROM AIRLINE";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Airline airline = new Airline();

                                airline.id = reader.GetString(0);
                                airline.name = reader.GetString(1);
                                airline.contact = reader.GetString(2);
                                
                                airlineList.Add(airline);
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
                airline.id = Request.Form["id"];
                airline.name = Request.Form["name"];
                airline.contact = Request.Form["contact"];
                
            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }

            // saving to DB
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO AIRLINE VALUES (@id, @name, @contact)";

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
                            message = "Airline Added Successfully";
                            airline = new Airline(); // empty the inputs
                            loadAirlineList();
                        }
                        else
                        {
                            message = "Airline Not Added Successfully";
                            loadAirlineList();
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY "))
                    {
                        message = "There's a problem: Airline already exists";
                        loadAirlineList();
                    }
                    else
                    {
                        message = "There's a problem: " + ex.Message;
                        loadAirlineList();
                    }

                }
            }
        }
    }
}
