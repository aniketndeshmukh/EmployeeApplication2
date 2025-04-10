using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeApplication2.Models
{
    public class HistoryCodeValue
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}