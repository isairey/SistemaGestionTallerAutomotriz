
namespace SistemaTallerAutomorizWPF.Repositories
{
    public class Vehicle
    {
        public string? Marca { get; internal set; }
        public int Id { get; internal set; }
        public string? Modelo { get; internal set; }
        public int Anio { get; internal set; }
        public string? Placa { get; internal set; }
        public string? Color { get; internal set; }
        public int ClienteId { get; internal set; }
        public DateTime FechaRegistro { get; internal set; }
        public string? Dueño { get; internal set; }
    }
}