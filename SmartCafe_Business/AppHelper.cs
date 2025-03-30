using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCafe_Business
{
    internal class AppHelper
    {
        public static string GetDBConnectionString()
        {
            return "Server=(localdb)\\MSSQLLocalDB;Database=SmartCafe;Trusted_Connection=True;";
        }
    }
}
