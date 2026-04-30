using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTallerAutomorizWPF.Models
{
    public class Orden
    {
        public int IdOrden { get; set; }
        public int IdCliente { get; set; }
        public int IdVehiculo { get; set; }
        public string NombreCliente { get; set; }
        public string MarcaVehiculo { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public int NumeroOrden { get; internal set; }
    }

}
