using RoboNEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MazeNEAT
{
    /// <summary>
    /// Logika interakcji dla klasy Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        bool state = false;
        public MazeWindow mazeWindow;// = new MazeWindow(50,10);
        
        public Main()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            state = !state;
            if (state)
            {
                if (mazeWindow != null)
                {
                    mazeWindow.dispatcherTimer.Tick += new EventHandler(mazeWindow.dispatcherTimer_Tick);
                    mazeWindow.dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)tick_size.Value);
                    mazeWindow.dispatcherTimer.Start();
                }
                StartBtn.Content = "Stop";
            }
            else
            {
                if(mazeWindow!=null)
                    mazeWindow.dispatcherTimer.Stop();
                StartBtn.Content = "Start";
            }
        }

        private void OpenMapBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mazeWindow != null) 
                mazeWindow.Close();
            mazeWindow = null;
            mazeWindow = new MazeWindow((int)maze_size.Value, (int)cell_size.Value);
            mazeWindow.Show();
            mazeWindow.DataContext = this;
        }

        private void NewMapBtn_Click(object sender, RoutedEventArgs e)
        {
            mazeWindow.NewMaze();
        }

        private void tick_size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mazeWindow!=null && mazeWindow.dispatcherTimer.IsEnabled && mazeWindow.dispatcherTimer != null)
            {
                mazeWindow.dispatcherTimer.Stop();
                mazeWindow.dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)tick_size.Value);
                mazeWindow.dispatcherTimer.Start();
            }
            tick_groupbox.Header = "Time: " + (int)tick_size.Value + " [ms]";


        }

        private void cell_size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cell_groupbox.Header = "Cell size: " + (int)cell_size.Value;
        }

        private void maze_size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            maze_groupbox.Header = "Maze size: " + (int)maze_size.Value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mazeWindow != null)
                mazeWindow.Close();
        }
    }
}
