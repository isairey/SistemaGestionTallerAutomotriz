using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Spreadsheet;
using SistemaTallerAutomorizWPF.Models;
using SistemaTallerAutomorizWPF.Repositories;
using SistemaTallerAutomorizWPF.ViewModels;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.Linq;
using System.Text;

namespace SistemaTallerAutomorizWPF.View
{
    /// <summary>
    /// Lógica de interacción para VehicleView.xaml
    /// </summary>
    public partial class VehicleView : UserControl
    {
        public VehicleView()
        {
            InitializeComponent();
            DataContext = new VehicleViewModel();
            ((VehicleViewModel)DataContext).CargarDatos();
            ClienteComboBox.ItemsSource = ClientsList;

            var viewModel = (VehicleViewModel)DataContext;
            ClienteComboBox.ItemsSource = viewModel.ClientsList;

        }


        // Lista de vehiculos
        public ObservableCollection<Vehicle> VehicleList { get; set; } = new ObservableCollection<Vehicle>();
        // Lista de clientes
        public ObservableCollection<Client> ClientsList { get; set; } = new ObservableCollection<Client>();



    private void AgregarVehiculoBtn_Click(object sender, RoutedEventArgs e)
    {
    Button boton = (Button)sender;
    boton.IsEnabled = false;

    // Validación de campos
    if (MarcaTextBox.IsPlaceHolderVisible || PlacaTextBox.IsPlaceHolderVisible ||
        ColorTextBox.IsPlaceHolderVisible || ModeloTextBox.IsPlaceHolderVisible)
    {
        MessageBox.Show("Por favor, completa todos los campos obligatorios.");
        boton.IsEnabled = true;
        return;
    }

    // Verificar si se ha seleccionado un cliente
    if (ClienteComboBox.SelectedItem == null)
    {
        MessageBox.Show("Por favor, selecciona un cliente.");
        boton.IsEnabled = true;
        return;
    }

            int clienteId = (int)ClienteComboBox.SelectedValue;

            if (!int.TryParse(AñoTextBox.Text, out int anio) || anio < 1900 || anio > DateTime.Now.Year)
    {
        MessageBox.Show("Año inválido.");
        boton.IsEnabled = true;
        return;
    }

    string placa = PlacaTextBox.Text.Trim().ToUpper();

    // Validar formato de placa dominicana básica
    if (!System.Text.RegularExpressions.Regex.IsMatch(placa, @"^[A-Z]\d{7}$"))
    {
        MessageBox.Show("La placa debe tener el formato correcto (ej. A1234567).");
        boton.IsEnabled = true;
        return;
    }

    using (SqlConnection connection = Models.Connections.GetConnection())
    {
        string query = @"INSERT INTO Vehiculos (ClienteId, Anio, Placa, Color, FechaRegistro, Marca, Modelo)
                         VALUES (@ClienteId, @Anio, @Placa, @Color, @FechaRegistro, @Marca, @Modelo)";
        SqlCommand command = new SqlCommand(query, connection);

        try
        {
            connection.Open();

                    // Buscar cliente que tenga la marca ingresada (relación por texto)
                    if (ClienteComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Por favor, selecciona un cliente.");
                        boton.IsEnabled = true;
                        return;
                    }

                    var clienteSeleccionado = (Client)ClienteComboBox.SelectedItem;
                    int clientId = clienteSeleccionado.Id;

                    int ClientId = ((Client)ClienteComboBox.SelectedItem).Id;

                    // ✅ Validar si ya tiene un vehículo registrado
                    string checkVehiculoQuery = "SELECT COUNT(*) FROM Vehiculos WHERE ClienteId = @ClienteId";
            SqlCommand checkCmd = new SqlCommand(checkVehiculoQuery, connection);
            checkCmd.Parameters.AddWithValue("@ClienteId", clienteId);
            int countVehiculos = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (countVehiculos > 0)
            {
                MessageBox.Show("Este cliente ya tiene un vehículo registrado.");
                boton.IsEnabled = true;
                return;
            }

            // Asignar valores
            command.Parameters.AddWithValue("@ClienteId", clienteId);
            command.Parameters.AddWithValue("@Anio", anio);
            command.Parameters.AddWithValue("@Placa", placa);
            command.Parameters.AddWithValue("@Color", ColorTextBox.Text.Trim());
            command.Parameters.AddWithValue("@FechaRegistro", DateTime.Now);
            command.Parameters.AddWithValue("@Marca", MarcaTextBox.Text.Trim());
            command.Parameters.AddWithValue("@Modelo", ModeloTextBox.Text.Trim());

            command.ExecuteNonQuery();
            MessageBox.Show("Vehículo agregado correctamente.");

            // ✅ Animación del botón (verde suave)
            var brush = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#F0F0F0"));
            boton.Background = brush;

            var animationToGreen = new ColorAnimation
            {
                To = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#9FA324"),
                Duration = TimeSpan.FromSeconds(0.4)
            };
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGreen);

            Task.Delay(3000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    var backToGray = new ColorAnimation
                    {
                        To = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#F0F0F0"),
                        Duration = TimeSpan.FromSeconds(0.5)
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, backToGray);
                    boton.IsEnabled = true;
                });
            });

            // Limpiar campos
            MarcaTextBox.Text = "";
            ModeloTextBox.Text = "";
            AñoTextBox.Text = "";
            PlacaTextBox.Text = "";
            ColorTextBox.Text = "";

            // Refrescar DataGrid
            LoadVehiculosFromDB();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al agregar el vehículo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            boton.IsEnabled = true;
        }
    }
}

        private void EditarVehiculoBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MarcaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AñoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void VehicleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoadVehiculosFromDB()
        {
            VehicleList.Clear();

            using (SqlConnection connection = Models.Connections.GetConnection())
            {
                string query = @"SELECT 
                                V.Id,
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
                        VehicleList.Add(new Vehicle
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Dueño = reader["NombreCliente"].ToString(),
                            Marca = reader["MarcaVehiculo"].ToString(),
                            Modelo = reader["Modelo"].ToString(),
                            Anio = Convert.ToInt32(reader["Anio"]),
                            Placa = reader["Placa"].ToString(),
                            Color = reader["Color"].ToString(),
                            FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los vehículos: " + ex.Message);
                }
            }

            VehicleDataGrid.ItemsSource = (System.Collections.IEnumerable)VehicleList;
        }

        private void BuscarVehiculoBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as VehicleViewModel;
            if (vm == null) return;

            string filtro = BuscarTextBox.Text.Trim();
            vm.FiltrarVehiculos(filtro);
        }

        private void ExportarExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as VehicleViewModel;
            if (vm == null) return;

            if (vm.VehiculosList == null || vm.VehiculosList.Count == 0)
            {
                MessageBox.Show("No hay vehículos para exportar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                FileName = "Vehiculos Exportados.xlsx"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Vehículos");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "Dueño";
                    worksheet.Cell(1, 2).Value = "Marca";
                    worksheet.Cell(1, 3).Value = "Modelo";
                    worksheet.Cell(1, 4).Value = "Año";
                    worksheet.Cell(1, 5).Value = "Placa";
                    worksheet.Cell(1, 6).Value = "Color";
                    worksheet.Cell(1, 7).Value = "Fecha Registro";

                    // Agregar datos
                    int fila = 2;
                    foreach (var v in vm.VehiculosList)
                    {
                        worksheet.Cell(fila, 1).Value = v.NombreCliente;
                        worksheet.Cell(fila, 2).Value = v.MarcaVehiculo;
                        worksheet.Cell(fila, 3).Value = v.Modelo;
                        worksheet.Cell(fila, 4).Value = v.Anio;
                        worksheet.Cell(fila, 5).Value = v.Placa;
                        worksheet.Cell(fila, 6).Value = v.Color;
                        worksheet.Cell(fila, 7).Value = v.FechaRegistro?.ToString("dd/MM/yyyy") ?? "";
                        fila++;
                    }

                    // Formato encabezados: negrita, fondo gris claro
                    var headerRange = worksheet.Range(1, 1, 1, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Agregar autofiltros
                    worksheet.Range(1, 1, fila - 1, 6).SetAutoFilter();

                    // Guardar archivo
                    workbook.SaveAs(saveFileDialog.FileName);

                    // Crear un SolidColorBrush mutable (si el botón no lo tiene aún)
                    var brush = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    ExportarExcelBtn.Background = brush;

                    // Animación suave al verde
                    var animationToGreen = new ColorAnimation
                    {
                        To = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#9FA324"), // Verde SITAUTO
                        Duration = TimeSpan.FromSeconds(0.5)
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGreen);

                    // Desactivar el botón
                    ExportarExcelBtn.Foreground = Brushes.White;
                    ExportarExcelBtn.IsEnabled = false;

                    // Esperar y luego restaurar visualmente
                    Task.Delay(3000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var animationToGray = new ColorAnimation
                            {
                                To = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#F0F0F0"), // Color original
                                Duration = TimeSpan.FromSeconds(1)
                            };
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGray);

                            ExportarExcelBtn.Foreground = Brushes.Black;
                            ExportarExcelBtn.IsEnabled = true;
                        });
                    });
                    MessageBox.Show("Exportación a Excel exitosa.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClienteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
