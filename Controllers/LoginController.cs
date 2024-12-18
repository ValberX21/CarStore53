using CarStore53.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CarStore53.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var userToken = HttpContext.Session.GetString("UserToken");

            if (userToken != null || !string.IsNullOrEmpty(userToken))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }


        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                string userName = login.Username;
                string PassWord = login.Password;

                byte[] userPassword = Encoding.UTF8.GetBytes(PassWord);

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    try
                    {
                        connection.Open();

                        string query = "SELECT * FROM [dbo].[Users] WHERE FullName = @FullName and Password = @Password";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FullName", userName);
                            command.Parameters.AddWithValue("@Password", userPassword);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    UserModel userDt = new UserModel()
                                    {
                                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        CPF = reader.GetInt32(reader.GetOrdinal("CPF")),
                                        Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : (byte[])reader["Password"],
                                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                        DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                        UserLevel = reader.GetString(reader.GetOrdinal("UserLevel")),
                                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                                    };

                                    string token = GenerateJwtToken(userDt);

                                    TempData["success"] = "Login successfully";

                                    var userDtJson = JsonSerializer.Serialize(userDt);

                                    HttpContext.Session.SetString("userDtJson", userDtJson);
                                    HttpContext.Session.SetString("UserToken", token);

                                    return RedirectToAction("Index", "Home");

                                }
                                else 
                                {
                                    TempData["usernotfound"] = "User not found";
                                }
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        TempData["error"] = ex.Message;
                    }                   
                }
            }
            return View("Index");
        }

        private string GenerateJwtToken(UserModel user)
        {
            IdentityUser usu = new IdentityUser();

            usu.UserName = user.FullName;
            usu.Id = user.UserId.ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usu.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usu.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
