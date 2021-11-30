using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        string connectionString;

        public RegistrationWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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

            // проверка на заполненость всех полей
            if (login == "" || password == "" || nickname == "" || fname == "" || lname == "" || mname == "" || birthday == "")
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                // проверка на существование такого же логина в БД
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                string sqlQuery = "SELECT * FROM Users WHERE Login = '" + login + "';";
                SqlCommand command = new SqlCommand(sqlQuery, connection);

                adapter.SelectCommand = command;
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Такой логин уже существует!");
                    LoginTB.Text = "";
                    return;
                }

                // добавление нового пользователя в БД
                string sqlQuery1 = "INSERT INTO Users (Login, Password, FName, LName, MName, Nickname, Birthday) VALUES(N'" + login + "', N'" + password + "', N'" + fname + "', N'" + lname + "', N'" + mname + "', N'" + nickname + "', '" + birthday + "')";
                SqlCommand command1 = new SqlCommand(sqlQuery1, connection);
                command1.ExecuteNonQuery();

                // возврат к авторизации
                MessageBox.Show("Регистрация прошла успешно!");
                Authorization wnd = new Authorization();
                wnd.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
