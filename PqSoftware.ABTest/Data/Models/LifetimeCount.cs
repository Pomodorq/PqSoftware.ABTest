using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data.Models
{
    [Keyless]
    public class LifetimeCount
    {
        public int Lifetime { get; set; }
        public int Count { get; set; }
    }
}
