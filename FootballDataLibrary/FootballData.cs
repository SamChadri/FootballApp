using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;
using System.Diagnostics;

namespace FootballDataLibrary
{

    public class Game
    {
        public int id { get; }
        public int number { get; }
        public DateTime date { get; }
        public String opponent { get; }
        public char result { get; }
        public int points { get; }
        public int opPoints { get; }



        public Game(int id = -1, int number = -1, DateTime date = new DateTime(), 
            String opponent = "", char result = ' ', int points = -1, int opPoints = -1)
        {
            this.id = id;
            this.date = date;
            this.number = number;
            this.opponent = opponent;
            this.result = result;
            this.points = points;
            this.opPoints = opPoints;
        }
    }

    public class Player
    {
        public int id { get; }
        public int number { get; }
        public String name { get; }
        public String position { get; }
        public String year { get; }
        public Player(int id = -1, int number = -1, String name = "", String position = "", String year = "")
        {
            this.id = id;
            this.number = number;
            this.name = name;
            this.position = position;
            this.year = year;
        }

    }
    public class Play
    {
        public int id { get; }
        public int playNum { get; }
        public String calls { get; }
        public int playerNum { get; }
        public int tech { get; }
        public int purs { get; }
        public int mtp { get; }
        public char type { get; }
        public String stat1 { get; }
        public String stat2 { get; }
        public Boolean loaf { get; }
        public String comment { get; }
        public String position { get; }
        public int gameNum { get; }

        public Play(int id = -1, int playNum = -1, String calls = "", int playerNum = -1, int tech = -1, int purs = -1,
            int mtp = -1, char type = ' ', String stat1 = "", String stat2 = "",
            Boolean loaf = false, String comment = "", String position = "", int gameNum = -1)
        {
            this.id = id;
            this.playNum = playNum;
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
            this.position = position;
            this.gameNum = gameNum;

        }
    }
    public class FootballData
    {
        public async static Task InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("Football.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Games (ID INTEGER PRIMARY KEY, " + 
                     "Number INTEGER, " + "Date TEXT, " + "Opponent VARCHAR(255), Result CHAR(1), Points INTEGER, OpPoints INTEGER);";

                SqliteCommand createGame = new SqliteCommand(tableCommand, db);


                createGame.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Players (ID INTEGER PRIMARY KEY, " + 
                    "Number INTEGER, " + "Name VARCHAR(255), " + " Position VARCHAR(255), Year VARCHAR(25));";

                SqliteCommand createPlayer = new SqliteCommand(tableCommand, db);

                createPlayer.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Plays (ID INTEGER PRIMARY KEY, " + "PlayNum INTEGER, " +
                    "Calls VARCHAR(255), " + "PlayerNum INTEGER, " + "Tech INTEGER, " + " Purs INTEGER, " +
                    "Mtp INTEGER, " + "Type CHAR(1), " + "Stat1 VARCHAR(50), " + "Stat2 VARCHAR(50), " + "Loaf BOOLEAN, " + 
                    "Comment VARCHAR(2048), Position VARCHAR(255), GameNum INTEGER);";

                SqliteCommand createPlay = new SqliteCommand(tableCommand, db);

                createPlay.ExecuteReader();


                db.Close();

            }

        }


        public static bool GetTableStatus()
        {
            bool empty = false;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using(SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "SELECT COUNT(*) FROM Players;";
                SqliteCommand playerCount = new SqliteCommand(tableCommand, db);

                SqliteDataReader query = playerCount.ExecuteReader();

                while (query.Read())
                {
                    empty = !query.HasRows;
                }

                db.Close();

            }
            return empty;

        }

        public static int AddGame(int number, String date,  String opponent, char result, int points, int opPoints)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Games(Number, Date, Opponent, Result, Points, opPoints)" +
                    " VALUES(@Entry1, @Entry2, @Entry3, @Entry4, @Entry5, @Entry6);";
                insertCommand.Parameters.AddWithValue("@Entry1", number);
                insertCommand.Parameters.AddWithValue("@Entry2", date);
                insertCommand.Parameters.AddWithValue("@Entry3", opponent);
                insertCommand.Parameters.AddWithValue("@Entry4", result);
                insertCommand.Parameters.AddWithValue("@Entry5", points);
                insertCommand.Parameters.AddWithValue("@Entry6", opPoints);

                SqliteDataReader query = insertCommand.ExecuteReader();
                retval = query.RecordsAffected;
                db.Close();
            }
            return retval;
        }

        public static int AddPlayer(int number, String name, String position, String year)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Players(Number, Name, Position, Year)" +
                    " VALUES(@Entry1, @Entry2, @Entry3, @Entry4);";
                insertCommand.Parameters.AddWithValue("@Entry1", number);
                insertCommand.Parameters.AddWithValue("@Entry2", name);
                insertCommand.Parameters.AddWithValue("@Entry3", position);
                insertCommand.Parameters.AddWithValue("@Entry4", year);


                SqliteDataReader query = insertCommand.ExecuteReader();
                retval = query.RecordsAffected;

                db.Close();

            }
            return retval;

        }

        public static int AddPlay( int playNum, String calls, int playerNum, int tech, int purs,
            int mtp, char type, String stat1, String stat2, Boolean loaf, String comment, String position, int gameNum)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = 
                    "INSERT INTO Plays(PlayNum, Calls, PlayerNum, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameNum)" + 
                    " VALUES(@Entry, @Entry1, @Entry2, @Entry3, @Entry4, @Entry5, @Entry6, @Entry7, @Entry8, @Entry9, @Entry10, @Entry11, @Entry12 );";
                insertCommand.Parameters.AddWithValue("@Entry", playNum);
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
                insertCommand.Parameters.AddWithValue("@Entry11", position);
                insertCommand.Parameters.AddWithValue("@Entry12", gameNum);


                SqliteDataReader query = insertCommand.ExecuteReader();
                retval = query.RecordsAffected;

                db.Close();

            }
            return retval;

        }
        public static List<Game> GetGames()
        {
            List<Game> games = new List<Game>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Games;";
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    var date = query.GetDateTime(2);
                    Game entry = new Game(query.GetInt32(0), query.GetInt32(1), query.GetDateTime(2),
                        query.GetString(3), query.GetChar(4), query.GetInt32(5), query.GetInt32(6));
                    games.Add(entry);
                }

                db.Close();
            }
            return games;
        }

        public static Game GetGame(int number, string opponent)
        {
            Game retval = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Games WHERE Number=@Entry1 and Opponent=@Entry2;";
                selectCommand.Parameters.AddWithValue("@Entry1", number);
                selectCommand.Parameters.AddWithValue("@Entry2", opponent);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    retval = new Game(query.GetInt32(0), query.GetInt32(1), query.GetDateTime(2),
                        query.GetString(3), query.GetChar(4), query.GetInt32(5), query.GetInt32(6));
                }

                db.Close();
            }
            return retval;
        }

        public static List<Player> GetPlayers()
        {
            List<Player> players = new List<Player>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Players;";
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Player entry = new Player(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetString(3), query.GetString(4));
                    players.Add(entry);
                }

                db.Close();

            }

            return players;

        }

        public static Player GetPlayer(String name )
        {
            Player entry = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Players WHERE Name=@Entry;";
                selectCommand.Parameters.AddWithValue("@Entry", name);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entry = new Player(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetString(3), query.GetString(4));
                }
                

                db.Close();

            }

            return entry;

        }
        public static List<Play> GetPlays()
        {
            List<Play> plays = new List<Play>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Plays;";
                SqliteDataReader query = selectCommand.ExecuteReader();

                while( query.Read())
                {
                    Play entry =
                        new Play(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetInt32(3),
                        query.GetInt32(4), query.GetInt32(5), query.GetInt32(6), query.GetChar(7), query.GetString(8),
                        query.GetString(9), query.GetBoolean(10), query.GetString(11), query.GetString(12), query.GetInt32(13));

                    plays.Add(entry);
                }
                db.Close();
            }

            return plays;
        }

        public static List<Play> GetPositionPlays(String position)
        {
            List<Play> plays = new List<Play>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Plays WHERE Position=@Entry;";
                selectCommand.Parameters.AddWithValue("@Entry", position);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Play entry =
                        new Play(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetInt32(3),
                        query.GetInt32(4), query.GetInt32(5), query.GetInt32(6), query.GetChar(7), query.GetString(8),
                        query.GetString(9), query.GetBoolean(10), query.GetString(11), query.GetString(12), query.GetInt32(13));

                    plays.Add(entry);
                }
                db.Close();
            }
            return plays;
        }

        public static List<Play> GetPlayerPlays(int playerNum)
        {
            List<Play> plays = new List<Play>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Plays WHERE PlayerNum=@Entry;";
                selectCommand.Parameters.AddWithValue("@Entry", playerNum);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Play entry =
                        new Play(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetInt32(3),
                        query.GetInt32(4), query.GetInt32(5), query.GetInt32(6), query.GetChar(7), query.GetString(8),
                        query.GetString(9), query.GetBoolean(10), query.GetString(11), query.GetString(12), query.GetInt32(13)); ;

                    plays.Add(entry);
                }
                db.Close();
            }
            return plays;
        }

        public static List<Play> GetPositionGamePlays(String position, int gameNum)
        {
            List<Play> plays = new List<Play>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Plays WHERE Position=@Entry1 AND GameNum=@Entry2;";
                selectCommand.Parameters.AddWithValue("@Entry1", position);
                selectCommand.Parameters.AddWithValue("@Entry2", gameNum);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Play entry =
                        new Play(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetInt32(3),
                        query.GetInt32(4), query.GetInt32(5), query.GetInt32(6), query.GetChar(7), query.GetString(8),
                        query.GetString(9), query.GetBoolean(10), query.GetString(11), query.GetString(12), query.GetInt32(13));

                    plays.Add(entry);
                }
                db.Close();
            }
            return plays;
        }

        public static List<Play> GetPlayerGamePlays(int playerNum, int gameNum)
        {
            List<Play> plays = new List<Play>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Plays WHERE PlayerNum=@Entry1 AND GameNum=@Entry2;";
                selectCommand.Parameters.AddWithValue("@Entry1", playerNum);
                selectCommand.Parameters.AddWithValue("@Entry2", gameNum);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Play entry =
                        new Play(query.GetInt32(0), query.GetInt32(1), query.GetString(2), query.GetInt32(3),
                        query.GetInt32(4), query.GetInt32(5), query.GetInt32(6), query.GetChar(7), query.GetString(8),
                        query.GetString(9), query.GetBoolean(10), query.GetString(11), query.GetString(12), query.GetInt32(13));

                    plays.Add(entry);
                }
                db.Close();
            }
            return plays;
        }

        public static void DeleteTable(String table)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                if (table.Equals("Games"))
                {
                    deleteCommand.CommandText = "DROP TABLE IF EXISTS Games;";
                    deleteCommand.ExecuteReader();

                }
                else if (table.Equals("Players")) {
                    deleteCommand.CommandText = "DROP TABLE IF EXISTS Players;";
                    deleteCommand.ExecuteReader();

                }
                else if (table.Equals("Plays")) {
                    deleteCommand.CommandText = "DROP TABLE IF EXISTS Plays;";
                    deleteCommand.ExecuteReader();

                }
                else
                {
                    Debug.WriteLine("Table does not exist");
                }

                db.Close();
            }
                
        }

        public static int DeleteGame(int gameId)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Games WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", gameId);

                SqliteDataReader query = deleteCommand.ExecuteReader();

                retval = query.RecordsAffected;

                db.Close();
            }
            return retval;
        }
        
        public static int DeletePlayer(int playerId)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Players WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", playerId);

                SqliteDataReader query = deleteCommand.ExecuteReader();

                retval = query.RecordsAffected;

                db.Close();

            }
            return retval;
        }
        
        public static int DeletePlay(int playId)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Football.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Plays WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", playId);

                SqliteDataReader query = deleteCommand.ExecuteReader();

                retval = query.RecordsAffected;


                db.Close();

            }

            return retval;
        }
    }
}
