using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для TeamProfileWindow.xaml
    /// </summary>
    public partial class TeamProfileWindow : Window
    {
        SQL query;
        string teamId;

        List<PlayerInTeamList> teamList = new List<PlayerInTeamList>();

        public TeamProfileWindow(string teamId)
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            this.teamId = teamId;

            // Адаптация к размеру окна

            // Указываем строку подключения для запросов
            query = new SQL("DefaultConnection");

            // Подгрузка краткой информации
            dt = query.SendSelectQuery("SELECT " +
                                            "TeamName," +
                                            "Country," +
                                            "DateOfFoundation," +
                                            "PhoneNumber," +
                                            "Email," +
                                            "AboutTeam," +
                                            "LogoURL " +
                                            "FROM Teams WHERE TeamId = " + this.teamId);

            TeamNameTB.Text = dt.Rows[0][0].ToString();
            CountryTB.Text = "Страна: " + dt.Rows[0][1].ToString();
            FoundationDateTB.Text = "Дата основания: " + Convert.ToDateTime(dt.Rows[0][2]).ToString("D");
            PhoneNumberTB.Text = "Номер телефона: " + "+" + dt.Rows[0][3].ToString();
            EmailTB.Text = "Email: " + dt.Rows[0][4].ToString();
            AboutTeamTB.Text = "О команде: " + dt.Rows[0][5].ToString();

            // Подгрузка аватара
            if (dt.Rows[0][6] != DBNull.Value)
            {
                TeamLogoImg.Source = new BitmapImage(new Uri(dt.Rows[0][6].ToString()));
            }

            // Подгрузка статистики
            var (winCount, drawCount, loseCount) = GetWinDrawLoseCount(this.teamId);
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

            // Подгрузка состава -- происходит в NowStatusRB_Checked
            NowStatusRB.IsChecked = true;

            // Подгрузка истории матчей

            // Подгрузка достижений
            List<Achievement> achievementList = new List<Achievement>(3)
            {
                new Achievement { Place="1", Tournament="The International 10 (2021)", Venue="Бухарест, Румыния", DateOfVenue="17.10.2021", Prize="18 000 000$", PrizeFond="40 000 000$"},
                new Achievement { Place="7", Tournament="ESL One Summer 2021", Venue="Онлайн", DateOfVenue="20.07.2021", Prize="30 000$", PrizeFond="1 000 000$"},
            };
            AchievementDataGrid.ItemsSource = achievementList;
        }

        private void NowStatusRB_Checked(object sender, RoutedEventArgs e)
        {
            TeamListStatusExpander.Header = NowStatusRB.Content;
            TeamListStatusExpander.IsExpanded = false;

            teamList = new List<PlayerInTeamList>();
            List<string> userIds = GetPlayerIdsInTeamPresent(this.teamId);
            foreach (string userId in userIds)
            {
                teamList.Add(GetPlayerInTeamList(userId));
            }
            TeamLB.ItemsSource = new List<string>();
            TeamLB.ItemsSource = teamList;
        }

        private void PastStatusRB_Checked(object sender, RoutedEventArgs e)
        {
            TeamListStatusExpander.Header = PastStatusRB.Content;
            TeamListStatusExpander.IsExpanded = false;

            teamList = new List<PlayerInTeamList>();
            List<string> userIds = GetPlayerIdsInTeamPast(this.teamId);
            foreach (string userId in userIds)
            {
                teamList.Add(GetPlayerInTeamList(userId));
            }
            TeamLB.ItemsSource = new List<string>();
            TeamLB.ItemsSource = teamList;
        }

        // Возвращает список всех игроков команды в настоящий момент
        private List<string> GetPlayerIdsInTeamPresent(string teamId)
        {
            List<string> playerIds = new List<string>();

            DataTable dt = query.SendSelectQuery("SELECT UserId, DateOut FROM TeamCompositions WHERE TeamId = " + teamId);

            foreach (DataRow dr in dt.Rows)
            {
                if (dr.IsNull(1))
                {
                    playerIds.Add(dr[0].ToString());
                }
            }

            return playerIds;
        }

        // Возвращает список всех игроков команды в прошлом
        private List<string> GetPlayerIdsInTeamPast(string teamId)
        {
            if (teamId == null)
            {
                return new List<string>();
            }

            List<string> playerIds = new List<string>();

            DataTable dt = query.SendSelectQuery("SELECT UserId, DateOut FROM TeamCompositions WHERE TeamId = " + teamId);

            foreach (DataRow dr in dt.Rows)
            {
                if (!dr.IsNull(1))
                {
                    playerIds.Add(dr[0].ToString());
                }
            }

            return playerIds;
        }

        // Возвращает количество (побед, ничьих, поражений) команды
        private (int, int, int) GetWinDrawLoseCount(string teamId)
        {
            // инициализация счетчиков
            int winCounter = 0;
            int loseCounter = 0;
            int drawCounter = 0;

            // получаем список матчей в игрока в команде
            DataTable dt = query.SendSelectQuery("SELECT TeamIdFrst, TeamIdScnd, IsFrstWin FROM Matches WHERE TeamIdFrst = " + teamId + " OR TeamIdScnd = " + teamId);

            // для каждого матча узнать (победа, ничья, поражение)
            foreach (DataRow dtRow in dt.Rows)
            {
                if (dtRow[0].ToString() == teamId)
                {

                    if (!dtRow.IsNull(2))
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
                    if (!dtRow.IsNull(2))
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

                    if (!dtRow.IsNull(2))
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
                    if (!dtRow.IsNull(2))
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

        // Возвращает все матчи игрока в команде
        private DataTable GetMatchesUserInTeam(string userId, string teamId)
        {
            // Получаем даты нахождения в команде
            string queryText = "SELECT DateIn, DateOut FROM TeamCompositions WHERE UserId = " + userId + " AND TeamId = " + teamId;
            DataTable dt = query.SendSelectQuery(queryText);
            string dateIn = dt.Rows[0][0].ToString();

            string dateOut = DateTime.Now.ToString("u");
            if (!dt.Rows[0].IsNull(1))
            {
                dateOut = dt.Rows[0][1].ToString();
            }



            // Получаем все матчи игрока в одной команде
            queryText = "SELECT TeamIdFrst, TeamIdScnd, IsFrstWin, GameDate, TournamentID, FrstScores, ScndScores FROM Matches WHERE (TeamIdFrst = " + teamId + " OR TeamIDScnd = " + teamId + " ) AND GameDate BETWEEN '" + dateIn + "'  AND '" + dateOut + "'"
                + "\n ORDER BY GameDate DESC" + ";";
            dt = query.SendSelectQuery(queryText);

            return dt;
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

            if (!dt.Rows[0].IsNull(0))
            {
                return Convert.ToDateTime(dt.Rows[0][0]).ToString("D");

            }
            else
            {
                return "настоящее время";
            }
        }

        // Возвращает объект класса PlayerInTeamList
        private PlayerInTeamList GetPlayerInTeamList(string userId)
        {
            PlayerInTeamList player = new PlayerInTeamList();

            DataTable dt = query.SendSelectQuery("SELECT "+
                                                        "CONCAT(LName, ' ', FName, ' ', MName), " +
                                                        "Nickname, " +
                                                        "TeamId, " +
                                                        "CountryId, " +
                                                        "PhotoURL " +
                                                        "FROM Users WHERE UserId = " + userId);
            // имя
            player.RealName = dt.Rows[0][0].ToString();
            player.Nickname = dt.Rows[0][1].ToString();

            // фото игрока
            if (!dt.Rows[0].IsNull(4))
            {
                player.UserPhotoPath = dt.Rows[0][4].ToString();
            }
            else
            {
                player.UserPhotoPath = "C:/Users/nikich4523/source/repos/Курсовой проект 3.1/Images/WithoutPhoto.png";
            }

            // иконка страны
            if (dt.Rows[0][3].ToString() != "0")
            {
                DataTable dtCountry = query.SendSelectQuery("SELECT ImgPath FROM Country WHERE CountryId = " + dt.Rows[0][3].ToString());
                player.UserPhotoPath = dtCountry.Rows[0][0].ToString();
            }
            else
            {
                player.CountryIconPath = "C:/Users/nikich4523/source/repos/Курсовой проект 3.1/Images/Cup.png";
            }

            // даты в команде
            player.Dates = GetDateInTeam(userId, this.teamId) + " - " + GetDateOutTeam(userId, this.teamId);


            // статистика в команде
            int winCount, drawCount, loseCount;
            (winCount, drawCount, loseCount) = GetWinDrawLoseCount(userId, this.teamId);
            player.Statistick = winCount.ToString() + " побед, " + drawCount.ToString() + " ничьих, " + loseCount.ToString() + " поражений";

            return player;
        }
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
