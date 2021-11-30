using System.Windows;
using System.Data;
using System.Configuration;
using System;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public int mainUserId;
        SQL query;

        public Authorization()
        {
            InitializeComponent();
            TeamProfileWindow wnd = new TeamProfileWindow("1");
            wnd.ShowDialog();
            Close();

            // получаем строку подключения из app.config
            query = new SQL("DefaultConnection");
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTB.Text != "" || PasswordPB.Password != "")
            {
                string login = "'" + LoginTB.Text + "'";
                string password = "'" + PasswordPB.Password + "'";

                DataTable dt = new DataTable();
                dt = query.SendSelectQuery("SELECT * FROM Users WHERE Login = " + login + "AND Password = " + password);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Вход выполнен");

                    // переход в новое окно
                    mainUserId = Convert.ToInt32(dt.Rows[0][0]);
                    MainWindow wnd = new MainWindow(mainUserId);
                    wnd.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Такого пользователя не существует!");
                }
            }
            else
            {
                MessageBox.Show("Поля логин и пароль должны быть заполнены!");
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
