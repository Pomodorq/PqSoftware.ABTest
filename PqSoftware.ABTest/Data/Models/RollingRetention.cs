using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data.Models
{
    public class RollingRetention
    {
        public double Value { get; set; }

        public RollingRetention(double value)
        {
            Value = value;
        }
    }
}
