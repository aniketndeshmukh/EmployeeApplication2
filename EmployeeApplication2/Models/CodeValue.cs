using System;

namespace EmployeeApplication2.Models
{
    public class CodeValue
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
