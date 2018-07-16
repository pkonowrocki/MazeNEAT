using MazeNEAT;
using NEAT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MazeNEAT
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MazeWindow : Window
    {
        public int maze_size = 10;
        public int cell_size = 50;
        public Maze maze;
        public Destination destination;
        public Population population = new Population(10, 7, 4);
        public List<Robot> robots = new List<Robot>();
        public System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        int generacja = 1;
        int dist = 999999999;
        int iter = 0;
        int map = 1;
        int solved_map = 0;
        public string result  {
            get { return "Generation: " + generacja + "\nSpecies: " + population.species.Count + "\nCells from finish: " + (dist-1)+ "\nMap: " + map + "\nMap solved: " + solved_map + "\nRatio: " + (double)solved_map / (map) *100 + "%"; }
        }

        public void NewMaze()
        {
            dispatcherTimer.Stop();
            canvas.Children.Remove(maze.path);
            maze = new Maze(maze.s, maze.cs);
            maze.RateMaze(destination);
            canvas.Children.Add(maze.path);
            (DataContext as Main).ResultsText.Text = result;
            dispatcherTimer.Start();
            map++;
            //dist = 999999999;
        }

        public MazeWindow(int _maze_size=10, int _cell_size = 50)
        {
            InitializeComponent();

            maze_size = _maze_size;
            cell_size = _cell_size;
            canvas.Width = cell_size * maze_size;
            canvas.Height = cell_size * maze_size;
            maze = new Maze(maze_size, cell_size);
            destination = new Destination(maze_size, maze_size, cell_size);
            maze.RateMaze(destination);
            for (int i = 0; i < population.agents.Count; i++)
                robots.Add(new Robot(maze.cells[0,0]));

            for (int i = 0; i < population.agents.Count; i++)
            {
                canvas.Children.Add(robots[i].path);
            }
            canvas.Children.Add(maze.path);
            canvas.Children.Add(destination.path);
            
        }
        
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dist = 999999999;
            iter++;

            for(int i = population.agents.Count - 1; i >= 0; i--)
            {
                robots[i].run(maze,population.agents[i],destination);
                if (dist > robots[i].now.rate)
                {
                    dist = robots[i].now.rate;
                    robots[i].path.Fill = Brushes.Red;
                }
                else
                {
                    robots[i].path.Fill = Brushes.Aqua;
                }
                if (robots[i].hit(destination))
                {
                    dispatcherTimer.Stop();
                    NewMaze();
                    NewGen();
                    solved_map++;
                    dispatcherTimer.Start();
                }
            }
            foreach( Robot r in robots)
            {
                if (dist == r.now.rate)
                {
                    r.path.Fill = Brushes.Red;
                }
                else
                {
                    r.path.Fill = Brushes.Aqua;
                }
            }

            if (iter > maze_size*4)
            {
                dispatcherTimer.Stop();
                NewGen();
                iter = 0;
                (DataContext as Main).ResultsText.Text = result;
                dispatcherTimer.Start();
            }

            if (generacja > 1001)
            {
                iter = 0;
                generacja = 1;
                NewMaze();
            }
            
        }
        
        void NewGen()
        {
            
            for (int i = population.agents.Count - 1; i >= 0; i--)
                population.KillAgent(population.agents[i], robots[i].Evaluate(destination));
            
            for (int i = 0; i < population.population; i++)
            {
                canvas.Children.Remove(robots[i].path);
                robots[i] = new Robot(maze.cells[0, 0]);
                canvas.Children.Add(robots[i].path);
            }
                
           
            population.NextGeneration();
            generacja++;
        }
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S) maze.solve(maze.cells[0, 0]);
            if(e.Key == Key.N)
            {
                NewGen();
                iter = 0;
                //dispatcherTimer.Start();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer = null;
            (DataContext as Main).mazeWindow = null;
        }
    }
    
}
