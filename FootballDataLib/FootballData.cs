using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Microsoft.Data.Sqlite;

namespace FootballDataLib
{

    public class Game
    {
        public int id { get; }
        public int year { get; }
        public String opponent { get; }

        public Game(int id, int year, int opponenet)
        {
            this.id = id;
            this.year = year;
            this.opponent = opponent;
        }
    }

    public class Player
    {
        public int id { get; }
        public int number { get; }
        public String name { get; }
        public String position { get; }
        public Player(int id, int number, String name, String position)
        {
            this.id = id;
            this.number = number;
            this.name = name;
            this.position = position;
        }

    }
    public class Play
    {
        public String calls { get; }
        public int playerNum { get; }
        public Boolean tech { get; }
        public Boolean purs { get; }
        public int mtp { get; }
        public char type { get; }
        public String stat1 { get; }
        public String stat2 { get; }
        public Boolean loaf { get; }
        public String comment { get; }

        public Play(String calls, int playerNum, Boolean tech, Boolean purs,
            int mtp, char type, String stat1, String stat2, Boolean loaf, String comment)
        {
            this.calls = calls;
            this.playerNum = playerNum;
            this.tech = tech;
            this.purs = purs;
            this.mtp = mtp;
            this.type = type;
            this.stat1 = stat1;
            this.stat2 = stat2;
            this.loaf = loaf;
            this.comment = comment;

        }
    }
    public class FootballData
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("Football.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Games (ID INTEGER PRIMARY KEY, " +
                     "Year INTEGER, " + "Number INTEGER, " + "Opponent VARCHAR(255));";

                SqliteCommand createGame = new SqliteCommand(tableCommand, db);


                createGame.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Players (ID INTEGER PRIMARY KEY, " +
                    "Number INTEGER, " + "Name VARCHAR(255), " + " Position VARCHAR(255));";

                SqliteCommand createPlayer = new SqliteCommand(tableCommand, db);

                createPlayer.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Plays (ID INTEGER PRIMARY KEY, " +
                    "Calls VARCHAR(255), " + "PlayerNum INTEGER, " + "Tech BOOLEAN, " + " PURS BOOLEAN, " +
                    "MTP INTEGER, " + "Type CHAR(1), " + "Stat1 VARCHAR(50), " + "Stat2 VARCHAR(50), " + "Loaf BOOLEAN, " + "Comment VARCHAR(2048));";

                SqliteCommand createPlay = new SqliteCommand(tableCommand, db);

                createPlay.ExecuteReader();

            }
        }

        public static void AddGame(int year, int number, String opponent)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filenmae=football.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Games VALUES(NULL, @Entry1, @Entry2, @Entry3 );";
                insertCommand.Parameters.AddWithValue("@Entry1", year);
                insertCommand.Parameters.AddWithValue("@Entry2", number);
                insertCommand.Parameters.AddWithValue("@Entry3", opponent);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public static void AddPlayer(int number, String name, String position)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Players VALUES(NULL, @Entry1, @Entry2, @Entry3);";
                insertCommand.Parameters.AddWithValue("@Entry1", number);
                insertCommand.Parameters.AddWithValue("@Entry2", name);
                insertCommand.Parameters.AddWithValue("@Entry3", position);


                insertCommand.ExecuteReader();

                db.Close();

            }

        }

        public static void AddPlay(String calls, int playerNum, Boolean tech, Boolean purs,
            int mtp, char type, String stat1, String stat2, Boolean loaf, String comment)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText =
                    "INSERT INTO Plays VALUES(NULL, @Entry1, @Entry2, @Entry3, @Entry4, @Entry5, @Entry6, @Entry7, @Entry8, @Entry9, @Entry10 );";
                insertCommand.Parameters.AddWithValue("@Entry1", calls);
                insertCommand.Parameters.AddWithValue("@Entry2", playerNum);
                insertCommand.Parameters.AddWithValue("@Entry3", tech);
                insertCommand.Parameters.AddWithValue("@Entry4", purs);
                insertCommand.Parameters.AddWithValue("@Entry5", mtp);
                insertCommand.Parameters.AddWithValue("@Entry6", type);
                insertCommand.Parameters.AddWithValue("@Entry7", stat1);
                insertCommand.Parameters.AddWithValue("@Entry8", stat2);
                insertCommand.Parameters.AddWithValue("@Entry9", loaf);
                insertCommand.Parameters.AddWithValue("@Entry10", comment);

                insertCommand.ExecuteReader();

                db.Close();

            }

        }
        public static List<Game> GetGames()
        {
            List<Game> games = new List<Game>();
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Games;";
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Game entry = new Game(query.GetInt32(0), query.GetInt32(1), query.GetInt32(2));
                    games.Add(entry);
                }

                db.Close();
            }
            return games;
        }
        public static List<Player> GetPlayers()
        {
            List<Player> players = new List<Player>();
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Players;";
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Player entry = new Player(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetString(3));
                    players.Add(entry);
                }

                db.Close();

            }

            return players;

        }
        public static List<Play> GetPlays()
        {
            List<Play> plays = new List<Play>();
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Plays";
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Play entry =
                        new Play(query.GetString(0), query.GetInt32(1), query.GetBoolean(2), query.GetBoolean(3),
                        query.GetInt32(4), query.GetChar(5), query.GetString(6), query.GetString(7), query.GetBoolean(8), query.GetString(9));

                    plays.Add(entry);
                }
                db.Close();
            }

            return plays;
        }

        public static void DeleteTable(String table)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DROP TABLE @Entry";
                deleteCommand.Parameters.AddWithValue("@Entry", table);

                deleteCommand.ExecuteReader();

                db.Close();
            }

        }

        public static void DeleteGame(int gameId)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Games WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", gameId);

                deleteCommand.ExecuteReader();

                db.Close();
            }
        }

        public static void DeletePlayer(int playerId)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=football.db"))
            {
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Players WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", playerId);

                deleteCommand.ExecuteReader();

                db.Close();

            }
        }

        public static void DeletePlay(int playId)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filname=football.db"))
            {
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Plays WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", playId);

                deleteCommand.ExecuteReader();

                db.Close();

            }
        }
    }
}

