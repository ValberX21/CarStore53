using System;

namespace CarStore53.Models
{
    public class UserModel
    {
        public int UserId { get; set; }           
        public string FullName { get; set; }       
        public string Email { get; set; }        
        public int CPF { get; set; }              
        public byte[] Password { get; set; }      
        public string PhoneNumber { get; set; }    
        public DateTime? DateOfBirth { get; set; }  
        public string UserLevel { get; set; }      
        public DateTime? CreatedAt { get; set; }  
        public DateTime? UpdatedAt { get; set; }
    }
}
