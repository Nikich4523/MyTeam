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
    /// Логика взаимодействия для PlayerProfileSettingsWindow.xaml
    /// </summary>
    public partial class PlayerProfileSettingsWindow : Window
    {
        MyTeamContext _context;
        Users user;
        bool isPhotoChanged;

        public PlayerProfileSettingsWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
            isPhotoChanged = false;

            List<string> countryList = _context.Countries.Select(c => c.Name).ToList();
            CountryCB.ItemsSource = countryList;

            user = _context.Users.Find(App.UserId);
            EmailTBox.Text = user.Email;
            PhoneNumberTBox.Text = "+" + user.PhoneNumber;
            CountryCB.Text = user.Countries.Name;
            AboutTBox.txtLimitedInput.Text = user.About;
            UserPhoto.Source = new BitmapImage(new Uri(user.PhotoPath));
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            string email, phoneNumber, selectedCountry;

            // проверка на корректность введенной почты
            if (!string.IsNullOrWhiteSpace(EmailTBox.Text))
            {
                email = EmailTBox.Text;
                string nowEmailUser = _context.Users.Find(App.UserId).Email;

                if (email != nowEmailUser)
                {
                    if (_context.Users.Where(u => u.Email == email).Count() == 0)
                    {
                        if (Func.IsValidEmail(email))
                        {
                            user.Email = email;
                        }
                        else
                        {
                            MessageBox.Show("Некорректный формат Email!");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Такой Email уже зарегистриован!");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Email должен быть заполнен!");
                return;
            }

            // проверка на корректность введенного номера телефона
            if (!string.IsNullOrWhiteSpace(PhoneNumberTBox.Text))
            {
                phoneNumber = PhoneNumberTBox.Text.TrimStart('+');
                string nowPhoneNumberUser = _context.Users.Find(App.UserId).PhoneNumber;

                if (phoneNumber != nowPhoneNumberUser)
                {

                    if (_context.Users.Where(u => u.PhoneNumber == phoneNumber).Count() == 0)
                    {
                        if (Func.IsValidPhoneNumber(phoneNumber))
                        {
                            user.PhoneNumber = phoneNumber;
                        }
                        else
                        {
                            MessageBox.Show("Некорректный формат номера телефона!");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Такой номер телефона уже зарегистриован!");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Номер телефона должен быть заполнен!");
                return;
            }

            // проверка на корректность страны
            if (!string.IsNullOrWhiteSpace(CountryCB.Text))
            {
                selectedCountry = CountryCB.Text;
                user.FK_Country_Id = _context.Countries.Where(c => c.Name == selectedCountry).Select(c => c.Id).Single();
            }

            // проверка на корректность поля о себе
            user.About = AboutTBox.txtLimitedInput.Text;

            // если фото изменено
            if (isPhotoChanged)
            {
                user.PhotoPath = UserPhoto.Source.ToString();
            }

            _context.SaveChanges();

            MessageBox.Show("Изменения успешно применены!");
        }

        private void ChangePhotoBtn_Click(object sender, RoutedEventArgs e)
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
                UserPhoto.Source = new BitmapImage(new Uri(filename));
                isPhotoChanged = true;
            }
        }
    }
}
