using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTallerAutomorizWPF.Models
{
    public class Client
    {
        public string NameClient { get; set; }
        public string Email { get; set; }
        public string Vehicle { get; set; }
        public string Modelo { get; set; }
        public int Orders { get; set; }
        public decimal Debts { get; set; }
        public int Id { get; set; }
        public string Placa { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
