using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Grid;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Text;

using Data;

namespace MinotaurPathfinder
{
    public partial class Map : Form
    {
        private GridMap map_;

        private Point current_ = new Point();   // current cell that mouse is on
        
        private bool mouseDown_ = false;
        private bool addObstacle;               // true if mouse down on empty space

        private DataLogger dataLogger_;

       // private Thread visualize_;

        private Random rand;
        

        public Map()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();

            rand = new Random();
            dataLogger_ = new DataLogger(@"\\psf\Home\Desktop\Research_(SharkTrack)\Exploration\", fileNametb.Text);
            boardControl1.MouseDown += new MouseEventHandler(boardControl1_MouseDown);
            boardControl1.MouseUp += new MouseEventHandler(boardControl1_MouseUp);
            boardControl1.MouseMove += new MouseEventHandler(boardControl1_MouseMove);
            boardControl1.MouseMoveSpecial += new EventHandler(boardControl1_MouseMoveSpecial);
            boardControl1.LostFocus += new EventHandler(form_LostFocus);

            toolStripStatusLabel1.Text = "";
            ResetMap(); 
            ReDraw();
        }


        void boardControl1_MouseMoveSpecial(object sender, EventArgs e)
        {                        
            Point point = boardControl1.Pos;
            if (point.X == -1 || point.Y == -1) { return; }
            toolStripStatusLabel1.Text = point.ToString();
        }

       
        void boardControl1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown_ = true;

            // Get coordinate of where click down occured
            Point point = boardControl1.Pos;
            if (point.X == -1 || point.Y == -1) { return; }
            if (point.X > BoardControl.DIMENSION - 1 || point.Y > BoardControl.DIMENSION - 1) { return; }

            current_.X = point.X;
            current_.Y = point.Y;

            // Decide if obstacles should be added or removed
            if (map_.Obstacle(point)) { addObstacle = false; }
            else { addObstacle = true; }

            map_.ToggleObstacle(point);

            ReDraw();
        }

        void boardControl1_MouseUp(object sender, MouseEventArgs e)
        {
            // reset flags
            mouseDown_ = false;

            // calculate path and redraw upon mouse click release
            boardControl1.DrawPath = true;

            ReDraw();
        }

        void boardControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = boardControl1.Pos;
          
            // try catch block to handle exception when cursor goes out of drawing board
            try 
            {
                // only update if mouse has moved to a different cell
                if (mouseDown_ && !current_.Equals(point))
                {
                        if (addObstacle) { map_.AddObstacle(point); }
                        else { map_.RemoveObstacle(point); }

                    current_.X = point.X;
                    current_.Y = point.Y;

                    ReDraw();
                }
            }
            catch (IndexOutOfRangeException) { }        
        }

        private void ReDraw()
        {
            Color setColor = Color.Gainsboro;
            Point point = new Point();

            for (int x = 0; x < BoardControl.DIMENSION; x++)
            {
                point.X = x;

                for (int y = 0; y < BoardControl.DIMENSION; y++)
                {
                    point.Y = y;

                    if (map_.Obstacle(point)) { setColor = Color.Gray; }
                    else { setColor = Color.White; }

                    boardControl1.SetHighlight(setColor, x, y);
                }
            }
            boardControl1.Invalidate();
         }

        // clears board of paths and traces
        private void ClearDrawing()
        {
            for (int x = 0; x < BoardControl.DIMENSION; x++)
            {
                for (int y = 0; y < BoardControl.DIMENSION; y++)
                {
                    Color current = boardControl1.GetHighlight(x, y);

                    if (current.Equals(Color.Turquoise) || current.Equals(Color.LightGreen))
                    {
                        boardControl1.SetHighlight(Color.White, x, y);
                    }
                }
            }

            boardControl1.Refresh();
        }

        // prevents mouse down from "sticking" when clicking and holding and focus
        // of window is lost
        private void form_LostFocus(object sender, EventArgs e)
        {
            mouseDown_ = false;
            ReDraw();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetMap();
            ReDraw();
        }

        private void ResetMap()
        {
            map_ = new GridMap(new Point(BoardControl.DIMENSION, BoardControl.DIMENSION), false);
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
           /* if (btnVisualize.Text.Equals("Visualize"))
            {
                ClearDrawing();
                // serach visualization on seperate thread to prevent whole form from locking up
                //visualize_ = new Thread(new ThreadStart(LiveDraw));
                visualize_.Start();
            }
            else
            {
                visualize_.Abort();
                boardControl1.Enabled = true;
                btnVisualize.Text = "Visualize";
                boardControl1.DrawPath = true;
                ReDraw();
            }
            */
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            map_.GetTransitionMat();
            logData();
        }

        // Log simulation Data
        private void logData()
        {
            StringBuilder trans = new StringBuilder();
            for (int i = 0; i < map_.transition.Count; i++)
            {
                trans.Clear();
                for (int j = 0; j < map_.transition[i].Length; j++)
                {
                    trans.Append(map_.transition[i][j].ToString());
                    trans.Append(",");
                }
                dataLogger_.Log(trans.ToString());
            }
        }
    }
}
