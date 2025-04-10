using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using EmployeeApplication2.Models;

namespace EmployeeApplication2.Controllers
{
    public class IndexController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        // Search Employee by EmpID
        public ActionResult SearchEmployee(string empId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM employeeData WHERE empno = @EmpID";
                var employee = connection.Query<Employee>(query, new { EmpID = empId }).FirstOrDefault();
                if (employee == null)
                {
                    return PartialView("_NoRecordsFound");
                }

                return PartialView("_EmployeeDetails", employee);
            }
        }

        // Get Code List for Employee
        public JsonResult GetEmployeeCodes(int empId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Code FROM CodeValue WHERE EmpID = @EmpID";
                var codeValues = connection.Query<string>(query, new { EmpID = empId });
                return Json(codeValues, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetEmployeeValues(int empId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Code, Value FROM HistoryCodeValue WHERE EmpID = @EmpID";
                var values = connection.Query<HistoryCodeValue>(query, new { EmpID = empId }).ToList();
                return Json(values, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetHistoryCodeValues(string empId, string code = "")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM HistoryCodeValue WHERE empid = @empId";

                if (!string.IsNullOrEmpty(code))
                {
                    query += " AND code = @code";
                }

                var history = connection.Query<HistoryCodeValue>(query, new { empId, code }).ToList();
                return PartialView("_HistoryCodeValue", history);
            }
        }
        // Get History Code Values for Employee
       /* public ActionResult GetHistoryCodeValues(int empId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM HistoryCodeValue WHERE EmpID = @EmpID ORDER BY UpdatedOn DESC";
                var historyValues = connection.Query<HistoryCodeValue>(query, new { EmpID = empId }).ToList();
                return PartialView("_HistoryCodeValue", historyValues);
            }
        }*/
        public JsonResult GetValueForCode(int empId, string code)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Value FROM HistoryCodeValue WHERE EmpID = @EmpID AND Code = @Code";
                var value = connection.QueryFirstOrDefault<string>(query, new { EmpID = empId, Code = code });
                return Json(value ?? "", JsonRequestBehavior.AllowGet);
            }
        }
        // Save Code & Value to History Table
        [HttpPost]
        public JsonResult SaveCodeValue(int empId, string code, int value)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Fetch the existing value
                string query = "SELECT value FROM HistoryCodeValue WHERE EmpID = @empId AND Code = @code";
                int? existingValue = connection.QueryFirstOrDefault<int?>(query, new { empId, code });

                // Check if value has changed
                if (existingValue.HasValue && existingValue.Value == value)
                {
                    return Json(new { success = false, message = "Change not detected." });
                }

                // If value changed, update it
                string upsertQuery = @"
            IF EXISTS (SELECT 1 FROM HistoryCodeValue WHERE EmpId = @empId AND Code = @code)
                UPDATE HistoryCodeValue SET Value = @value, UpdatedOn = GETDATE() WHERE EmpId = @empId AND Code = @code
            ELSE
                INSERT INTO HistoryCodeValue (EmpId, Code, Value, UpdatedOn) VALUES (@empId, @code, @value, GETDATE())";

                connection.Execute(upsertQuery, new { empId, code, value });

                return Json(new { success = true, message = "Value updated successfully." });
            }
        }

    }
}
