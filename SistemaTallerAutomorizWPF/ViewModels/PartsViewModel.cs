using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SistemaTallerAutomorizWPF.Models;
using SistemaTallerAutomorizWPF.ViewModel;

namespace SistemaTallerAutomorizWPF.ViewModels
{
    public class PartsViewModel : ViewModelBase
    {
        public ObservableCollection<Orden> OrdenesList { get; set; } = new ObservableCollection<Orden>();

        private List<Orden> OrdenesBackup = new List<Orden>();

        private Orden _ordenSeleccionada;
        public Orden OrdenSeleccionada
        {
            get => _ordenSeleccionada;
            set
            {
                if (_ordenSeleccionada != value)
                {
                    _ordenSeleccionada = value;
                    OnPropertyChanged(nameof(OrdenSeleccionada));
                    // Aquí puedes poner lógica para mostrar detalles
                }
            }
        }

        public void CargarOrdenes()
        {
            OrdenesList.Clear();
            OrdenesBackup.Clear();

            using (var connection = Connections.GetConnection())
            {
                string query = @"
            SELECT 
                O.IdOrden, O.IdCliente, O.IdVehiculo, O.Total, O.Estado, O.Fecha,
                C.NameClient, C.Vehicle AS MarcaVehiculo
            FROM Ordenes O
            INNER JOIN Clientes C ON O.IdCliente = C.Id
            INNER JOIN Vehiculos V ON O.IdVehiculo = V.Id";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var orden = new Orden
                        {
                            IdOrden = Convert.ToInt32(reader["IdOrden"]),
                            IdCliente = Convert.ToInt32(reader["IdCliente"]),
                            IdVehiculo = Convert.ToInt32(reader["IdVehiculo"]),
                            Total = Convert.ToDecimal(reader["Total"]),
                            Estado = reader["Estado"]?.ToString(),
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            NombreCliente = reader["NameClient"].ToString(),
                            MarcaVehiculo = reader["MarcaVehiculo"].ToString()
                        };

                        OrdenesList.Add(orden);
                        OrdenesBackup.Add(orden);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar las órdenes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void FiltrarOrdenes(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                OrdenesList.Clear();
                foreach (var o in OrdenesBackup)
                    OrdenesList.Add(o);
            }
            else
            {
                var filtroLower = filtro.ToLower();

                var filtrados = OrdenesBackup.Where(o =>
                    (o.NombreCliente != null && o.NombreCliente.ToLower().Contains(filtroLower)) ||
                    (o.MarcaVehiculo != null && o.MarcaVehiculo.ToLower().Contains(filtroLower)) ||
                    (o.Estado != null && o.Estado.ToLower().Contains(filtroLower))
                ).ToList();

                OrdenesList.Clear();
                foreach (var o in filtrados)
                    OrdenesList.Add(o);
            }
        }
    }
}
