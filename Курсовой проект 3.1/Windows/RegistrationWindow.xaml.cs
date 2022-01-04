using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        MyTeamContext _context;

        public RegistrationWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
        }

        private void AuthLink_Click(object sender, RoutedEventArgs e)
        {
            Authorization wnd = new Authorization();
            wnd.Show();
            Close();
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text.Trim();
            string password = PasswordPB.Password.Trim();
            string nickname = NicknameTB.Text.Trim();
            string fname = FNameTB.Text.Trim();
            string lname = LNameTB.Text.Trim();
            string mname = MNameTB.Text.Trim();
            string birthday = BirthdayDP.Text;
            string email = EmailTBox.Text.Trim();
            string phoneNumber = PhoneNumberTBox.Text.Trim().TrimStart('+'); 
            
           
            // проверка на заполненость всех полей
            if (Func.IsNullOrWhiteSpace(new string[] { login, password, nickname, fname, lname, mname, birthday }))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }

            // проверка на уникальность полей
            if (_context.Users.Where(user => user.Login == login).Count() > 0)
            {
                MessageBox.Show("Такой логин уже существует!");
                LoginTB.Text = "";
                return;
            }

            if (_context.Users.Where(user => user.Nickname == nickname).Count() > 0)
            {
                MessageBox.Show("Такой никнейм уже существует!");
                NicknameTB.Text = "";
                return;
            }

            if (_context.Users.Where(user => user.PhoneNumber == phoneNumber.ToString()).Count() > 0)
            {
                MessageBox.Show("Такой номер телефона уже зарегистрирован!");
                NicknameTB.Text = "";
                return;
            }

            if (_context.Users.Where(user => user.Email == email).Count() > 0)
            {
                MessageBox.Show("Такой Email уже зарегистрирован!");
                NicknameTB.Text = "";
                return;
            }

            // проверка на корректность
            if (!Func.IsValidPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Некорректный номер телефона!");
                return;
            }

            if (!Func.IsValidEmail(email))
            {
                MessageBox.Show("Некорректный Email!");
                return;
            }

            // добавление нового пользователя в БД
            Users newUser = new Users();
            newUser.Login = login;
            newUser.Password = password;
            newUser.FName = fname;
            newUser.LName = lname;
            newUser.MName = mname;
            newUser.Nickname = nickname;
            newUser.Birthday = Convert.ToDateTime(birthday);
            newUser.Email = email;
            newUser.PhoneNumber = phoneNumber;

            _context.Users.Add(newUser);
            _context.SaveChanges();

            // возврат к авторизации
            MessageBox.Show("Регистрация прошла успешно!");
            Authorization wnd = new Authorization();
            wnd.Show();
            Close();
        }
    }
}
