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
    /// Interaction logic for Custom.xaml
    /// </summary>
    public partial class Custom : Window
    {
        public object[] items;

        public Custom(string s1, string s2, string s3)
        {
            InitializeComponent();

            tbWidth.Text = s1;
            tbHeight.Text = s2;
            tbMines.Text = s3;

        }

        private void btSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                items = new object[] { tbWidth.Text, tbHeight.Text, (Int32.Parse(tbMines.Text) - 1).ToString() };
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}