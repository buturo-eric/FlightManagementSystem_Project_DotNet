using FMS.Pages.Passenger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace FMS.Pages.Users
{
    public class SignupModel : PageModel
    {
        String conString = "Data Source=SQL5073.site4now.net;Initial Catalog=db_aa2c17_fmsdb;User Id=db_aa2c17_fmsdb_admin;Password=Hosting123!";

        public User user1 = new User();
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            string name = Request.Form["name"];
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string confirmPassword = Request.Form["confirmPassword"];

            if (!ModelState.IsValid)
            {
                Message = "Invalid input. Please check the form for errors.";
                return;
            }

            if (password != confirmPassword)
            {
                Message = "Password and Confirm Password don't match.";
                return;
            }

            // Hash the password
            string encryptPassword = EncryptPassword(password);

            // Create a User instance
            User newUser = new User
            {
                name = name,
                email = email,
                password = encryptPassword,
                role = "Staff"
            };

            // Save user to the database
            SignUp(newUser);

            // Handle additional logic or redirect
            Message = "User Registered Successfully";
            user1 = new User(); // empty the inputs

        }

        private string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string base64Hash = Convert.ToBase64String(hashBytes);
                return base64Hash;
            }

        }

        private void SignUp(User user)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO Users (name, email, password, role) VALUES (@name, @email, @password, @role)";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@name", user.name);
                        cmd.Parameters.AddWithValue("@email", user.email);
                        cmd.Parameters.AddWithValue("@password", user.password);
                        cmd.Parameters.AddWithValue("@role", user.role);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected <= 0)
                        {
                            Message = "User Not Registered Successfully";
                        }
                    }
                    con.Close();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint violation error number
                    {
                        Message = "Email is already registered. Please use a different email.";
                    }
                    else
                    {
                        Message = "There's a problem: " + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    Message = "There's a problem: " + ex.Message;
                }
            }
        }

    }
}
