using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Курсовой_проект_3._1.Windows;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для TeamProfileWindow.xaml
    /// </summary>
    public partial class TeamProfileWindow : Window
    {
        MyTeamContext _context;
        int nowTeamId;

        List<PlayerInTeamList> teamList = new List<PlayerInTeamList>();

        List<Match> matchList = new List<Match>();
        List<int> matchIds = new List<int>();
        int offsetMatch = 0;

        List<Achievement> achievementList = new List<Achievement>();
        List<int> tournamentIds = new List<int>();
        int offsetAchievment = 0;

        public TeamProfileWindow(int teamId)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTeamId = teamId;

            // Адаптация к размеру окна
            SizeChanged += MainWindow_SizeChanged;

            // Доступность доп. кнопок
            if (_context.TeamsUsers.Where(tu => tu.FK_User_Id == App.UserId && tu.FK_Team_Id == nowTeamId && tu.DateEnd == null).Count() > 0)
            {
                ExitBtn.Visibility = Visibility.Visible;
            }

            if (_context.Teams.Find(teamId).FK_Creater_Id == App.UserId)
            {
                AddPlayerBtn.Visibility = Visibility.Visible;
                SettingsBtn.Visibility = Visibility.Visible;
            }

            // Подгрузка краткой информации
            Teams team = _context.Teams.Where(t => t.Id == nowTeamId).ToList()[0];

            TeamNameTB.Text = team.Name;
            CountryTB.Text = "Страна: " + _context.Countries.Where(c => c.Id == team.FK_Country_Id).Select(c => c.Name).Single().ToString();
            FoundationDateTB.Text = "Дата основания: " + team.FoundationDate.ToString("D");

            PhoneNumberTB.Text = "Номер телефона: " + "+" + team.PhoneNumber;
            EmailTB.Text = "Email: " + team.Email;
            AboutTeamTB.Text = "О команде: " + team.About;

            // Подгрузка аватара
            if (team.LogoPath != null)
            {
                TeamLogoImg.Source = new BitmapImage(new Uri(team.LogoPath));
            }

            // Подгрузка кубков и призовых
            int frstCupCount = _context.Achievements.Where(a => a.FK_Team_Id == nowTeamId && a.Place == "1").Count();
            int scndCupCount = _context.Achievements.Where(a => a.FK_Team_Id == nowTeamId && a.Place == "2").Count();
            int thrdCupCount = _context.Achievements.Where(a => a.FK_Team_Id == nowTeamId && a.Place == "3").Count();
            decimal prize = 0;
            if (_context.Achievements.Where(a => a.FK_Team_Id == nowTeamId).Select(a => a.Prize).Count() != 0)
            {
                prize = _context.Achievements.Where(a => a.FK_Team_Id == nowTeamId).Select(a => a.Prize).Sum();
            }

            if (prize == 0)
            {
                PriceMoneyTB.Text = "Призовые: " + Convert.ToString(prize) + "$";
                FirstPlaceCountTB.Text = ":" + Convert.ToString(frstCupCount);
                SecondPlaceCountTB.Text = ":" + Convert.ToString(scndCupCount);
                ThirdPlaceCountTB.Text = ":" + Convert.ToString(thrdCupCount);
            }

            // Подгрузка статистики
            var (winCount, drawCount, loseCount) = Func.GetWinDrawLoseCountTeam(nowTeamId);
            int gamesCount = winCount + drawCount + loseCount;
            if (gamesCount != 0)
            {
                LineBarWin.Width = new GridLength(winCount * 100 / gamesCount, GridUnitType.Star);
                LineBarDraw.Width = new GridLength(drawCount * 100 / gamesCount, GridUnitType.Star);
                LineBarLose.Width = new GridLength(loseCount * 100 / gamesCount, GridUnitType.Star);

                WinCountTB.Text = winCount.ToString() + " побед";
                DrawCountTB.Text = drawCount.ToString() + " ничьих";
                LoseCountTB.Text = loseCount.ToString() + " поражений";

                WinPercentTB.Text = Math.Round(Convert.ToDouble(winCount) * 100.0 / Convert.ToDouble(gamesCount), 2).ToString() + "%";
                DrawPercentTB.Text = Math.Round(Convert.ToDouble(drawCount) * 100.0 / Convert.ToDouble(gamesCount), 2).ToString() + "%";
                LosePercentTB.Text = Math.Round(Convert.ToDouble(loseCount) * 100.0 / Convert.ToDouble(gamesCount), 2).ToString() + "%";
            }

            // Подгрузка состава -- происходит в NowStatusRB_Checked
            NowStatusRB.IsChecked = true;

            // Подгрузка истории матчей
            matchIds = Func.GetTeamMatchIds(nowTeamId);
            int offsetMatchInc = 0;
            if (matchIds.Count > 5)
            {
                for (int i = offsetMatch; i < 5; i++)
                {
                    int id = matchIds[i];
                    matchList.Add(GetMatch(_context.Matches.Where(m => m.Id == id).ToList()[0]));
                    offsetMatchInc++;
                }
            }
            else
            {
                for (int i = offsetMatch; i < matchIds.Count; i++)
                {
                    int id = matchIds[i];
                    matchList.Add(GetMatch(_context.Matches.Where(m => m.Id == id).ToList()[0]));
                    offsetMatchInc++;
                }
            }
            offsetMatch += offsetMatchInc;
            MatchesGrid.ItemsSource = matchList;

            // Подгрузка достижений
            tournamentIds = Func.GetTeamTournamentIds(nowTeamId);
            int offsetAchievementInc = 0;
            if (tournamentIds.Count > 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    achievementList.Add(GetAchievement(nowTeamId, tournamentIds[i]));
                    offsetAchievementInc++;
                }
            }
            else
            {
                for (int i = 0; i < tournamentIds.Count; i++)
                {
                    achievementList.Add(GetAchievement(nowTeamId, tournamentIds[i]));
                    offsetAchievementInc++;
                }
            }
            offsetAchievment += offsetAchievementInc;
            AchievementDataGrid.ItemsSource = achievementList;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProfileDockPanel.MinHeight = MyWindow.Height * 4 / 10;
            ProfileDockPanel.MaxHeight = ProfileDockPanel.MinHeight + 100;
        }

        private void NowStatusRB_Checked(object sender, RoutedEventArgs e)
        {
            TeamListStatusExpander.Header = NowStatusRB.Content;
            TeamListStatusExpander.IsExpanded = false;

            teamList = new List<PlayerInTeamList>();
            List<int> userIds = Func.GetPlayerIdsInTeamPresent(nowTeamId);
            foreach (int userId in userIds)
            {
                teamList.Add(GetPlayerInTeamList(userId));
            }
            TeamLB.ItemsSource = new List<int>();
            TeamLB.ItemsSource = teamList;
        }

        private void PastStatusRB_Checked(object sender, RoutedEventArgs e)
        {
            TeamListStatusExpander.Header = PastStatusRB.Content;
            TeamListStatusExpander.IsExpanded = false;

            teamList = new List<PlayerInTeamList>();
            List<int> userIds = Func.GetPlayerIdsInTeamPast(nowTeamId);
            foreach (int userId in userIds)
            {
                teamList.Add(GetPlayerInTeamList(userId));
            }
            TeamLB.ItemsSource = new List<int>();
            TeamLB.ItemsSource = teamList;
        }

        private void MoreAchievementBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tournamentIds.Count <= achievementList.Count)
            {
                MessageBox.Show("Больше достижений нет!");
                return;
            }

            int offsetInc = 0;
            if (tournamentIds.Count - achievementList.Count > 5)
            {
                for (int i = offsetAchievment; i < offsetAchievment + 5; i++)
                {
                    achievementList.Add(GetAchievement(nowTeamId, tournamentIds[i]));
                    offsetInc++;
                }
                offsetAchievment += offsetInc;
            }
            else
            {
                int achievementListCount = achievementList.Count;
                for (int i = offsetAchievment; i < tournamentIds.Count - achievementListCount + offsetAchievment; i++)
                {
                    achievementList.Add(GetAchievement(nowTeamId, tournamentIds[i]));
                }
            }
            AchievementDataGrid.ItemsSource = new List<Achievement>();
            AchievementDataGrid.ItemsSource = achievementList;
        }

        private void MoreGamesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (matchIds.Count == matchList.Count)
            {
                MessageBox.Show("Больше матчей нет!");
                return;
            }

            int offsetInc = 0;
            if (matchIds.Count - matchList.Count > 5)
            {
                for (int i = offsetMatch; i < offsetMatch + 5; i++)
                {
                    matchList.Add(GetMatch(_context.Matches.Where(m => m.Id == matchIds[i]).ToList()[0]));
                    offsetInc++;
                }
                offsetMatch += offsetInc;
            }
            else
            {
                int matchListCount = matchList.Count;
                for (int i = offsetMatch; i < matchIds.Count - matchListCount + offsetMatch; i++)
                {
                    matchList.Add(GetMatch(_context.Matches.Where(m => m.Id == matchIds[i]).ToList()[0]));
                }
            }
            MatchesGrid.ItemsSource = new List<Match>();
            MatchesGrid.ItemsSource = matchList;
        }

        private void AddPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            AddPlayerInTeamWindow wnd = new AddPlayerInTeamWindow();
            wnd.ShowDialog();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            TeamProfileSettingsWindow wnd = new TeamProfileSettingsWindow();
            wnd.ShowDialog();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            TeamsUsers teamUsersRow = _context.TeamsUsers.Where(tu => tu.FK_User_Id == App.UserId && tu.FK_Team_Id == nowTeamId && tu.DateEnd == null).FirstOrDefault();
            teamUsersRow.DateEnd = DateTime.Now;

            _context.SaveChanges();

            // переинициализируем окном
            TeamProfileWindow wnd = new TeamProfileWindow(nowTeamId);
            wnd.Show();
            Close();
        }

        private void ToMainBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplicationWindow wnd = new ApplicationWindow();
            wnd.Show();
            Close();
        }

        private void ToSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow wnd = new SearchWindow();
            wnd.ShowDialog();
        }

        private void ToProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow wnd = new MainWindow(App.UserId);
            wnd.Show();
            Close();
        }

        // Возвращает объект класса PlayerInTeamList
        private PlayerInTeamList GetPlayerInTeamList(int userId)
        {
            PlayerInTeamList player = new PlayerInTeamList();

            Users user = _context.Users.Find(userId);

            // имя
            player.RealName = $"{user.LName} {user.FName} {user.MName}";
            player.Nickname = user.Nickname;

            // фото игрока
            if (user.PhotoPath != null)
            {
                player.UserPhotoPath = user.PhotoPath;
            }
            else
            {
                player.UserPhotoPath = "C:/Users/nikich4523/source/repos/Курсовой проект 3.1/Images/WithoutPhoto.png";
            }

            // иконка страны
            if (user.Countries.IconPath != null)
            {
                player.CountryIconPath = user.Countries.IconPath;
            }
            else
            {
                player.CountryIconPath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\countrys - 24\_unknown.png";
            }

            // даты в команде
            string dateIn = _context.TeamsUsers.Where(tu => tu.FK_Team_Id == nowTeamId && tu.FK_User_Id == userId).Select(tu => tu.DateStart).ToList()[0].ToString("d");
            string dateOut = _context.TeamsUsers.Where(tu => tu.FK_Team_Id == nowTeamId && tu.FK_User_Id == userId).Select(tu => tu.DateEnd).ToList()[0] == null? "наст. время" : _context.TeamsUsers.Where(tu => tu.FK_Team_Id == nowTeamId && tu.FK_User_Id == userId).Select(tu => tu.DateStart).ToList()[0].ToString("d");
            player.Dates = dateIn  + " - " + dateOut;


            // статистика в команде
            int winCount, drawCount, loseCount;
            (winCount, drawCount, loseCount) = Func.GetWinDrawLoseCountUserInOneTeam(userId, nowTeamId);
            player.Statistick = winCount.ToString() + " побед, " + drawCount.ToString() + " ничьих, " + loseCount.ToString() + " поражений";

            return player;
        }

        // Возвращает объект класса Achievment
        private Achievement GetAchievement(int teamId, int tournamentId)
        {
            Achievement achievement = new Achievement();
            Tournaments tournament = _context.Tournaments.Where(t => t.Id == tournamentId).ToList()[0];


            string place = Func.GetPlaceInTournament(tournamentId, teamId);
            int prize = Func.GetPrizeInTournament(tournamentId, teamId);

            achievement.Tournament = tournament.Name;
            achievement.Venue = tournament.Venue + ",";
            achievement.DateOfVenue = tournament.DateStart.ToString("d") + " - " + tournament.DateStart.ToString("d");
            achievement.Place = place;
            achievement.Prize = prize.ToString();
            achievement.PrizeFond = tournament.PrizeFond.ToString();

            return achievement;
        }

        // Возвращает объект класса Match
        private Match GetMatch(Matches oldMatch)
        {
            Match match = new Match();
            match.FrstTeamName = Func.GetTeamName(oldMatch.FK_FrstTeam_Id);
            match.ScndTeamName = Func.GetTeamName(oldMatch.FK_ScndTeam_Id);
            match.Scores = oldMatch.FrstScore + " : " + oldMatch.ScndScore;
            match.Tournament = Func.GetTournamentName(oldMatch.FK_Tournament_Id);
            match.GameDate = oldMatch.GameDate.ToString("D");

            if (oldMatch.FK_WinnerTeam_Id != 1)
            {
                if (oldMatch.FK_WinnerTeam_Id == oldMatch.FK_FrstTeam_Id)
                {
                    match.WinTeam = match.FrstTeamName;
                }
                else
                {
                    match.WinTeam = match.ScndTeamName;
                }
            }

            return match;
        }
    }

    // Класс, описывающий объект/элемент ListBox - TeamListGroupBox
    public class PlayerInTeamList
    {
        public string UserPhotoPath { get; set; }
        public string CountryIconPath { get; set; }
        public string Nickname { get; set; }
        public string RealName { get; set; }
        public string Dates { get; set; }
        public string Statistick { get; set; }
    }

    // Класс, описывающий объект/элемент DataGrid - AchievementListGroupBox
    public class Achievement
    {
        public string Place { get; set; }
        public string Tournament { get; set; }
        public string Venue { get; set; }
        public string DateOfVenue { get; set; }
        public string Prize { get; set; }
        public string PrizeFond { get; set; }
    }
}