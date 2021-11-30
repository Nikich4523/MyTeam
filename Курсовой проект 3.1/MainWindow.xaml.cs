using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int userId; // id открытого профиля
        SQL query;

        // TeamsGrid
        List<PartInTeams> teams = new List<PartInTeams>(5);
        int offsetTeams = 0;
        int offsetNextFetchTeams = 5;

        // MatchesGrid
        List<Match> matches = new List<Match>(5);
        int offsetMatches = 0;
        int offsetNextFetchMatches = 5;

        public MainWindow(int userId)
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            this.userId = userId;

            // адаптация к размеру окна
            SizeChanged += MainWindow_SizeChanged;

            // указываем строку подключения для запросов
            query = new SQL("DefaultConnection");

            // Подгрузка краткой информации
            dt = query.SendSelectQuery("SELECT " +
                                            "CONCAT(LName, ' ', FName, ' ', MName)," +
                                            "Nickname," +
                                            "Birthday," +
                                            "(0 + Convert(Char(8), GETDATE(), 112) - Convert(Char(8), Birthday, 112)) / 10000," +
                                            "Country," +
                                            "PhoneNumber," +
                                            "Email," +
                                            "AboutShort," +
                                            "PhotoURL," + //
                                            "TeamId " +
                                            "FROM Users WHERE UserId = " + Convert.ToString(this.userId));

            RealNameTB.Text += Convert.ToString(dt.Rows[0][0]);
            NicknameTB.Text += Convert.ToString(dt.Rows[0][1]);
            BirthDateTB.Text += Convert.ToDateTime(dt.Rows[0][2]).ToString("D") + " (" + Convert.ToString(dt.Rows[0][3]) + " лет)";
            CountryTB.Text += Convert.ToString(dt.Rows[0][4]);
            PhoneNumberTB.Text += Convert.ToString(dt.Rows[0][5]);
            EmailTB.Text += Convert.ToString(dt.Rows[0][6]);
            AboutMeTB.Text += Convert.ToString(dt.Rows[0][7]);

            // Подгрузка статистики
            var (winCount, drawCount, loseCount) = GetWinDrawLoseCount(Convert.ToString(this.userId));
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
            int offsetTeamsInc = 0;
            foreach (string teamId in GetTeamIds(Convert.ToString(this.userId), Convert.ToString(offsetTeams), Convert.ToString(offsetNextFetchTeams)))
            {
                teams.Add(GetPartInTeams(Convert.ToString(this.userId), teamId));
                offsetTeamsInc++;
            }
            offsetTeams += offsetTeamsInc;
            TeamsGrid.ItemsSource = teams;

            // Подгрузка матчей
            int offsetMatchesInc = 0;
            foreach (DataRow dr in GetAllMatchesUser(Convert.ToString(this.userId), Convert.ToString(offsetMatches), Convert.ToString(offsetNextFetchMatches)).Rows)
            {
                matches.Add(GetMatch(dr));
                offsetMatchesInc++;
            }
            offsetMatches += offsetMatchesInc;
            MatchesGrid.ItemsSource = matches;

            // Если зашел главные пользователь, то добавить кнопку настроек
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProfileDockPanel.MinHeight = MyWindow.Height * 4 / 10;
            ProfileDockPanel.MaxHeight = ProfileDockPanel.MinHeight + 100;
        }

        private void MoreGamesButton_Click(object sender, RoutedEventArgs e)
        {
            int offsetMatchesInc = 0;
            DataTable dt = GetAllMatchesUser(Convert.ToString(this.userId), Convert.ToString(offsetMatches), Convert.ToString(offsetNextFetchMatches));

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    matches.Add(GetMatch(dr));
                    offsetMatchesInc++;
                }
                offsetMatches += offsetMatchesInc;
                MatchesGrid.ItemsSource = new List<Match>(1);
                MatchesGrid.ItemsSource = matches;
            }
            else
            {
                MessageBox.Show("Больше матчей нет");
                return;
            }
        }

        private void MoreTeamsBtn_Click(object sender, RoutedEventArgs e)
        {
            int offsetTeamsInc = 0;
            List<string> teamIds = GetTeamIds(Convert.ToString(this.userId), Convert.ToString(offsetTeams), Convert.ToString(offsetNextFetchTeams));

            try
            {
                string check = teamIds[0];
                foreach (string teamId in teamIds)
                {
                    teams.Add(GetPartInTeams(Convert.ToString(this.userId), teamId));
                    offsetTeamsInc++;
                }
                offsetTeams += offsetTeamsInc;
                TeamsGrid.ItemsSource = teams;
            }
            catch
            {
                MessageBox.Show("Больше матчей нет");
                return;
            }
        }

        // МАТЧИ ///
        // Возвращает все матчи игрока в команде
        private DataTable GetMatchesUserInTeam(string userId, string teamId)
        {
            // Получаем даты нахождения в команде
            string queryText = "SELECT DateIn, DateOut FROM TeamCompositions WHERE UserId = " + userId + " AND TeamId = " + teamId;
            DataTable dt = query.SendSelectQuery(queryText);
            string dateIn = dt.Rows[0][0].ToString();
            string dateOut = dt.Rows[0][1].ToString();

            // Получаем все матчи игрока в одной команде
            queryText = "SELECT TeamIdFrst, TeamIdScnd, IsFrstWin, GameDate, TournamentID, FrstScores, ScndScores FROM Matches WHERE (TeamIdFrst = " + teamId + " OR TeamIDScnd = " + teamId + " ) AND GameDate BETWEEN '" + dateIn + "'  AND '" + dateOut + "'" 
                + "\n ORDER BY GameDate DESC" + ";";
            dt = query.SendSelectQuery(queryText);

            return dt;
        }

        // Возвращает все матчи игрока (order by game date desc)
        private DataTable GetAllMatchesUser(string userId)
        {
            // получаем список команд
            List<string> teamIds = GetTeamIds(userId);

            // для каждой команды получаем матчи
            DataTable dt = new DataTable();
            foreach (string teamId in teamIds)
            {
                dt.Merge(GetMatchesUserInTeam(userId, teamId));
            }

            return dt;
        }

        // Возвращает все матчи игрока со сдвигом
        private DataTable GetAllMatchesUser(string userId, string offset, string fetchNext)
        {
            // получаем список всех матчей (order by game date desc)
            DataTable dt = GetAllMatchesUser(userId);

            // проверяем количество оставшихся матчей
            if (dt.Rows.Count < Convert.ToInt32(offset) + Convert.ToInt32(fetchNext))
            {
                fetchNext = Convert.ToString(Convert.ToInt32(fetchNext) - (Convert.ToInt32(offset) + Convert.ToInt32(fetchNext) - dt.Rows.Count));
            }

            // создаем подмассив для datarow
            DataRow[] subDr = new DataRow[Convert.ToInt32(fetchNext)];
            int drCounter = 0;

            // заполняем подмассив строками из списка матчей со сдвигом
            for (int i = Convert.ToInt32(offset) + 1; i <= Convert.ToInt32(offset) + Convert.ToInt32(fetchNext); i++)
            {

                subDr[drCounter] = dt.Rows[i - 1];
                drCounter++;
            }

            DataTable subDt;
            try
            {
                subDt = subDr.CopyToDataTable();
            }
            catch
            {
                return null;
            }

            return subDt;
        }


        // ОБЪЕКТЫ ///
        // Возвращает объект класса PartInTeams
        private PartInTeams GetPartInTeams(string userId, string teamId)
        {
            PartInTeams player = new PartInTeams();
            player.TeamName = GetTeamName(teamId);
            player.DateOfStart = GetDateInTeam(userId, teamId);
            player.DateOfEnd = GetDateOutTeam(userId, teamId);

            int winCount, drawCount, loseCount;
            (winCount, drawCount, loseCount) = GetWinDrawLoseCount(userId, teamId);
            player.Stat = winCount.ToString() + " побед, " + drawCount.ToString() + " ничьих, " + loseCount.ToString() + " поражений";

            return player;
        }

        // Возвращает объект класса Match
        private Match GetMatch(DataRow dr)
        {
            Match match = new Match();
            match.FrstTeamName = GetTeamName(dr[0].ToString());
            match.ScndTeamName = GetTeamName(dr[1].ToString());
            match.Scores = dr[5].ToString() + " : " + dr[6].ToString();
            match.Tournament = dr[4].ToString();
            match.GameDate = Convert.ToDateTime(dr[3]).ToString("D");
            
            if (dr[2] != null)
            {
                if (Convert.ToBoolean(dr[2]))
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


        // КОМАНДЫ ///
        // Возвращает название команды
        private string GetTeamName(string teamId)
        {
            string queryText = "SELECT TeamName FROM Teams WHERE TeamId = " + teamId;
            DataTable dt = query.SendSelectQuery(queryText);

            return dt.Rows[0][0].ToString();
        }

        // Возвращает дату вступления игрока в команду
        private string GetDateInTeam(string userId, string teamId)
        {
            string queryText = "SELECT DateIn FROM TeamCompositions WHERE UserId = " + userId + " AND TeamId = " + teamId;
            DataTable dt = query.SendSelectQuery(queryText);

            return Convert.ToDateTime(dt.Rows[0][0]).ToString("D");
        }

        // Возвращает дату выхода игрока из команды
        private string GetDateOutTeam(string userId, string teamId)
        {
            string queryText = "SELECT DateOut FROM TeamCompositions WHERE UserId = " + userId + " AND TeamId = " + teamId;
            DataTable dt = query.SendSelectQuery(queryText);

            return Convert.ToDateTime(dt.Rows[0][0]).ToString("D");
        }

        // Возвращает список всех команд игрока
        private List<string> GetTeamIds(string userId)
        {
            string queryText = "SELECT TeamId FROM TeamCompositions WHERE UserId = " + userId;
            DataTable dt = query.SendSelectQuery(queryText);

            List<string> teamIds = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                teamIds.Add(dtRow[0].ToString());
            }

            return teamIds;
        }

        // Возвращает список команд игрока со сдвигом
        private List<string> GetTeamIds(string userId, string offset, string fetchNext)
        {
            string queryText = "SELECT TeamId FROM TeamCompositions WHERE UserId = " + userId + "\n" +
                               "ORDER BY DateIn DESC, TeamId" + "\n" +
                               "OFFSET " + offset + " ROWS FETCH NEXT " + fetchNext + " ROWS ONLY";
            DataTable dt = query.SendSelectQuery(queryText);

            List<string> teamIds = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                teamIds.Add(dtRow[0].ToString());
            }

            return teamIds;
        }


        // КОЛИЧЕСТВО ПОБЕД ///
        // Возвращает количество (побед, ничьих, поражений) игрока во ВСЕХ командах
        private (int, int, int) GetWinDrawLoseCount(string userId)
        {
            int winCounter = 0;
            int loseCounter = 0;
            int drawCounter = 0;

            // Получаем список всех команд
            List<string> teamIds = GetTeamIds(userId);

            // Считаем количество побед, ничьих, поражений
            for (int i = 0; i < teamIds.Count; i++)
            {
                string teamId = teamIds[i];

                int winIncrement, drawIncrement, loseIncrement;

                (winIncrement, drawIncrement, loseIncrement) = GetWinDrawLoseCount(userId, teamId);
                winCounter += winIncrement;
                drawCounter += drawIncrement;
                loseCounter += loseIncrement;
            }

            return (winCounter, drawCounter, loseCounter);
        }

        // Возвращает количество (побед, ничьих, поражений) игрока в ОДНОЙ команде
        private (int, int, int) GetWinDrawLoseCount(string userId, string teamId)
        {
            // инициализация счетчиков
            int winCounter = 0;
            int loseCounter = 0;
            int drawCounter = 0;

            // получаем список матчей в игрока в команде
            DataTable dt = GetMatchesUserInTeam(userId, teamId);

            // для каждого матча узнать (победа, ничья, поражение)
            foreach (DataRow dtRow in dt.Rows)
            {
                if (dtRow[0].ToString() == teamId)
                {

                    if (dtRow[2] != null)
                    {
                        if (Convert.ToBoolean(dtRow[2]))
                        {
                            winCounter++;
                        }
                        else
                        {
                            loseCounter++;
                        }
                    }
                    else
                    {
                        drawCounter++;
                    }
                }
                else
                {
                    if (dtRow[2] != null)
                    {
                        if (!Convert.ToBoolean(dtRow[2]))
                        {
                            winCounter++;
                        }
                        else
                        {
                            loseCounter++;
                        }
                    }
                    else
                    {
                        drawCounter++;
                    }
                }
            }  // foreach

            return (winCounter, drawCounter, loseCounter);
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
