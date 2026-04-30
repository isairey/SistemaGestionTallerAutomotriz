using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SistemaTallerAutomorizWPF.Models;

namespace SistemaTallerAutomorizWPF.View
{
    /// <summary>
    /// Lógica de interacción para AddEmployeeView.xaml
    /// </summary>
    public partial class AddEmployeeView : Window
    {
        public AddEmployeeView()
        {
            InitializeComponent();
        }

        private void GuardarEmpleado_Click(object sender, RoutedEventArgs e)
        {
            var nuevoEmpleado = new Employee
            {
                Nombre = NombreTextBox.Text.Trim(),
                Apellido = ApellidoTextBox.Text.Trim(),
                Cedula = CedulaTextBox.Text.Trim(),
                Telefono = TelefonoTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                Cargo = CargoTextBox.Text.Trim(),
                Rol = RolTextBox.Text.Trim(),
                FechaIngreso = DateTime.Now,
                Estado = "Activo"
            };

            using (var connection = Connections.GetConnection())
            {
                string query = @"INSERT INTO Empleados 
                        (Nombre, Apellido, Cedula, Telefono, Email, Cargo, Rol, FechaIngreso, Estado)
                         VALUES
                        (@Nombre, @Apellido, @Cedula, @Telefono, @Email, @Cargo, @Rol, @FechaIngreso, @Estado)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", nuevoEmpleado.Nombre);
                    command.Parameters.AddWithValue("@Apellido", nuevoEmpleado.Apellido);
                    command.Parameters.AddWithValue("@Cedula", nuevoEmpleado.Cedula);
                    command.Parameters.AddWithValue("@Telefono", nuevoEmpleado.Telefono);
                    command.Parameters.AddWithValue("@Email", nuevoEmpleado.Email);
                    command.Parameters.AddWithValue("@Cargo", nuevoEmpleado.Cargo);
                    command.Parameters.AddWithValue("@Rol", nuevoEmpleado.Rol);
                    command.Parameters.AddWithValue("@FechaIngreso", nuevoEmpleado.FechaIngreso);
                    command.Parameters.AddWithValue("@Estado", nuevoEmpleado.Estado);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Empleado agregado exitosamente.");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al agregar empleado: " + ex.Message);
                    }
                }
            }
        }
    }
}
