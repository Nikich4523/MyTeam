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

namespace Курсовой_проект_3._1.Windows
{
    /// <summary>
    /// Логика взаимодействия для PlayerApplicationAddWindow.xaml
    /// </summary>
    public partial class PlayerApplicationAddWindow : Window
    {
        MyTeamContext _context;

        public PlayerApplicationAddWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleLimTBox.txtLimitedInput.Text;
            string text = TextLimTBox.txtLimitedInput.Text;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }
            else
            {
                PlayerApps app = new PlayerApps();
                app.Title = title;
                app.AppText = text;
                app.AppDate = DateTime.Now;

                app.FK_User_Id = App.UserId;

                List<PlayerApps> apps = _context.PlayerApps.Where(a => a.FK_User_Id == App.UserId).ToList();
                if (apps.Count() != 0)
                {
                    apps[0].Title = title;
                    apps[0].AppText = text;
                    apps[0].AppDate = DateTime.Now;

                    MessageBox.Show("Предыдущая заявка изменена!");
                }
                else
                {
                    _context.PlayerApps.Add(app);
                    MessageBox.Show("Заявка добавлена!");
                }
            }

            _context.SaveChanges();
        }
    }
}
