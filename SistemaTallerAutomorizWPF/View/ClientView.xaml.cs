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
using ClosedXML.Excel;
using Microsoft.Win32;
using SistemaTallerAutomorizWPF.Models;
using SistemaTallerAutomorizWPF.ViewModels;
using System.IO;

namespace SistemaTallerAutomorizWPF.View
{
    /// <summary>
    /// Lógica de interacción para ClientView.xaml
    /// </summary>
    public partial class ClientView : UserControl
    {
        public ClientView()
        {
            InitializeComponent();
            ClientsList = new ObservableCollection<Client>();
            ClientDataGrid.ItemsSource = ClientsList;
            LoadClientsFromDB();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BuscarTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BuscarTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (BuscarTextBox.Text == "Buscar...")
            {
                BuscarTextBox.Text = "";
                BuscarTextBox.Foreground = Brushes.Black;
            }
        }

        private void BuscarTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuscarTextBox.Text))
            {
                BuscarTextBox.Text = "Buscar...";
            }
        }

        private void BuscarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscarTextBox.Text = "Buscar...";
                BuscarTextBox.Foreground = Brushes.Gray;
                Keyboard.ClearFocus();
                BuscarTextBox_GotFocus(sender, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //validación del Email
        private bool EmailEsValido(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public ObservableCollection<Client> ClientsList { get; set; }

        private void LoadClientsFromDB()
        {
            ClientsList.Clear();

            using (SqlConnection connection = Models.Connections.GetConnection())
            {
                string query = @"SELECT Id, NameClient, Email, Vehicle, Orders, Debts, Placa FROM Clientes";

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
                            NameClient = reader["NameClient"].ToString(),
                            Email = reader["Email"].ToString(),
                            Vehicle = reader["Vehicle"].ToString(),
                            Orders = Convert.ToInt32(reader["Orders"]),
                            Debts = Convert.ToDecimal(reader["Debts"]),
                            Placa = reader["Placa"] == DBNull.Value ? null : reader["Placa"].ToString()
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los clientes: " + ex.Message);
                }
            }

            ClientDataGrid.ItemsSource = ClientsList;
        }

        private void Button_Click_1()
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Exported_Clients.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Clients");

                        //Encabezados
                        worksheet.Cell(1, 1).Value = "Nombre del cliente";
                        worksheet.Cell(1, 2).Value = "Email";
                        worksheet.Cell(1, 3).Value = "Vehículo";
                        worksheet.Cell(1, 4).Value = "Órdenes";
                        worksheet.Cell(1, 5).Value = "Dedudas";
                        worksheet.Cell(1, 6).Value = "Placa";

                        //Datos
                        for (int i = 0; i < ClientsList.Count; i++)
                        {
                            var Client = ClientsList[i];
                            worksheet.Cell(i + 2, 1).Value = Client.NameClient;
                            worksheet.Cell(i + 2, 2).Value = Client.Email;
                            worksheet.Cell(i + 2, 3).Value = Client.Vehicle;
                            worksheet.Cell(i + 2, 4).Value = Client.Orders;
                            worksheet.Cell(i + 2, 5).Value = Client.Debts;
                            worksheet.Cell(i + 2, 6).Value = Client.Placa;
                        }

                        //Autoajustar columnas
                        worksheet.Columns().AdjustToContents();

                        //Fondo de encabezados
                        var headerRange = worksheet.Range("A1:E1");
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

                        //Estilo de encabezados
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        //Bordes
                        worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        //formato de moneda para la columna de deudas
                        worksheet.Column(5).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";

                        //Filtros automaticos en los encabezados
                        worksheet.RangeUsed().SetAutoFilter();

                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Exportación completada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AgregarCliente_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;
            boton.IsEnabled = false; // Desactiva el botón

            // Para no guardar los placeholders
            if (NombreTextBox.IsPlaceHolderVisible || EmailTextBox.IsPlaceHolderVisible || VehiculoTextBox.IsPlaceHolderVisible || PlacaTextBox.IsPlaceHolderVisible )
            {
                MessageBox.Show("Por favor, completa todos los campos obligatorios.");
                boton.IsEnabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(PlacaTextBox.Text))
            {
                MessageBox.Show("La placa es obligatoria.");
                boton.IsEnabled = true;
                return;
            }

            // Validación básica
            if (string.IsNullOrWhiteSpace(NombreTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(VehiculoTextBox.Text))
            {
                MessageBox.Show("Nombre, Email y Vehículo son campos obligatorios.");
                boton.IsEnabled = true;
                return;
            }

            // Alerta de validación del formato de Email
            if (!EmailEsValido(EmailTextBox.Text.Trim()))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido.");
                boton.IsEnabled = true;
                return;
            }

            //Validar Deudas
            if (!decimal.TryParse(DeudasTextBox.IsPlaceHolderVisible ? "0" : DeudasTextBox.Text, out decimal debts) || debts < 0)
            {
                MessageBox.Show("El monto de deudas debe ser un número decimal positivo.");
                boton.IsEnabled = true;
                return;
            }

            using (SqlConnection connection = SistemaTallerAutomorizWPF.Models.Connections.GetConnection())
            {
                string insertQuery = "INSERT INTO Clientes (NameClient, Email, Vehicle, Debts)\r\n VALUES (@NameClient, @Email, @Vehicle, @Debts);\r\n SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(insertQuery, connection);

                command.Parameters.AddWithValue("@NameClient", NombreTextBox.Text.Trim());
                command.Parameters.AddWithValue("@Email", EmailTextBox.Text.Trim());
                command.Parameters.AddWithValue("@Vehicle", VehiculoTextBox.Text.Trim());
                command.Parameters.AddWithValue("@Debts", debts);
                command.Parameters.AddWithValue("@Placa", PlacaTextBox.Text.Trim());

                try
                {
                    connection.Open();
                    //Validar si el correo electrónico ya existe
                    string checkQuery = "SELECT COUNT(*) FROM Clientes WHERE Email = @Email";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@Email", EmailTextBox.Text.Trim());
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un cliente con ese correo.");
                        boton.IsEnabled = true;
                        return;
                    }

                    command.ExecuteNonQuery();
                    MessageBox.Show("Cliente agregado correctamente.");

                    // Crear un SolidColorBrush mutable (si el botón no lo tiene aún)
                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    AgregarClienteBtn.Background = brush;

                    // Animación suave al verde
                    var animationToGreen = new ColorAnimation
                    {
                        To = (Color)ColorConverter.ConvertFromString("#9FA324"), // Verde SITAUTO
                        Duration = TimeSpan.FromSeconds(0.5)
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGreen);

                    // Desactivar el botón
                    AgregarClienteBtn.Foreground = Brushes.White;
                    AgregarClienteBtn.IsEnabled = false;

                    // Esperar y luego restaurar visualmente
                    Task.Delay(3000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var animationToGray = new ColorAnimation
                            {
                                To = (Color)ColorConverter.ConvertFromString("#F0F0F0"), // Color original
                                Duration = TimeSpan.FromSeconds(1)
                            };
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGray);

                            AgregarClienteBtn.Foreground = Brushes.Black;
                            AgregarClienteBtn.IsEnabled = true;
                        });
                    });

                    // ✅ Guardar agregado en el log diario
                    string nombreLog = $"LogClientes_{DateTime.Today:yyyy-MM-dd}.txt";
                    string rutaLogs = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

                    if (!Directory.Exists(rutaLogs))
                        Directory.CreateDirectory(rutaLogs);

                    string rutaLogFinal = System.IO.Path.Combine(rutaLogs, nombreLog);
                    string logAgregado = $"[{DateTime.Now:HH:mm:ss}] [AGREGADO] {NombreTextBox.Text.Trim()}, {EmailTextBox.Text.Trim()}, {VehiculoTextBox.Text.Trim()}, Deuda: {debts:C}";

                    File.AppendAllText(rutaLogFinal, logAgregado + Environment.NewLine);


                    // Limpiar campos
                    NombreTextBox.Text = "";
                    EmailTextBox.Text = "";
                    VehiculoTextBox.Text = "";
                    PlacaTextBox.Text = "";
                    DeudasTextBox.Text = "";

                    //Vuelve el foco al primer campo
                    NombreTextBox.Focus();


                    // Recargar datos
                    ClientsList.Clear();
                    LoadClientsFromDB();
                    NombreTextBox.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el cliente: " + ex.Message);
                }
                finally
                {
                    boton.IsEnabled = true; // Reactiva el botón
                }
            }
        }

        private void ActivarModoEdicion_Click(object sender, RoutedEventArgs e)
        {
            ClientDataGrid.IsReadOnly = !ClientDataGrid.IsReadOnly;

            if (ClientDataGrid.IsReadOnly)
            {
                EditarClienteBtn.Content = "Modo Edición";
            }
            else
            {
                EditarClienteBtn.Content = "Bloquear Edición";
            }
        }

        private void EliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            var seleccionados = ClientDataGrid.SelectedItems.Cast<Client>().ToList();

            if (seleccionados == null || seleccionados.Count == 0)
            {
                MessageBox.Show("Selecciona al menos un cliente para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string mensaje = seleccionados.Count == 1
                ? $"¿Seguro que deseas eliminar a {seleccionados[0].NameClient}?"
                : $"¿Seguro que deseas eliminar estos {seleccionados.Count} clientes?";

            var confirm = MessageBox.Show(mensaje, "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            using (SqlConnection connection = Connections.GetConnection())
            {
                try
                {
                    connection.Open();
                    foreach (var cliente in seleccionados)
                    {
                        string query = "DELETE FROM Clientes WHERE Id = @Id";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", cliente.Id);
                        command.ExecuteNonQuery();

                        // ✅ Guardar eliminación en el log diario
                        string nombreLog = $"LogClientes_{DateTime.Today:yyyy-MM-dd}.txt";
                        string rutaLogs = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

                        if (!Directory.Exists(rutaLogs))
                            Directory.CreateDirectory(rutaLogs);

                        string rutaLogFinal = System.IO.Path.Combine(rutaLogs, nombreLog);
                        string logEliminado = $"[{DateTime.Now:HH:mm:ss}] [ELIMINADO] {cliente.NameClient}, {cliente.Email}, {cliente.Vehicle}, Órdenes: {cliente.Orders}, Deuda: {cliente.Debts:C}";

                        File.AppendAllText(rutaLogFinal, logEliminado + Environment.NewLine);
                    }

                    // ✅ Animación visual en el botón
                    Button boton = (Button)sender;
                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                    boton.Background = brush;

                    var animationToRed = new ColorAnimation
                    {
                        To = (Color)ColorConverter.ConvertFromString("#FFCDD2"), // rojo suave
                        Duration = TimeSpan.FromSeconds(0.4)
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToRed);

                    Task.Delay(3000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var backToGray = new ColorAnimation
                            {
                                To = (Color)ColorConverter.ConvertFromString("#F0F0F0"),
                                Duration = TimeSpan.FromSeconds(0.5)
                            };
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, backToGray);
                        });
                    });

                    // ✅ Actualizar el listado
                    ClientsList.Clear();
                    LoadClientsFromDB();

                    MessageBox.Show("Cliente(s) eliminado(s) correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar cliente(s): " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportarLogDiario_Click(object sender, RoutedEventArgs e)
        {
            // Obtener la fecha seleccionada del DatePicker
            if (FechaLogDatePicker.SelectedDate is not DateTime fechaSeleccionada)
            {
                MessageBox.Show("Selecciona una fecha válida para exportar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nombreArchivo = $"LogClientes_{fechaSeleccionada:yyyy-MM-dd}.txt";
            string rutaArchivo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", nombreArchivo);

            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show("No hay log guardado para esa fecha.", "Sin datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var todasLasLineas = File.ReadAllLines(rutaArchivo).ToList();

            // Clasificamos las entradas por tipo
            var agregados = new List<string>();
            var modificados = new List<string>();
            var eliminados = new List<string>();

            foreach (var linea in todasLasLineas)
            {
                if (linea.Contains("[AGREGADO]"))
                    agregados.Add(linea);
                else if (linea.Contains("[MODIFICADO]"))
                    modificados.Add(linea);
                else if (linea.Contains("[ELIMINADO]"))
                    eliminados.Add(linea);
            }

            // Si no se van a incluir los eliminados, los quitamos
            if (IncluirEliminadosCheckBox.IsChecked != true)
                eliminados.Clear();

            // Validar que haya algo que exportar
            if (agregados.Count == 0 && modificados.Count == 0 && eliminados.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar ese día.");
                return;
            }

            // Construcción del contenido final
            var contenidoFinal = new List<string>
    {
        "===============================================",
        $"      LOG DE CLIENTES - {fechaSeleccionada:dd/MM/yyyy}",
        "===============================================",
        ""
    };

            if (agregados.Any())
            {
                contenidoFinal.Add("➡️ CLIENTES AGREGADOS:");
                contenidoFinal.AddRange(agregados);
                contenidoFinal.Add("");
            }

            if (modificados.Any())
            {
                contenidoFinal.Add("🔁 CLIENTES MODIFICADOS:");
                contenidoFinal.AddRange(modificados);
                contenidoFinal.Add("");
            }

            if (eliminados.Any())
            {
                contenidoFinal.Add("❌ CLIENTES ELIMINADOS:");
                contenidoFinal.AddRange(eliminados);
                contenidoFinal.Add("");
            }

            // Guardar
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Archivo de texto (*.txt)|*.txt",
                Title = "Exportar log diario",
                FileName = $"LogClientes_{fechaSeleccionada:yyyyMMdd}.txt"
            };

            if (saveDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveDialog.FileName, contenidoFinal, Encoding.UTF8);
                MessageBox.Show("Log exportado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ClientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientDataGrid.SelectedItem is Client cliente)
            {
                NombreTextBox.Text = cliente.NameClient;
                EmailTextBox.Text = cliente.Email;
                VehiculoTextBox.Text = cliente.Vehicle;
                PlacaTextBox.Text = cliente.Orders.ToString();
                DeudasTextBox.Text = cliente.Debts.ToString("0.00");
                PlacaTextBox.Text = cliente.Placa;
            }
        }

        private void GuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;
            boton.IsEnabled = false;

            bool huboCambios = false;

            foreach (var cliente in ClientsList)
            {
                using (SqlConnection connection = Connections.GetConnection())
                {
                    try
                    {
                        connection.Open();

                        string selectQuery = "SELECT NameClient, Email, Vehicle, Placa, Orders, Debts FROM Clientes WHERE Id = @Id";
                        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                        selectCommand.Parameters.AddWithValue("@Id", cliente.Id);

                        SqlDataReader reader = selectCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            string nameBD = reader["NameClient"].ToString();
                            string emailBD = reader["Email"].ToString();
                            string vehicleBD = reader["Vehicle"].ToString();
                            string placaBD = reader["Placa"].ToString();
                            int ordersBD = Convert.ToInt32(reader["Orders"]);
                            decimal debtsBD = Convert.ToDecimal(reader["Debts"]);

                            reader.Close();

                            if (cliente.NameClient != nameBD ||
                                cliente.Email != emailBD ||
                                cliente.Vehicle != vehicleBD ||
                                cliente.Placa != placaBD ||
                                cliente.Orders != ordersBD ||
                                cliente.Debts != debtsBD)
                            {
                                string updateQuery = @"
                                                    UPDATE Clientes
                                                    SET NameClient = @NameClient,
                                                        Email = @Email,
                                                        Vehicle = @Vehicle,
                                                        Orders = @Orders,
                                                        Debts = @Debts,
                                                        Placa = @Placa
                                                    WHERE Id = @Id";

                                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                                updateCommand.Parameters.AddWithValue("@Id", cliente.Id);
                                updateCommand.Parameters.AddWithValue("@NameClient", cliente.NameClient);
                                updateCommand.Parameters.AddWithValue("@Email", cliente.Email);
                                updateCommand.Parameters.AddWithValue("@Vehicle", cliente.Vehicle);
                                updateCommand.Parameters.AddWithValue("@Orders", cliente.Orders);
                                updateCommand.Parameters.AddWithValue("@Debts", cliente.Debts);
                                updateCommand.Parameters.AddWithValue("@Placa", cliente.Placa);

                                updateCommand.ExecuteNonQuery();

                                // 📝 Guardar en el log
                                string nombreLog = $"LogClientes_{DateTime.Today:yyyy-MM-dd}.txt";
                                string rutaLogs = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                                if (!Directory.Exists(rutaLogs)) Directory.CreateDirectory(rutaLogs);

                                string rutaLogFinal = System.IO.Path.Combine(rutaLogs, nombreLog);
                                string logModificado = $"[{DateTime.Now:HH:mm:ss}] [MODIFICADO] {cliente.NameClient}, {cliente.Email}, {cliente.Vehicle}, Placa: {cliente.Placa}, Órdenes: {cliente.Orders}, Deuda: {cliente.Debts:C}";
                                File.AppendAllText(rutaLogFinal, logModificado + Environment.NewLine);

                                huboCambios = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar el cliente {cliente.NameClient}: {ex.Message}");
                    }
                }
            }

            if (huboCambios)
            {
                MessageBox.Show("Cambios guardados correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // ✅ Animación visual del botón
                var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
                boton.Background = brush;

                var animationToGreen = new ColorAnimation
                {
                    To = (Color)ColorConverter.ConvertFromString("#9FA324"),
                    Duration = TimeSpan.FromSeconds(0.4)
                };
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGreen);

                boton.Foreground = Brushes.White;

                Task.Delay(3000).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var animationToGray = new ColorAnimation
                        {
                            To = (Color)ColorConverter.ConvertFromString("#F0F0F0"),
                            Duration = TimeSpan.FromSeconds(0.5)
                        };
                        brush.BeginAnimation(SolidColorBrush.ColorProperty, animationToGray);

                        boton.Foreground = Brushes.Black;
                        boton.IsEnabled = true;
                    });
                });
            }
            else
            {
                MessageBox.Show("No se detectaron cambios para guardar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                boton.IsEnabled = true;
            }
        }



        private void ClientDataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ModeloTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
