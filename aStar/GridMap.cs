using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;


namespace Grid
{
    public class GridMap
    {
        private Grid map_;
        private Point offset_ = new Point(0, 0);
        public List<double[]> transition;

        private StringBuilder debug = new StringBuilder();

         /// <summary>
        /// Construct a planner object. Initial map is free of obstacles.
        /// </summary>
        /// <param name="dimensions">Width and height of map.</param>
        /// /// <param name="allFourQuandrants">Set to true to use all quadrants in cartesian coordinates</param>
        public GridMap(Point dimensions, bool allFourQuandrants)
        {
            map_ = new Grid(dimensions);
            transition = new List<double[]>();

            if (allFourQuandrants)
            {
                offset_ = new Point(dimensions.X / 2, dimensions.Y / 2);
            }
            else
            {
                offset_ = new Point(0, 0);
            }
        }

        // Calculate the transition matrix by going through a cell one by one can
        // find its number of neighbor, and then find the probability of going
        // to its neighbor cells
        public void GetTransitionMat()
        {
            Point current;
            GridCell[,] neighbors = new GridCell[3, 3];
            double[] zeroTrans = new double[map_.dimensions_.X * map_.dimensions_.Y];
            double[] trans;
            double stayCurrentCell = 0.2; // Probability of staying at the same cell

            for (int i = 0; i < zeroTrans.Length; i++)
            {
                zeroTrans[i] = 0;
            }

            // Looping through every cell
            for (int i = 0; i < map_.dimensions_.X; i++ )
            {
                for (int j = 0; j < map_.dimensions_.Y; j++)
                {
                    current = new Point(i, j);
                    Console.WriteLine(current.ToString());
                    if (map_.Cell(current).Obstacle)
                    {
                        transition.Add(zeroTrans);
                    }
                    else
                    {
                        trans = new double[map_.dimensions_.X * map_.dimensions_.Y];
                        // Reset transition matrix
                        for (int m = 0; m < trans.Length; m++)
                        {
                            trans[m] = 0;
                        }
                        neighbors = map_.Neighbours(current);
                        // Add transition probability according to neighbors
                        trans[i * map_.dimensions_.X + j] = stayCurrentCell;
                        for (int nborx = 0; nborx < 3; nborx++)
                        {
                            for (int nbory = 0; nbory < 3; nbory++)
                            {
                                if (neighbors[nborx, nbory] != null)
                                {
                                   // Weird indexing because (0,0) is the bottom left neighbor...
                                   trans[i * map_.dimensions_.X + j + (nborx-1) * map_.dimensions_.X + (nbory-1)] =
                                                                (1 - stayCurrentCell) / map_.Cell(current).Neighbors;
                                }
                            }
                        }
                        transition.Add(trans);
                    }
                }
            }

            printTran();
        }
        
        /// <summary>
        /// For debugging
        /// </summary>
        public void printTran()
        {
            for (int i = 0; i < transition.Count; i++)
            {
                debug.Clear();
                for (int j = 0; j < transition[i].Length; j++)
                {
                    debug.Append(transition[i][j].ToString());
                    debug.Append(",");
                }
                Console.WriteLine(debug.ToString());
            }
           // Console.WriteLine(transition.Count.ToString() + "," + transition[50].Length.ToString());
        }


        // set start and end cells and resets the grid map.
        private void Initialize()
        {
            map_.Initialize();
        }

        /// <summary>
        /// Add a set of obstacles.
        /// </summary>
        /// <param name="obstacles">The set of cartesian coordinates of cells 
        /// to set as obstacles.</param>
        public void AddObstacle(HashSet<Point> obstacles)
        {
            foreach (Point p in obstacles)
            {
                Point corrected = new Point(p.X + offset_.X, p.Y + offset_.Y);
                AddObstacle(corrected);
            }
        }


        /// <summary>
        /// Remove a set of obstacles
        /// </summary>
        /// <param name="obstacles">The set of cartesian coordinates of cells to 
        /// unset as obstacles.</param>
        public void RemoveObstacle(HashSet<Point> obstacles)
        {
            foreach (Point p in obstacles)
            {
                Point corrected = new Point(p.X + offset_.X, p.Y + offset_.Y);
                RemoveObstacle(corrected);
            }
        }

        /// <summary>
        /// Add a single obstacle.
        /// </summary>
        /// <param name="p">The cartesian coordinate of cell to set as obstacles.</param>
        public void AddObstacle(Point p)
        {
            p = new Point(p.X + offset_.X, p.Y + offset_.Y);
            map_.Cell(p).Obstacle = true;
        }

        /// <summary>
        /// Remove a single obstacle.
        /// </summary>
        /// <param name="p"> Cartesian coordinate of cell to unset as obstacles.</param>
        public void RemoveObstacle(Point p)
        {
            p = new Point(p.X + offset_.X, p.Y + offset_.Y);
            map_.Cell(p).Obstacle = false;
        }

        /// <summary>
        /// Toggle the obstacle state of a cell, only work on cells that are not start or end cells.
        /// </summary>
        /// <param name="p">Cartesian coordinates of cell to toggle.</param>
        public void ToggleObstacle(Point p)
        {
            p = new Point(p.X + offset_.X, p.Y + offset_.Y);

            if (!map_.Cell(p).Start && !map_.Cell(p).Goal)
            {
                map_.Cell(p).Obstacle = !map_.Cell(p).Obstacle;
            }
        }

        /// <summary>
        /// ASCII rendition of map.
        /// </summary>
        /// <returns></returns>
        public String Ascii()
        {
            return map_.Ascii();
        }

        /// <summary>
        /// Check if a cell in the map is an obstacle.
        /// </summary>
        /// <param name="p">The cartesian coordinate of the cell.</param>
        /// <returns>True if cell is an obstacle.</returns>
        public bool Obstacle(Point p)
        {
            p = new Point(p.X + offset_.X, p.Y + offset_.Y);

            if (p.X >= 0 && p.X < map_.dimensions_.X && p.Y >= 0 && p.Y < map_.dimensions_.Y)
            { return map_.Cell(p).Obstacle; }
            return true;

        }

    }
}
