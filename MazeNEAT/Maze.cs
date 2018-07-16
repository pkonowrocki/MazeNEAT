using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MazeNEAT
{
    public enum direction
    {
        L = 0, R = 1, U = 2, D = 3
    };
    public class Wall
    {
        public bool raised = true;
        public Cell a, b;
        public Point aP, bP;
        public Wall(Cell _a, Cell _b, int i, int j, int size, direction dir)
        {
            a = _a;
            b = _b;
            if (dir == direction.D)
            {
                aP = new Point(size * i, size * j + size);
                bP = new Point(size * i + size, size * j + size);
            }
            if (dir == direction.R)
            {
                aP = new Point(size * i + size, size * j + size);
                bP = new Point(size * i + size, size * j);
            }
        }
    }
    public class Cell
    {
        public double len;
        public bool visited = false;
        public Dictionary<direction, Wall> roads;
        public Point center;
        public Dictionary<direction, Wall> walls = new Dictionary<direction, Wall>();
        public int i, j;
        public Cell(int _i, int _j, int size)
        {
            i = _i;
            j = _j;
            center = new Point(size * _i + size / 2, size * _j + size / 2);
            len = size;
        }
        public void Roads()
        {
            roads= walls.Where(it => !it.Value.raised).ToDictionary(i => i.Key, i=> i.Value);
        }
        public int rate=999999999;
        public Cell bef;
       // public Path path;
        public void RateCell(int r, Cell before)
        {
            if (r + 1 < rate)
            {
                rate = r + 1;
                bef = before;
            }
                
            foreach(Wall w in roads.Values)
            {
                    if (w.a != this)
                    {
                        if(w.a!=before)
                            w.a.RateCell(rate,this);
                    }
                    else
                    {
                        if(w.b!=before)
                            w.b.RateCell(rate,this);
                    }
                
                
            }
            
        }
        
    }
    public class Destination 
    {
        public Path path = new Path();
        public EllipseGeometry dest;
        public double x, y;
        public int i, j;
        public Destination(int _i, int _j, int cell_size)
        {
            path.Fill = Brushes.Yellow;
            i = _i - 1;
            j = _j - 1;
            path.Stroke = Brushes.Yellow;
            x = (i+1) * cell_size - cell_size / 2;
            y = (j+1) * cell_size - cell_size / 2;
            dest = new EllipseGeometry(new Point(x, y), cell_size / 2.5, cell_size / 2.5);
            
            path.Data = dest;
        }
       
    }
    public class Maze
    {
        public Cell[,] cells;
        List<Wall> walllist = new List<Wall>();
        public PathGeometry pathGeometry = new PathGeometry();
        public Path path = new Path();
        public int s;
        public int cs;

        public Maze(int size, int cell_size)
        {
            s = size;
            cs = cell_size;
            cells = new Cell[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    cells[i, j] = new Cell(i, j, cell_size);

            for (int i = 0; i < size - 1; i++)
                for (int j = 0; j < size; j++)
                {
                    Wall temp = new Wall(cells[i, j], cells[i + 1, j], i, j, cell_size, direction.R);
                    cells[i, j].walls.Add(direction.R, temp);
                    cells[i + 1, j].walls.Add(direction.L, temp);
                }


            for (int i = 0; i < size; i++)
                for (int j = 0; j < size - 1; j++)
                {
                    Wall temp = new Wall(cells[i, j], cells[i, j + 1], i, j, cell_size, direction.D);
                    cells[i, j].walls.Add(direction.D, temp);
                    cells[i, j + 1].walls.Add(direction.U, temp);
                }

            cells[0, 0].visited = true;
            walllist.AddRange(cells[0, 0].walls.Values);
            var r = new Random();
            while (walllist.Count != 0)
            {
                Wall temp = walllist[r.Next(walllist.Count)];
                if (temp.a.visited ^ temp.b.visited)
                {
                    temp.raised = false;
                    if (!temp.a.visited)
                    {
                        temp.a.visited = true;
                        walllist.AddRange(temp.a.walls.Values.Where(it => it.raised == true));
                    }

                    if (!temp.b.visited)
                    {
                        temp.b.visited = true;
                        walllist.AddRange(temp.b.walls.Values.Where(it => it.raised == true));
                    }

                }
                walllist.Remove(temp);
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    cells[i, j].Roads();
                    //pathGeometry.AddGeometry(new EllipseGeometry(cells[i, j].center, cell_size / 3, cell_size / 3));
                    foreach (KeyValuePair<direction, Wall> w in cells[i, j].walls)
                    {
                        if (w.Value.raised)
                        {
                           // pathGeometry.AddGeometry(new LineGeometry(w.Value.a.center, w.Value.b.center));
                            pathGeometry.AddGeometry(new LineGeometry(w.Value.aP, w.Value.bP));
                        }
                    }
                }
            }
            path.Data = pathGeometry;
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
        }

        public void RateMaze(Destination d)
        {
            cells[d.i, d.j].RateCell(1,null);           
        }

        public void solve(Cell start)
        {
            while (start != null)
            {
                pathGeometry.AddGeometry(new EllipseGeometry(start.center, start.len / 3, start.len / 3));
                start = start.bef;
                
            }
        }

        public List<Path> colorCells()
        {
            List<Path> w = new List<Path>();
            int max = 0;
            for (int i = 0; i < s; i++)
                for (int j = 0; j < s; j++)
                    if (cells[i, j].rate > max)
                        max = cells[i, j].rate;
            for (int i = 0; i < s; i++)
                for (int j = 0; j < s; j++)
                {
                    Path a = new Path();
                    a.Data = new EllipseGeometry(cells[i, j].center, cells[i, j].len / 2.1, cells[i, j].len / 2.1);
                    a.Fill = new SolidColorBrush(Color.FromRgb((byte)(255 * cells[i, j].rate / max), (byte)(255 *  cells[i, j].rate / max), (byte)(255 * cells[i, j].rate / max)  ));
                    a.Stroke = a.Fill;
                    a.StrokeThickness = 1;
                    w.Add(a);
                }
            return w;    
        }
        

    }
}
