using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Курсовой_проект_3._1.Windows;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для ApplicationWindow.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        List<Item> countryList = new List<Item>();
        List<Item> disciplineList = new List<Item>();

        MyTeamContext _context;

        public ApplicationWindow()
        {
            InitializeComponent();
            _context = new MyTeamContext();
            PlayerListShowButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            foreach (Countries country in _context.Countries)
            {
                countryList.Add(CountryToItem(country));
            }

            foreach (Disciplines discipline in _context.Disciplines)
            {
                disciplineList.Add(DisciplineToItem(discipline));
            }
           
            CountryFilterPlayersLB.ItemsSource = countryList;
            DiciplineFilterPlayersLB.ItemsSource = disciplineList;
        }

        private Item CountryToItem(Countries country)
        {
            return new Item() {ItemName = country.Name, IsSelected = false, ItemId = country.Id };
        }

        private Item DisciplineToItem(Disciplines discipline)
        {
            return new Item() { ItemName = discipline.Name, IsSelected = false, ItemId = discipline.Id };
        }

        // NavigationBtns
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow wnd = new SearchWindow();
            wnd.ShowDialog();
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow wnd = new MainWindow(App.UserId);
            wnd.Show();
            Close();
        }

        // ListShowBtns
        private void PlayerListShowButton_Click(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            PlayerListShowButton.Background = (Brush)bc.ConvertFrom("#68A4C8");
            TeamListShowButton.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            TournamentListShowButton.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");

            PlayersFilter.Visibility = Visibility.Visible;
            TeamsFilter.Visibility = Visibility.Hidden;
            TournamentsFilter.Visibility = Visibility.Hidden;

            PlayerAppAddBtn.Visibility = Visibility.Visible;
            TeamAppAddBtn.Visibility = Visibility.Hidden;
            TournamentAddBtn.Visibility = Visibility.Hidden;
            TeamAddBtn.Visibility = Visibility.Collapsed;

            applicationsLB.ItemsSource = new List<string>();

            ApplyFilterPlayersButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void TeamListShowButton_Click(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            PlayerListShowButton.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            TeamListShowButton.Background = (Brush)bc.ConvertFrom("#68A4C8");
            TournamentListShowButton.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");

            PlayersFilter.Visibility = Visibility.Hidden;
            TeamsFilter.Visibility = Visibility.Visible;
            TournamentsFilter.Visibility = Visibility.Hidden;

            PlayerAppAddBtn.Visibility = Visibility.Hidden;
            TeamAppAddBtn.Visibility = Visibility.Visible;
            TournamentAddBtn.Visibility = Visibility.Hidden;
            TeamAddBtn.Visibility = Visibility.Visible;

            // очищаем фильтры
            LowerDataPickerPlayers.Text = "";
            UpperDataPickerPlayers.Text = "";
            LowerAgePlayersTBox.Text = "";
            UpperAgePlayersTBox.Text = "";

            applicationsLB.ItemsSource = new List<byte>();

            ApplyFilterTeamsButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void TournamentListShowButton_Click(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            PlayerListShowButton.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            TeamListShowButton.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            TournamentListShowButton.Background = (Brush)bc.ConvertFrom("#68A4C8");

            PlayersFilter.Visibility = Visibility.Hidden;
            TeamsFilter.Visibility = Visibility.Hidden;
            TournamentsFilter.Visibility = Visibility.Visible;

            PlayerAppAddBtn.Visibility = Visibility.Hidden;
            TeamAppAddBtn.Visibility = Visibility.Hidden;
            TournamentAddBtn.Visibility = Visibility.Visible;
            TeamAddBtn.Visibility = Visibility.Collapsed;

            // очищаем фильтры
            LowerDataPickerPlayers.Text = "";
            UpperDataPickerPlayers.Text = "";
            LowerAgePlayersTBox.Text = "";
            UpperAgePlayersTBox.Text = "";
            CountryFilterPlayersLB.ItemsSource = new List<byte>();
            DiciplineFilterPlayersLB.ItemsSource = new List<byte>();

            applicationsLB.ItemsSource = new List<byte>();

            ApplyFilterTournamentsButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        // ApplyFilterBtns
        private void ApplyFilterPlayersButton_Click(object sender, RoutedEventArgs e)
        {
            // обновляем шаблон для заявок
            templateSelector.Text = "PlayersItemTemplate";

            // инициализируем переменные
            DateTime lowerDate, upperDate;
            int lowerAge, upperAge;
            List<int> countries = new List<int>();
            List<int> disciplines = new List<int>();

            lowerDate = String.IsNullOrWhiteSpace(LowerDataPickerPlayers.Text) ? Convert.ToDateTime("01.12.2021") : Convert.ToDateTime(LowerDataPickerPlayers.Text.Trim());
            upperDate =  String.IsNullOrWhiteSpace(UpperDataPickerPlayers.Text) ? DateTime.Now : Convert.ToDateTime(UpperDataPickerPlayers.Text.Trim());
            lowerAge = String.IsNullOrWhiteSpace(LowerAgePlayersTBox.Text) ? 0 : Convert.ToInt32(LowerAgePlayersTBox.Text.Trim());
            upperAge = String.IsNullOrWhiteSpace(UpperAgePlayersTBox.Text) ? 99 : Convert.ToInt32(UpperAgePlayersTBox.Text.Trim());

            foreach (Item item in CountryFilterPlayersLB.Items)
            {
                if (item.IsSelected)
                {
                    countries.Add(item.ItemId);
                }
            }

            foreach(Item item in DiciplineFilterPlayersLB.Items)
            {
                if (item.IsSelected)
                {
                    disciplines.Add(item.ItemId);
                }
            }

            string countriesString = countries.Count == 0 ? "*" : string.Join(",", countries);
            string disciplinesString = disciplines.Count == 0 ? "*" : string.Join(",", disciplines);

            // проверка на корректность введенных значений
            if (Convert.ToInt32(lowerAge) > Convert.ToInt32(upperAge))
            {
                MessageBox.Show("Некорректно введен возвраст!");
                return;
            }

            if (DateTime.Compare(Convert.ToDateTime(lowerDate), Convert.ToDateTime(upperDate)) > 0)
            {
                MessageBox.Show("Некорректно введены даты!");
                return;
            }

            // очищаем текущий список
            applicationsLB.ItemsSource = new List<string>();

            List<PlayerApps> apps = new List<PlayerApps>();
            List<PlayerApps> beforeApps = new List<PlayerApps>();
            // получаем список заявок (lowerAgeParam, upperAgeParam, lowerDateParam, upperDateParam, countriesParam, disciplinesParam)
            if (countriesString == "*" || disciplinesString == "*")
            {
                if (countriesString == "*" && disciplinesString == "*")
                {
                    beforeApps = _context.PlayerApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate).ToList();
                    foreach (PlayerApps beforeApp in beforeApps)
                    {
                        if (Func.CalculateAge(beforeApp.Users.Birthday) >= lowerAge && Func.CalculateAge(beforeApp.Users.Birthday) <= upperAge)
                        {
                            apps.Add(beforeApp);
                        }
                    }
                }
                else if (countriesString == "*")
                {
                    foreach (int discipline in disciplines)
                    {
                        beforeApps.AddRange(_context.PlayerApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate && app.Users.FK_Discipline_Id == discipline).ToList());
                    }

                    foreach (PlayerApps beforeApp in beforeApps)
                    {
                        if (Func.CalculateAge(beforeApp.Users.Birthday) >= lowerAge && Func.CalculateAge(beforeApp.Users.Birthday) <= upperAge)
                        {
                            apps.Add(beforeApp);
                        }
                    }
                }
                else
                {
                    foreach (int country in countries)
                    {
                        beforeApps.AddRange(_context.PlayerApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate && app.Users.FK_Country_Id == country).ToList());
                    }

                    foreach (PlayerApps beforeApp in beforeApps)
                    {
                        if (Func.CalculateAge(beforeApp.Users.Birthday) >= lowerAge && Func.CalculateAge(beforeApp.Users.Birthday) <= upperAge)
                        {
                            apps.Add(beforeApp);
                        }
                    }
                }
            }
            else
            {
                foreach (int country in countries)
                {
                    foreach (int discipline in disciplines)
                    {
                        beforeApps.AddRange(_context.PlayerApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate && app.Users.FK_Country_Id == country && app.Users.FK_Discipline_Id == discipline).ToList());
                    }
                }

                foreach (PlayerApps beforeApp in beforeApps)
                {
                    if (Func.CalculateAge(beforeApp.Users.Birthday) >= lowerAge && Func.CalculateAge(beforeApp.Users.Birthday) <= upperAge)
                    {
                        apps.Add(beforeApp);
                    }
                }
            }

            // заполняем список заявок
            List<PlayerApplication> playerApps = new List<PlayerApplication>();
            foreach (PlayerApps app in apps)
            {
                PlayerApplication playerApp = new PlayerApplication();
                playerApp.Name = app.Users.LName + " " + app.Users.FName + " " + app.Users.MName;
                playerApp.Nickname = app.Users.Nickname;
                playerApp.Age = Func.CalculateAge(app.Users.Birthday).ToString();

                

                if (string.IsNullOrEmpty(app.Users.PhotoPath))
                {
                    playerApp.PhotoURL = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Images\WithoutPhoto.png";
                }
                else
                {
                    playerApp.PhotoURL = app.Users.PhotoPath;
                }

                playerApp.CountryName = app.Users.Countries.Name;

                if (string.IsNullOrEmpty(app.Users.Countries.IconPath))
                {
                    playerApp.CountryURL = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\countrys - 24\_unknown.png";
                }
                else
                {
                    playerApp.CountryURL = app.Users.Countries.IconPath;
                }

                playerApp.DisciplineName = app.Users.Disciplines != null ? app.Users.Disciplines.Name : "Не указано";

                playerApp.Email = app.Users.Email;
                playerApp.PhoneNumber = "+" + app.Users.PhoneNumber;

                playerApp.Title = app.Title;
                playerApp.Text = app.AppText;
                playerApp.Date = app.AppDate.ToString("D");
                playerApp.PlayerId = app.Users.Id.ToString();

                playerApps.Add(playerApp);
            }

            applicationsLB.ItemsSource = playerApps;
        }

        private void ApplyFilterTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            // обновляем шаблон для заявок
            templateSelector.Text = "TeamsItemTemplate";

            // очищаем текущий список
            applicationsLB.ItemsSource = new List<string>();

            // инициализируем переменные
            DateTime lowerDate, upperDate;
            List<int> countries = new List<int>();
            List<int> disciplines = new List<int>();

            lowerDate = string.IsNullOrWhiteSpace(LowerDataPickerTeams.Text) ? Convert.ToDateTime("01.12.2021") : Convert.ToDateTime(LowerDataPickerTeams.Text.Trim());
            upperDate = string.IsNullOrWhiteSpace(UpperDataPickerTeams.Text) ? DateTime.Now : Convert.ToDateTime(UpperDataPickerTeams.Text.Trim());

            foreach (Item item in CountryFilterTeamsLB.Items)
            {
                if (item.IsSelected)
                {
                    countries.Add(item.ItemId);
                }
            }

            foreach (Item item in DiciplineFilterTeamsLB.Items)
            {
                if (item.IsSelected)
                {
                    disciplines.Add(item.ItemId);
                }
            }

            string countriesString = countries.Count == 0 ? "*" : string.Join(",", countries);
            string disciplinesString = disciplines.Count == 0 ? "*" : string.Join(",", disciplines);

            // проверка на корректность введенных значений
            if (DateTime.Compare(lowerDate, upperDate) > 0)
            {
                MessageBox.Show("Некорректно введены даты!");
                return;
            }

            List<TeamApps> apps = new List<TeamApps>();
            // получаем список заявок (lowerAgeParam, upperAgeParam, lowerDateParam, upperDateParam, countriesParam, disciplinesParam)
            if (countriesString == "*" || disciplinesString == "*")
            {
                if (countriesString == "*" && disciplinesString == "*")
                {
                    apps = _context.TeamApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate).ToList();
                }
                else if (countriesString == "*")
                {
                    foreach (int discipline in disciplines)
                    {
                        apps.AddRange(_context.TeamApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate && app.Teams.Users.FK_Discipline_Id == discipline).ToList());
                    }
                }
                else
                {
                    foreach (int country in countries)
                    {
                        apps.AddRange(_context.TeamApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate && app.Teams.Users.FK_Country_Id == country).ToList());
                    }
                }
            }
            else
            {
                foreach (int country in countries)
                {
                    foreach (int discipline in disciplines)
                    {
                        apps.AddRange(_context.TeamApps.Where(app => app.AppDate >= lowerDate && app.AppDate <= upperDate && app.Teams.Users.FK_Discipline_Id == discipline
                        && app.Teams.Users.FK_Country_Id == country).ToList());
                    }
                }
            }

            // заполняем список заявок
            List<TeamApplication> teamApps = new List<TeamApplication>();
            foreach (TeamApps app in apps)
            {
                TeamApplication teamApp = new TeamApplication();
                teamApp.Name = app.Teams.Name;
                teamApp.DateOfFoundation = app.Teams.FoundationDate.ToString("d");

                if (string.IsNullOrEmpty(app.Teams.LogoPath))
                {
                    teamApp.LogoURL = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Images\WithoutPhoto.png";
                }
                else
                {
                    teamApp.LogoURL = app.Teams.LogoPath;
                }

                teamApp.CountryName = app.Teams.Users.Countries.Name;

                if (string.IsNullOrEmpty(app.Teams.Users.Countries.IconPath))
                {
                    teamApp.CountryURL = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Курсовой проект 3.1\img\countrys - 24\_unknown.png";
                }
                else
                {
                    teamApp.CountryURL = app.Teams.Countries.IconPath;
                }

                teamApp.DisciplineName = app.Teams.Users.Disciplines.Name;
                teamApp.Title = app.Title;
                teamApp.Text = app.AppText;
                teamApp.Date = app.AppDate.ToString("D");
                teamApp.TeamId = app.Teams.Id.ToString();

                teamApps.Add(teamApp);
            }

            applicationsLB.ItemsSource = teamApps;
        }

        private void ApplyFilterTournamentsButton_Click(object sender, RoutedEventArgs e)
        {
            // обновляем шаблон для заявок
            templateSelector.Text = "TournamentsItemTemplate";

            // очищаем текущий список
            applicationsLB.ItemsSource = new List<string>();

            // инициализируем переменные
            DateTime lowerDate, upperDate;
            int lowerPrize, upperPrize;
            bool withFreePlace;
            List<int> disciplines = new List<int>();

            lowerDate = string.IsNullOrWhiteSpace(LowerDataPickerTournaments.Text) ? Convert.ToDateTime("01.01.2021") : Convert.ToDateTime(LowerDataPickerTournaments.Text.Trim());
            upperDate = string.IsNullOrWhiteSpace(UpperDataPickerTournaments.Text) ? DateTime.Now : Convert.ToDateTime(UpperDataPickerTournaments.Text.Trim());
            lowerPrize = string.IsNullOrWhiteSpace(LowerPrizeTournamentsTBox.Text) ? 0 : Convert.ToInt32(LowerPrizeTournamentsTBox.Text.Trim());
            upperPrize = string.IsNullOrWhiteSpace(UpperPrizeTournamentsTBox.Text) ? 99999999 : Convert.ToInt32(UpperPrizeTournamentsTBox.Text.Trim());
            withFreePlace = (bool)FreePlaceTournamentsCB.IsChecked;

            foreach (Item item in DiciplineFilterTournamentsLB.Items)
            {
                if (item.IsSelected)
                {
                    disciplines.Add(item.ItemId);
                }
            }

            string disciplinesString = disciplines.Count == 0 ? "*" : string.Join(",", disciplines);

            // проверка на корректность введенных значений
            if (DateTime.Compare(Convert.ToDateTime(lowerDate), Convert.ToDateTime(upperDate)) > 0)
            {
                MessageBox.Show("Некорректно введены даты!");
                return;
            }

            // получаем список заявок
            List<Tournaments> apps = new List<Tournaments>();
            if (disciplinesString == "*")
            {
                if (withFreePlace)
                {
                    apps = _context.Tournaments.Where(app => app.DateStart >= lowerDate && app.DateStart <= upperDate && app.PrizeFond >= lowerPrize && app.PrizeFond <= upperPrize
                    && app.MaxTeamsCount > _context.Matches.Where(m => m.FK_Tournament_Id == app.Id && m.RoundNum == 1).Count()).ToList();
                }
                else
                {
                    apps = _context.Tournaments.Where(app => app.DateStart >= lowerDate && app.DateStart <= upperDate && app.PrizeFond >= lowerPrize && app.PrizeFond <= upperPrize).ToList();
                }
            }
            else
            {
                foreach (int discipline in disciplines)
                {
                    if (withFreePlace)
                    {
                        apps.AddRange(_context.Tournaments.Where(app => app.DateStart >= lowerDate && app.DateStart <= upperDate && app.PrizeFond >= lowerPrize && app.PrizeFond <= upperPrize
                        && app.Users.FK_Discipline_Id == discipline && app.MaxTeamsCount > _context.Matches.Where(m => m.FK_Tournament_Id == app.Id && m.RoundNum == 1).Count()).ToList());
                    }
                    else
                    {
                        apps.AddRange(_context.Tournaments.Where(app => app.DateStart >= lowerDate && app.DateStart <= upperDate && app.PrizeFond >= lowerPrize && app.PrizeFond <= upperPrize
                        && app.Users.FK_Discipline_Id == discipline).ToList());
                    }
                }
            }

            // заполняем список заявок
            List<TournamentApplication> tournamentApps = new List<TournamentApplication>();
            foreach (Tournaments app in apps)
            {
                TournamentApplication tournamentApp = new TournamentApplication();
                tournamentApp.TournamentId = app.Id.ToString();
                tournamentApp.Title = app.Name;
                tournamentApp.Date = app.DateStart.ToString("d") + " - " + app.DateEnd.ToString("d");
                tournamentApp.PrizeFond = app.PrizeFond.ToString();
                tournamentApp.PhoneNumber = app.Users.PhoneNumber;
                tournamentApp.Email = app.Users.Email;

                if (string.IsNullOrEmpty(app.LogoPath))
                {
                    tournamentApp.LogoPath = @"C:\Users\nikich4523\source\repos\Курсовой проект 3.1\Images\WithoutPhoto.png";
                }
                else
                {
                    tournamentApp.LogoPath = app.LogoPath;
                }
                tournamentApp.Venue = app.Venue;

                if (app.Users.Disciplines != null)
                {
                    tournamentApp.DisciplineName = app.Users.Disciplines.Name;
                    tournamentApp.IconPath = app.Users.Disciplines.IconPath;
                }

                int teamCount = _context.Matches.Where(m => m.FK_Tournament_Id == app.Id && m.RoundNum == 1).Count() * 2;
                tournamentApp.TeamCount = teamCount.ToString() + " / " + app.MaxTeamsCount.ToString();
                tournamentApp.Format = app.TournamentFormats.Name;

                if (withFreePlace)
                {
                    if (teamCount < app.MaxTeamsCount)
                    {
                        tournamentApps.Add(tournamentApp);
                    }
                }
                else
                {
                    tournamentApps.Add(tournamentApp);
                }
            }

            applicationsLB.ItemsSource = tournamentApps;
        }

        // AddAppBtns
        private void PlayerAppAddBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayerApplicationAddWindow wnd = new PlayerApplicationAddWindow();
            wnd.ShowDialog();
        }

        private void TeamAppAddBtn_Click(object sender, RoutedEventArgs e)
        {
            TeamApplicationAddWindow wnd = new TeamApplicationAddWindow();
            wnd.ShowDialog();
        }

        private void TeamAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_context.TeamsUsers.Where(tu => tu.FK_User_Id == App.UserId && tu.DateEnd == null).Count() == 0)
            {
                CreateTeamWindow wnd = new CreateTeamWindow();
                wnd.Show();
                Close();
            }
            else
            {
                MessageBox.Show("У вас уже есть команда!");
            }
        }

        private void TournamentAddBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateTournamentWindow wnd = new CreateTournamentWindow();
            wnd.ShowDialog();
        }

        // applicationsLB
        private void applicationsLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (applicationsLB.SelectedItem != null)
            {
                if (applicationsLB.SelectedItem is PlayerApplication)
                {
                    PlayerApplication player = (PlayerApplication)applicationsLB.SelectedItem;
                    MainWindow wnd = new MainWindow(Convert.ToInt32(player.PlayerId));
                    wnd.Show();
                }
                else if (applicationsLB.SelectedItem is PlayerApplication)
                {
                    TeamApplication team = (TeamApplication)applicationsLB.SelectedItem;
                    TeamProfileWindow wnd = new TeamProfileWindow(Convert.ToInt32(team.TeamId));
                    wnd.Show();
                }
                else
                {
                    TournamentApplication tournament = (TournamentApplication)applicationsLB.SelectedItem;
                    TournamentWindow wnd = new TournamentWindow(Convert.ToInt32(tournament.TournamentId));
                    wnd.Show();
                }
                Close();
            }
        }

        // Players filter expander
        private void CountryExpanderPlayers_Expanded(object sender, RoutedEventArgs e)
        {
            CountryFilterExpanderRowPlayers.Height = new GridLength(2, GridUnitType.Star);
            BuffExpanderRowPlayers.Height = new GridLength(0.2, GridUnitType.Star);
            DiciplineExpanderPlayers.IsExpanded = false;
        }

        private void DiciplineExpanderPlayers_Expanded(object sender, RoutedEventArgs e)
        {
            DiciplineFilterExpanderRowPlayers.Height = new GridLength(2, GridUnitType.Star);
            BuffExpanderRowPlayers.Height = new GridLength(0.2, GridUnitType.Star);
            CountryExpanderPlayers.IsExpanded = false;
        }

        private void CountryExpanderPlayers_Collapsed(object sender, RoutedEventArgs e)
        {
            CountryFilterExpanderRowPlayers.Height = new GridLength(0.2, GridUnitType.Star);
            BuffExpanderRowPlayers.Height = new GridLength(0.8, GridUnitType.Star);
        }

        private void DiciplineExpanderPlayers_Collapsed(object sender, RoutedEventArgs e)
        {
            DiciplineFilterExpanderRowPlayers.Height = new GridLength(0.2, GridUnitType.Star);
            BuffExpanderRowPlayers.Height = new GridLength(0.8, GridUnitType.Star);
        }

        // Teams filter expander
        private void CountryExpanderTeams_Expanded(object sender, RoutedEventArgs e)
        {
            CountryFilterExpanderRowTeams.Height = new GridLength(2, GridUnitType.Star);
            BuffExpanderRowTeams.Height = new GridLength(0.2, GridUnitType.Star);
            DiciplineExpanderTeams.IsExpanded = false;
        }

        private void DiciplineExpanderTeams_Expanded(object sender, RoutedEventArgs e)
        {
            DiciplineFilterExpanderRowTeams.Height = new GridLength(2, GridUnitType.Star);
            BuffExpanderRowTeams.Height = new GridLength(0.2, GridUnitType.Star);
            CountryExpanderTeams.IsExpanded = false;
        }

        private void CountryExpanderTeams_Collapsed(object sender, RoutedEventArgs e)
        {
            CountryFilterExpanderRowTeams.Height = new GridLength(0.2, GridUnitType.Star);
            BuffExpanderRowTeams.Height = new GridLength(0.8, GridUnitType.Star);
        }

        private void DiciplineExpanderTeams_Collapsed(object sender, RoutedEventArgs e)
        {
            DiciplineFilterExpanderRowTeams.Height = new GridLength(0.2, GridUnitType.Star);
            BuffExpanderRowTeams.Height = new GridLength(0.8, GridUnitType.Star);
        }

        // Tournaments filter expander
        private void VenueExpanderTournaments_Expanded(object sender, RoutedEventArgs e)
        {
            VenueFilterExpanderRowTournaments.Height = new GridLength(2, GridUnitType.Star);
            BuffExpanderRowTournaments.Height = new GridLength(0.1, GridUnitType.Star);
            DiciplineExpanderTournaments.IsExpanded = false;
        }

        private void DiciplineExpanderTournaments_Expanded(object sender, RoutedEventArgs e)
        {
            DiciplineFilterExpanderRowTournaments.Height = new GridLength(2, GridUnitType.Star);
            BuffExpanderRowTournaments.Height = new GridLength(0.1, GridUnitType.Star);
            VenueExpanderTournaments.IsExpanded = false;
        }

        private void VenueExpanderTournaments_Collapsed(object sender, RoutedEventArgs e)
        {
            VenueFilterExpanderRowTournaments.Height = new GridLength(0.2, GridUnitType.Star);
            BuffExpanderRowTournaments.Height = new GridLength(0.8, GridUnitType.Star);
        }

        private void DiciplineExpanderTournaments_Collapsed(object sender, RoutedEventArgs e)
        {
            DiciplineFilterExpanderRowTournaments.Height = new GridLength(0.2, GridUnitType.Star);
            BuffExpanderRowTeams.Height = new GridLength(0.8, GridUnitType.Star);
        }

    }

    // Класс, описывающий элементы для фильтрации (Страна, Дисциплина, Место проведения) (ListBox)
    public class Item
    {
        public string ItemName { get; set;}
        public bool IsSelected { get; set; }
        public int ItemId { get; set; }
    }

    // Класс, описывающий заявку от игрока
    public class PlayerApplication
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Age { get; set; }
        public string DisciplineName { get; set; }
        public string PhotoURL { get; set; }
        public string CountryName { get; set; }
        public string CountryURL { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string PlayerId { get; set; }
    }

    // Класс, описывающий заявку от команды
    public class TeamApplication
    {
        public string Name { get; set; }
        public string DateOfFoundation { get; set; }
        public string DisciplineName { get; set; }
        public string LogoURL { get; set; }
        public string CountryName { get; set; }
        public string CountryURL { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string TeamId { get; set; }
    }

    // Класс, описывающий заявку от команды
    public class TournamentApplication
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string DisciplineName { get; set; }
        public string IconPath { get; set; }
        public string LogoPath { get; set; }
        public string Venue { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PrizeFond { get; set; }
        public string Format { get; set; }
        public string TeamCount { get; set; }
        public string Title { get; set; }
        public string TournamentId { get; set; }
    }
}
