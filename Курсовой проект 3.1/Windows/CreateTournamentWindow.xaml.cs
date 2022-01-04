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
    /// Логика взаимодействия для CreateTournamentWindow.xaml
    /// </summary>
    public partial class CreateTournamentWindow : Window
    {
        MyTeamContext _context;

        public CreateTournamentWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            // проверка, есть ли ещё незаконченные турниры от этого пользователя
            if (_context.Tournaments.Where(t => t.FK_Organizer_Id == App.UserId).ToList().Count() != 0)
            {
                MessageBox.Show("У вас ещё есть незаконченные турниры!");
                return;
            }

            // инициализируем переменные
            string tournamentName = TournamentNameTBox.Text;
            string prizeFondstr = PrizeFondTBox.Text.Trim();
            string teamsCountstr = TeamCountCB.Text.Trim();
            string venue = VenueTBox.Text;
            string dataStart = DateOfStartDP.Text;
            string dataEnd = DateOfEndDP.Text;


            // проверка на заполнение всех полей
            if (string.IsNullOrWhiteSpace(tournamentName) || !Func.IsOnlyDigitString(prizeFondstr) || !Func.IsOnlyDigitString(teamsCountstr)
                || string.IsNullOrWhiteSpace(venue) || string.IsNullOrWhiteSpace(dataStart) || string.IsNullOrWhiteSpace(dataEnd))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }

            int prizeFond = Convert.ToInt32(prizeFondstr);
            int teamsCount = Convert.ToInt32(teamsCountstr);

            // проверка, есть ли турниры с таким названием
            if (_context.Tournaments.Where(t => t.Name == tournamentName).ToList().Count() != 0)
            {
                MessageBox.Show("У вас ещё есть незаконченные турниры!");
                return;
            }

            // создаем новый объект Tournaments
            Tournaments tournament = new Tournaments();
            tournament.Name = tournamentName;
            tournament.PrizeFond = prizeFond;
            tournament.MaxTeamsCount = teamsCount;
            tournament.DateStart = Convert.ToDateTime(dataStart);
            tournament.DateEnd = Convert.ToDateTime(dataEnd);
            tournament.Venue = venue;
            tournament.FK_Organizer_Id = App.UserId;
            tournament.FK_Format_Id = 1;

            tournament.LogoPath = LogoImg.Source.ToString();

            _context.Tournaments.Add(tournament);
            _context.SaveChanges();

            int tournamentId = _context.Tournaments.Where(t => t.Name == tournamentName).Select(t => t.Id).Single();

            TournamentWindow tWnd = new TournamentWindow(tournamentId);
            tWnd.Show();

            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is ApplicationWindow)
                {
                    ApplicationWindow appWnd = (ApplicationWindow)wnd;
                    appWnd.Close();
                    break;
                }
            }

            Close();
        }

        private void ChangeLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            // Создаем OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Устанавливаем фильтры и стандартное расширение файла
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Отображаем OpenFileDialog
            Nullable<bool> result = dlg.ShowDialog();

            // Получаем и устанавливаем новое изображение 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                LogoImg.Source = new BitmapImage(new Uri(filename));
            }
        }
    }
}
