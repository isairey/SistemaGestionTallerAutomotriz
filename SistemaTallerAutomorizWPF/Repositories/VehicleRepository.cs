using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SistemaTallerAutomorizWPF.Models;
using System.Windows;

namespace SistemaTallerAutomorizWPF.Repositories
{
    internal class VehicleRepository
    {
        public static void AgregarVehiculo(Vehicle vehiculo)
        {
            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = @"INSERT INTO Vehiculos (Marca, Modelo, Anio, Placa, Color, ClienteId, FechaRegistro)
                         VALUES (@Marca, @Modelo, @Anio, @Placa, @Color, @ClienteId, @FechaRegistro)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
                command.Parameters.AddWithValue("@Anio", vehiculo.Anio);
                command.Parameters.AddWithValue("@Placa", vehiculo.Placa);
                command.Parameters.AddWithValue("@Color", vehiculo.Color);
                command.Parameters.AddWithValue("@ClienteId", vehiculo.ClienteId);
                command.Parameters.AddWithValue("@FechaRegistro", vehiculo.FechaRegistro);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el vehículo: " + ex.Message);
                }
            }
        }

            public static List<Vehicle> ObtenerVehiculos()
        {
            List<Vehicle> vehiculos = new List<Vehicle>();

            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = "SELECT * FROM Vehiculos";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        vehiculos.Add(new Vehicle
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Marca = reader["Marca"].ToString(),
                            Modelo = reader["Modelo"].ToString(),
                            Anio = Convert.ToInt32(reader["Anio"]),
                            Placa = reader["Placa"].ToString(),
                            Color = reader["Color"].ToString(),
                            ClienteId = Convert.ToInt32(reader["ClienteId"]),
                            FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener los vehículos: " + ex.Message);
                }
            }

            return vehiculos;
        }


        public static void ActualizarVehiculo(Vehicle vehiculo)
        {
            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = @"UPDATE Vehiculos
                         SET Marca = @Marca, Modelo = @Modelo, Anio = @Anio,
                             Placa = @Placa, Color = @Color, ClienteId = @ClienteId
                         WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", vehiculo.Id);
                command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
                command.Parameters.AddWithValue("@Anio", vehiculo.Anio);
                command.Parameters.AddWithValue("@Placa", vehiculo.Placa);
                command.Parameters.AddWithValue("@Color", vehiculo.Color);
                command.Parameters.AddWithValue("@ClienteId", vehiculo.ClienteId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el vehículo: " + ex.Message);
                }
            }
        }


        public static void EliminarVehiculo(int id)
        {
            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = "DELETE FROM Vehiculos WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el vehículo: " + ex.Message);
                }
            }
        }
    }

}

