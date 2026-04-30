using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTallerAutomorizWPF.Models
{
    public class ResumenServicio
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Email { get; set; }
        public string Vehiculo { get; set; }
        public int OrdenesTotales { get; set; }
        public string Marca { get; set; }
        public string Placa { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public decimal Deuda { get; set; }
        public string EstadoUltimaOrden { get; set; }

    }
}
