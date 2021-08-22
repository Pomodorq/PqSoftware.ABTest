using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public class LogicException : Exception
    {
        public string Type { get; set; }
        public string Detail { get; set; }
        public string Title { get; set; }
        public LogicException(string detail)
        {
            Type = "logic-exception";
            Detail = detail;
            Title = "Business Logic Exception";
        }
    }
}
