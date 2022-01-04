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
    /// Логика взаимодействия для CreateTeamWindow.xaml
    /// </summary>
    public partial class CreateTeamWindow : Window
    {
        MyTeamContext _context;
        bool isPhotoChanged;

        public CreateTeamWindow()
        {
            InitializeComponent();
            isPhotoChanged = false;
            _context = new MyTeamContext();

            PhoneNumberTBox.Text = "+" + _context.Users.Find(App.UserId).PhoneNumber;
            EmailTBox.Text = _context.Users.Find(App.UserId).Email;
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            // инициализируем переменные
            string teamName = TeamNameTBox.Text;
            string email = EmailTBox.Text;
            string phoneNumber = PhoneNumberTBox.Text.Trim('+');
            string about = AboutTBox.txtLimitedInput.Text;

            // проверка на заполнение всех полей
            if (string.IsNullOrWhiteSpace(teamName))
            {
                MessageBox.Show("Название команды должно быть заполнено!");
                return;
            }

            if (email != String.Empty)
            {
                if (!Func.IsValidEmail(email))
                {
                    MessageBox.Show("Некорректный Email!");
                    return;
                }
            }

            if (phoneNumber != String.Empty)
            {
                if (!Func.IsValidPhoneNumber(phoneNumber))
                {
                    MessageBox.Show("Некорректный номер телефона!");
                    return;
                }
            }

            // проверка на уникальность email
            if (_context.Teams.Where(t => t.Email == email).ToList().Count() != 0)
            {
                MessageBox.Show("Email занят!");
                return;
            }

            // проверка, на уникальность номера телефона
            if (_context.Teams.Where(t => t.PhoneNumber == phoneNumber).ToList().Count() != 0)
            {
                MessageBox.Show("Номер телефона занят!");
                return;
            }

            // проверка, есть ли команда с таким названием
            if (_context.Teams.Where(t => t.Name == teamName).ToList().Count() != 0)
            {
                MessageBox.Show("Команда с таким названием уже существует!");
                return;
            }

            // создаем новый объект Teams
            Teams team = new Teams();
            team.Name = teamName;
            team.FoundationDate = DateTime.Now;
            team.FK_Creater_Id = App.UserId;
            team.FK_Country_Id = _context.Users.Find(App.UserId).Countries.Id;
            
            if (!string.IsNullOrWhiteSpace(email))
            {
                team.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                team.PhoneNumber = phoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(about))
            {
                team.About = about;
            }

            if (isPhotoChanged)
            {
                team.LogoPath = LogoImg.Source.ToString();
            }

            // создаем новый объект TeamsUsers
            TeamsUsers teamUser = new TeamsUsers();
            teamUser.FK_Team_Id = team.Id;
            teamUser.FK_User_Id = App.UserId;
            teamUser.DateStart = DateTime.Now;

            _context.Teams.Add(team);
            _context.TeamsUsers.Add(teamUser);
            _context.SaveChanges();

            TeamProfileWindow tWnd = new TeamProfileWindow(team.Id);
            tWnd.Show();

            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is ApplicationWindow)
                {
                    ApplicationWindow appWnd = (ApplicationWindow)wnd;
                    appWnd.Close();
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
                isPhotoChanged = true;
            }
        }
    }
}
