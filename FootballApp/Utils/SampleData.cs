using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballDataLibrary;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace FootballApp.Utils
{

    public class SampleViewModel
    {
        public string position { get; set; }
        public SampleViewModel( String position)
        {
            this.position = position;
        }

        public SampleData data = new SampleData();



        public StatLine groupStat
        {
            get 
            {
                switch (this.position)
                {
                    case "Defensive Line":
                        return this.data.SeasonDline();
                    case "Offensive Line":
                        return this.data.SeasonOline();
                    case "Safeties":
                        return this.data.SeasonSafety();
                    case "Receivers":
                        return this.data.SeasonReceivers();
                }
                return null;
            }
        }

        public StatLine gameStat
        {
            get
            {
                switch (this.position)
                {
                    case "Defensive Line":
                        return this.data.GameDline();
                    case "Offensive Line":
                        return this.data.GameOline();
                    case "Safeties":
                        return this.data.GameSafety();
                    case "Receivers":
                        return this.data.GameReceivers();
                }
                return null;
            }
        }

        public List<StatLine> playerStats
        {
            get
            {
                List<StatLine> retval = new List<StatLine>();
                retval.Add(this.data.generateStatline("#57", false));
                retval.Add(this.data.generateStatline("#21", false));
                retval.Add(this.data.generateStatline("#44", false));
                retval.Add(this.data.generateStatline("#39", false));
                retval.Add(this.data.generateStatline("#20", false));

                return retval;
            }
        }

        public List<StatLine> playerSeasonStats
        {
            get
            {
                List<StatLine> retval = new List<StatLine>();
                retval.Add(this.data.generateStatline("#57", true));
                retval.Add(this.data.generateStatline("#21", true));
                retval.Add(this.data.generateStatline("#44", true));
                retval.Add(this.data.generateStatline("#39", true));
                retval.Add(this.data.generateStatline("#20", true));

                return retval;
            }
        }

    }
    public class SampleData
    {
        private StatLine _seasonDline = new StatLine();

        private StatLine _seasonSafety = new StatLine();

        private StatLine _seasonOline = new StatLine();

        private StatLine _seasonReceivers = new StatLine();

        private StatLine _gameDline = new StatLine();

        private StatLine _gameSafety = new StatLine();

        private StatLine _gameOline = new StatLine();

        private StatLine _gameReceivers = new StatLine();



        public StatLine SeasonDline()
        {
            _seasonDline.setStatLine(408, 265, 142, 66, 203, 116, 76);

            _seasonDline.addStat("BUST");
            _seasonDline.setStat("BUST", 8);

            _seasonDline.addStat("PENALTY");
            _seasonDline.setStat("PENALTY", 2);
            _seasonDline.label = "D-Line";

            return _seasonDline;

        }
        public StatLine SeasonOline()
        {
            _seasonOline.setStatLine(678, 459, 110, 34, 123, 53, 46);

            _seasonOline.addStat("BUST");
            _seasonOline.setStat("BUST", 5);

            _seasonOline.addStat("PENALTY");
            _seasonOline.setStat("PENALTY", 8);
            _seasonOline.label = "O-Line";

            return _seasonOline;

        }
        public StatLine SeasonSafety()
        {
            _seasonSafety.setStatLine(185, 94, 185, 154, 73, 24, 31);

            _seasonSafety.addStat("BUST");
            _seasonSafety.setStat("BUST", 9);

            _seasonSafety.addStat("PENALTY");
            _seasonSafety.setStat("PENALTY", 2);
            _seasonSafety.label = "Safeties";

            return _seasonSafety;
        }

        public StatLine SeasonReceivers()
        {
            _seasonReceivers.setStatLine(167, 112, 214, 196, 43, 36, 20);

            _seasonReceivers.addStat("BUST");
            _seasonReceivers.setStat("BUST", 7);

            _seasonReceivers.addStat("PENALTY");
            _seasonReceivers.setStat("PENALTY", 4);
            _seasonReceivers.label = "Receivers";

            return _seasonReceivers;
        }

        public StatLine GameDline()
        {
            _gameDline.setStatLine(233, 160, 78, 56, 98, 67, 22);

            _gameDline.addStat("BUST");
            _gameDline.setStat("BUST", 8);

            _gameDline.addStat("PENALTY");
            _gameDline.setStat("PENALTY", 2);
            _gameDline.label = "D-Line";

            return _gameDline;
        }

        public StatLine GameOline()
        {
            _gameOline.setStatLine(252, 137, 63, 42, 89, 56, 14);

            _gameOline.addStat("BUST");
            _gameOline.setStat("BUST", 5);

            _gameOline.addStat("PENALTY");
            _gameOline.setStat("PENALTY", 4);
            _gameOline.label = "O-Line";
            return _gameOline;

        }
        public StatLine GameSafety()
        {
            _gameSafety.setStatLine(96, 69, 102, 75, 49, 21, 11);

            _gameSafety.addStat("BUST");
            _gameSafety.setStat("BUST", 3);

            _gameSafety.addStat("PENALTY");
            _gameSafety.setStat("PENALTY", 1);
            _gameSafety.label = "Safety";

            return _gameSafety;
        }

        public StatLine GameReceivers()
        {
            _gameReceivers.setStatLine(56, 23, 114, 78, 19, 11, 5);

            _gameReceivers.addStat("BUST");
            _gameReceivers.setStat("BUST", 3);

            _gameReceivers.addStat("PENALTY");
            _gameReceivers.setStat("PENALTY", 0);
            _gameReceivers.label = "Receivers";

            return _gameReceivers;
        }
        public static void insertGameSamples()
        {
            string format = "yyyy-MM-dd";
            FootballData.AddGame(1, new DateTime(2019, 8, 31).ToString(format), "Akron", 'W', 42, 3);
            FootballData.AddGame(2, new DateTime(2019, 9, 7).ToString(format), "UConn", 'W', 31, 23);
            FootballData.AddGame(3, new DateTime(2019, 9, 14).ToString(format), "EMU", 'L', 31, 4);
            FootballData.AddGame(4, new DateTime(2019, 9, 21).ToString(format), "Nebraska", 'L', 38, 42);
            FootballData.AddGame(5, new DateTime(2019, 10, 5).ToString(format), "Minnesota", 'L', 17, 40);
            FootballData.AddGame(6, new DateTime(2019, 10, 12).ToString(format), "Michigan", 'L', 25, 42);
            FootballData.AddGame(7, new DateTime(2019, 10, 19).ToString(format), "Wisconsin", 'W', 24, 23);
            FootballData.AddGame(8, new DateTime(2019, 10, 26).ToString(format), "Purdue", 'W', 24, 6);
            FootballData.AddGame(9, new DateTime(2019, 11, 2).ToString(format), "Rutgers", 'W', 38, 10);
            FootballData.AddGame(10, new DateTime(2019, 11, 9).ToString(format), "Michigan State", 'W', 37, 34);

            Debug.WriteLine("Inserted Games Successfully");

        }


        //Generates random PlayerSeason Statlines for testing purposes.
        public StatLine generateStatline(String label, bool season)
        {
            var rand = new Random();
            StatLine retval = new StatLine();
            int playNum = 1260;

            if (!season) { playNum = 84; }

            int plays = rand.Next(1, playNum);

            int techTotal = plays, pursTotal = plays;

            int mtpTotal = (int)(plays * .08);

            retval.setStatLine(techTotal, rand.Next(1, techTotal), pursTotal, rand.Next(1, pursTotal),
                mtpTotal, rand.Next(0, mtpTotal), rand.Next(1, 50));


            retval.addStat("BUST");
            retval.setStat("BUST", rand.Next(1, 5));

            retval.addStat("PENALTY");
            retval.setStat("PENALTY", rand.Next(1, 5));

            retval.addStat("AT");
            retval.setStat("AT", rand.Next(1, 5));


            retval.totalPlays = plays;
            retval.label = label;
            retval.loaf = rand.Next(0, 10);

            return retval;
        }

        public static void insertPlayers()
        {
            using (StreamReader r = new StreamReader("Assets/TempData/playerRoster.json"))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);

                foreach(var player in array)
                {
                    var arg = (int)player.number;
                    var arg1 = player.name;
                    var arg2 = player.position;
                    var arg3 = player.year;
                    FootballData.AddPlayer((int)player.number, (string)player.name, (string)player.position, (string)player.year);
                }


            }


        }
    }
}
