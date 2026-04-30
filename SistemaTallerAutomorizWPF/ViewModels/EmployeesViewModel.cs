using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SistemaTallerAutomorizWPF.Commands;
using SistemaTallerAutomorizWPF.Models;
using SistemaTallerAutomorizWPF.Repositories;
using SistemaTallerAutomorizWPF.View;
using SistemaTallerAutomorizWPF.ViewModel;

namespace SistemaTallerAutomorizWPF.ViewModels
{
    public class EmployeesViewModel : ViewModelBase
    {
        public ObservableCollection<Employee> Empleados { get; set; } = new ObservableCollection<Employee>();

        public ICommand AgregarEmpleadoCommand { get; }
        public ICommand EditarEmpleadoCommand { get; }
        public ICommand EliminarEmpleadoCommand { get; }

        public EmployeesViewModel()
        {
            AgregarEmpleadoCommand = new RelayCommand(AgregarEmpleado);
            EditarEmpleadoCommand = new RelayCommand(EditarEmpleado);
            EliminarEmpleadoCommand = new RelayCommand(EliminarEmpleado);

            LoadEmployeesFromDB();
        }

        private void LoadEmployeesFromDB()
        {
            Empleados.Clear();

            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = "SELECT * FROM Empleados";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Empleados.Add(new Employee
                        {
                            Id = (int)reader["Id"],
                            Nombre = reader["Nombre"].ToString(),
                            Apellido = reader["Apellido"].ToString(),
                            Cedula = reader["Cedula"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            Email = reader["Email"].ToString(),
                            Cargo = reader["Cargo"].ToString(),
                            Rol = reader["Rol"].ToString(),
                            FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]),
                            Estado = reader["Estado"].ToString()
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar empleados: " + ex.Message);
                }
            }
        }

        private void AgregarEmpleado(object obj)
        {
            var ventana = new AddEmployeeView();
            ventana.ShowDialog();
        }

        private void EditarEmpleado(object obj)
        {
            if (obj is Employee empleado)
            {
                MessageBox.Show($"Aquí abrirías un formulario de edición para: {empleado.Nombre}.");
                // Podrías cargar los datos en un formulario editable.
            }
        }

        private void EliminarEmpleado(object obj)
        {
            if (obj is Employee empleado)
            {
                var resultado = MessageBox.Show($"¿Estás seguro de que deseas eliminar a {empleado.Nombre}?", "Confirmar eliminación", MessageBoxButton.YesNo);
                if (resultado == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = Connections.GetConnection())
                    {
                        string query = "DELETE FROM Employees WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Id", empleado.Id);

                        try
                        {
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            Empleados.Remove(empleado);
                            MessageBox.Show("Empleado eliminado correctamente.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el empleado: " + ex.Message);
                        }
                    }
                }
            }
        }

        private Employee _selectedEmpleado;
        public Employee SelectedEmpleado
        {
            get => _selectedEmpleado;
            set
            {
                _selectedEmpleado = value;
                OnPropertyChanged(nameof(SelectedEmpleado));
            }
        }
    }
}

