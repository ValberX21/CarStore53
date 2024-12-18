using CarStore53.Azure.Storage;
using CarStore53.Models;
using CarStore53.Security;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;

namespace CarStore53.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly BlobService _blobService;
        public string UploadedFileUrl { get; set; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, BlobService blobService)
        {
            _logger = logger;
            _blobService = blobService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
       
        public IActionResult Index(LotModel lotModel)
        {
            var userJson = HttpContext.Session.GetString("userDtJson");
            var userToken = HttpContext.Session.GetString("UserToken");

            if (userToken == null || string.IsNullOrEmpty(userToken))
            {
                return RedirectToAction("Index", "Login");
            }

            var userData = JsonSerializer.Deserialize<UserModel>(userJson);
            var tokenService = new TokenService(_configuration);
            var isValid = tokenService.ValidateToken(userToken);

            if (isValid)
            {
                lotModel.LotList = getLots();

                HttpContext.Session.SetString("UserRole", userData.UserLevel);
                return View("Index", lotModel);
            }

            return View();
        }

        public List<LotModel> getLots()
        {
            List<LotModel> LotModels = new List<LotModel>();
                        
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("sp_GetAllLots", con)
                {
                    CommandType = CommandType.StoredProcedure 
                };

                try
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        LotModel lots = new LotModel
                        {
                            LotNumber = reader.GetString(reader.GetOrdinal("LotNumber")),
                            ImportDate = reader.GetDateTime(reader.GetOrdinal("ImportDate")),
                            ShipName = reader.GetString(reader.GetOrdinal("ShipName")),
                            StatusName = reader.GetString(reader.GetOrdinal("StatusName"))
                        };

                        LotModels.Add(lots);
                    }
                    return LotModels;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public IActionResult GenerateReport()
        {
            BlobService blobService =  new BlobService();
            var filePath = Path.Combine("GeneratedReports", "Lots" + DateTime.Now.Date.ToString("dd/MM/yyyy").Replace("/","-") + ".pdf");

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var writer = new PdfWriter(filePath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    List<LotModel> listLots = getLots();

                    document.Add(new Paragraph("Report of imports").SetFontSize(18));
                    document.Add(new Paragraph("Total of imports: " + listLots.Count));

                    Table table = new Table(new float[] { 1, 2, 3 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell(new Cell().Add(new Paragraph("LotNumber").SimulateBold()));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("ImportDate").SimulateBold()));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("StatusName").SimulateBold()));

                    foreach(var lot in listLots)
                    {
                        table.AddCell(new Paragraph(lot.LotNumber));
                        table.AddCell(new Paragraph(lot.ImportDate.ToString()));           
                        table.AddCell(new Paragraph(lot.StatusName));
                    }

                    document.Add(table);

                    document.Close();
                }
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath+"1");
            var contentType = "application/pdf";
            IFormFile formFile = blobService.ConvertToIFormFile(fileBytes, fileName, contentType);

            if (formFile != null)
            {
                UploadedFileUrl = _blobService.UploadFileAsync(formFile);
            }

            return File(fileBytes, "application/pdf", "Lots" + DateTime.Now.Date.ToString("dd/MM/yyyy").Replace("/", "-") + ".pdf");
        }



        public IActionResult CallRegisterCar()
        {
            HttpContext.Session.Remove("ListCarModel");
            return RedirectToAction("Car", "Register");
        }

        public IActionResult CallRegisterShip()
        {
            HttpContext.Session.Remove("ListCarModel");
            return RedirectToAction("Ship", "Register");
        }

        public IActionResult CallRegisterLot()
        {
            HttpContext.Session.Remove("ListCarModel");
            return RedirectToAction("Lot", "Register");
        }

        public IActionResult BackHome()
        {
            HttpContext.Session.Remove("ListCarModel");
            return RedirectToAction("Index", "Home");
        }        


    }
}
