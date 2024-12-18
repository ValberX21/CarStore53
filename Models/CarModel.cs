using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CarStore53.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Mark { get; set; } 
        public string Model { get; set; } 
        public int Year { get; set; } 
        public string VIN { get; set; } 
        public int LotId { get; set; } 
        public int SelectedMark { get; set; } 
        public List<SelectListItem> Marks { get; set; } 
        public List<CarModel> CarList { get; set; }
    }
}
