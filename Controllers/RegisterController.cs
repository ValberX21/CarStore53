using CarStore53.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Reflection;

namespace CarStore53.Controllers
{
  
    public class RegisterController : Controller
    {
        private const string SessionKey = "CarList";

        private readonly IConfiguration _configuration;
        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(CarModel carModel)
        {
            var userJson = HttpContext.Session.GetString("userDtJson");
            var userToken = HttpContext.Session.GetString("UserToken");

            if (userToken == null || string.IsNullOrEmpty(userToken))
            {
                return RedirectToAction("Index", "Login");
            }

            List<CarModel> Cars = GetCars();

            List<CarMarks> cmk = LoadMarks();

            foreach (var cm in Cars)
            {
                var mark = cmk.FirstOrDefault(m => m.Id == int.Parse(cm.Mark));

                cm.Mark = mark.Mark;
            }

            var marksList = cmk.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Mark
            }).ToList();

            carModel.Marks = marksList;
            carModel.CarList = Cars;
            return View(carModel);
        }

        public IActionResult Car()
        {
            var userJson = HttpContext.Session.GetString("userDtJson");
            var userToken = HttpContext.Session.GetString("UserToken");

            if (userToken == null || string.IsNullOrEmpty(userToken))
            {
                return RedirectToAction("Index", "Login");
            }

            CarModel carModel = new CarModel();
            List<CarModel> Cars = GetCars();

            List<CarMarks> cmk = LoadMarks();

            foreach (var cm in Cars)
            {
                var mark = cmk.FirstOrDefault(m => m.Id == int.Parse(cm.Mark));

                cm.Mark = mark.Mark;
            }

            var marksList = cmk.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Mark
            }).ToList();

            carModel.Marks = marksList;
            carModel.CarList = Cars;
            return View(carModel);
        }

        public IActionResult Ship()
        {
            ShipModel shipModel = new ShipModel();

            List<ShipModel> Ships = GetShips();

            shipModel.ShipList = Ships;

            return View("Ship", shipModel);
        }

        public IActionResult AddCar(CarModel carModel)
        {
            if (ModelState.IsValid)
            {
                int id = carModel.Id;
                int mark = carModel.SelectedMark;
                string model = carModel.Model;
                int year = carModel.Year;
                string vin = carModel.VIN;

                using (SqlConnection con =  new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if(id == 0) 
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Car_Add", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.Add("@MARK", SqlDbType.VarChar).Value = mark.ToString();
                            cmd.Parameters.Add("@MODEL", SqlDbType.VarChar).Value = model.ToString();
                            cmd.Parameters.Add("@YEAR", SqlDbType.VarChar).Value = year.ToString();
                            cmd.Parameters.Add("@VIN", SqlDbType.VarChar, 17).Value = vin.ToString();

                            con.Open();
                            cmd.ExecuteNonQuery();
                            TempData["success"] = "Car add successfully";

                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Car_Update", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                            cmd.Parameters.Add("@MARK", SqlDbType.VarChar).Value = mark.ToString();
                            cmd.Parameters.Add("@MODEL", SqlDbType.VarChar).Value = model.ToString();
                            cmd.Parameters.Add("@YEAR", SqlDbType.VarChar).Value = year.ToString();
                            cmd.Parameters.Add("@VIN", SqlDbType.VarChar, 17).Value = vin.ToString();

                            con.Open();
                            cmd.ExecuteNonQuery();
                            TempData["success"] = "Car update successfully";

                        }
                    }

                }
            }
            return RedirectToAction("Index");
        }

        public List<CarModel> GetCars()
        {
            List<CarModel> carModels = new List<CarModel>();

            string selectMarks = "SELECT * FROM CAR";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand comand = new SqlCommand(selectMarks, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = comand.ExecuteReader();

                    while (reader.Read())
                    {
                        CarModel cars = new CarModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Mark = reader.GetInt32(reader.GetOrdinal("Mark")).ToString(),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            Year = reader.GetInt32(reader.GetOrdinal("Year")),
                            VIN = reader.GetString(reader.GetOrdinal("VIN"))
                        };

                        carModels.Add(cars);

                    }
                    return carModels;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public List<ShipModel> GetShips()
        {
            List<ShipModel> listShips = new List<ShipModel>();

            string queryListShips = "SELECT * FROM Ship";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand comand = new SqlCommand(queryListShips, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = comand.ExecuteReader();

                    while (reader.Read())
                    {
                        ShipModel ships = new ShipModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("ShipName")),
                            RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber")),
                            CaptainName = reader.GetString(reader.GetOrdinal("CaptainName"))                            
                        };

                        listShips.Add(ships);

                    }
                    return listShips;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public List<CarMarks> LoadMarks()
        {
            List<CarMarks> marks = new List<CarMarks>();

            string selectMarks = "SELECT * FROM CarMark";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand comand = new SqlCommand(selectMarks, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = comand.ExecuteReader();

                    while (reader.Read())
                    {
                        CarMarks mark = new CarMarks
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")), 
                            Mark = reader.GetString(reader.GetOrdinal("Mark")), 
                            DataContract = reader.GetDateTime(reader.GetOrdinal("DataContract"))
                        };

                        marks.Add(mark);

                    }
                    return marks;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }        

        }

        public IActionResult EditShip(int Id)
        {
            ShipModel shipss = new ShipModel();

            string querySelectCar = "SELECT * FROM SHIP WHERE ID = @ID";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(querySelectCar, con))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = Id;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        shipss = new ShipModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("ShipName")),
                            RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber")),
                            CaptainName = reader.GetString(reader.GetOrdinal("CaptainName"))
                        };
                    }
                }
            }

            List<ShipModel> Ships = GetShips();

            shipss.ShipList = Ships;

            return View("Ship", shipss);
        }

        public IActionResult Edit(int Id)
        {
            List<CarModel> carModels = new List<CarModel>();

            CarModel carss = new CarModel();

            string querySelectCar = "SELECT * FROM CAR WHERE ID = @ID";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(querySelectCar, con))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = Id;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        carss = new CarModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Mark = reader.GetInt32(reader.GetOrdinal("Mark")).ToString(),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            Year = reader.GetInt32(reader.GetOrdinal("Year")),
                            VIN = reader.GetString(reader.GetOrdinal("VIN"))
                        };
                    }
                }
            }       

            if(carss != null)
            {
                List<CarModel> Cars = GetCars();

                List<CarMarks> cmk = LoadMarks();

                foreach (var cm in Cars)
                {
                    var mark = cmk.FirstOrDefault(m => m.Id == int.Parse(cm.Mark));

                    cm.Mark = mark.Mark;
                }

                var marksList = cmk.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Mark
                }).ToList();
                
                carss.Marks = marksList;
                carss.CarList = Cars;
                carss.SelectedMark = int.Parse(carss.Mark.ToString());
                return View("Index",carss);
            }
            else
            {
                return View("Index");
            }
              
        }

        public IActionResult Delete(int Id)
        {
            CarModel carss = new CarModel();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Car_Delete", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = Id;

                    con.Open();

                    cmd.ExecuteNonQuery();

                    TempData["success"] = "Car deleted";

                }
            }

            List<CarModel> Cars = GetCars();

            List<CarMarks> cmk = LoadMarks();

            foreach (var cm in Cars)
            {
                var mark = cmk.FirstOrDefault(m => m.Id == int.Parse(cm.Mark));

                cm.Mark = mark.Mark;
            }

            var marksList = cmk.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Mark
            }).ToList();

            carss.Marks = marksList;
            carss.CarList = Cars;
            return RedirectToAction("Index",carss);
        }

        public IActionResult DeleteShip(int Id)
        {
            ShipModel shipss = new ShipModel();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ship_Delete", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = Id;

                    con.Open();

                    cmd.ExecuteNonQuery();

                    TempData["success"] = "Ship deleted";

                }
            }

            List<ShipModel> Ships = GetShips();

            shipss.ShipList = Ships;

            return View("Ship", shipss);
        }

        public IActionResult AddShip(ShipModel shipModel)
        {
            if (ModelState.IsValid)
            {
                int id = shipModel.Id;
                string shipName = shipModel.Name;
                string registrationName = shipModel.RegistrationNumber;
                string captainName = shipModel.CaptainName;                

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if (id == 0)
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Ship_Add", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.Add("@SHIPNAME", SqlDbType.VarChar).Value = shipName;
                            cmd.Parameters.Add("@REGISTRATIONNUMBER", SqlDbType.VarChar).Value = registrationName;
                            cmd.Parameters.Add("@CAPTAIONNAME", SqlDbType.VarChar).Value = captainName;

                            con.Open();
                            cmd.ExecuteNonQuery();
                            TempData["success"] = "Ship add successfully";

                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Ship_Update", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                            cmd.Parameters.Add("@SHIPNAME", SqlDbType.VarChar).Value = shipName;
                            cmd.Parameters.Add("@REGISTRATIONNUMBER", SqlDbType.VarChar).Value = registrationName;
                            cmd.Parameters.Add("@CAPTAIONNAME", SqlDbType.VarChar).Value = captainName;

                            con.Open();
                            cmd.ExecuteNonQuery();
                            TempData["success"] = "Ship update successfully";

                        }
                    }

                }
            }
            return RedirectToAction("Ship");
        }
    
        public IActionResult Lot(LotModel lotModel)
        {
            var userToken = HttpContext.Session.GetString("UserToken");

            if (userToken == null || string.IsNullOrEmpty(userToken))
            {
                return RedirectToAction("Index", "Login");
            }
            List<CarModel> carModeList = new List<CarModel>();
            lotModel.LotNumber = Guid.NewGuid().ToString("N").Substring(0, 18);
            lotModel.ImportDate = DateTime.Now;
            lotModel.CarsLot = carModeList;

            ShipModel shipModel = new ShipModel();

            List<ShipModel> ships = GetShips();

            var shipList = ships.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            lotModel.Ships = shipList;

            return View("Lot", lotModel);
        }

        public IActionResult GenerateLot(LotModel lotModel)
        {
            if (ModelState.IsValid)
            {
                List<CarModel> carsLot;

                string carsLotJson = HttpContext.Session.GetString("ListCarModel");

                carsLot = JsonSerializer.Deserialize<List<CarModel>>(carsLotJson) ?? new List<CarModel>();

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (CarModel car in carsLot)
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "INSERT INTO Lot (LotNumber, ImportDate, ShipId, Car) VALUES (@LotNumber, @ImportDate, @ShipId, @Car)", con, transaction))
                                {
                                    cmd.Parameters.Add("@LOTNUMBER", SqlDbType.NVarChar).Value = lotModel.LotNumber;
                                    cmd.Parameters.Add("@IMPORTDATE", SqlDbType.DateTime).Value = lotModel.ImportDate;
                                    cmd.Parameters.Add("@SHIPID", SqlDbType.Int).Value = lotModel.ShipId;
                                    cmd.Parameters.Add("@STATUSID", SqlDbType.Int).Value = 1;
                                    cmd.Parameters.Add("@CAR", SqlDbType.Int).Value = car.Id;

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            TempData["success"] = "Lot created successfully";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            TempData["error"] = "Erro to create lot" + ex.Message;
                        }
                    }
                }
                lotModel.LotNumber = Guid.NewGuid().ToString("N").Substring(0, 18);
                lotModel.ImportDate = DateTime.Now;
                lotModel.CarsLot = new List<CarModel>();
                return View("Lot", lotModel);
            }

            lotModel.LotNumber = Guid.NewGuid().ToString("N").Substring(0, 18);
            lotModel.ImportDate = DateTime.Now;
            lotModel.CarsLot = new List<CarModel>();
            return View("Lot", lotModel);
        }

        public IActionResult AddCarInLot(LotModel lotModel)
        {
            List<CarModel> CM = GetCars();

            CarModel carNegotiated = CM.Find(c => c.Model == lotModel.SelecteCar);

            if (carNegotiated != null)
            {
                string carsLotJson = HttpContext.Session.GetString("ListCarModel");
                List<CarModel> carsLot;

                if (string.IsNullOrEmpty(carsLotJson))
                {
                    carsLot = new List<CarModel> { carNegotiated };
                }
                else
                {
                    carsLot = JsonSerializer.Deserialize<List<CarModel>>(carsLotJson) ?? new List<CarModel>();

                    if (!carsLot.Any(car => car.Model == carNegotiated.Model))
                    {
                        carsLot.Add(carNegotiated);
                    }
                }

                lotModel.CarsLot = carsLot;
                HttpContext.Session.SetString("ListCarModel", JsonSerializer.Serialize(carsLot));
            }
            else
            {
                TempData["error"] = "This car is not negotiated";
                lotModel.CarsLot = new List<CarModel>();
            }

            ShipModel shipModel = new ShipModel();

            List<ShipModel> ships = GetShips();

            var shipList = ships.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            lotModel.Ships = shipList;

            return View("Lot", lotModel);
        }

        public IActionResult DeleteCarLot(int Id, LotModel lotModel)
        {
            List<CarModel> carsLot;

            string carsLotJson = HttpContext.Session.GetString("ListCarModel");

            carsLot = JsonSerializer.Deserialize<List<CarModel>>(carsLotJson) ?? new List<CarModel>();

            carsLot.RemoveAll(car => car.Id == Id);

            string updatedCarsLotJson = JsonSerializer.Serialize(carsLot);

            HttpContext.Session.SetString("ListCarModel", updatedCarsLotJson);

            lotModel.CarsLot = carsLot;

            return View("Lot",lotModel);
        }
    }
}
