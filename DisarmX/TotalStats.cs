using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisarmX
{
    class TotalStats
    {

        public int totalTimePlayed { get; private set; }
        public int totalGamesPlayed { get; private set; }
        public int totalGamesWin { get; private set; }
        public int totalGamesLost { get; private set; }


        IEnumerable<Stats> allStats;

        public TotalStats()
        {
            totalTimePlayed = 0;
            totalGamesPlayed = 0;
            totalGamesWin = 0;
            totalGamesLost = 0;

            
        }

        //public void Update(IEnumerable<Stats> s1) { }
        public void Update(IEnumerable<Stats> s1)
        { 
            totalTimePlayed = 0;
            totalGamesPlayed = 0;
            totalGamesWin = 0;
            totalGamesLost = 0;


            allStats = s1;

            foreach (Stats item in allStats)
            {
                totalTimePlayed += item.gameTime;

                switch (item.gameResult)
                {
                    case 0: totalGamesLost++; break;
                    case 1: totalGamesWin++; break;
                }

            }

            
            // Wszystkie grane gry
            totalGamesPlayed = allStats.Count();
        }

        




        
    }
}
