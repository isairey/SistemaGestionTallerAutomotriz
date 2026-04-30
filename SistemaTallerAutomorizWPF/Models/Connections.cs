using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SistemaTallerAutomorizWPF.Models
{
    public static class Connections
    {
        private static string connectionString = "Data Source=localhost;Initial Catalog=SITAUTODB;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}