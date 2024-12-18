using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace CarStore53.Models
{
    public class LotModel
    {
        public int Id { get; set; } 
        public string LotNumber { get; set; } 
        public DateTime ImportDate { get; set; } 
        public int ShipId { get; set; } 
        public string ShipName { get; set; }
        public string Car { get; set; }
        public string SelecteCar { get; set; }
        public List<SelectListItem> Ships { get; set; }
        public List<CarModel> CarsLot { get; set; } 
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public List<LotModel> LotList { get; set; }
    }
}
