using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой_проект_3._1
{
    public static class Test
    {
        // for test only
        public static bool PlayerInfoOutput()
        {
            MyTeamContext _context = new MyTeamContext(); 

            return true;
        }

        // for test only
        public static bool CreateTeamApp(int teamId, string title, string text)
        {
            int teamIdWithApp = 1;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (teamId == teamIdWithApp)
            {
                return false;
            }

            if (title.Length > 20)
            {
                return false;
            }

            if (text.Length > 255)
            {
                return false;
            }

            return true;
        }

        // for test only
        public static bool CreatePlayerApp(int userId, string title, string text)
        {
            int userIdWithApp = 1;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            
            if (userId == userIdWithApp)
            {
                return false;
            }

            if (title.Length > 20)
            {
                return false;
            }

            if (text.Length > 255)
            {
                return false;
            }

            return true;
        }

        // for test only
        public static bool ChangeSettingsTest(int userId, string email, string phoneNumber, string about)
        {
            phoneNumber = phoneNumber.TrimStart('+');

            string existEmail, existPhoneNumber;
            existEmail = "nikich4523@mail.ru";
            existPhoneNumber = "79641166987";

            string nowUserEmail, nowUserPhoneNumber;
            nowUserEmail = "alen.20.plo@mail.ru";
            nowUserPhoneNumber = "79132818850";

            // проверка на корректность введенной почты
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (email != nowUserEmail)
                {
                    if (email != existEmail)
                    {
                        if (Func.IsValidEmail(email))
                        {
                            //
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            // проверка на корректность введенного номера телефона
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                if (phoneNumber != nowUserPhoneNumber)
                {

                    if (phoneNumber != existPhoneNumber)
                    {
                        if (Func.IsValidPhoneNumber(phoneNumber))
                        {
                            //
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        // for test only
        public static bool AuthorizationTest(string login, string password)
        {

            if (Func.IsValidLogAndPass(login, password))
            {
                if (login == "nikich4523" && password == "123qaz")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // for test only
        public static bool RegistrationTest(string login, string pass, string fname, string lname, string mname, string nickname, string email, string phoneNumber, string birthday)
        {
            phoneNumber = phoneNumber.TrimStart('+');

            string existLogin, existEmail, existPhoneNumber, existNickname;
            existLogin = "nikich4523";
            existEmail = "nikich4523@mail.ru";
            existNickname = "Arxont";
            existPhoneNumber = "+79132818850";

            // проверка на заполненость всех полей
            if (Func.IsNullOrWhiteSpace(new string[] { login, pass, nickname, fname, lname, mname, birthday }))
            {
                return false;
            }

            // проверка на уникальность полей
            if (login == existLogin || email == existEmail || existNickname == nickname || existPhoneNumber == phoneNumber)
            {
                return false;
            }

            // проверка на корректность
            if (!Func.IsValidPhoneNumber(phoneNumber))
            {
                return false;
            }
            if (!Func.IsValidEmail(email))
            {
                return false;
            }

            return true;
        }
    }
}
