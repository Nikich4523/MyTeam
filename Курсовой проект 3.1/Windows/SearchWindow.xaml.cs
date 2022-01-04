using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace Курсовой_проект_3._1.Windows
{
    /// <summary>
    /// Логика взаимодействия для SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        MyTeamContext _context;
        List<PlayerItem> playerList;

        public SearchWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
            playerList = new List<PlayerItem>();
            playerRB.IsChecked = true;
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            SearchResultLB.ItemsSource = new List<int>();

            if (e.Key == Key.Enter)
            {
                if ((bool)playerRB.IsChecked)
                {
                    List<Users> userList = _context.Users.Where(u => DbFunctions.Like(u.Nickname, "%" + SearchTBox.Text + "%")
                    || DbFunctions.Like(u.LName + " " + u.FName + " " + u.MName, "%" + SearchTBox.Text + "%")).ToList();

                    List<PlayerItem> playerItemList = new List<PlayerItem>();
                    foreach (Users user in userList)
                    {
                        PlayerItem item = new PlayerItem();
                        item.Name = $"{user.LName} {user.FName} {user.MName}";
                        item.Nickname = user.Nickname;

                        if (user.PhotoPath != null)
                        {
                            item.PhotoPath = user.PhotoPath;
                        }
                        else
                        {
                            item.PhotoPath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Images\WithoutPhoto.png";
                        }

                        if (user.FK_Country_Id != null)
                        {
                            item.CountryName = user.Countries.Name;
                            item.CountryIconPath = user.Countries.IconPath;
                        }
                        else
                        {
                            item.CountryName = "Не указана";
                            item.CountryIconPath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\countrys - 24\_unknown.png";
                        }
                        
                        if (user.FK_Discipline_Id != null)
                        {
                            item.DisciplineName = user.Disciplines.Name;
                        }
                        else
                        {
                            item.DisciplineName = "Не указана";
                        }

                        item.UserId = user.Id;

                        playerItemList.Add(item);
                    }

                    SearchResultLB.ItemsSource = playerItemList;
                }
                else if ((bool)teamRB.IsChecked)
                {
                    List<Teams> teamList = _context.Teams.Where(t => DbFunctions.Like(t.Name, "%" + SearchTBox.Text + "%")).ToList();

                    List<TeamItem> teamItemList = new List<TeamItem>();
                    foreach (Teams team in teamList)
                    {
                        TeamItem item = new TeamItem();
                        item.Name = team.Name;
                        item.FoundationDate = team.FoundationDate.ToString("d");

                        DateTime dissolationDate;
                        if (DateTime.TryParse(team.DissolationDate.ToString(), out dissolationDate))
                        {
                            item.DissolationDate = dissolationDate.ToString("d");
                        }
                        else
                        {
                            item.DissolationDate = "не распущена.";
                        }

                        item.LogoPath = team.LogoPath;
                        item.CountryName = team.Countries.Name;
                        item.CountryIconPath = team.Countries.IconPath;
                        if (_context.Users.Find(team.FK_Creater_Id).Disciplines != null)
                            item.DisciplineName = _context.Users.Find(team.FK_Creater_Id).Disciplines.Name;

                        item.TeamId = team.Id;

                        teamItemList.Add(item);
                    }

                    SearchResultLB.ItemsSource = teamItemList;
                }
                else
                {
                    List<Tournaments> tournamentList = _context.Tournaments.Where(t => DbFunctions.Like(t.Name, "%" + SearchTBox.Text + "%")).ToList();

                    List<TournamentItem> tournamentItemList = new List<TournamentItem>();
                    foreach (Tournaments tournament in tournamentList)
                    {
                        TournamentItem item = new TournamentItem();
                        item.Name = tournament.Name;
                        item.LogoPath = tournament.LogoPath;
                        item.DateStart = tournament.DateStart.ToString("d");
                        item.DateEnd = tournament.DateEnd.ToString("d");
                        item.Format = tournament.TournamentFormats.Name;
                        if (tournament.Users.Disciplines != null)
                            item.DisciplineName = tournament.Users.Disciplines.Name;

                        item.TournamentId = tournament.Id;

                        tournamentItemList.Add(item);
                    }

                    SearchResultLB.ItemsSource = tournamentItemList;
                }
            }
        }

        private void SearchResultLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchResultLB.SelectedItem != null)
            {
                if ((bool)playerRB.IsChecked)
                {
                    PlayerItem user = (PlayerItem)SearchResultLB.SelectedItem;

                    MainWindow mainWnd = new MainWindow(user.UserId);
                    mainWnd.Show();

                    // закрываем остальные окна
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
                else if ((bool)teamRB.IsChecked)
                {
                    TeamItem team = (TeamItem)SearchResultLB.SelectedItem;

                    TeamProfileWindow teamWnd = new TeamProfileWindow(team.TeamId);
                    teamWnd.Show();

                    // закрываем остальные окна
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
                else
                {
                    TournamentItem tournament = (TournamentItem)SearchResultLB.SelectedItem;

                    TournamentWindow teamWnd = new TournamentWindow(tournament.TournamentId);
                    teamWnd.Show();

                    // закрываем остальные окна
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
            }
        }

        private void playerRB_Checked(object sender, RoutedEventArgs e)
        {
            SearchResultLB.ItemsSource = new List<int>();
        }

        private void teamRB_Checked(object sender, RoutedEventArgs e)
        {
            SearchResultLB.ItemsSource = new List<int>();
        }

        private void tournamentRB_Checked(object sender, RoutedEventArgs e)
        {
            SearchResultLB.ItemsSource = new List<int>();
        }
    }

    class PlayerItem
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string PhotoPath { get; set; }
        public string CountryName { get; set; }
        public string CountryIconPath { get; set; }
        public string DisciplineName { get; set; }
        public int UserId { get; set; }
    }

    class TeamItem
    {
        public string Name { get; set; }
        public string FoundationDate { get; set; }
        public string DissolationDate { get; set; }
        public string LogoPath { get; set; }
        public string CountryName { get; set; }
        public string CountryIconPath { get; set; }
        public string DisciplineName { get; set; }
        public int TeamId { get; set; }
    }

    class TournamentItem
    {
        public string Name { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string LogoPath { get; set; }
        public string DisciplineName { get; set; }
        public string Format { get; set; }
        public int TournamentId { get; set; }
    }
}
