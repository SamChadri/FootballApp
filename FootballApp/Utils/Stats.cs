using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballDataLibrary;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace FootballApp.Utils
{
    public class Stat
    {
        public String statName { get; set; }

        public int statVal { get; set; }

        public Stat(String statName, int statVal)
        {
            this.statName = statName;
            this.statVal = statVal;
        }
    }
    public class StatLine
    {
        public double techTotal { get; set; }
        public double tech { get; set; }

        public double pursTotal { get; set; }
        public double purs { get; set; }

        public double mtpTotal { get; set; }
        public double mtp { get; set;}

        public int totalPlays { get; set; }

        public String label { get; set; }

        public void setStatLine( int techTotal, int tech, int pursTotal, int purs, 
            int mtpTotal, int mtp, int loaf)
        {
            this.techTotal = techTotal;
            this.tech = tech;
            this.pursTotal = pursTotal;
            this.purs = purs;
            this.mtpTotal = mtpTotal;
            this.mtp = mtp;
            this.loaf = loaf;
        }

        private Hashtable statDict = new Hashtable();
        private ObservableCollection<Stat> statList = new ObservableCollection<Stat>();


        public int loaf { get; set; }

        public ObservableCollection<Stat> StatList
        {
            get
            {
                foreach(DictionaryEntry entry in statDict)
                {
                    statList.Add(new Stat((string)entry.Key, (int)entry.Value));
                }
                return statList;
            }
        }

        public Boolean checkStat(String stat)
        {
            return statDict.ContainsKey(stat);

        }
        public void addStat(String stat)
        {
            statDict.Add(stat, 1);
        }
        public void updateStat(String stat)
        {
            statDict[stat] = (int)statDict[stat] + 1;
        }

        public void setStat(String stat, int num)
        {
            statDict[stat] = num;
        }
        public int techPercent
        {
            get {
                if (this.pursTotal == 0) { return 0; }
                return (int)((this.tech / this.techTotal) * 100);
            }
        }

        public int pursPercent
        {
            get
            {
                if(this.pursTotal == 0) { return 0; }
                return (int)((this.purs / this.pursTotal) * 100); 
            }
        }
        public double mtpPercent
        {
            get
            {
                if(this.mtpTotal == 0) { return 0; }
                return (int)((this.mtp / this.mtpTotal) * 100);
            }
        }
        
    }
    public class Stats
    {

        public static StatLine PlayerSeasonStat(int number)
        {
            List<Play> plays = FootballData.GetPlayerPlays(number);
            return createStatline(plays);    
        }
        public static StatLine PlayerGameStat(int number, int gameNum)
        {
            List<Play> plays = FootballData.GetPlayerGamePlays(number, gameNum);
            return createStatline(plays);
        }
        public static StatLine PositionSeasonStat(String position)
        {
            List<Play> plays = FootballData.GetPositionPlays(position);
            return createStatline(plays);
        }
        public static StatLine PositionGameStat(String position, int gameNum)
        {
            List<Play> plays = FootballData.GetPositionGamePlays(position, gameNum);
            return createStatline(plays);
        }

        private static StatLine createStatline(List<Play> plays)
        {
            int tech = 0; //bool
            int purs = 0; //bool
            int mtp = 0; // num
            int loaf = 0;
            int techTotal = 0;
            int pursTotal = 0;
            int mtpTotal = 0;
            StatLine stat = new StatLine();


            foreach (var play in plays)
            {
                if (play.tech != 0)
                {
                    techTotal++;
                    if (play.tech == 1)
                    {
                        tech++;
                    }
                }
                if (play.purs != 0)
                {
                    pursTotal++;
                    if (play.purs == 1)
                    {
                        purs++;
                    }
                }
                if (play.mtp != 0)
                {
                    mtpTotal++;
                    if (play.mtp > 0)
                    {
                        mtp += play.mtp;
                    }
                }
                if (play.loaf)
                {
                    loaf++;
                }
                if (stat.checkStat(play.stat1))
                {
                    stat.updateStat(play.stat1);
                }
                else
                {
                    stat.addStat(play.stat1);
                }
                if (stat.checkStat(play.stat2))
                {
                    stat.updateStat(play.stat2);
                }
                else
                {
                    stat.addStat(play.stat2);
                }
            }

            stat.totalPlays = plays.Count;
            stat.setStatLine(techTotal, tech, pursTotal, purs, mtpTotal, mtp, loaf);



            return stat;
        }

    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = parameter as string;
            if (!String.IsNullOrEmpty(format))
                return String.Format(format, value);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
