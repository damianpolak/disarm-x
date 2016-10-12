using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;

namespace DisarmX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct Level
        {
            public struct Custom
            {
                public int X;
                public int Y;
                public int Mines;
            }

            public int Value;
            public int X;
            public int Y;
            public int Mines;

            public Custom custom;

        }

        private bool HiddenStuff = false;
        private bool GameOver = false;
        private int RemainingCountToWin;
        private int timerCount, minesCount;
        private int FieldWidth = 20, FieldHeight = 20;

        private Area area { get; set; }
        DateTime thisDay;

        // Stats
        private List<Stats> listStats { get; set; }
        private Stats stats { get; set; }
        private TotalStats totalStats { get; set; }

        private static string statsFileDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\BlackJack\\";
        private string statsFilePath = statsFileDir + "stats.bin";

        private Level level;

        System.Windows.Threading.DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();
            KeyDown +=HandleKeyDown;

            thisDay = DateTime.Today;

            // New level struct
            level = new Level();

            level.Value = 4;
            level.custom.X = 20;
            level.custom.Y = 35;
            level.custom.Mines = 150;
            
            // Virtual Click's
            menuMediumLevel_Click(null, null);
            menuLowLevel_Click(null, null);
            menuNewGame_Click(null, null);

            dispatcherTimer1.Tick += dis_Tick;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 1);

            stats = new Stats();
            listStats = new List<Stats>();
            
            totalStats = new TotalStats();

            if (File.Exists(statsFilePath))
            {
                listStats = LoadStatsFromFile(statsFilePath);
                totalStats.Update(listStats);
            }
            else
            {
                Directory.CreateDirectory(statsFileDir);
            }
            
        }

        private void SaveStatsToFile(string filePath, Stats s1)
        {
            listStats.Add(s1);
            totalStats.Update(listStats);
            
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, listStats);
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.ToString(), "SaveStatsToFile()");
            }
        }

        private List<Stats> LoadStatsFromFile(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    List<Stats> st = (List<Stats>)bin.Deserialize(stream);
                    return st;
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.ToString(), "LoadStatsFromFile()");
                return null;
            }
        }

        private int SetCustomArea(int width, int height, int m)
        {
            try
            {
                level.X = width;
                level.Y = height;
                level.Mines = m;
                return 1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        private void ForEachField(Action<int, int> action)
        {
            for (int i = 0; i <= level.X - 1; i++)
                for (int j = 0; j <= level.Y - 1; j++)
                    action(i, j);
        }

        // Resize and Center Window after change level
        private void ResizeWindow()
        {
            // Level 1 : 10x10
            // Level 2 : 16x16
            // Level 3 : 16x30

            this.Width = (level.Y * FieldWidth) + ((1 + level.Y) * 3) + 33;
            this.Height = (level.X * FieldHeight) + ((1 + level.X) * 3) + 125;
            CenterWindow();
            
        }

        private void CenterWindow()
        {
            double xRes = SystemParameters.PrimaryScreenWidth;
            double yRes = SystemParameters.PrimaryScreenHeight;

            try
            {
                mainWindow.Left = (xRes - mainWindow.Width) / 2;
                mainWindow.Top = (yRes - mainWindow.Height) / 2;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "CenterWindow()");
            }

        }

        // Remove buttons from Canvas 
        private void ClearFields()
        {
            canvas1.Children.Clear();
        }

        private void AddFields()
        {
            GameOver = false;
            RemainingCountToWin = (level.X * level.Y) - level.Mines;
            minesCount = level.Mines;
            ClearFields();
            area = new Area(level.X, level.Y, level.Mines);

            int xPos = 3, yPos = 3;
            for (int i = 0; i <= level.X - 1; i++)
            {
                for (int j = 0; j <= level.Y - 1; j++)
                {
                    Position pos = new Position();
                    pos.X = xPos; pos.Y = yPos;
                    area.Matrix[i, j].button.Width = FieldWidth;
                    area.Matrix[i, j].button.Height = FieldHeight;
                    area.Matrix[i, j].Location = pos;
                    if (HiddenStuff == true)
                    {
                        area.Matrix[i, j].button.Content = area.Matrix[i, j].Value;
                        if (area.Matrix[i, j].button.Content.ToString() == "0") area.Matrix[i, j].button.Foreground = Brushes.Red;
                        if (area.Matrix[i, j].button.Content.ToString() == "0") area.Matrix[i, j].button.FontWeight = FontWeights.Bold;
                        if (area.Matrix[i, j].button.Content.ToString() == "m") area.Matrix[i, j].button.FontWeight = FontWeights.Bold;
                    }
                    Canvas.SetLeft(area.Matrix[i,j].button, xPos);
                    Canvas.SetTop(area.Matrix[i, j].button, yPos);
                    canvas1.Children.Add(area.Matrix[i, j].button);
                    area.Matrix[i, j].button.Click +=button_Click;
                    area.Matrix[i, j].button.MouseRightButtonUp +=button_MouseRightButtonUp;
                    xPos = xPos + 3 + FieldWidth;
                }
                xPos = 3;
                yPos = yPos + 3 + FieldHeight;
            }
        }

        public void DiscoverEmptyCells(int i, int j) 
        {
            if (i >= 0 && j >= 0 && i < level.X && j < level.Y) 
            {
                if (area.Matrix[i, j].Value == "0" && area.Matrix[i, j].Status != Field.Mark.DISCOVERED && area.Matrix[i,j].Status != Field.Mark.EXCLAMATION)
                {
                    RemainingCountToWin--;
                    if (CheckWin() == 1) return;
                    area.Matrix[i, j].button.Visibility = System.Windows.Visibility.Hidden;
                    area.Matrix[i, j].Status = Field.Mark.DISCOVERED;

                    DiscoverEmptyCells(i - 1, j); 
                    DiscoverEmptyCells(i + 1, j); 
                    DiscoverEmptyCells(i, j + 1); 
                    DiscoverEmptyCells(i, j - 1); 
                    DiscoverEmptyCells(i - 1, j + 1);
                    DiscoverEmptyCells(i + 1, j + 1);
                    DiscoverEmptyCells(i - 1, j - 1);
                    DiscoverEmptyCells(i + 1, j - 1);

                }
                else if (area.Matrix[i, j].Value != "0" && area.Matrix[i, j].Status != Field.Mark.DISCOVERED && area.Matrix[i, j].Value != "m" && area.Matrix[i,j].Status != Field.Mark.EXCLAMATION)
                {
                    RemainingCountToWin--;
                    if (CheckWin() == 1) return;
                    area.Matrix[i, j].button.Visibility = System.Windows.Visibility.Hidden;
                    area.Matrix[i, j].Status = Field.Mark.DISCOVERED;

                    Label label = new Label()
                    {
                        //Background = Brushes.LightGray,
                        Width = FieldWidth + 2,
                        Height = FieldHeight + 2,
                        Content = area.Matrix[i, j].Value,
                        FontWeight = FontWeights.Bold,
                        FontSize = 14,
                        Padding = new Thickness(0, 0, 0, 0),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center

                    };

                    label.Foreground = ChangeColorOfNumber(area.Matrix[i, j].Value);
                    Canvas.SetLeft(label, area.Matrix[i, j].Location.X);
                    Canvas.SetTop(label, area.Matrix[i, j].Location.Y);
                    canvas1.Children.Add(label);

                    return;
                }
            }
            else
            {
                return;
            }

        }

        private void DiscoverMines()
        {
            foreach (Position p in area.MinesPosition)
            {
                
                area.Matrix[p.X, p.Y].button.Visibility = Visibility.Hidden;
                Label label = new Label()
                {
                    //Background = Brushes.LightGray,
                    Width = FieldWidth + 2,
                    Height = FieldHeight + 2,
                    Content = "*",

                    Foreground = Brushes.DarkRed,
                    FontSize = 22,
                    FontWeight = FontWeights.Bold,
                    Padding = new Thickness(0, 0, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center

                };
                Canvas.SetLeft(label, area.Matrix[p.X, p.Y].Location.X);
                Canvas.SetTop(label, area.Matrix[p.X, p.Y].Location.Y);
                canvas1.Children.Add(label);
                
            }
        }

        private double FieldsDiscovered()
        {
            return ((((double)level.X * (double)level.Y) - ((double)RemainingCountToWin + (double)level.Mines)) * (double)100) / ((double)level.X * (double)level.Y);
        }

        private void StopGame()
        {
            GameOver = true;
            dispatcherTimer1.Stop();
        }

        private int CheckWin()
        {
            if (RemainingCountToWin == 0)
            {   
                // Win game

                StopGame();

                Stats winnerStats = new Stats();

                winnerStats.gameLevel = level.Value;
                winnerStats.gameTime = timerCount;
                winnerStats.gameResult = 1;
                winnerStats.dateTime = thisDay;
                winnerStats.fieldsDiscovered = 100; //%
                winnerStats.minesMarked = area.MarkedFields.Count;

                SaveStatsToFile(statsFilePath, winnerStats);
                ShowResultWindow(winnerStats);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // Zdarzenie lewy klawisz myszy, odkrycie pola
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (GameOver != true)
            {
                if (timerCount == 0)
                {
                    dispatcherTimer1.Start();
                }
                
                var b = sender as Button;

                ForEachField((i, j) =>
                {
                    if (b == area.Matrix[i, j].button)
                    {
                        if(area.Matrix[i,j].Status != Field.Mark.EXCLAMATION)
                        if (area.Matrix[i, j].Value != "0" && area.Matrix[i, j].Value != "m")
                        {
                            RemainingCountToWin--;
                            if (CheckWin() == 1) return;
                            area.Matrix[i, j].Status = Field.Mark.DISCOVERED;
                            Label label = new Label()
                            {
                                //Background = Brushes.LightGray,
                                Width = FieldWidth + 2,
                                Height = FieldHeight + 2,
                                Content = area.Matrix[i, j].Value,

                                FontSize = 14,
                                Padding = new Thickness(0, 0, 0, 0),
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center

                            };

                            {
                                label.FontWeight = FontWeights.Bold;
                            }

                            label.Foreground = ChangeColorOfNumber(area.Matrix[i, j].Value);
                            Canvas.SetLeft(label, area.Matrix[i, j].Location.X);
                            Canvas.SetTop(label, area.Matrix[i, j].Location.Y);
                            canvas1.Children.Add(label);
                            area.Matrix[i, j].button.Visibility = System.Windows.Visibility.Hidden;

                        }
                        else if (area.Matrix[i, j].Value == "0")
                        {
                            DiscoverEmptyCells(i, j);
                        }
                        else if (area.Matrix[i, j].Value == "m")
                        {
                            // Lost game
                            StopGame();
                            
                            Label label = new Label()
                            {
                                //Background = Brushes.LightGray,
                                Width = FieldWidth + 2,
                                Height = FieldHeight + 2,
                                Content = area.Matrix[i, j].Value,

                                FontSize = 14,
                                Padding = new Thickness(0, 0, 0, 0),
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center

                            };

                            label.Foreground = Brushes.DarkRed;
                            label.Content = "*";
                            label.FontSize = 22;
                            Canvas.SetLeft(label, area.Matrix[i, j].Location.X);
                            Canvas.SetTop(label, area.Matrix[i, j].Location.Y);
                            canvas1.Children.Add(label);
                            area.Matrix[i, j].button.Visibility = System.Windows.Visibility.Hidden;
                            
                            DiscoverMines();

                            Stats looserStats = new Stats();

                            looserStats.gameLevel = level.Value;
                            looserStats.gameResult = 0;
                            looserStats.gameTime = timerCount;
                            looserStats.dateTime = thisDay;
                            looserStats.fieldsDiscovered = FieldsDiscovered();
                            looserStats.minesMarked = area.MarkedFields.Count;
                            SaveStatsToFile(statsFilePath, looserStats);
                            ShowResultWindow(looserStats);
                        }

                    }
                });

            }

            lbDiscovered.Content = "DISCOVERED\r\n" + string.Format("{0:0.##}", FieldsDiscovered()) + "%";
        }

        // Zdarzenie prawy klawisz myszy - oznaczenie pola (MarkField) [!,?,   ]
        private void button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (GameOver != true)
            {
                var b = sender as Button;

                ForEachField((i, j) =>
                {
                    if (b == area.Matrix[i, j].button)
                    {
                        b.Content = area.MarkField(i, j);

                        if (b.Content.ToString() == "!")
                        {
                            b.Foreground = Brushes.Red;
                            b.FontWeight = FontWeights.Bold;
                            minesCount--;
                            lbMines.Content = "MINES\r\n" + minesCount.ToString();
                        }

                        if (b.Content.ToString() == "?")
                        {
                            b.Foreground = Brushes.Green;
                            b.FontWeight = FontWeights.Bold;
                            minesCount++;
                            lbMines.Content = "MINES\r\n" + minesCount.ToString();
                        }
                    }
                });
            }
        }

        private SolidColorBrush ChangeColorOfNumber(string value)
        {
            switch (value)
            {
                case "1": return Brushes.Blue;
                case "2": return Brushes.Green;
                case "3": return Brushes.Red;
                case "4": return Brushes.DarkOrange;
                case "5": return Brushes.DarkViolet;
                default: return Brushes.Black;
            }
        }

        private void ResetVariables()
        {
            switch (level.Value)
            {
                case 1:
                    level.Mines = 10;
                    level.X = 10;
                    level.Y = 10;
                    break;

                case 2:
                    level.Mines = 40;
                    level.X = 16;
                    level.Y = 16;
                    break;

                case 3:
                    level.Mines = 100;
                    level.X = 16;
                    level.Y = 30;
                    break;

                case 4:
                    level.Mines = level.custom.Mines;
                    level.X = level.custom.X;
                    level.Y = level.custom.Y;
                    break;
            }
        }

        private void ShowResultWindow(Stats s1)
        {
            Result result = new Result(listStats);
            if (s1.gameResult == 0)
            {
                result.Title = "YOU LOSE";
                result.tbMainText.Text = "Sorry, but you lose. Your results are presented below. Good luck!";
            }
            else
            {
                result.Title = "YOU WON";
                result.tbMainText.Text = "Congratulations! You won this game. Your results are presented below.";
            }

            result.tbTime.Content = "Time: " + s1.gameTime.ToString() + "s";
            result.tbDate.Content = "Date: " + s1.dateTime.ToString("d");
            result.tbLevel.Content = "Level: " + s1.gameLevel.ToString();
            result.tbDiscovered.Content = "Discovered: " + string.Format("{0:0.##}", s1.fieldsDiscovered) + "%";

            result.tbMarked.Content = "Marked mines: " + s1.minesMarked.ToString();

            if (result.ShowDialog() == true)
            {
                menuNewGame_Click(null, null);
            }
            else
            {
                //
            }
        }

        // DispatchTimer do odmierzania czasu gry
        // Start timera występuje dopiero po naciśnieciu na pole
        void dis_Tick(object sender, EventArgs e)
        {
            timerCount++;
            lbTimer.Content = "TIME\r\n" + timerCount + "s";
        }

        // Nowa gra
        private void menuNewGame_Click(object sender, RoutedEventArgs e)
        {
            ResetVariables();

            dispatcherTimer1.Stop();
            timerCount = 0;
            lbTimer.Content = "TIME\r\n0s";
            lbMines.Content = "MINES\r\n" + level.Mines.ToString();
            lbDiscovered.Content = "DISCOVERED\r\n0%";
            ResizeWindow();
            AddFields();

        }

        // Poziom 1
        private void menuLowLevel_Click(object sender, RoutedEventArgs e)
        {
            if (menuLowLevel.IsChecked == false)
            {
                menuLowLevel.IsChecked = true;
                menuMidLevel.IsChecked = false;
                menuHighLevel.IsChecked = false;
                menuCustom.IsChecked = false;

                level.Value = 1;
                level.Mines = 10;
                level.X = 10;
                level.Y = 10;

                menuNewGame_Click(sender, e);
            }

        }

        // Wybranie poziom 2
        private void menuMediumLevel_Click(object sender, RoutedEventArgs e)
        {
            if (menuMidLevel.IsChecked == false)
            {
                menuMidLevel.IsChecked = true;
                menuLowLevel.IsChecked = false;
                menuHighLevel.IsChecked = false;
                menuCustom.IsChecked = false;

                level.Value = 2;
                level.Mines = 40;
                level.X = 16;
                level.Y = 16;

                menuNewGame_Click(sender, e);
            }

        }

        // Wybranie poziom 3
        private void menuHighLevel_Click(object sender, RoutedEventArgs e)
        {
            if (menuHighLevel.IsChecked == false)
            {
                menuHighLevel.IsChecked = true;
                menuLowLevel.IsChecked = false;
                menuMidLevel.IsChecked = false;
                menuCustom.IsChecked = false;

                level.Value = 3;
                level.Mines = 100;
                level.X = 16;
                level.Y = 30;

                menuNewGame_Click(sender, e);
            }

        }

        private void menuCustom_Click(object sender, RoutedEventArgs e)
        {
            Custom custom = new Custom(level.custom.X.ToString(),
                                        level.custom.Y.ToString(),
                                        level.custom.Mines.ToString());

            if (custom.ShowDialog() == true)
            {
                if (custom.items[0].ToString() == "hiddenstuff" &&
                    custom.items[1].ToString() == "hiddenstuff")
                {

                    if (HiddenStuff == true)
                    {
                        HiddenStuff = false;
                        menuNewGame_Click(null, null);
                    }
                    else
                    {
                        HiddenStuff = true;
                        menuNewGame_Click(null, null);
                    }

                }
                else
                {
                    if (menuCustom.IsChecked != true)
                    {

                        menuCustom.IsChecked = true;
                        menuCustom.Header = "Custom " + custom.items[0].ToString() +
                            "x" + custom.items[1].ToString() + "/" + custom.items[2].ToString();
                    }

                    menuLowLevel.IsChecked = false;
                    menuMidLevel.IsChecked = false;
                    menuHighLevel.IsChecked = false;

                    level.Value = 4;
                    level.Mines = Int32.Parse(custom.items[2].ToString());
                    level.X = Int32.Parse(custom.items[0].ToString());
                    level.Y = Int32.Parse(custom.items[1].ToString());

                    level.custom.Mines = level.Mines;
                    level.custom.X = level.X;
                    level.custom.Y = level.Y;

                    menuNewGame_Click(sender, e);
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Author author = new Author();
            author.ShowDialog();
        }

        private void menuShowHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics(listStats);
            statistics.ShowDialog();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                menuNewGame_Click(sender, e);
            }

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (timerCount > 0)
            {
                if (MessageBox.Show("Are you sure you want to quit?", "Quit",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown(); // test
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

    }
}
