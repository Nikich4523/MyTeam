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

namespace Курсовой_проект_3._1.Windows
{
    /// <summary>
    /// Логика взаимодействия для MatchWindow.xaml
    /// </summary>
    public partial class MatchWindow : Window
    {
        MyTeamContext _context;
        int nowTournamentId;
        int roundNum;

        public MatchWindow(int tournamentId, string frstTeamName, string scndTeamName, int roundNum)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTournamentId = tournamentId;
            this.roundNum = roundNum;

            FrstTeamTBlock.Text = frstTeamName;
            ScndTeamTBlock.Text = scndTeamName;
        }

        public MatchWindow(MatchControl match, int tournamentId, int roundNum)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTournamentId = tournamentId;
            this.roundNum = roundNum;

            FrstTeamTBlock.Text = match.TeamNameFrst;
            ScndTeamTBlock.Text = match.TeamNameScnd;

            FrstTeamScoreTBox.Text = match.ScoreFrst;
            ScndTeamScoreTBox.Text = match.ScoreScnd;
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            List<Matches> match = _context.Matches.Where(m => m.FK_Tournament_Id == nowTournamentId && m.RoundNum == roundNum
                                                           && m.Teams.Name == FrstTeamTBlock.Text && m.Teams1.Name == ScndTeamTBlock.Text).ToList();
            if (match.Count() != 0)
            {
                Matches editMatch = match[0];
                editMatch.FK_FrstTeam_Id = _context.Teams.Where(t => t.Name == FrstTeamTBlock.Text).Select(t => t.Id).Single();
                editMatch.FK_ScndTeam_Id = _context.Teams.Where(t => t.Name == ScndTeamTBlock.Text).Select(t => t.Id).Single();
                editMatch.FrstScore = Convert.ToInt32(FrstTeamScoreTBox.Text); 
                editMatch.ScndScore = Convert.ToInt32(ScndTeamScoreTBox.Text);

                if ((bool)FrstTeamRB.IsChecked)
                {
                    editMatch.FK_WinnerTeam_Id = editMatch.FK_FrstTeam_Id;
                }
                else if ((bool)ScndTeamRB.IsChecked)
                {
                    editMatch.FK_WinnerTeam_Id = editMatch.FK_ScndTeam_Id;
                }
                else
                {
                    editMatch.FK_WinnerTeam_Id = null;
                }
            }
            else
            {
                string frstTeam = FrstTeamTBlock.Text;
                string scndTeam = ScndTeamTBlock.Text;

                Matches newMatch = new Matches()
                {
                    FK_FrstTeam_Id = _context.Teams.Where(t => t.Name == frstTeam).Select(t => t.Id).Single(),
                    FK_ScndTeam_Id = _context.Teams.Where(t => t.Name == scndTeam).Select(t => t.Id).Single(),
                    FrstScore = Convert.ToInt32(FrstTeamScoreTBox.Text),
                    ScndScore = Convert.ToInt32(ScndTeamScoreTBox.Text),
                    RoundNum = roundNum,
                    GameDate = DateTime.Now,
                    FK_Tournament_Id = nowTournamentId
                };

                if ((bool)FrstTeamRB.IsChecked)
                {
                    newMatch.FK_WinnerTeam_Id = newMatch.FK_FrstTeam_Id;
                }
                else if ((bool)ScndTeamRB.IsChecked)
                {
                    newMatch.FK_WinnerTeam_Id = newMatch.FK_ScndTeam_Id;
                }
                else
                {
                    newMatch.FK_WinnerTeam_Id = null;
                }

                _context.Matches.Add(newMatch);
            }

            _context.SaveChanges();

            // Закрываем окно и пересоздаем окно турнира
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

        private void TeamTBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                SelectTeamsInMatchWindow wnd = new SelectTeamsInMatchWindow(nowTournamentId, roundNum);
                wnd.ShowDialog();
                Close();
            }
        }
    }
}
