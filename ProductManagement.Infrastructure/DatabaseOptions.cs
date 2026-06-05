using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure
{
    public class DatabaseOptions
    {
        public required string DBConnectionString { get; set; }
    }
}
