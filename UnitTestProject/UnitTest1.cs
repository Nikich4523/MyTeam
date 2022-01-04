using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Курсовой_проект_3._1;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AuthorizationTest()
        {
            string login, password;

            // 1
            login = "nikich4523";
            password = "123qaz";

            Assert.IsTrue(Test.AuthorizationTest(login, password));

            // 2
            login = "gr692_pav";
            password = "uncorrect_pass";

            Assert.IsFalse(Test.AuthorizationTest(login, password));

            // 3
            login = "nikich4523";
            password = "";

            Assert.IsFalse(Test.AuthorizationTest(login, password));

            // 4
            login = "";
            password = "123qaz";

            Assert.IsFalse(Test.AuthorizationTest(login, password));

            // 5
            login = "nikich45233";
            password = "123qaz";

            Assert.IsFalse(Test.AuthorizationTest(login, password));

            // 6
            login = "";
            password = "";

            Assert.IsFalse(Test.AuthorizationTest(login, password));
        }

        [TestMethod]
        public void RegistrationTest()
        {
            string login, password, fname, lname, mname, nickname, email, phoneNumber, birthday;

            // 1
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsTrue(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 2
            login = "";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 3
            login = "newUser";
            password = "";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 4
            login = "newUser";
            password = "123qaz";
            fname = "";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 5
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 6
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 7
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 8
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 9
            login = "nikich4523";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 10
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 11
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+7964116698AAAAA";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 12
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "newNickname";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));

            // 13
            login = "newUser";
            password = "123qaz";
            fname = "Иван";
            lname = "Иванов";
            mname = "Иванович";
            nickname = "Arxont";
            email = "ivan4523@mail.ru";
            phoneNumber = "+79641166980";
            birthday = "02.04.2003";

            Assert.IsFalse(Test.RegistrationTest(login, password, fname, lname, mname, nickname, email, phoneNumber, birthday));
        }

        [TestMethod]
        public void ChangeSettingsTest()
        {
            int userId;
            string email, phoneNumber, about;

            // 1
            userId = 1;
            email = "newEmail@mail.ru";
            phoneNumber = "+79132818850";
            about = "Немного обо мне!)";

            Assert.IsTrue(Test.ChangeSettingsTest(userId, email, phoneNumber, about));

            // 2
            userId = 1;
            email = "newEmail@mail.ru";
            phoneNumber = "+79132818850";
            about = "";

            Assert.IsTrue(Test.ChangeSettingsTest(userId, email, phoneNumber, about));

            // 3
            userId = 1;
            email = "nikich4523@mail.ru";
            phoneNumber = "+79132818850";
            about = "Немного обо мне!)";

            Assert.IsFalse(Test.ChangeSettingsTest(userId, email, phoneNumber, about));

            // 4
            userId = 1;
            email = "newEmail@mail.ru";
            phoneNumber = "+79641166987";
            about = "Немного обо мне!)";

            Assert.IsFalse(Test.ChangeSettingsTest(userId, email, phoneNumber, about));

            // 5
            userId = 1;
            email = "";
            phoneNumber = "";
            about = "Немного обо мне!)";

            Assert.IsFalse(Test.ChangeSettingsTest(userId, email, phoneNumber, about));

            // 6
            userId = 1;
            email = "alen.20.plo@mail.ru";
            phoneNumber = "+79642DDD6432";
            about = "Немного обо мне!)";

            Assert.IsFalse(Test.ChangeSettingsTest(userId, email, phoneNumber, about));

        }

        [TestMethod]
        public void CreatePlayerAppTest()
        {
            string title, text;
            int userId;

            // 1
            userId = 2;
            title = "Ищу команду!";
            text = "Играю в Dota 2. Опыта игры на профессинональной сцене не имею.";

            Assert.IsTrue(Test.CreatePlayerApp(userId, title, text));

            // 2
            userId = 1;
            title = "Ищу команду!";
            text = "Играю в Dota 2. Опыта игры на профессинональной сцене не имею.";

            Assert.IsFalse(Test.CreatePlayerApp(userId, title, text));

            // 3
            userId = 2;
            title = "";
            text = "Играю в Dota 2. Опыта игры на профессинональной сцене не имею.";

            Assert.IsFalse(Test.CreatePlayerApp(userId, title, text));

            // 4
            userId = 2;
            title = "Ищу команду!";
            text = "";

            Assert.IsFalse(Test.CreatePlayerApp(userId, title, text));

            // 5
            userId = 2;
            title = "";
            text = "";

            Assert.IsFalse(Test.CreatePlayerApp(userId, title, text));
        }

        [TestMethod]
        public void CreateTeamAppTest()
        {
            string title, text;
            int teamId;

            // 1
            teamId = 2;
            title = "Ищем игрока!";
            text = "Команда по Counter-Strike ищет игрока на роль снайпера. Ранг глобал или выше!";

            Assert.IsTrue(Test.CreateTeamApp(teamId, title, text));

            // 2
            teamId = 1;
            title = "Ищем игрока!";
            text = "Команда по Counter-Strike ищет игрока на роль снайпера. Ранг глобал или выше!";

            Assert.IsFalse(Test.CreateTeamApp(teamId, title, text));

            // 3
            teamId = 2;
            title = "";
            text = "Команда по Counter-Strike ищет игрока на роль снайпера. Ранг глобал или выше!";

            Assert.IsFalse(Test.CreateTeamApp(teamId, title, text));

            // 4
            teamId = 2;
            title = "Ищем игрока!";
            text = "";

            Assert.IsFalse(Test.CreateTeamApp(teamId, title, text));

            // 5
            teamId = 2;
            title = "";
            text = "";

            Assert.IsFalse(Test.CreateTeamApp(teamId, title, text));
        }

        [TestMethod]
        public void TeamInfoOutput()
        {
            int tournamentId, teamId;
            int expected, result;

            // GetPrizeInTournament ///
            // 1
            tournamentId = 1;
            teamId = 6;
            expected = 0;
            result = Func.GetPrizeInTournament(tournamentId, teamId);

            Assert.AreEqual(expected, result);

            // 2
            tournamentId = 4;
            teamId = 1008;
            expected = 18000000;
            result = Func.GetPrizeInTournament(tournamentId, teamId);

            Assert.AreEqual(expected, result);

            // 3
            tournamentId = 4;
            teamId = 1016;
            expected = 5000000;
            result = Func.GetPrizeInTournament(tournamentId, teamId);

            Assert.AreEqual(expected, result);

            // GetPlaceInTournament ///
            // 1
            string resultStr;
            tournamentId = 1;
            teamId = 6;
            string expectedStr = "";
            resultStr = Func.GetPlaceInTournament(tournamentId, teamId);

            Assert.AreEqual(expectedStr, resultStr);

            // 2
            tournamentId = 4;
            teamId = 1008;
            expectedStr = "1";
            resultStr = Func.GetPlaceInTournament(tournamentId, teamId);

            Assert.AreEqual(expectedStr, resultStr);

            // 3
            tournamentId = 4;
            teamId = 1016;
            expectedStr = "2";
            resultStr = Func.GetPlaceInTournament(tournamentId, teamId);

            Assert.AreEqual(expectedStr, resultStr);


            // GetTeamTournamentIds ///
            // 1 
            List<int> expectedTournamentIds = new List<int>();
            List<int> resultTournamentIds = Func.GetTeamTournamentIds(1);

            Assert.IsTrue(expectedTournamentIds.SequenceEqual(resultTournamentIds));

            // 2 
            expectedTournamentIds = new List<int>() { 4 };
            resultTournamentIds = Func.GetTeamTournamentIds(1008);

            Assert.IsTrue(expectedTournamentIds.SequenceEqual(resultTournamentIds));
        }

        [TestMethod]
        public void PlayerInfoOutput()
        {
            // ConvertToStringMoneyTest ///
            int moneyInt;
            string expectedStr, resultStr;

            // 1
            moneyInt = 40000000;
            expectedStr = "40 000 000";

            resultStr = Func.ConvertIntToStringMoney(moneyInt);

            StringAssert.Equals(expectedStr, resultStr);
            
            // 2
            moneyInt = 0;
            expectedStr = "0";

            resultStr = Func.ConvertIntToStringMoney(moneyInt);
            StringAssert.Equals(expectedStr, resultStr);

            // 3
            moneyInt = 1000;
            expectedStr = "1 000";

            resultStr = Func.ConvertIntToStringMoney(moneyInt);
            StringAssert.Equals(expectedStr, resultStr);

            // 4
            moneyInt = 1623451;
            expectedStr = "1 623 451";

            resultStr = Func.ConvertIntToStringMoney(moneyInt);
            StringAssert.Equals(expectedStr, resultStr);

            // 5
            moneyInt = 235;
            expectedStr = "235";

            resultStr = Func.ConvertIntToStringMoney(moneyInt);
            StringAssert.Equals(expectedStr, resultStr);


            // GetTeamIdsTest ///
            List<int> expectedTeamIds;
            List<int> resultTeamIds;

            // 1
            expectedTeamIds = new List<int>() { };
            resultTeamIds = Func.GetTeamIds(1);
            Assert.IsTrue(expectedTeamIds.SequenceEqual(resultTeamIds));

            // 2
            expectedTeamIds = new List<int>() { 1006 };
            resultTeamIds = Func.GetTeamIds(7);
            Assert.IsTrue(expectedTeamIds.SequenceEqual(resultTeamIds));

            // 3
            expectedTeamIds = new List<int>() { 6 };
            resultTeamIds = Func.GetTeamIds(8);
            Assert.IsTrue(expectedTeamIds.SequenceEqual(resultTeamIds));


            // GetWinDrawLoseCountUser ///
            int winCount, drawCount, loseCount;
            (int, int, int) resultCount;

            // 1
            (winCount, drawCount, loseCount) = (0, 0, 0);
            resultCount = Func.GetWinDrawLoseCountUser(1);

            Assert.AreEqual((winCount, drawCount, loseCount), resultCount);

            // 2
            (winCount, drawCount, loseCount) = (0, 0, 0);
            resultCount = Func.GetWinDrawLoseCountUser(1);

            Assert.AreEqual((winCount, drawCount, loseCount), resultCount);

            // 3
            (winCount, drawCount, loseCount) = (0, 0, 0);
            resultCount = Func.GetWinDrawLoseCountUser(1);

            Assert.AreEqual((winCount, drawCount, loseCount), resultCount);


            // GetTeamMatchIds ///
            List<int> expectedMatchIds;
            List<int> resultMatchIds;

            // 1
            expectedMatchIds = new List<int>();
            resultMatchIds = Func.GetTeamMatchIds(1);

            Assert.IsTrue(expectedMatchIds.SequenceEqual(resultMatchIds));

            // 2
            expectedMatchIds = new List<int>() { 1, 9, 14};
            resultMatchIds = Func.GetTeamMatchIds(6);

            Assert.IsTrue(expectedMatchIds.SequenceEqual(resultMatchIds));

            // 3
            expectedMatchIds = new List<int>() { 5 };
            resultMatchIds = Func.GetTeamMatchIds(1007);

            Assert.IsTrue(expectedMatchIds.SequenceEqual(resultMatchIds));

        }
    }
}
