using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Grid
{
    internal class GridCell : IComparable
    {
        /// <summary>
        /// Cartesian coordinates of cell.
        /// </summary>
        public Point XY;

        /// <summary>
        /// The cell traversed before the current cell in the optimal path.
        /// </summary>
        public GridCell Parent = null;

        /// <summary>
        /// Cost to get to cell from start.
        /// </summary>
        public double G = int.MaxValue; // cost to get to cell from start

        /// <summary>
        /// Straigh line distance from cell to goal.
        /// </summary>
        public double H = int.MaxValue; // straight line distance from cell to goal

        /// <summary>
        /// How many times being visited
        /// </summary>
        public double Explored = 0; // How many times being visited

        /// <summary>
        /// How many neighbors the cell has
        /// </summary>
        public double Neighbors = 0; // How many times being visited

        /// <summary>
        /// True if cell is part of an optimal path.
        /// </summary>
        public bool Use = false;

        /// <summary>
        /// True if cell has already been checked.
        /// </summary>
        public bool Checked = false;
        
        /// <summary>
        /// True if cell is an obstacle.
        /// </summary>
        public bool Obstacle;
        
        /// <summary>
        /// True if cell is start.
        /// </summary>
        public bool Start = false;
        
        /// <summary>
        /// True if cell is goal.
        /// </summary>
        public bool Goal = false;

        public bool Forced = false;

        /// <summary>
        /// Handle (reference) for modifying GridCell when it's in the priority queue.
        /// </summary>
        public C5.IPriorityQueueHandle<GridCell> Handle = null; // probably a cleaner way to do this?

        /// <summary>
        /// Construct a GridCell.
        /// </summary>
        /// <param name="xy">Coordinates of cell.</param>
        /// <param name="obstacle">True if cell is an obstacle.</param>
        public GridCell(Point xy, bool obstacle)
        {
            XY = new Point(xy.X, xy.Y);
            Obstacle = obstacle;
        }

        // comparision by sum of g_ and h_;
        int IComparable.CompareTo(object obj)
        {
            GridCell c = (GridCell)obj;

            if (G + H< c.G + c.H)
            {
                return -1;
            }
            else if (G + H > c.G + c.H)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return XY.ToString();
        }
    }
}
