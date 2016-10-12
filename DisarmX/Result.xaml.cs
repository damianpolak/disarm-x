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
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        private List<Stats> ListStats;

        public Result(List<Stats> ls)
        {
            InitializeComponent();
            ListStats = ls;
        }

        private void btEnd_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btReturn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btStats_Click(object sender, RoutedEventArgs e)
        {
            // new Stats window and DialogResult = false
        }

        private void btShowStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics stats = new Statistics(ListStats);
            stats.ShowDialog();
        }
    }
}
