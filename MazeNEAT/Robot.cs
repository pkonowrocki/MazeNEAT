using NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MazeNEAT
{
    public class Robot 
    {
        public EllipseGeometry ellipse;
        //double x0, y0;
        public double x = 0, y = 0;
        public Path path = new Path();
        public Cell now;
        int steps;
       
        
       

        public Robot(Cell start)
        {
            now = start;
            ellipse = new EllipseGeometry(now.center, now.len/4, now.len/4);
            path.Data = ellipse;
            path.Stroke = Brushes.Black;
            path.Fill = Brushes.Aqua;
        }
        
        
        public void run(Maze maze, Genome genome, Destination d)
        {
            double[] outp = new double[4];
            double[] inp = new double[7];
            inp[0] = (double)(d.i - now.i) / maze.s; //(d.x - now.center.X);
            inp[1] = (double)(d.j - now.j) / maze.s;//(d.y - now.center.Y);
            inp[2] = 1;

            if (now.roads.ContainsKey(direction.D))
                inp[3] = 1;
            else
                inp[3] = 0;

            if (now.roads.ContainsKey(direction.L))
                inp[4] = 1;
            else
                inp[4] = 0;

            if (now.roads.ContainsKey(direction.R))
                inp[5] = 1;
            else
                inp[5] = 0;

            if (now.roads.ContainsKey(direction.U))
                inp[6] = 1;
            else
                inp[6] = 0;

            outp = genome.Compute(inp);
            Dictionary<direction, double> a = new Dictionary<direction, double>();

            a.Add(direction.D, outp[0]);
            a.Add(direction.L, outp[1]);
            a.Add(direction.R, outp[2]);
            a.Add(direction.U, outp[3]);

            ///Move no matter what
            //direction best = direction.D;
            //foreach (direction dir in a.Keys)
            //    if (now.roads.Keys.Contains(dir))
            //        best = dir;

            //foreach (direction dir in a.Keys)
            //    if (a[best] < a[dir] && now.roads.Keys.Contains(dir))
            //        best = dir;
            //move(best);

            ///Not always move
            direction best = direction.D;
            foreach (direction dir in a.Keys)
                if (a[best] < a[dir])
                    best = dir;
            if (now.roads.ContainsKey(best))
                move(best);
        }


        public void move(direction dir)
        {
            
            if (now == now.walls[dir].b)
                now = now.walls[dir].a;
            else
                now = now.walls[dir].b;

            ellipse.Center = now.center;
            steps++;
        }

        public double Evaluate(Destination destination)
        {
            return 1000/now.rate;
        }

        internal bool hit(Destination dest)
        {
            return now.i == dest.i && now.j == dest.j;   
        }
    }
}
