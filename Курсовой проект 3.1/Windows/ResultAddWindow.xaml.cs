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
    /// Логика взаимодействия для ResultAddWindow.xaml
    /// </summary>
    public partial class ResultAddWindow : Window
    {
        MyTeamContext _context;
        Achievements achievement;
        int nowTournamentId;

        public ResultAddWindow(int tournamentId)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTournamentId = tournamentId;

            List<string> teamList = new List<string>();
            teamList.AddRange(_context.Matches.Where(m => m.FK_Tournament_Id == nowTournamentId && m.RoundNum == 1).Select(m => m.Teams.Name).ToList());
            teamList.AddRange(_context.Matches.Where(m => m.FK_Tournament_Id == nowTournamentId && m.RoundNum == 1).Select(m => m.Teams1.Name).ToList());

            TeamsCB.ItemsSource = teamList;
        }

        public ResultAddWindow(string TeamName, int tournamentId)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTournamentId = tournamentId;

            achievement = _context.Achievements.Where(a => a.Teams.Name == TeamName && a.FK_Tournament_Id == nowTournamentId).FirstOrDefault();
            PlaceTBox.Text = achievement.Place;
            TeamsCB.Text = achievement.Teams.Name;
            PrizeTBox.Text = Func.ConvertIntToStringMoney(achievement.Prize);
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (achievement != null)
            {
                Achievements editAchievement = achievement;
                editAchievement.Place = PlaceTBox.Text;
                editAchievement.FK_Team_Id = _context.Teams.Where(t => t.Name == TeamsCB.Text).Select(t => t.Id).FirstOrDefault();
                editAchievement.Prize = Convert.ToInt32(PrizeTBox.Text);

                _context.SaveChanges();

                MessageBox.Show("Изменения сохранены!");
            }
            else
            {
                Achievements newAchievement = new Achievements();
                newAchievement.Place = PlaceTBox.Text;
                newAchievement.FK_Team_Id = _context.Teams.Where(t => t.Name == TeamsCB.Text).Select(t => t.Id).FirstOrDefault();
                newAchievement.FK_Tournament_Id = nowTournamentId;
                newAchievement.Prize = Convert.ToInt32(PrizeTBox.Text);

                _context.Achievements.Add(newAchievement);
                _context.SaveChanges();

                MessageBox.Show("Добавлено!");
            }
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wnd in Application.Current.Windows)
            {
                if (wnd is TournamentWindow)
                {
                    TournamentWindow tournamentWnd = (TournamentWindow)wnd;
                    tournamentWnd.Close();

                    TournamentWindow newTournamentWnd = new TournamentWindow(nowTournamentId);
                    newTournamentWnd.Show();
                }
            }

            Close();
        }
    }
}
