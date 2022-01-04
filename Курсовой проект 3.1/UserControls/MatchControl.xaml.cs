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

namespace Курсовой_проект_3._1.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MatchControl.xaml
    /// </summary>
    public partial class MatchControl : UserControl
    {
        public string TeamIdFrst { get; set; }
        public string TeamNameFrst { get; set; }
        public string ScoreFrst { get; set; }
        public string TeamIdScnd { get; set; }
        public string TeamNameScnd { get; set; }
        public string ScoreScnd { get; set; }
        public string RoundNum { get; set; }

        public MatchControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
