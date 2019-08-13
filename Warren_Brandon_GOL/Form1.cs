﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// Explaination (StartUp Template Paint): https://youtu.be/c8dn8XAUmRk

namespace Warren_Brandon_GOL
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[25, 25];

        // The scratch pad array
        bool[,] scratchPad = new bool[25, 25];

        // To contain the cell's data
        static int columns = 25;
        static int rows = 25;
        string[,] cellData = new string[columns, rows];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            //application title
            this.Text = Properties.Resources.AppTitle;

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running
        }

        // Figures out how many neighbors each cell has
        private void Neighbors()
        {
            for(int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int neighbors = 0;
                    for(int i = row - 1; i < row + 2; i++)
                    {
                        for(int j = col - 1; j < col + 2; j++)
                        {
                            try
                            {
                                if (i == row && j == col)
                                {
                                    neighbors += 0;
                                }
                                //else if "Alive")
                                else if (universe[j,i] == true)
                                {
                                    neighbors += 1;
                                }
                            }
                            catch(Exception e)
                            {
                                neighbors += 0;
                            }
                        }
                    }
                    cellData[col, row] = neighbors.ToString();
                } // crashes here
            }
        }

        // Update the universe to the next generation
        private void UpdateUniverse()
        {
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);
            foreach (Control cell in graphicsPanel1.Controls)
            {
                int xInd = cell.Left / cellWidth;
                int yInd = cell.Top / cellHeight;
                int neighbors = Convert.ToInt32(cellData[xInd, yInd]);
                if (neighbors < 2 | neighbors > 3)
                {
                    //Dead
                    universe[xInd, yInd] = false;
                    cell.BackColor = Color.White;
                }
                if (neighbors == 3)
                {
                    //alive
                    universe[xInd, yInd] = true;
                    cell.BackColor = cellColor;
                }
            }
        }


        // Calculate the next generation of cells
        private void NextGeneration()
        {
            Neighbors();
            UpdateUniverse();

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        // e = 'device context' || interfaces with device drivers and converts our code to what the device understands
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Convert to FLOATS! https://youtu.be/aD-Y-3PT1Oo
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //RectangleF floatRect;
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes (helps garbage collector)
            gridPen.Dispose();
            cellBrush.Dispose();
        }
        // StartUp Template MouseClick: https://youtu.be/a6cx_MIUaeY
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            // Tell Windows you need to repaint
            graphicsPanel1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
