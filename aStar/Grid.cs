using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace Grid
{
    /// <summary>
    /// Map represented as a grid of cells.
    /// </summary>
    internal class Grid
    {
        private GridCell[,] cells_;
        public Point dimensions_;

        public HashSet<GridCell> Modified;

        /// <summary>
        /// If set to true, uses Manhattan distance as heuristic.
        /// </summary>
        public bool Fast = false;
        
        /// <summary>
        /// Construct a grid map.
        /// </summary>
        /// <param name="dimensions">Width and height of map.</param>
        public Grid(Point dimensions)
        {
            dimensions_ = new Point(dimensions.X, dimensions.Y);
            cells_ = new GridCell[dimensions.X, dimensions.Y];
            Modified = new HashSet<GridCell>();

            for (int i = 0; i < dimensions_.X; i++)
            {
                for (int j = 0; j < dimensions_.Y; j++)
                {
                    cells_[i, j] = new GridCell(new Point(i, j), false);
                }
            }

            Initialize();
        }

        /// <summary>
        /// Set the start and goal.
        /// </summary>
        internal void Initialize()
        {

            foreach (GridCell c in Modified)
            {
                c.Parent = null;
                c.H = int.MaxValue;
                c.G = int.MaxValue;
                c.Use = false;
                c.Checked = false;
                c.Handle = null;
                c.Forced = false;
            }

            foreach (GridCell c in cells_)
            {
                c.Explored *= 1.0;//  0.98;
            }

            Modified.Clear();
        } 

        private double ManhattanDist(Point p1, Point p2)
        {
            double dx = Math.Abs(p1.X - p2.X);
            double dy = Math.Abs(p1.Y - p2.Y);

            return (dx + dy);
        }

        internal double EucledianDist(Point p1, Point p2)
        {
            double dx = (p1.X - p2.X);
            double dy = (p1.Y - p2.Y);

            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }

        internal double Dist(Point p1, Point p2)
        {
            if (Fast) { return ManhattanDist(p1, p2); }
            else { return EucledianDist(p1, p2); }
        }

        /// <summary>
        /// Checks if a cell is within the bounds of the map, not already checked and not an obstacle.
        /// </summary>
        /// <param name="p">Cartesian coordinate of the cell.</param>
        /// <returns>True if p satisfies the conditions described.</returns>
        internal bool ValidCell(Point p)
        {
            return p.X >= 0 && p.X < dimensions_.X && p.Y >= 0 && p.Y < dimensions_.Y
               && !cells_[p.X, p.Y].Obstacle && !cells_[p.X, p.Y].Checked;
        }

        /// <summary>
        /// Same as above but for exploration....need to fix this
        /// </summary>
        /// <param name="p">Cartesian coordinate of the cell.</param>
        /// <returns>True if p satisfies the conditions described.</returns>
        internal bool ValidCellExplored(Point p)
        {
            return p.X >= 0 && p.X < dimensions_.X && p.Y >= 0 && p.Y < dimensions_.Y
               && !cells_[p.X, p.Y].Obstacle;
        }

        /// <summary>
        /// </summary>
        /// <param name="p">Cartesian coordinate of the cell.</param>
        /// <returns>The neighbours of p excluding those that are already checked.</returns>
        internal GridCell[,] Neighbours(Point p)
        {
            GridCell[,] neighbours = new GridCell[3,3];

            if (ValidCell(new Point(p.X + 1, p.Y)))
            {
                neighbours[2, 1] = cells_[p.X + 1, p.Y];
            }

            if (ValidCell(new Point(p.X - 1, p.Y)))
            {
                neighbours[0, 1] = cells_[p.X - 1, p.Y];
            }

            if (ValidCell(new Point(p.X, p.Y + 1)))
            {
                neighbours[1, 2] = cells_[p.X, p.Y + 1];
            }

            if (ValidCell(new Point(p.X, p.Y - 1)))
            {
                neighbours[1, 0] = cells_[p.X, p.Y - 1];
            }

            // check cases for diagonal neighbours
            // a disaster waiting to happen right here

            // bottom right
            if (ValidCell(new Point(p.X + 1, p.Y + 1)) 
                && !(cells_[p.X, p.Y + 1].Obstacle && cells_[p.X + 1, p.Y].Obstacle))
            {
                neighbours[2, 2] = cells_[p.X + 1, p.Y + 1];
            }

            // top right
            if (ValidCell(new Point(p.X - 1, p.Y + 1))
                && !(cells_[p.X, p.Y + 1].Obstacle && cells_[p.X - 1, p.Y].Obstacle))
            {
                neighbours[0, 2] = cells_[p.X - 1, p.Y + 1];
            }

            // top left
            if (ValidCell(new Point(p.X - 1, p.Y - 1))
                && !(cells_[p.X, p.Y - 1].Obstacle && cells_[p.X - 1, p.Y].Obstacle))
            {
                neighbours[0, 0] = cells_[p.X - 1, p.Y - 1];
            }

            // bottom left
            if (ValidCell(new Point(p.X + 1, p.Y - 1))
                && !(cells_[p.X, p.Y - 1].Obstacle && cells_[p.X + 1, p.Y].Obstacle))
            {
                neighbours[2, 0] = cells_[p.X + 1, p.Y - 1];
            }

            foreach (GridCell c in neighbours) { if (c != null) { cells_[p.X,p.Y].Neighbors++; } }
         
            return neighbours;
        }

        // Count the number of neighbors of a point
        internal int CountNeighbors(Point p)
        {
            int numNeighbor = 0;
            if (ValidCell(new Point(p.X + 1, p.Y)))
            {
                numNeighbor++;
            }

            if (ValidCell(new Point(p.X - 1, p.Y)))
            {
                numNeighbor++;
            }

            if (ValidCell(new Point(p.X, p.Y + 1)))
            {
                numNeighbor++;
            }

            if (ValidCell(new Point(p.X, p.Y - 1)))
            {
                numNeighbor++;
            }

            // check cases for diagonal neighbours
            // a disaster waiting to happen right here

            // top right
            if (ValidCell(new Point(p.X + 1, p.Y + 1))
                && !(cells_[p.X, p.Y + 1].Obstacle && cells_[p.X + 1, p.Y].Obstacle))
            {
                numNeighbor++;
            }

            // top left
            if (ValidCell(new Point(p.X - 1, p.Y + 1))
                && !(cells_[p.X, p.Y + 1].Obstacle && cells_[p.X - 1, p.Y].Obstacle))
            {
                numNeighbor++;
            }

            // bottom left 
            if (ValidCell(new Point(p.X - 1, p.Y - 1))
                && !(cells_[p.X, p.Y - 1].Obstacle && cells_[p.X - 1, p.Y].Obstacle))
            {
                numNeighbor++;
            }

            // bottom right
            if (ValidCell(new Point(p.X + 1, p.Y - 1))
                && !(cells_[p.X, p.Y - 1].Obstacle && cells_[p.X + 1, p.Y].Obstacle))
            {
                numNeighbor++;
            }

            return numNeighbor;
        }

        /// <summary>
        /// Use to get and set properties of a cell in the grid.
        /// </summary>
        /// <param name="p">Cartesian Coordinate of cell.</param>
        /// <returns>GridCell at point p.</returns>
        internal GridCell Cell(Point p)
        {
            return cells_[p.X, p.Y];
        }

        /// <summary>
        /// </summary>
        /// <returns>ASCII representation of grid map.</returns>
        internal string Ascii()
        {
            StringBuilder accum = new StringBuilder();

            for (int y = 0; y < dimensions_.Y + 2; y++)
            {
                accum.Append("-");
            }

            accum.Append("\n");

            //accum.Append("|");

            for (int y = dimensions_.Y - 1; y >= 0; y--)
            {
                accum.Append("|");
                for (int x = 0; x < dimensions_.X; x++)
                {
                    if (cells_[x, y].Obstacle)
                    {
                        accum.Append("X");
                    }
                    else if (cells_[x, y].Start)
                    {
                        accum.Append("s");
                    }
                    else if (cells_[x, y].Goal)
                    {
                        accum.Append("g");
                    }
                    else if (cells_[x, y].Use)
                    {
                        accum.Append("*");
                    }
                    else if (cells_[x, y].Checked)
                    {
                        accum.Append("·");
                    }
                    else
                    {
                        accum.Append(" ");
                    }
                }
                accum.Append("|");
                accum.Append("\n");
                
            }

            for (int y = 0; y < dimensions_.Y + 2; y++)
            {
                accum.Append("-");
            }

            return accum.ToString();
        }
    }
}

