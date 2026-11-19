using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppSistemaDeLoja
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            PageTitleText.Text = "Visão geral";
            BreadcrumbText.Text = "PÁGINAS  /  VISÃO GERAL";
            DashboardContent.Visibility = Visibility.Visible;
            DashboardButton.Background = new SolidColorBrush(Color.FromRgb(38, 59, 89));
            DashboardButton.Foreground = Brushes.White;
        }

        private void Navigation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not string section)
                return;

            MessageBox.Show(
                $"A área de {section.ToLowerInvariant()} será implementada na próxima etapa.\\n\\n" +
                "A navegação já está configurada; em seguida vamos conectar esta tela às entidades do projeto.",
                section,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("O formulário de novo pedido será desenvolvido na próxima etapa.",
                "Novo pedido", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A listagem de pedidos será desenvolvida na próxima etapa.",
                "Pedidos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}