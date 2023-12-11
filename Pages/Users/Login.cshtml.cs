using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace FMS.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly string conString = "Data Source=SQL5073.site4now.net;Initial Catalog=db_aa2c17_fmsdb;User Id=db_aa2c17_fmsdb_admin;Password=Hosting123!";

        User users = new User();

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Invalid input. Please check the form for errors.";
                return Page();
            }

            // Hash the password
            string encryptPassword = EncryptPassword(Password);


            // Validate user credentials
            User user = ValidateUser(Email, encryptPassword);

            if (user != null)
            {
                int userId = GetUserIdFromDatabase(Email, encryptPassword);
                Console.WriteLine($"User role: {user.role}");
                // Redirect based on user role
                if (user.role == "Admin")
                {
                    HttpContext.Session.SetInt32("UserId", userId);
                    Message = "Admin Logged In";
                    return RedirectToPage("/Users/Admin");
                }
                else if (user.role == "Staff")
                {
                    HttpContext.Session.SetInt32("UserId", userId);
                    Message = "Staff Logged In";
                    return RedirectToPage("/Users/Users");
                    
                }
                // Add more roles and redirects as needed
                else
                {
                    Message = "Invalid user role";
                    return Page();
                }
            }
            else
            {
                Message = "Invalid email or password";
                return Page();
            }
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

        private int GetUserIdFromDatabase(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();

                string query = "SELECT id FROM USERS WHERE email = @email AND password = @password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int userId = (int)result;
                        Console.WriteLine("User ID retrieved from database: " + userId);
                        return userId;
                    }
                }
            }

            return 0;
        }

        private User ValidateUser(string email, string password)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "SELECT * FROM Users WHERE email=@email AND password=@password";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                return new User
                                {

                                    email = reader.GetString(reader.GetOrdinal("Email")),
                                    role = reader.GetString(reader.GetOrdinal("Role"))
                                };
                                
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message = "There's a problem: " + ex.Message;
                    Console.WriteLine($"Exception: {ex}");
                    return null;
                }
            }
        }
    }
}