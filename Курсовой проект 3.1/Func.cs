using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Курсовой_проект_3._1
{
    public class Func
    {
        static MyTeamContext _context = new MyTeamContext();

        // КОМАНДЫ ///
        // Возвращает название команды
        public static string GetTeamName(int teamId)
        {
            return _context.Teams.Where(t => t.Id == teamId).Select(t => t.Name).ToList()[0];
        }

        // Возвращает список всех команд игрока
        public static List<int> GetTeamIds(int userId)
        {
            return _context.TeamsUsers.Where(dr => dr.FK_User_Id == userId).Select(dr => dr.FK_Team_Id).ToList();
        }

        // Возвращает список всех игроков команды в настоящий момент
        public static List<int> GetPlayerIdsInTeamPresent(int teamId)
        {
            return _context.TeamsUsers.Where(tu => tu.FK_Team_Id == teamId && tu.DateEnd == null).Select(tu => tu.FK_User_Id).ToList();
        }

        // Возвращает список всех игроков команды в прошлом
        public static List<int> GetPlayerIdsInTeamPast(int teamId)
        {
            return  _context.TeamsUsers.Where(tu => tu.FK_Team_Id == teamId && tu.DateEnd != null).Select(tu => tu.FK_User_Id).ToList(); ;
        }


        // КОЛИЧЕСТВО ПОБЕД ///
        // Возвращает количество (побед, ничьих, поражений) игрока во ВСЕХ командах
        public static (int, int, int) GetWinDrawLoseCountUser(int userId)
        {
            int winCounter = 0;
            int loseCounter = 0;
            int drawCounter = 0;

            // Получаем список всех команд
            List<int> teamIds = GetTeamIds(userId);

            // Считаем количество побед, ничьих, поражений
            for (int i = 0; i < teamIds.Count; i++)
            {
                int teamId = teamIds[i];

                int winIncrement, drawIncrement, loseIncrement;

                (winIncrement, drawIncrement, loseIncrement) = GetWinDrawLoseCountUserInOneTeam(userId, teamId);
                winCounter += winIncrement;
                drawCounter += drawIncrement;
                loseCounter += loseIncrement;
            }

            return (winCounter, drawCounter, loseCounter);
        }

        // Возвращает количество (побед, ничьих, поражений) игрока в ОДНОЙ команде
        public static (int, int, int) GetWinDrawLoseCountUserInOneTeam(int userId, int teamId)
        {
            // инициализация счетчиков
            int winCounter = 0;
            int loseCounter = 0;
            int drawCounter = 0;

            // получаем список матчей в игрока в команде
            List<Matches> matchList = GetMatchesUserInTeam(userId, teamId);

            // для каждого матча узнать (победа, ничья, поражение)
            foreach (Matches match in matchList)
            {
                if (match.FK_WinnerTeam_Id == teamId)
                {
                    winCounter++;
                }
                else if (match.FK_WinnerTeam_Id == null)
                {
                    drawCounter++;
                }
                else
                {
                    loseCounter++;
                }
            }  // foreach

            return (winCounter, drawCounter, loseCounter);
        }

        // Возвращает количество (побед, ничьих, поражений) команды
        public static (int, int, int) GetWinDrawLoseCountTeam(int teamId)
        {
            // инициализация счетчиков
            int winCounter = 0;
            int loseCounter = 0;
            int drawCounter = 0;

            // получаем список матчей команды
            List<Matches> matchesList = _context.Matches.Where(m => m.FK_FrstTeam_Id == teamId || m.FK_ScndTeam_Id == teamId).ToList();

            // для каждого матча узнать (победа, ничья, поражение)
            foreach (Matches match in matchesList)
            {
                if (match.FK_WinnerTeam_Id == teamId)
                {
                    winCounter++;
                }
                else if (match.FK_WinnerTeam_Id == null)
                {
                    drawCounter++;
                }
                else
                {
                    loseCounter++;
                }
            }  // foreach

            return (winCounter, drawCounter, loseCounter);
        }


        // МАТЧИ ///
        // Возвращает все матчи игрока в команде
        public static List<Matches> GetMatchesUserInTeam(int userId, int teamId)
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

            // Получаем все матчи игрока в одной команде
            List<Matches> matchList = _context.Matches.Where(m => m.GameDate >= dateStart && m.GameDate <= dateEnd && (m.FK_FrstTeam_Id == teamId || m.FK_ScndTeam_Id == teamId)).OrderByDescending(m => m.GameDate).ToList();

            return matchList;
        }

        // Возвращает все матчи игрока (order by gameDate desc)
        public static List<Matches> GetAllMatchesUser(int userId)
        {
            // получаем список команд
            List<int> teamIds = _context.TeamsUsers.Where(p => p.FK_User_Id == userId).Select(p => p.FK_Team_Id).ToList();

            // для каждой команды получаем матчи
            List<Matches> matchList = new List<Matches>();
            foreach (int teamId in teamIds)
            {
                matchList.AddRange(GetMatchesUserInTeam(userId, teamId));
            }

            return matchList;
        }

        // Возвращает список матчей, в которых участвовала команад
        public static List<int> GetTeamMatchIds(int teamId)
        {
            return _context.Matches.Where(m => m.FK_FrstTeam_Id == teamId || m.FK_ScndTeam_Id == teamId).OrderBy(m => m.GameDate).Select(m => m.Id).ToList();
        }


        // ТУРНИРЫ ///
        // Возвращает название турнира
        public static string GetTournamentName(int tournamentId)
        {
            return _context.Tournaments.Where(t => t.Id == tournamentId).Select(t => t.Name).ToList()[0];
        }

        // Возвращает список турниров, в котором участвовала команда
        public static List<int> GetTeamTournamentIds(int teamId)
        {
            return _context.Achievements.Where(t => t.FK_Team_Id == teamId).Select(t => t.FK_Tournament_Id).ToList();
        }

        // Возвращает место команды в турнире
        public static string GetPlaceInTournament(int tournamentId, int teamId)
        {
            List<string> places = _context.Achievements.Where(a => a.FK_Team_Id == teamId && a.FK_Tournament_Id == tournamentId).Select(t => t.Place).ToList();
            if (places.Count != 0)
                return places[0];

            return String.Empty;
        }

        // Возвращает приз команды за турнир
        public static int GetPrizeInTournament(int tournamentId, int teamId)
        {
            List<int> prizes = _context.Achievements.Where(a => a.FK_Team_Id == teamId && a.FK_Tournament_Id == tournamentId).Select(t => t.Prize).ToList();
            if (prizes.Count() != 0)
                return prizes[0];

            return 0;
        }


        // ПРОЧЕЕ ///
        // Возвращает число в формате 1623451 => 1 623 451
        public static string ConvertIntToStringMoney(int num)
        {
            string moneyStr = num.ToString("#,#").Replace(',', ' ');
            return moneyStr;
        }

        // Возвращает возраст
        public static int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;

            if (DateTime.Now.Month < birthDate.Month || (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
                age--;

            return age;
        }

        // Возвращает True, если все строки содержат значения. В ином случае False
        public static bool IsNullOrWhiteSpace(string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(strs[i]))
                {
                    return true;
                }
            }

            return false;
        }


        // Возвращает True, если логин и пароль корректен. В ином случае False
        public static bool IsValidLogAndPass(string login, string password)
        {
            if (String.IsNullOrWhiteSpace(login) || String.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return true;
        }

        // Возвращает True, если email корректен. В ином случае False
        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$");

            if (regex.IsMatch(email))
            {
                return true;
            }

            return false;
        }

        // Возвращает True, если номер телефона корректен. В ином случае False
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            foreach (char ch in phoneNumber)
            {
                if (!Char.IsDigit(ch))
                {
                    return false;
                }
            }

            return true;
        }

        // Возвращает True, если строка состоит только из чисел. В ином случае False
        public static bool IsOnlyDigitString(string str)
        {
            foreach (char ch in str)
            {
                if (!Char.IsDigit(ch))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
