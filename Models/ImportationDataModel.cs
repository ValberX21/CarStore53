using System;

namespace CarStore53.Models
{
    public class ImportationDataModel
    {
        public int Id { get; set; } 
        public int LotId { get; set; } 
        public int ShipId { get; set; } 
        public string ContainerNumber { get; set; } 
        public DateTime InspectionDate { get; set; } 
        public string InspectorName { get; set; }
        public string Notes { get; set; } 
    }
}
