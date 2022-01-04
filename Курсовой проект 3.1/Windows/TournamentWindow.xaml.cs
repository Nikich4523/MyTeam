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
using Курсовой_проект_3._1.UserControls;
using Курсовой_проект_3._1.Windows;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для TournamentWindow.xaml
    /// </summary>
    public partial class TournamentWindow : Window
    {
        MyTeamContext _context;
        int nowTournamentId;
        bool isAccess;

        public TournamentWindow(int tournamentId)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTournamentId = tournamentId;
            isAccess = false;

            // Подгрузка краткой информации
            Tournaments tournament = _context.Tournaments.Find(nowTournamentId);

            // Доступность кнопок
            if (App.UserId == tournament.FK_Organizer_Id)
            {
                AddResultBtn.Visibility = Visibility.Visible;
                isAccess = true;
            }

            if (tournament.Users.Disciplines != null)
            {
                DiciplineLogoImg.Source = new BitmapImage(new Uri(tournament.Users.Disciplines.IconPath));
                DiciplineNameTB.Text = tournament.Users.Disciplines.Name;
            }
            TournamentNameTB.Text = tournament.Name;
            TournamentShortNameTB.Text = tournament.Name;
            OrganizerTB.Text = tournament.Users.Nickname;
            VenueTB.Text = tournament.Venue;
            FormatTB.Text = tournament.TournamentFormats.Name;
            PrizeFondTB.Text = Func.ConvertIntToStringMoney(tournament.PrizeFond) + "$";

            // Получаем кол-во команд на турнире
            int teamCount = _context.Tournaments.Find(nowTournamentId).MaxTeamsCount;

            // Создаем сетку на такой размер
            MainGrid.Children.Add(GenerateSingleEliminationBracket(teamCount));

            // Получаем все матчи турнира
            List<TournamentMatch> matchList = new List<TournamentMatch>();
            List<Matches> matchesDb = _context.Matches.Where(m => m.FK_Tournament_Id == nowTournamentId).OrderBy(m => m.GameDate).ToList();
            int addGameCounter = 0;
            foreach (Matches match in matchesDb)
            {
                matchList.Add(new TournamentMatch(match.FK_FrstTeam_Id.ToString(), match.FK_ScndTeam_Id.ToString(), match.FrstScore.ToString(), 
                                                  match.ScndScore.ToString(), match.RoundNum, match.FK_WinnerTeam_Id.ToString(), addGameCounter));
                addGameCounter++;
            }

            // Высчитывание размеров сетки
            int roundCount = (int)Math.Log(teamCount, 2);  // количество раундов
            int gridColumnCount = roundCount + roundCount - 1 + 2;  // количество колонок в grid
            int gridRowCount = teamCount + (teamCount - 2) + 4;  // количество строк в grid
            int roundCounter = 0;
            int gameCounter = 0;
            Grid bracketGrid = (Grid)MainGrid.Children[0];
            for (int column = 1; column < gridColumnCount; column++)
            {
                if (column % 2 != 0)
                {
                    roundCounter++;

                    for (int row = 0; row < gridRowCount; row++)
                    {

                        UIElement element = GetElementInGridPosition(column, row, bracketGrid);

                        if (element is MatchControl)
                        {
                            foreach (TournamentMatch match in matchList)
                            {
                                if (match.roundNum == roundCounter && match.gameNum == gameCounter)
                                {
                                    bracketGrid.Children.Remove(GetElementInGridPosition(column, row, bracketGrid));

                                    MatchControl newElement = new MatchControl();
                                    if (isAccess)
                                    {
                                        newElement.MouseDown += MatchControl_Click;
                                        newElement.Cursor = Cursors.Hand;
                                    }

                                    newElement.TeamNameFrst = _context.Teams.Find(Convert.ToInt32(match.teamIdFrst)).Name;
                                    newElement.ScoreFrst = match.ScoreFrst;

                                    newElement.TeamNameScnd = _context.Teams.Find(Convert.ToInt32(match.teamIdScnd)).Name;
                                    newElement.ScoreScnd = match.ScoreScnd;
                                    newElement.RoundNum = roundCounter.ToString();



                                    Grid.SetColumn(newElement, column);
                                    Grid.SetRow(newElement, row);
                                    Grid.SetRowSpan(newElement, 2);
                                    bracketGrid.Children.Add(newElement);
                                }
                            }

                            gameCounter++;
                        }
                    }
                }
            }

            // Результаты
            List<Achievements> achievementList = _context.Achievements.Where(a => a.FK_Tournament_Id == nowTournamentId).ToList();

            List<Result> resultList = new List<Result>();
            foreach (Achievements achievement in achievementList)
            {
                Result result = new Result();
                result.Place = achievement.Place;
                result.Prize = Func.ConvertIntToStringMoney(achievement.Prize) + " $";
                result.TeamName = achievement.Teams.Name;
                result.LogoPath = _context.Teams.Find(achievement.FK_Team_Id).LogoPath;
                result.CountryIconPath = _context.Teams.Find(achievement.FK_Team_Id).Countries.IconPath;

                if (result.Place == "1")
                {
                    result.Place = "";
                    result.PlacePath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\Default\cupGold.png";
                }
                else if (result.Place == "2")
                {
                    result.Place = "";
                    result.PlacePath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\Default\cupSilver.png";
                }
                else if (result.Place == "3")
                {
                    result.Place = "";
                    result.PlacePath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\Default\cupBronze.png";
                }

                resultList.Add(result);
            }

            ResultLB.ItemsSource = resultList;
        }

        // Генерирует турнирную сетку SingleElimination (работает со степенями 2ки) - работает не трогай
        public Grid GenerateSingleEliminationBracket(int teamCount)
        {
            Grid grid = new Grid();
            grid.Background = Brushes.WhiteSmoke;
            grid.ShowGridLines = false;

            // Создание Grid'ов ///
            // Высчитывание размеров сетки
            int roundCount = (int)Math.Log(teamCount, 2);  // количество раундов
            int gridColumnCount = roundCount + roundCount - 1 + 2;  // количество колонок в grid
            int gridRowCount = teamCount + (teamCount - 2) + 4;  // количество строк в grid

            // Добавление колонок
            for (int i = 0; i < gridColumnCount; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Добавление строк
            for (int j = 0; j < gridRowCount; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            // Всё остальное
            int roundNum = 1;  // номер раунда
            int skipStart = 2;  // сколько пропустить строк с начала
            int skipBetween = 2;  // сколько пропускать строк между играми

            // Наполнение сетки
            for (int i = 1; i < gridColumnCount - 1; i++)
            {
                if (i % 2 != 0)  // раунды через одну колонку
                {
                    for (int j = skipStart; j < gridRowCount - 2; j++)
                    {
                        MatchControl match = new MatchControl();
                        if (isAccess)
                        {
                            match.MouseDown += MatchControl_Click;
                            match.Cursor = Cursors.Hand;
                            match.RoundNum = roundNum.ToString();
                        }

                        Grid.SetColumn(match, i);
                        Grid.SetRow(match, j);
                        Grid.SetRowSpan(match, 2);
                        grid.Children.Add(match);

                        // после каждой игры нужен пропуск (skipBetween) до следующей игры
                        j += skipBetween + 1;

                    }
                    // пересчет значений для следующего раунда
                    roundNum++;
                    int gamesInRoundCount = (int)(teamCount / Math.Pow(2, roundNum));
                    skipStart *= 2;
                    skipBetween = roundNum == roundCount ? skipStart : (gridRowCount - (skipStart * 2) - (gamesInRoundCount * 2)) / (gamesInRoundCount - 1);
                }
            }

            // соединяющие линии между раундами
            int skipStartOrEnd = 4;
            for (int i = 2; i < gridColumnCount - 1; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 2)
                    {
                        for (int j = 4; j < gridRowCount; j++)
                        {
                            TextBox linetb1 = new TextBox();
                            linetb1.BorderBrush = Brushes.Black;
                            linetb1.IsReadOnly = true;
                            var bc = new BrushConverter();
                            linetb1.Background = Brushes.Transparent;
                            linetb1.BorderThickness = new Thickness(2, 0, 0, 2);

                            Grid.SetColumn(linetb1, i);
                            Grid.SetRow(linetb1, j);
                            grid.Children.Add(linetb1);

                            TextBox linetb2 = new TextBox();
                            linetb2.BorderBrush = Brushes.Black;
                            linetb2.IsReadOnly = true;
                            linetb2.Background = Brushes.Transparent;
                            linetb2.BorderThickness = new Thickness(2, 2, 0, 0);

                            Grid.SetColumn(linetb2, i);
                            Grid.SetRow(linetb2, j + 1);
                            grid.Children.Add(linetb2);

                            j += 7;
                        }
                    }
                    else
                    {
                        int drawStatus = 0; // 0 - не рисовать, 1 - leftonly

                        for (int j = 4; j < gridRowCount - skipStartOrEnd; j++)
                        {
                            // тут leftonly
                            if (drawStatus == 1)
                            {
                                // leftonly
                                TextBox linetb3 = new TextBox();
                                linetb3.BorderBrush = Brushes.Black;
                                linetb3.IsReadOnly = true;
                                var bc = new BrushConverter();
                                linetb3.Background = Brushes.Transparent;
                                linetb3.BorderThickness = new Thickness(2, 0, 0, 0);

                                Grid.SetColumn(linetb3, i);
                                Grid.SetRow(linetb3, j);
                                grid.Children.Add(linetb3);
                            }


                            // тут проверка
                            if (GetElementInGridPosition(i - 1, j, grid) is MatchControl)
                            {
                                if (drawStatus == 1)
                                {
                                    grid.Children.Remove(GetElementInGridPosition(i, j, grid));
                                }

                                j += 2; // пропуск текущего MatchControl

                                if (j < gridRowCount - skipStartOrEnd)
                                {
                                    // leftonly
                                    TextBox linetb3 = new TextBox();
                                    linetb3.BorderBrush = Brushes.Black;
                                    linetb3.IsReadOnly = true;
                                    var bc = new BrushConverter();
                                    linetb3.Background = Brushes.Transparent;
                                    linetb3.BorderThickness = new Thickness(2, 0, 0, 0);

                                    Grid.SetColumn(linetb3, i);
                                    Grid.SetRow(linetb3, j);
                                    grid.Children.Add(linetb3);
                                }

                                drawStatus = 1;
                            }
                            else if (GetElementInGridPosition(i + 1, j, grid) is MatchControl)
                            {
                                var bc = new BrushConverter();

                                // leftbottom
                                TextBox linetb1 = new TextBox();
                                linetb1.BorderBrush = Brushes.Black;
                                linetb1.IsReadOnly = true;
                                linetb1.Background = Brushes.Transparent;
                                linetb1.BorderThickness = new Thickness(2, 0, 0, 2);
                                Grid.SetColumn(linetb1, i);
                                Grid.SetRow(linetb1, j);
                                grid.Children.Add(linetb1);

                                j++; // переход на следующую строку

                                // lefttop
                                TextBox linetb2 = new TextBox();
                                linetb2.BorderBrush = Brushes.Black;
                                linetb2.IsReadOnly = true;
                                linetb2.Background = Brushes.Transparent;
                                linetb2.BorderThickness = new Thickness(2, 2, 0, 0);
                                Grid.SetColumn(linetb2, i);
                                Grid.SetRow(linetb2, j);
                                grid.Children.Add(linetb2);
                            }
                        }
                        skipStartOrEnd = skipStartOrEnd * 2;
                    }
                }
            }
            
            return grid;
        }

        private UIElement GetElementInGridPosition(int column, int row, Grid RootGrid)
        {
            foreach (UIElement element in RootGrid.Children)
            {
                if (Grid.GetColumn(element) == column && Grid.GetRow(element) == row)
                    return element;
            }

            return null;
        }

        private void MatchControl_Click(object sender, RoutedEventArgs e)
        {
            MatchControl match = (MatchControl)sender;
            if(match.TeamNameFrst == "" && match.TeamNameScnd == "")
            {
                SelectTeamsInMatchWindow wnd = new SelectTeamsInMatchWindow(nowTournamentId, Convert.ToInt32(match.RoundNum));
                wnd.ShowDialog();
            }
            else
            {
                MatchWindow wnd = new MatchWindow(match, nowTournamentId, Convert.ToInt32(match.RoundNum));
                wnd.ShowDialog();
            }
        }

        private void AddResultBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultAddWindow wnd = new ResultAddWindow(nowTournamentId);
            wnd.ShowDialog();
        }

        private void ResultLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ResultLB.SelectedItem != null)
            {
                Result result = (Result)ResultLB.SelectedItem;
                ResultAddWindow wnd = new ResultAddWindow(result.TeamName, nowTournamentId);
                wnd.ShowDialog();
            }
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
    }

    // Класс, описывающий матч в турнирной сетке
    public class TournamentMatch
    {
        public string teamIdFrst { get; private set; }
        public string teamIdScnd { get; private set; }
        public string ScoreFrst { get; private set; }
        public string ScoreScnd { get; private set; }
        public int roundNum { get; private set; }
        public int gameNum { get; private set; }
        public string teamIdWinner { get; private set; }


        public TournamentMatch(string teamIdFrst, string teamIdScnd, string ScoreFrst, string ScoreScnd, int roundNum, string teamIdWinner, int gameNum)
        {
            this.teamIdFrst = teamIdFrst;
            this.teamIdScnd = teamIdScnd;
            this.ScoreFrst = ScoreFrst;
            this.ScoreScnd = ScoreScnd;
            this.roundNum = roundNum;
            this.gameNum = gameNum;
            this.teamIdWinner = teamIdWinner;
        }
    }

    // Класс, описывающий результат в ResultLB
    public class Result
    {
        public string Place { get; set; }
        public string PlacePath { get; set; }
        public string LogoPath { get; set; }
        public string CountryIconPath { get; set; }
        public string TeamName { get; set; }
        public string Prize { get; set; }
    }
}
