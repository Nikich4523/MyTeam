using System.Windows;
using System.Data;
using System;
using Курсовой_проект_3._1.Windows;
using System.Linq;
using System.Collections.Generic;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        MyTeamContext _context;

        public Authorization()
        {
            InitializeComponent();
            _context = new MyTeamContext();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Func.IsValidLogAndPass(LoginTB.Text, PasswordPB.Password))
            {
                List<Users> users;
                try
                {
                    users = _context.Users.Where(u => u.Login == LoginTB.Text && u.Password == PasswordPB.Password).ToList();
                }
                catch
                {
                    MessageBox.Show("Произошла непредвиденная ошибка!");
                    return;
                }

                if (users.Count() == 1)
                {
                    // переход в новое окно
                    App.UserId = users[0].Id;
                    ApplicationWindow wnd = new ApplicationWindow();
                    wnd.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Поля логин и пароль должны быть заполнены!");
                return;
            }
        }

        private void RegLink_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow wnd = new RegistrationWindow();
            wnd.Show();
            Close();
        }
    }
}
