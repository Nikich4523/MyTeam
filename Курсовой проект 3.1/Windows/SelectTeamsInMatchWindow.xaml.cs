using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для SelectTeamsInMatchWindow.xaml
    /// </summary>
    public partial class SelectTeamsInMatchWindow : Window
    {
        MyTeamContext _context;
        int nowTournamentId;
        int roundNum;

        public SelectTeamsInMatchWindow(int tournamentId, int roundNum)
        {
            InitializeComponent();
            _context = new MyTeamContext();
            nowTournamentId = tournamentId;
            this.roundNum = roundNum;
        }

        private void TeamList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                if (listBox.Name == "FrstList")
                {
                    Item item = (Item)listBox.SelectedItem;
                    FrstTitleTBlock.Text = item.TeamName;
                }
                else
                {
                    Item item = (Item)listBox.SelectedItem;
                    ScndTitleTBlock.Text = item.TeamName;
                }
            }
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            // если команды не выбраны
            if (FrstTitleTBlock.Text == "Команда №1" || ScndTitleTBlock.Text == "Команда №2")
            {
                MessageBox.Show("Выберите обе команды!");
                return;
            }

            // если выбраны одинаковые команды
            if (FrstTitleTBlock.Text == ScndTitleTBlock.Text)
            {
                MessageBox.Show("Выберите разные команды!");
                return;
            }

            MatchWindow wnd = new MatchWindow(nowTournamentId, FrstTitleTBlock.Text, ScndTitleTBlock.Text, roundNum);
            wnd.Show();
            Close();
        }

        private void FrstSearchTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                List<Matches> matchList = new List<Matches>();
                List<Teams> teamList = new List<Teams>();
                if (roundNum != 1)
                {
                    matchList = _context.Matches.Where(t => (DbFunctions.Like(t.Teams.Name, "%" + FrstSearchTBox.Text + "%") || DbFunctions.Like(t.Teams1.Name, "%" + FrstSearchTBox.Text + "%"))
                                                           && t.RoundNum == 1 && t.FK_Tournament_Id == nowTournamentId ).ToList();
                }
                else
                {
                    teamList = _context.Teams.Where(t => DbFunctions.Like(t.Name, "%" + FrstSearchTBox.Text + "%")).ToList();
                }

                foreach (Matches match in matchList)
                {
                    teamList.Add(match.Teams);
                    teamList.Add(match.Teams1);
                }

                List<Item> itemList = new List<Item>();
                foreach (Teams team in teamList)
                {
                    if (team.DissolationDate == null && team.Users.Disciplines.Id == _context.Users.Find(App.UserId).Disciplines.Id)
                    {
                        Item item = new Item()
                        {
                            TeamName = team.Name,
                            LogoPath = team.LogoPath
                            //Country = team.Countries.Name,
                            //Discipline = team.Users.Disciplines.Name,
                            //DateOfFoundation = team.FoundationDate.ToString("d")
                        };
                        itemList.Add(item);
                    }
                }

                FrstList.ItemsSource = new List<byte>();
                FrstList.ItemsSource = itemList;
            }
        }

        private void ScndSearchTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                List<Matches> matchList = new List<Matches>();
                List<Teams> teamList = new List<Teams>();
                if (roundNum != 1)
                {
                    matchList = _context.Matches.Where(t => (DbFunctions.Like(t.Teams.Name, "%" + ScndSearchTBox.Text + "%") || DbFunctions.Like(t.Teams1.Name, "%" + ScndSearchTBox.Text + "%"))
                                                           && t.RoundNum == 1 && t.FK_Tournament_Id == nowTournamentId).ToList();
                }
                else
                {
                    teamList = _context.Teams.Where(t => DbFunctions.Like(t.Name, "%" + ScndSearchTBox.Text + "%")).ToList();
                }

                foreach (Matches match in matchList)
                {
                    teamList.Add(match.Teams);
                    teamList.Add(match.Teams1);
                }

                List<Item> itemList = new List<Item>();
                foreach (Teams team in teamList)
                {
                    if (team.DissolationDate == null && team.Users.Disciplines.Id == _context.Users.Find(App.UserId).Disciplines.Id)
                    {
                        Item item = new Item()
                        {
                            TeamName = team.Name,
                            LogoPath = team.LogoPath
                            //Country = team.Countries.Name,
                            //Discipline = team.Users.Disciplines.Name,
                            //DateOfFoundation = team.FoundationDate.ToString("d")
                        };
                        itemList.Add(item);
                    }
                }

                ScndList.ItemsSource = new List<byte>();
                ScndList.ItemsSource = itemList;
            }
        }
    }

    class Item
    {
        public string TeamName { get; set; }
        public string LogoPath { get; set; }
        public string Discipline { get; set; }
        public string Country { get; set; }
        public string DateOfFoundation { get; set; }
    }
}
