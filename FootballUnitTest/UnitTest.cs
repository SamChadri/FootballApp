
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FootballDataLibrary;
using UserAccountLibrary;
using FootballApp.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballUnitTest
{
    [TestClass]
    public class UserAccountTest
    {
        private static string username;
        private static string access;
        private static string email;
        private static string firstName;
        private static string lastName;
        private static int userId;
        private static User account;

        [ClassInitialize]
        public async static Task init(TestContext ctx)
        {
            await UserAccount.InitializeDatabase();

            username = "testUser";
            access = "coach";
            email = "smith@illinois.edu";
            firstName = "Lovie";
            lastName = "Smith";
        }




        [TestMethod]
        public void TestUser()
        {

            Assert.AreEqual(UserAccount.InsertUser(username, access, email, firstName, lastName),1);
            account = UserAccount.GetUser(username);

            Assert.IsNotNull(account);
            Assert.AreEqual(username, account.username);
            Assert.AreEqual(access, account.access);
            Assert.AreEqual(email, account.email);
            Assert.AreEqual(firstName, account.firstName);
            Assert.AreEqual(lastName, account.lastName);

            userId = account.userId;

            Assert.AreEqual(UserAccount.DeleteUser(account.userId),1);

        }


        [TestMethod]
        public async Task TestAuthentication()
        {
            Assert.AreEqual(UserAccount.InsertUser(username, access, email, firstName, lastName),1);
            account = UserAccount.GetUser(username);

            Assert.IsTrue(await MicrosoftPassportHelper.CreatePassportKeyAsync(username, account.userId));

            Assert.IsTrue(await MicrosoftPassportHelper.GetAuthenticationMessageAsync(account));
            Assert.IsTrue(await MicrosoftPassportHelper.RemovePassportAccountAsync(account));


        }



        [TestMethod]
        public void TestSelectUser()
        {
            Assert.AreEqual(UserAccount.InsertUser(username, access, email, firstName, lastName), 1);
            User _user = UserAccount.GetUserByName(firstName, lastName);

            Assert.IsNotNull(_user);

            Assert.AreEqual(username, _user.username);
            Assert.AreEqual(access, _user.access);
            Assert.AreEqual(email, _user.email);
            Assert.AreEqual(firstName, _user.firstName);
            Assert.AreEqual(lastName, _user.lastName);

            Assert.IsTrue(UserAccount.ValidateAccount(username));
        }

        [ClassCleanup]
        public static void cleanup()
        {
            UserAccount.DeleteTable("Users");
            UserAccount.DeleteTable("UserKeys");
        }

    }

    [TestClass]
    public class FootballDataTest
    {
        public static Play testPlay;
        public static Game testGame;
        public static Player testPlayer;


        [ClassInitialize]
        public async static Task init(TestContext ctx)
        {
            await FootballData.InitializeDatabase();
            testPlay = new Play(1, 1, "NKL-OVER", 64, 1, 1, 3, 'R', "INT", "", true, "DEPTH", "Defensive Line", 1);
            testGame = new Game(1, 1, new DateTime(2019, 10, 1), "Ohio State", 'W', 43, 20);
            testPlayer = new Player(1, 64, "Test Player", "Defensive Line", "FR");
        }

        [TestMethod]
        public void TestAddGame()
        {
            Assert.AreEqual(FootballData.AddGame(testGame.number, testGame.date.ToString("yyyy-MM-dd"), testGame.opponent,
                testGame.result, testGame.points, testGame.opPoints), 1);

            List<Game> result = FootballData.GetGames();
            Assert.IsTrue(result.Count == 1);

            Game _game = result[0];
            Assert.IsNotNull(_game);
            Assert.AreEqual(testGame.number, _game.number);
            Assert.AreEqual(testGame.date, _game.date);
            Assert.AreEqual(testGame.opponent, _game.opponent);
            Assert.AreEqual(testGame.result, _game.result);
            Assert.AreEqual(testGame.points, _game.points);
            Assert.AreEqual(testGame.opPoints, _game.opPoints);

            Assert.AreEqual(FootballData.DeleteGame(_game.id), 1);

        }

        [TestMethod]
        public void TestAddPlayer()
        {
            Assert.AreEqual(FootballData.AddPlayer(testPlayer.number, testPlayer.name, testPlayer.position, testPlayer.year), 1);

            List<Player> result = FootballData.GetPlayers();
            Assert.IsTrue(result.Count == 1);

            Player _player = result[0];
            Assert.IsNotNull(_player);
            Assert.AreEqual(testPlayer.number, _player.number);
            Assert.AreEqual(testPlayer.name, _player.name);
            Assert.AreEqual(testPlayer.position, _player.position);
            Assert.AreEqual(testPlayer.year, _player.year);

            Assert.AreEqual(FootballData.DeletePlayer(_player.id), 1);


        }

        [TestMethod]
        public void TestAddPlay()
        {
           Assert.AreEqual(FootballData.AddPlay(testPlay.playerNum, testPlay.calls, testPlay.playerNum, testPlay.tech, testPlay.purs, testPlay.mtp,
                testPlay.type, testPlay.stat1, testPlay.stat2, testPlay.loaf, testPlay.comment, testPlay.position, testPlay.gameNum), 1);

            List<Play> result = FootballData.GetPlays();
            Assert.IsTrue(result.Count == 1);

            Play _play = result[0];
            Assert.IsNotNull(_play);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.calls, _play.calls);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.tech, _play.tech);
            Assert.AreEqual(testPlay.purs, _play.purs);
            Assert.AreEqual(testPlay.mtp, _play.mtp);
            Assert.AreEqual(testPlay.type, _play.type);
            Assert.AreEqual(testPlay.stat1, _play.stat1);
            Assert.AreEqual(testPlay.stat2, _play.stat2);
            Assert.AreEqual(testPlay.loaf, _play.loaf);
            Assert.AreEqual(testPlay.comment, _play.comment);
            Assert.AreEqual(testPlay.position, _play.position);
            Assert.AreEqual(testPlay.gameNum, _play.gameNum);

            Assert.AreEqual(FootballData.DeletePlay(_play.id), 1);


        }

        [TestMethod]
        public void TestGetPlayerPlays()
        {
            FootballData.AddPlay(testPlay.playerNum, testPlay.calls, testPlay.playerNum, testPlay.tech, testPlay.purs, testPlay.mtp,
                testPlay.type, testPlay.stat1, testPlay.stat2, testPlay.loaf, testPlay.comment, testPlay.position, testPlay.gameNum);

            List<Play> result = FootballData.GetPlayerPlays(testPlayer.number);
            Assert.IsTrue(result.Count == 1);

            Play _play = result[0];
            Assert.IsNotNull(_play);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.calls, _play.calls);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.tech, _play.tech);
            Assert.AreEqual(testPlay.purs, _play.purs);
            Assert.AreEqual(testPlay.mtp, _play.mtp);
            Assert.AreEqual(testPlay.type, _play.type);
            Assert.AreEqual(testPlay.stat1, _play.stat1);
            Assert.AreEqual(testPlay.stat2, _play.stat2);
            Assert.AreEqual(testPlay.loaf, _play.loaf);
            Assert.AreEqual(testPlay.comment, _play.comment);
            Assert.AreEqual(testPlay.position, _play.position);
            Assert.AreEqual(testPlay.gameNum, _play.gameNum);

            Assert.AreEqual(FootballData.DeletePlay(_play.id), 1);

        }

        [TestMethod]
        public void TestPlayerGamePlays()
        {
            FootballData.AddPlay(testPlay.playerNum, testPlay.calls, testPlay.playerNum, testPlay.tech, testPlay.purs, testPlay.mtp,
    testPlay.type, testPlay.stat1, testPlay.stat2, testPlay.loaf, testPlay.comment, testPlay.position, testPlay.gameNum);

            List<Play> result = FootballData.GetPlayerGamePlays(testPlayer.number, testGame.number);
            Assert.IsTrue(result.Count == 1);

            Play _play = result[0];
            Assert.IsNotNull(_play);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.calls, _play.calls);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.tech, _play.tech);
            Assert.AreEqual(testPlay.purs, _play.purs);
            Assert.AreEqual(testPlay.mtp, _play.mtp);
            Assert.AreEqual(testPlay.type, _play.type);
            Assert.AreEqual(testPlay.stat1, _play.stat1);
            Assert.AreEqual(testPlay.stat2, _play.stat2);
            Assert.AreEqual(testPlay.loaf, _play.loaf);
            Assert.AreEqual(testPlay.comment, _play.comment);
            Assert.AreEqual(testPlay.position, _play.position);
            Assert.AreEqual(testPlay.gameNum, _play.gameNum);

            Assert.AreEqual(FootballData.DeletePlay(_play.id), 1);


        }

        [TestMethod]
        public void TestGetPositionPlays()
        {
            FootballData.AddPlay(testPlay.playerNum, testPlay.calls, testPlay.playerNum, testPlay.tech, testPlay.purs, testPlay.mtp,
    testPlay.type, testPlay.stat1, testPlay.stat2, testPlay.loaf, testPlay.comment, testPlay.position, testPlay.gameNum);

            List<Play> result = FootballData.GetPositionPlays(testPlay.position);
            Assert.IsTrue(result.Count == 1);

            Play _play = result[0];
            Assert.IsNotNull(_play);

            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.calls, _play.calls);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.tech, _play.tech);
            Assert.AreEqual(testPlay.purs, _play.purs);
            Assert.AreEqual(testPlay.mtp, _play.mtp);
            Assert.AreEqual(testPlay.type, _play.type);
            Assert.AreEqual(testPlay.stat1, _play.stat1);
            Assert.AreEqual(testPlay.stat2, _play.stat2);
            Assert.AreEqual(testPlay.loaf, _play.loaf);
            Assert.AreEqual(testPlay.comment, _play.comment);
            Assert.AreEqual(testPlay.position, _play.position);
            Assert.AreEqual(testPlay.gameNum, _play.gameNum);

            Assert.AreEqual(FootballData.DeletePlay(_play.id), 1);

        }

        [TestMethod]
        public void TestPositionGamePlays()
        {
            FootballData.AddPlay(testPlay.playerNum, testPlay.calls, testPlay.playerNum, testPlay.tech, testPlay.purs, testPlay.mtp,
testPlay.type, testPlay.stat1, testPlay.stat2, testPlay.loaf, testPlay.comment, testPlay.position, testPlay.gameNum);

            List<Play> result = FootballData.GetPositionGamePlays(testPlay.position, testGame.number);
            Assert.IsTrue(result.Count == 1);

            Play _play = result[0];
            Assert.IsNotNull(_play);


            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.calls, _play.calls);
            Assert.AreEqual(testPlay.playerNum, _play.playerNum);
            Assert.AreEqual(testPlay.tech, _play.tech);
            Assert.AreEqual(testPlay.purs, _play.purs);
            Assert.AreEqual(testPlay.mtp, _play.mtp);
            Assert.AreEqual(testPlay.type, _play.type);
            Assert.AreEqual(testPlay.stat1, _play.stat1);
            Assert.AreEqual(testPlay.stat2, _play.stat2);
            Assert.AreEqual(testPlay.loaf, _play.loaf);
            Assert.AreEqual(testPlay.comment, _play.comment);
            Assert.AreEqual(testPlay.position, _play.position);
            Assert.AreEqual(testPlay.gameNum, _play.gameNum);

            Assert.AreEqual(FootballData.DeletePlay(_play.id), 1);

        }

        [ClassCleanup]
        public static void clean()
        {

            FootballData.DeleteTable("Games");
            FootballData.DeleteTable("Players");
            FootballData.DeleteTable("Plays");
        }
    }
}
