using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace MinotaurPathfinder
{
    /*
     * 
     * This class does the dirty work with drawing things on screen.
     * It has many optimizations that I developed in a previous program.
     * 
     * */
    public partial class BoardControl : UserControl
    {
        public static int DIMENSION = 10;
        public static int WIDTH = 8;

        public event EventHandler MouseMoveSpecial;
        public List<Point> Path = new List<Point>();
        public bool DrawPath = true;

        private Color[,] _highlights = new Color[DIMENSION, DIMENSION];
        private Point _pos = new Point(-1, -1);
        
        
        public Point Pos
        {
            get { return _pos; }
        }

        Rectangle _thisRect;
        Rectangle _thisRectOnPaint = new Rectangle();
        Rectangle _borderRectOnPaint = new Rectangle();

        public BoardControl()
        {
            InitializeComponent();
            _thisRect = this.Bounds;
            _thisRectOnPaint.Height = _thisRectOnPaint.Width = WIDTH;
            _borderRectOnPaint.Height = _borderRectOnPaint.Width = WIDTH - 1;
        }

        public void SetHighlight(Color highlightColor, int x, int y)
        {
            _highlights[x, y] = highlightColor;
        }

        public Color GetHighlight(int x, int y)
        {
            return _highlights[x, y];
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _pos.X = _pos.Y = -1;

            if (MouseMoveSpecial != null)
            {
                MouseMoveSpecial(null, e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_thisRect.Contains(e.Location) == false)
            {
                if (_pos.X == -1 || _pos.Y == -1)
                {
                    return;
                }
                _pos.X = _pos.Y = -1;
            }
            else
            {
                int posX = e.X / WIDTH;
                int posY = e.Y / WIDTH;

                if (posX == _pos.X && posY == _pos.Y)
                {
                    return;
                }
                _pos.X = posX;
                _pos.Y = posY;
            }

            if (MouseMoveSpecial != null)
            {
                MouseMoveSpecial(null, e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphicsContext = e.Graphics;

            


            for (int j = DIMENSION - 1, startY = WIDTH * (DIMENSION - 1); j >= 0; j--, startY -= WIDTH)
            {
                for (int i = 0, startX = 0; i < DIMENSION; i++, startX += WIDTH)
                {
                    _thisRectOnPaint.X = startX;
                    _thisRectOnPaint.Y = startY;

                    Color colorHere = _highlights[i, j];

                    if (colorHere != Color.White)
                    {
                        Brush brush = new SolidBrush(colorHere);
                        graphicsContext.FillRectangle(brush, _thisRectOnPaint);
                    }
                    
                }
            }

            Pen pen = new Pen(Color.Gray, 1);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Left;
            
            // draw grid
            for (int i = 1; i < DIMENSION; i++)
            {
                graphicsContext.DrawLine(pen, i * WIDTH, 0, i * WIDTH, DIMENSION * WIDTH);
            }

            for (int i = 1; i < DIMENSION; i++)
            {
                graphicsContext.DrawLine(pen, 0, i * WIDTH, DIMENSION * WIDTH, i * WIDTH);
            }
            
            pen = new Pen(Color.DarkBlue, 2);
            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
            
            for (int i = 0; i < Path.Count - 1 && DrawPath; i++)
            {
                graphicsContext.DrawLine(pen,
                    Path[i].X * WIDTH + WIDTH / 2, Path[i].Y * WIDTH + WIDTH / 2,
                    Path[i + 1].X * WIDTH + WIDTH / 2, Path[i + 1].Y * WIDTH + WIDTH / 2);
            }

        }
    }
}
