using System;
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
        bool[,] universe = new bool[20, 20];

        // The scratch pad array
        bool[,] scratchPad = new bool[20, 20];

        // To contain the cell's data
        const int Alive = 1;
        const int Dead = 0;
        static int columns = 25;
        static int rows = 25;
        string[,] cellData = new string[columns, rows];

        int neighbors = 0;
        bool neighborDisplay = true;

        // Drawing colors
        Color gridColor;
        Color cellColor;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            //application title
            this.Text = Properties.Resources.AppTitle;

            //window size
            Size = new Size(700, 650);

            // Setup the timer
            timer.Interval = Properties.Settings.Default.Time;
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;

        }

        // Figures out how many neighbors each cell has
        public int Neighbors(int cellX, int cellY)
        {
            int isAlive = 0;
            // checking for boundries
            for (int y = cellY - 1; y <= cellY + 1; y++)
            {
                int yNeighbor = y;
                if (y < 0)
                {
                    yNeighbor = universe.GetLength(1) - 1;
                }
                else if (y > universe.GetLength(1) - 1)
                {
                    yNeighbor = 0;
                }
                for (int x = cellX - 1; x <= cellX + 1; x++)
                {
                    int xNeighbor = x;
                    if (x < 0)
                    {
                        xNeighbor = universe.GetLength(0) - 1;
                    }
                    else if (x > universe.GetLength(0) - 1)
                    {
                        xNeighbor = 0;
                    }
                    if (universe[xNeighbor, yNeighbor] == true && (x != cellX || y != cellY))
                    {
                        isAlive++;
                    }
                }
            }
            return isAlive;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            generations++;

            // Rules
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    neighbors = Neighbors(x, y);
                    if (universe[x, y] == true)
                    {
                        if (neighbors < 2)
                        {
                            scratchPad[x, y] = false;
                        }
                        else if (neighbors > 3)
                        {
                            scratchPad[x, y] = false;
                        }
                        else if (neighbors == 2 || neighbors == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    else if (universe[x, y] == false && neighbors == 3)
                    {
                        scratchPad[x, y] = true;
                    }
                }
            }
            universe = (bool[,])scratchPad.Clone();
            graphicsPanel1.Invalidate();
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

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    if (scratchPad[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (neighborDisplay == true)
                    {
                        Brush brush = Brushes.Green;
                        neighbors = Neighbors(x, y);
                        if (neighbors >= 3)
                        {
                            brush = Brushes.Red;
                        }
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        Rectangle rectangle = new Rectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                        if (neighbors != 0)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), Font, brush, rectangle, format);
                        }
                    }
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
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    scratchPad[x, y] = false;
                    // resets the generation count to 0
                    generations = 0;
                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    // Tells windows to repaint
                    graphicsPanel1.Invalidate();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NewStrip_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    scratchPad[x, y] = false;
                    // resets the generation count to 0
                    generations = 0;
                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    // Tells windows to repaint
                    graphicsPanel1.Invalidate();
                }
            }
        }
    }
}
