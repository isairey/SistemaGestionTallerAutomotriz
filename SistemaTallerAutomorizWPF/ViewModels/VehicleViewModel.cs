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
    public class VehicleViewModel : ViewModelBase
    {
        public ObservableCollection<Vehiculo> VehiculosList { get; set; } = new ObservableCollection<Vehiculo>();
        public ObservableCollection<Client> ClientsList { get; set; } = new ObservableCollection<Client>();


        internal void CargarDatos()
        {
            LoadVehiculosFromDB();
            CargarClientes();
        }

        private List<Vehiculo> VehiculosBackup = new List<Vehiculo>();

        public void FiltrarVehiculos(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                VehiculosList.Clear();
                foreach (var v in VehiculosBackup)
                    VehiculosList.Add(v);
            }
            else
            {
                var filtroLower = filtro.ToLower();

                var filtrados = VehiculosBackup.Where(v =>
                    (v.NombreCliente != null && v.NombreCliente.ToLower().Contains(filtroLower)) ||
                    (v.Placa != null && v.Placa.ToLower().Contains(filtroLower))
                ).ToList();

                VehiculosList.Clear();
                foreach (var v in filtrados)
                    VehiculosList.Add(v);
            }
        }

        public void CargarClientes()
        {
            ClientsList.Clear();

            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = "SELECT Id, NameClient FROM Clientes"; // Solo lo necesario para el ComboBox
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ClientsList.Add(new Client
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NameClient = reader["NameClient"].ToString()
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los clientes: " + ex.Message);
                }
            }
        }


        private void LoadVehiculosFromDB()
        {
            VehiculosList.Clear();
            VehiculosBackup.Clear();

            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = @"SELECT 
                                    C.Id AS ClienteId,
                                    C.NameClient AS NombreCliente,
                                    V.Marca AS MarcaVehiculo,
                                    V.Modelo,
                                    V.Anio,
                                    V.Placa,
                                    V.Color,
                                    V.FechaRegistro
                                FROM Vehiculos V
                                LEFT JOIN Clientes C ON V.ClienteId = C.Id";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var vehiculo = new Vehiculo
                        {
                            ClienteId = Convert.ToInt32(reader["ClienteId"]),
                            NombreCliente = reader["NombreCliente"]?.ToString(),
                            MarcaVehiculo = reader["MarcaVehiculo"] == DBNull.Value ? null : reader["MarcaVehiculo"].ToString(),
                            Modelo = reader["Modelo"] == DBNull.Value ? null : reader["Modelo"].ToString(),
                            Anio = reader["Anio"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["Anio"]),
                            Placa = reader["Placa"]?.ToString(),
                            Color = reader["Color"]?.ToString(),
                            FechaRegistro = reader["FechaRegistro"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["FechaRegistro"]),
                        };
                        VehiculosList.Add(vehiculo);
                        VehiculosBackup.Add(vehiculo);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los vehículos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        internal void RecargarVehiculos()
        {
            LoadVehiculosFromDB();
        }
    }
}
