using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTallerAutomorizWPF.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Dueno { get; set; } 
        public string NombreCliente { get; set; }
        public string MarcaVehiculo { get; set; }

        // Datos del vehículo
        public string Modelo { get; set; }
        public int? Anio { get; set; }  // nullable por si aún no hay vehículo
        public string Placa { get; set; }
        public string Color { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
