using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Курсовой_проект_3._1.Windows;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int nowUserId; // id открытого профиля
        MyTeamContext _context;

        // TeamsGrid
        List<PartInTeams> teams = new List<PartInTeams>(5);

        // MatchesGrid
        List<Match> matches = new List<Match>(5);

        public MainWindow(int userId)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowUserId = userId;
            int? nowTeamId = _context.TeamsUsers.Where(tuIn => tuIn.FK_User_Id == nowUserId && tuIn.DateEnd == null).Select(tuIn => tuIn.FK_Team_Id).FirstOrDefault();

            // адаптация к размеру окна
            SizeChanged += MainWindow_SizeChanged;

            // Доступность кнопки настроек
            if (nowUserId == App.UserId)
            {
                SettingsBtn.Visibility = Visibility.Visible;
            }

            // Подгрузка краткой информации
            Users user = _context.Users.Find(nowUserId);

            RealNameTB.Text += $"{user.LName} {user.FName} {user.MName}";
            NicknameTB.Text += user.Nickname;
            BirthDateTB.Text += user.Birthday.ToString("D") + " (" + Func.CalculateAge(user.Birthday).ToString() + " лет)";

            if (user.Countries != null)
            {
                CountryTB.Text += user.Countries.Name;
            }

            PhoneNumberTB.Text += "+" + user.PhoneNumber;
            EmailTB.Text += user.Email;

            if (user.About != null)
            {
                AboutMeTB.Text += user.About;
            }

            if (user.PhotoPath != null)
            {
                UserPhotoImg.Source = new BitmapImage(new Uri(user.PhotoPath));
            }

            if (user.Disciplines != null)
            {
                DisciplineIconImg.Source = new BitmapImage(new Uri(user.Disciplines.IconPath));
            }
            else
            {
                DisciplineIconImg.Source = new BitmapImage(new Uri(@"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\Default\discipline.png"));
            }

            // Подгрузка кубков и призовых
            if (nowTeamId != 0)
            {
                TeamTBlock.Text = "(" +_context.Teams.Find(nowTeamId).Name + ")";

                int teamMembersCount = _context.TeamsUsers.Where(tu => tu.FK_Team_Id == nowTeamId && tu.DateEnd == null).Count();
                decimal teamPrize = 0;
                if (_context.Achievements.Where(a => a.FK_Team_Id == nowTeamId).Select(a => a.Prize).Count() != 0)
                    teamPrize = _context.Achievements.Where(a => a.FK_Team_Id == nowTeamId).Select(a => a.Prize).Sum();

                PriceMoneyTB.Text = "Призовые: " + Convert.ToString(teamPrize / teamMembersCount) + "$";

                List<int> teamIds = Func.GetTeamIds(nowUserId);
                int frstCupCount = 0;
                int scndCupCount = 0;
                int thrdCupCount = 0;

                foreach(int teamId in teamIds)
                {
                    // Получаем даты нахождения в команде
                    var param = _context.TeamsUsers.Where(p => p.FK_User_Id == userId && p.FK_Team_Id == teamId).Select(p => new
                    {
                        DateStart = p.DateStart,
                        DateEnd = p.DateEnd
                    }).ToList();

                    DateTime dateStart = param[0].DateStart;
                    DateTime? dateEnd = param[0].DateEnd;

                    if (dateEnd == null)
                    {
                        dateEnd = Convert.ToDateTime("01/01/2099");
                    }

                    frstCupCount += _context.Achievements.Where(a => a.Place == "1" && a.FK_Team_Id == teamId && a.Tournaments.DateEnd >= dateStart && a.Tournaments.DateEnd >= dateEnd).Count();
                    scndCupCount += _context.Achievements.Where(a => a.Place == "2" && a.FK_Team_Id == teamId && a.Tournaments.DateEnd >= dateStart && a.Tournaments.DateEnd >= dateEnd).Count();
                    thrdCupCount += _context.Achievements.Where(a => a.Place == "3" && a.FK_Team_Id == teamId && a.Tournaments.DateEnd >= dateStart && a.Tournaments.DateEnd >= dateEnd).Count();
                }
                
                FirstPlaceCountTB.Text = ":" + Convert.ToString(frstCupCount);
                SecondPlaceCountTB.Text = ":" + Convert.ToString(scndCupCount);
                ThirdPlaceCountTB.Text = ":" + Convert.ToString(thrdCupCount);
            }

            // Подгрузка статистики
            var (winCount, drawCount, loseCount) = Func.GetWinDrawLoseCountUser(nowUserId);
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
                DrawPercentTB.Text = Math.Round(Convert.ToDouble(drawCount) * 100 / Convert.ToDouble(gamesCount), 2).ToString() + "%";
                LosePercentTB.Text = Math.Round(Convert.ToDouble(loseCount) * 100 / Convert.ToDouble(gamesCount), 2).ToString() + "%";
            }
            
            // Подгрузка команд
            foreach (int teamId in Func.GetTeamIds(userId))
            {
                teams.Add(GetPartInTeams(userId, teamId));
            }
            
            List<PartInTeams> subTeamsList = new List<PartInTeams>(5);
            if (teams.Count < 5)
            {
                TeamsGrid.ItemsSource = teams;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    subTeamsList.Add(teams[i]);
                }

                TeamsGrid.ItemsSource = subTeamsList;
            }


            // Подгрузка матчей
            foreach (Matches match in Func.GetAllMatchesUser(userId))
            {
                matches.Add(GetMatch(match));
            }

            List<Match> subMatchList = new List<Match>();
            if (matches.Count < 5)
            {
                MatchesGrid.ItemsSource = matches;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    subMatchList.Add(matches[i]);
                }

                MatchesGrid.ItemsSource = subMatchList;
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProfileDockPanel.MinHeight = MyWindow.Height * 4 / 10;
            ProfileDockPanel.MaxHeight = ProfileDockPanel.MinHeight + 100;
        }

        private void MoreGamesButton_Click(object sender, RoutedEventArgs e)
        {
            if (matches.Count - MatchesGrid.Items.Count != 0)
            {
                List<Match> subMatchList = new List<Match>();

                for (int i = 0; i <= matches.Count - MatchesGrid.Items.Count; i++)
                {
                    subMatchList.Add(matches[matches.Count + i]);
                }

                List<Match> newMatch = new List<Match>();
                newMatch.AddRange((List<Match>)MatchesGrid.ItemsSource);
                newMatch.AddRange(subMatchList);

                MatchesGrid.ItemsSource = new List<byte>();
                MatchesGrid.ItemsSource = newMatch;
            }
            else
            {
                MessageBox.Show("Больше матчей нет");
                return;
            }
        }

        private void MoreTeamsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (teams.Count - TeamsGrid.Items.Count != 0)
            {
                List<PartInTeams> subTeamsList = new List<PartInTeams>();

                for (int i = 0; i <= teams.Count - TeamsGrid.Items.Count; i++)
                {
                    subTeamsList.Add(teams[teams.Count + i]);
                }

                List<PartInTeams> newTeam = new List<PartInTeams>();
                newTeam.AddRange((List<PartInTeams>)TeamsGrid.ItemsSource);
                newTeam.AddRange(subTeamsList);

                TeamsGrid.ItemsSource = new List<byte>();
                TeamsGrid.ItemsSource = newTeam;
            }
            else
            {
                MessageBox.Show("Больше команд нет");
                return;
            }
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayerProfileSettingsWindow wnd = new PlayerProfileSettingsWindow();
            wnd.ShowDialog();
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

        private void TeamTBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TeamProfileWindow wnd = new TeamProfileWindow(_context.TeamsUsers.Where(tu => tu.FK_User_Id == nowUserId && tu.DateEnd == null).Select(tu => tu.FK_Team_Id).Single());
            wnd.Show();
            Close();
        }

        // ОБЪЕКТЫ ///
        // Возвращает объект класса PartInTeams
        private PartInTeams GetPartInTeams(int userId, int teamId)
        {
            // Получаем даты нахождения в команде
            var param = _context.TeamsUsers.Where(p => p.FK_User_Id == userId && p.FK_Team_Id == teamId).Select(p => new
            {
                DateStart = p.DateStart,
                DateEnd = p.DateEnd
            }).ToList();

            DateTime dateStart = param[0].DateStart;
            DateTime? dateEnd = param[0].DateEnd;

            // Создаем объект
            PartInTeams player = new PartInTeams();
            player.TeamName = Func.GetTeamName(teamId);
            player.DateOfStart = dateStart.ToString("d");
            player.DateOfEnd = dateEnd == null ? "наст. время" : Convert.ToDateTime(dateEnd).ToString("d");


            int winCount, drawCount, loseCount;
            (winCount, drawCount, loseCount) = Func.GetWinDrawLoseCountUserInOneTeam(userId, teamId);
            player.Stat = winCount.ToString() + " побед, " + drawCount.ToString() + " ничьих, " + loseCount.ToString() + " поражений";

            return player;
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
    }  // class MainWindow


    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string team = value[0] as string;
            string winTeam = value[1] as string;

            if (team == winTeam)
            {
                return Brushes.LightGreen;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class PartInTeams
    {
        public string TeamName { get; set; }
        public string Stat { get; set; }
        public string DateOfStart { get; set; }
        public string DateOfEnd { get; set; }
    }

    public class Match
    {
        public string FrstTeamName { get; set; }
        public string ScndTeamName { get; set; }
        public string Scores { get; set; }
        public string GameDate { get; set; }
        public string Tournament { get; set; }
        public string WinTeam { get; set; }
    }
}  // namespace
