using System.Collections.Generic;
using System;

namespace CarStore53.Models
{
    public class ShipModel
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string RegistrationNumber { get; set; } // Ship's registration number
        public string CaptainName { get; set; } 
        public DateTime ArrivalDate { get; set; } 
        public List<LotModel> Lots { get; set; } = new(); // Lots on the ship
        public List<ShipModel> ShipList { get; set;}
    }
}
