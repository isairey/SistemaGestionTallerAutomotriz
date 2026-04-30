using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FontAwesome.Sharp;
using SistemaTallerAutomorizWPF.Models;
using SistemaTallerAutomorizWPF.Repositories;
using SistemaTallerAutomorizWPF.ViewModel;

namespace SistemaTallerAutomorizWPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public int TotalClientes { get; set; }
        public int TotalVehiculos { get; set; }
        public int OrdenesActivas { get; set; }

        public ObservableCollection<string> ActividadReciente { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AvisosImportantes { get; set; } = new ObservableCollection<string>();
        public ICommand ShowEmployeesViewCommand { get; }

        private void ExecuteShowEmployeesViewCommand(object obj)
        {
            CurrentChildView = new EmployeesViewModel();
        }

        //modelos
        private IUserRepository userRepository;
        private UserAccountModel _currentUserAcconut;
        private ViewModelBase _currentChildView;

        public string Bienvenida => $"👋 Bienvenido/a, {CurrentUserAccount?.DisplayName}";


        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAcconut;
            }

            set
            {
                _currentUserAcconut = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }


        public HomeViewModel()
        {
            CargarEstadisticas();
            CargarActividadReciente();
            CargarAvisos();

            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();

            //inicializar comandos
            ShowClientesViewCommand = new ViewModelCommand(ExecuteShowClientesViewCommand);
            ShowVehicleViewCommand = new ViewModelCommand(ExecuteShowVehicleViewCommand);
            ShowPartsViewCommand = new ViewModelCommand(ExecuteShowPartsViewCommand);
            ShowReportsViewCommand = new ViewModelCommand(ExecuteShowReportsViewCommand);
            ShowUserServicesViewCommand = new ViewModelCommand(ExecuteShowServicesViewCommand);

            LoadCurrentUserData();
        }
        //Comandos de interacción de usuario
        private void ExecuteShowVehicleViewCommand(object obj)
        {
            CurrentChildView = new VehicleViewModel();
        }

        private void ExecuteShowClientesViewCommand(object obj)
        {
            CurrentChildView = new ClientesViewModel();
        }

        private void ExecuteShowPartsViewCommand(object obj)
        {
            CurrentChildView = new PartsViewModel();
        }

        private void ExecuteShowReportsViewCommand(object obj)
        {
            CurrentChildView = new ReportsViewModel();
        }

        private void ExecuteShowServicesViewCommand(object obj)
        {
            CurrentChildView = new ServicesViewModel();
        }


        //Comandos de interacción de usuario
        public ICommand ShowClientesViewCommand { get; }
        public ICommand ShowVehicleViewCommand { get; }
        public ICommand ShowPartsViewCommand { get; }
        public ICommand ShowUserServicesViewCommand { get; }
        public ICommand ShowReportsViewCommand { get; }

        private void CargarEstadisticas()
        {
            using (SqlConnection connection = Models.Connections.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Total Clientes
                    var cmdClientes = new SqlCommand("SELECT COUNT(*) FROM Clientes", connection);
                    TotalClientes = Convert.ToInt32(cmdClientes.ExecuteScalar());

                    // Total Vehículos
                    var cmdVehiculos = new SqlCommand("SELECT COUNT(*) FROM Vehiculos", connection);
                    TotalVehiculos = Convert.ToInt32(cmdVehiculos.ExecuteScalar());

                    // Órdenes Activas (estado diferente de 'Finalizada')
                    var cmdOrdenes = new SqlCommand("SELECT COUNT(*) FROM Ordenes WHERE Estado != 'Finalizada'", connection);
                    OrdenesActivas = Convert.ToInt32(cmdOrdenes.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar estadísticas: " + ex.Message);
                }
            }
        }

        private void CargarActividadReciente()
        {
            try
            {
                string rutaLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string logDeHoy = Path.Combine(rutaLogs, $"LogClientes_{DateTime.Today:yyyy-MM-dd}.txt");

                if (File.Exists(logDeHoy))
                {
                    var lineas = File.ReadAllLines(logDeHoy);
                    foreach (var linea in lineas)
                        ActividadReciente.Add(linea);
                }
                else
                {
                    ActividadReciente.Add("No hay actividad registrada hoy.");
                }
            }
            catch
            {
                ActividadReciente.Add("Error al cargar el log de actividad.");
            }
        }

        private void CargarAvisos()
        {
            AvisosImportantes.Add("Recuerda realizar el backup del sistema.");
            AvisosImportantes.Add("Revisa órdenes con más de 15 días.");
        }

        private void LoadCurrentUserData()
        {
            var identity = Thread.CurrentPrincipal?.Identity;
            var user = userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            if (identity != null && identity.IsAuthenticated)
            {
                var userDisplay = userRepository.GetByUsername(identity.Name);
                if (user != null)
                {
                    CurrentUserAccount.UserName = user.Username;
                    CurrentUserAccount.DisplayName = $"{user.Name} {user.LastName}";
                    CurrentUserAccount.ProfilePicture = null;
                }
                else
                {
                    CurrentUserAccount.DisplayName = "Invalid user, not logged in";
                    //Hide child views.
                }
            }
        }
    }
}
