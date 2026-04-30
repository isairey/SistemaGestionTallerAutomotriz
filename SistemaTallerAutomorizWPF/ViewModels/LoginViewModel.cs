using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SistemaTallerAutomorizWPF.Models;
using SistemaTallerAutomorizWPF.ViewModel;
using SistemaTallerAutomorizWPF.Repositories;
using System.Net;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Windows;

namespace SistemaTallerAutomorizWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        //Campos
        private String _userName;
        private SecureString _password;
        private String _errorMessage;
        private bool _isViewVisible = true;

        private IUserRepository userRepository;

        //Propiedades
        public String UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public SecureString Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public String ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        // -> Comandos
        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        //Constructor
        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p=> ExecuteRecoverPasswordCommand("",""));
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            bool ValidData;
            if (String.IsNullOrWhiteSpace(UserName)|| UserName.Length<3 ||
                Password==null ||Password.Length<3)
                ValidData = false;
            else
                ValidData = true;
            return ValidData;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var isValidUser = userRepository.AuthenticateUser(new NetworkCredential(UserName, Password));
            if (isValidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity(UserName), null);
                IsViewVisible = false;
            }
            else
            {
                ErrorMessage = "* Invalid username or password";
            }
        }
        private void ExecuteRecoverPasswordCommand(String username, String email)
        {
            throw new NotImplementedException();
        }

        public UserModel GetUserByUsername(string username)
        {
            UserModel user = null;

            using (SqlConnection connection = Connections.GetConnection())
            {
                string query = "SELECT Username, Name, Rol FROM [User] WHERE Username = @username";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", username);

                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            Username = reader["Username"].ToString(),
                            Name = reader["Name"].ToString(),
                            Rol = reader["Rol"].ToString()
                        };
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener usuario: " + ex.Message);
                }
            }

            return user;
        }
    }
}
