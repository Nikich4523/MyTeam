using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace Курсовой_проект_3._1.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPlayerInTeamWindow.xaml
    /// </summary>
    public partial class AddPlayerInTeamWindow : Window
    {
        MyTeamContext _context;

        public AddPlayerInTeamWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            SearchResultLB.ItemsSource = new List<int>();

            if (e.Key == Key.Enter)
            {
                List<PlayerApps> playerAppList = _context.PlayerApps.Where(u => DbFunctions.Like(u.Users.Nickname, "%" + SearchTBox.Text + "%")
                || DbFunctions.Like(u.Users.LName + " " + u.Users.FName + " " + u.Users.MName, "%" + SearchTBox.Text + "%")).ToList();

                List<PlayerItem> playerItemList = new List<PlayerItem>();
                foreach (PlayerApps app in playerAppList)
                {
                    PlayerItem item = new PlayerItem();
                    item.Name = $"{app.Users.LName} {app.Users.FName} {app.Users.MName}";
                    item.Nickname = app.Users.Nickname;
                    item.PhotoPath = app.Users.PhotoPath;
                    item.CountryName = app.Users.Countries.Name;
                    item.CountryIconPath = app.Users.Countries.IconPath;
                        
                    if (app.Users.Disciplines != null)
                        item.DisciplineName = app.Users.Disciplines.Name;

                    item.UserId = app.Users.Id;

                    playerItemList.Add(item);
                }

                SearchResultLB.ItemsSource = playerItemList;
            }
        }

        private void SearchResultLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchResultLB.SelectedItem != null)
            {
                PlayerItem selectedUser = (PlayerItem)SearchResultLB.SelectedItem;

                if (_context.TeamsUsers.Where(tu => tu.FK_User_Id == selectedUser.UserId && tu.DateEnd == null).Count() != 0)
                {
                    MessageBox.Show("Игрок уже состоит в другой команде!");
                    return;
                }

                // создаем новую запись
                TeamsUsers newTeamUsersRow = new TeamsUsers();
                newTeamUsersRow.FK_Team_Id = _context.Teams.Where(t => t.FK_Creater_Id == App.UserId).Select(t => t.Id).FirstOrDefault();
                newTeamUsersRow.FK_User_Id = selectedUser.UserId;
                newTeamUsersRow.DateStart = DateTime.Now;
                newTeamUsersRow.DateEnd = null;

                _context.TeamsUsers.Add(newTeamUsersRow);

                _context.SaveChanges();

                MessageBox.Show("Игрок добавлен в команду!");
            }
        }
    }
}
