using System;
using System.Collections.Generic;
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
    public class MainViewModel : ViewModelBase
    {
        //Campos
        private UserAccountModel _currentUserAcconut;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        private IUserRepository userRepository;

        //Propiedades
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

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
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

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(IconChar));
            }
        }

        //Comandos de interacción de usuario
        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowClientesViewCommand { get; }
        public ICommand ShowVehicleViewCommand { get; }
        public ICommand ShowAppointmentsViewCommand { get; }
        public ICommand ShowReportsViewCommand { get; }
        public ICommand ShowEmployeesViewCommand { get; }
        public ICommand ShowConfigViewCommand { get; }
        public ICommand ShowPartsViewCommand { get; }
        public ICommand ShowPaymentsViewCommand { get; }
        public ICommand ShowUserServicesViewCommand { get; }

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();

            //inicializar comandos
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowClientesViewCommand = new ViewModelCommand(ExecuteShowClientesViewCommand);
            ShowVehicleViewCommand = new ViewModelCommand(ExecuteShowVehicleViewCommand);
            ShowAppointmentsViewCommand = new ViewModelCommand(ExecuteShowAppointmentsViewCommand);
            ShowReportsViewCommand = new ViewModelCommand(ExecuteShowReportsViewCommand);
            ShowEmployeesViewCommand = new ViewModelCommand(ExecuteShowEmployeesViewCommand);
            ShowConfigViewCommand = new ViewModelCommand(ExecuteShowConfigViewCommand);
            ShowPartsViewCommand = new ViewModelCommand(ExecuteShowPartsViewCommand);
            ShowPaymentsViewCommand = new ViewModelCommand(ExecuteShowPaymentsViewCommand);
            ShowUserServicesViewCommand = new ViewModelCommand(ExecuteShowServicesViewCommand);


            //Vista Predeterminada
            ExecuteShowHomeViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteShowVehicleViewCommand(object obj)
        {
            CurrentChildView = new VehicleViewModel();
            Caption = "Vehículos";
            Icon = IconChar.Car;
        }

        private void ExecuteShowClientesViewCommand(object obj)
        {
            CurrentChildView = new ClientesViewModel();
            Caption = "Clientes";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Inicio";
            Icon = IconChar.Home;
        }
        private void ExecuteShowAppointmentsViewCommand(object obj)
        {
            CurrentChildView = new AppointmentsViewModel();
            Caption = "Agenda";
            Icon = IconChar.CalendarCheck;
        }
        private void ExecuteShowReportsViewCommand(object obj)
        {
            CurrentChildView = new ReportsViewModel();
            Caption = "Reportes";
            Icon = IconChar.ChartBar;
        }
        private void ExecuteShowEmployeesViewCommand(object obj)
        {
            CurrentChildView = new EmployeesViewModel();
            Caption = "Empleados";
            Icon = IconChar.Users;
        }
        private void ExecuteShowConfigViewCommand(object obj)
        {
            CurrentChildView = new ConfigViewModel();
            Caption = "Configuración";
            Icon = IconChar.Tools;
        }
        private void ExecuteShowPartsViewCommand(object obj)
        {
            CurrentChildView = new PartsViewModel();
            Caption = "Repuestos";
            Icon = IconChar.Box;
        }
        private void ExecuteShowPaymentsViewCommand(object obj)
        {
            CurrentChildView = new PaymentsViewModel();
            Caption = "Facturación/Pagos";
            Icon = IconChar.Wallet;
        }
        private void ExecuteShowServicesViewCommand(object obj)
        {
            CurrentChildView = new ServicesViewModel();
            Caption = "Solicitudes/Servicios";
            Icon = IconChar.TruckRampBox;
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

                    CurrentUserAccount.Rol = user.Rol;
                    IsAdmin = user.Rol == "Admin";

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
