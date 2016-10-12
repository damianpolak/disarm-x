using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DisarmX
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {

        private List<Stats> ListStats;
        private IEnumerable<Stats> StatsQuery;

        public Statistics(List<Stats> list)
        {
            InitializeComponent();
            ListStats = list;

            MenuItem_Click_6(null, null);

        }


        private void tbStats_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            StatsQuery = from stat in ListStats
                         where stat.gameLevel > 0
                         select stat;

            ShowStats(StatsQuery);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 1
                         select stat;
            ShowStats(StatsQuery);

        }

        private void ShowStats(IEnumerable<Stats> iStats)
        {
            int i = 1;
            tbStats.Text = "";

            ListStats.Reverse();
            tbStats.Text += "ID\t| DATE\t\t| LEVEL\t| RESULT\t| TIME\t| DISCOVERED\t| \r\n";
            
            
            foreach (Stats item in iStats)
            {
                if (i >= 0 && i < 10) tbStats.Text += "00000";
                if (i > 9 && i < 100) tbStats.Text += "0000";
                if (i > 99 && i < 1000) tbStats.Text += "000";
                if (i > 999 && i < 10000) tbStats.Text += "00";
                tbStats.Text += " " + i.ToString() + "\t|";
                tbStats.Text += " " + item.dateTime.ToString("d") + "\t|";
                tbStats.Text += " " + item.gameLevel.ToString() + "\t|";
                tbStats.Text += (item.gameResult == 1) ? " WIN\t|" : " LOOSE\t|";
                tbStats.Text += " " + item.gameTime.ToString() + "s" + "\t|";
                tbStats.Text += " " + string.Format("{0:0.##}", item.fieldsDiscovered) + "%" + "\t\t|";
                tbStats.Text += "\r\n";
                i++;
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 2
                         select stat;
            ShowStats(StatsQuery);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 3
                         select stat;
            ShowStats(StatsQuery);
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 4
                         select stat;
            ShowStats(StatsQuery);
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            tbStats.Text = "";
            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 1
                         select stat;

            TotalStats totalLevel1 = new TotalStats();
            totalLevel1.Update(StatsQuery);

            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 2
                         select stat;
            TotalStats totalLevel2 = new TotalStats();
            totalLevel2.Update(StatsQuery);

            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 3
                         select stat;
            TotalStats totalLevel3 = new TotalStats();
            totalLevel3.Update(StatsQuery);

            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 4
                         select stat;
            TotalStats totalLevel4 = new TotalStats();
            totalLevel4.Update(StatsQuery);

            TotalStats totalLevels = new TotalStats();
            totalLevels.Update(ListStats);

            tbStats.Text += "\t| WINS\t| LOST\t| TIME\t| \r\n";
            tbStats.Text += "Level 1\t| " + totalLevel1.totalGamesWin.ToString() + "\t| " + totalLevel1.totalGamesLost.ToString() + "\t| " + totalLevel1.totalTimePlayed.ToString() + "s\t|" + "\r\n";
            tbStats.Text += "Level 2\t| " + totalLevel2.totalGamesWin.ToString() + "\t| " + totalLevel2.totalGamesLost.ToString() + "\t| " + totalLevel2.totalTimePlayed.ToString() + "s\t|" + "\r\n";
            tbStats.Text += "Level 3\t| " + totalLevel3.totalGamesWin.ToString() + "\t| " + totalLevel3.totalGamesLost.ToString() + "\t| " + totalLevel3.totalTimePlayed.ToString() + "s\t|" + "\r\n";
            tbStats.Text += "Level 4\t| " + totalLevel4.totalGamesWin.ToString() + "\t| " + totalLevel4.totalGamesLost.ToString() + "\t| " + totalLevel4.totalTimePlayed.ToString() + "s\t|" + "\r\n";
            tbStats.Text += "\r\nTOTAL\t| " + totalLevels.totalGamesWin.ToString() + "\t| " + totalLevels.totalGamesLost.ToString() + "\t| " + totalLevels.totalTimePlayed.ToString() + "s\t|" + "\r\n";

            /*
            StatsQuery = from stat in ListStats
                         where stat.gameLevel == 1
                         && stat.gameResult == 1
                         orderby stat.gameTime ascending
                         select stat;

            tbStats.Text += "\r\n\r\nLEVEL 1\r\n";

            tbStats.Text += "DATE\t\t| RESULT\t| TIME\t| DISCOVERED\t| \r\n";
            
            foreach (Stats item in StatsQuery)
            {
                tbStats.Text += " " + item.dateTime.ToString("d") + "\t|";
                tbStats.Text += (item.gameResult == 1) ? " WIN\t|" : " LOOSE\t|";
                tbStats.Text += " " + item.gameTime.ToString() + "s" + "\t|";
                tbStats.Text += " " + string.Format("{0:0.##}", item.fieldsDiscovered) + "%" + "\t\t|";
                tbStats.Text += "\r\n";
            }*/
        
        }


        

    }
}
