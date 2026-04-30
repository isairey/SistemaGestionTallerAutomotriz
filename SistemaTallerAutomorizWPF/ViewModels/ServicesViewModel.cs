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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaTallerAutomorizWPF.ViewModels
{
    public class ServicesViewModel : ViewModelBase
    {
        public ObservableCollection<ResumenServicio> ServiciosList { get; set; } = new();
        private ResumenServicio _clienteSeleccionado;
        public ResumenServicio ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged(nameof(ClienteSeleccionado));
            }
        }

        public void CargarDatos()
        {
            ServiciosList.Clear();

            using var connection = Connections.GetConnection();
            string query = @"
                    SELECT 
                        C.Id AS IdCliente,
                        C.NameClient,
                        C.Email,
                        C.Vehicle,
                        C.Debts,
                        V.Placa,
                        V.FechaRegistro,
                    COUNT(O.IdOrden) AS OrdenesTotales,
                    (
                    SELECT TOP 1 Estado
                    FROM Ordenes O2
                    WHERE O2.IdCliente = C.Id
                    ORDER BY O2.Fecha DESC
                    ) AS UltimoEstado

                    FROM Clientes C
                    LEFT JOIN Vehiculos V ON C.Id = V.ClienteId
                    LEFT JOIN Ordenes O ON C.Id = O.IdCliente

                    GROUP BY 
                        C.Id, C.NameClient, C.Email, C.Vehicle, C.Debts,
                        V.Placa, V.FechaRegistro";

            var command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var servicio = new ResumenServicio
                    {
                        IdCliente = Convert.ToInt32(reader["IdCliente"]),
                        NombreCliente = reader["NameClient"].ToString(),
                        Email = reader["Email"].ToString(),
                        Vehiculo = reader["Vehicle"].ToString(),
                        Deuda = Convert.ToDecimal(reader["Debts"]),
                        Placa = reader["Placa"]?.ToString(),
                        FechaRegistro = reader["FechaRegistro"] as DateTime?,
                        OrdenesTotales = Convert.ToInt32(reader["OrdenesTotales"]),
                        EstadoUltimaOrden = reader["UltimoEstado"]?.ToString() ?? "Sin órdenes"
                    };

                    ServiciosList.Add(servicio);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos de servicios: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Agregar orden nueva (simulada por ahora)
        public void CrearOrdenParaClienteSeleccionado()
        {
            if (ClienteSeleccionado == null) return;

            using var connection = Connections.GetConnection();
            var command = new SqlCommand(@"
                                INSERT INTO Ordenes (IdCliente, IdVehiculo, Total, Estado, Fecha)
                                VALUES (@IdCliente, 
                                (SELECT Id FROM Vehiculos WHERE ClienteId = @IdCliente), 
                                0.00, 'Nueva', GETDATE());
        ", connection);

            command.Parameters.AddWithValue("@IdCliente", ClienteSeleccionado.IdCliente);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Orden creada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear orden: " + ex.Message);
            }
        }

        private Orden _ordenSeleccionada;
        public Orden OrdenSeleccionada
        {
            get => _ordenSeleccionada;
            set
            {
                _ordenSeleccionada = value;
                OnPropertyChanged();
                IsEditarOrdenVisible = _ordenSeleccionada != null;
            }
        }

        private bool _isEditarOrdenVisible;
        public bool IsEditarOrdenVisible
        {
            get => _isEditarOrdenVisible;
            set
            {
                _isEditarOrdenVisible = value;
                OnPropertyChanged();
            }
        }

        public void CargarUltimaOrdenDeCliente(int idCliente)
        {
            using var con = Connections.GetConnection();
            var cmd = new SqlCommand(@"
            SELECT TOP 1 
                O.IdOrden,
                O.IdCliente,
                O.IdVehiculo,
                O.Total,
                O.Estado,
                O.Fecha,
                C.NameClient AS NombreCliente,
                C.Vehicle AS MarcaVehiculo
            FROM Ordenes O
            INNER JOIN Clientes C ON O.IdCliente = C.Id
            WHERE O.IdCliente = @id
            ORDER BY O.Fecha DESC", con);

            cmd.Parameters.AddWithValue("@id", idCliente);

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                OrdenSeleccionada = new Orden
                {
                    IdOrden = Convert.ToInt32(reader["IdOrden"]),
                    NumeroOrden = Convert.ToInt32(reader["IdOrden"]),
                    IdCliente = Convert.ToInt32(reader["IdCliente"]),
                    IdVehiculo = Convert.ToInt32(reader["IdVehiculo"]),
                    Estado = reader["Estado"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    NombreCliente = reader["NombreCliente"].ToString(),
                    MarcaVehiculo = reader["MarcaVehiculo"].ToString()
                };
            }
            else
            {
                // Cliente sin órdenes
                OrdenSeleccionada = null;
                IsEditarOrdenVisible = false;
                MessageBox.Show("Este cliente aún no tiene órdenes registradas.");
            }

            reader.Close();
        }

        // Lista de estados ordenados
        public List<string> EstadosOrden { get; } = new()
        {
            "Nueva",
            "En Proceso",
            "Finalizada",
            "Entregada",
            "Cancelada"
        };

    }
}
