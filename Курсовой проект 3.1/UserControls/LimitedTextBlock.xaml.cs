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
    /// Логика взаимодействия для LimitedTextBlock.xaml
    /// </summary>
    public partial class LimitedTextBlock : UserControl
    {
        public LimitedTextBlock()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string TitleFontSize { get; set; }

        public string Title { get; set; }

        public int MaxLength { get; set; }
    }
}
