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
using System.Windows.Shapes;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для ApplicationWindow.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        public ApplicationWindow()
        {
            InitializeComponent();

            List<Country> countryList = new List<Country>
            {
                new Country{ ItemName="Russia"},
                new Country{ ItemName="Ukraine"},
                new Country{ ItemName="USA"},
                new Country{ ItemName="Canada"},
                new Country{ ItemName="Kazahstan"}
            };
            CountryFilterLB.ItemsSource = countryList;
        }

        private void PlayerListShowButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("kek");
        }
    }

    //
    public class Country
    {
        public string ItemName { get; set;}
    }
}
