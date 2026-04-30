using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SistemaTallerAutomorizWPF.ViewModels;

namespace SistemaTallerAutomorizWPF.View
{
    /// <summary>
    /// Lógica de interacción para PartsView.xaml
    /// </summary>
    public partial class PartsView : UserControl
    {
        private PartsViewModel viewModel;

        public PartsView()
        {
            InitializeComponent();
            viewModel = new PartsViewModel();
            DataContext = viewModel;
            viewModel.CargarOrdenes();
        }

        private void BuscarBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.FiltrarOrdenes(BuscarTextBox.Text);
        }

        private void BuscarTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.FiltrarOrdenes(BuscarTextBox.Text);
        }
    }
}
