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

namespace GUI_RDF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Main_Get_Abstract_Click(object sender, RoutedEventArgs e)
        {
            Get_Abstract new_abstract = new Get_Abstract();
            new_abstract.Show();
            this.Close();

        }

        private void Main_Informations_Click(object sender, RoutedEventArgs e)
        {
            Informations new_informations = new Informations();
            new_informations.Show();
            this.Close();
        }

        private void Main_Navigation_Click(object sender, RoutedEventArgs e)
        {
            Navigation new_navigation = new Navigation();
            new_navigation.Show();
            this.Close();
        }

        private void Main_Relations_Click(object sender, RoutedEventArgs e)
        {
            Relations new_relation = new Relations();
            new_relation.Show();
            this.Close();
        }

        private void Main_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
