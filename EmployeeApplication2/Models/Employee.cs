using System;

namespace EmployeeApplication2.Models
{
    public class Employee
    {
        public int EmpNo { get; set; }  // Employee Number (Primary Key)
        public string Name { get; set; }
        public DateTime Dob { get; set; }  // Date of Birth
        public DateTime Doj { get; set; }  // Date of Joining
        public string Gender { get; set; }
        public string Pan { get; set; }  // PAN number (Previously "pen")
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
    }
}
