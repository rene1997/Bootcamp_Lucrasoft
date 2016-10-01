using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;

namespace bootcamp_arduino_program
{

    public partial class Form1 : Form
    {
        bool readingScore = false;
        SerialPort port;
        List<myButtonObject> buttons = new List<myButtonObject>();

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPressed);

            try
            {
                port = new SerialPort("COM3", 9600);
                port.Open();
            }
            catch
            {
                label6.Text = "could not connect";
                label6.ForeColor = Color.Red;
                return;
            }
            

            //set difficulty
            port.Write("" + 6);

            makeDisplay(5, 7);

            Thread newThread = new Thread(readPort);
            newThread.Start();
        }
        
        private void makeDisplay(int rows, int columns)
        {
            
            for (int i = 0; i < columns; i++)
            {
                for(int j = 0; j < rows; j++)
                {
                    myButtonObject newButton = new myButtonObject();
                    newButton.Location = new Point(450 + i * 35, 60 + j * 35);
                    newButton.Size = new Size(25, 25);
                    buttons.Add(newButton);
                    this.Controls.Add(newButton);
                }
            }

            foreach (myButtonObject b in buttons)
            {
                b.BackColor = Color.LightGray;
            }
        }

        void Form1_KeyPressed(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'w':
                    SetSnakeDirection(0);
                    break;
                case 'a':
                    SetSnakeDirection(3);
                    break;
                case 's':
                    SetSnakeDirection(2);
                    break;
                case 'd':
                    SetSnakeDirection(1);
                    break;
                case 'r':
                    SetSnakeDirection(4);
                    this.Invoke((MethodInvoker)delegate {
                        label5.ForeColor = Color.Black;
                        label5.Text = "2";
                        label6.ForeColor = Color.Black;
                    });
                    break;
                case 'p':
                    SetSnakeDirection(9);
                    break;
            }
        }

        private void readPort()
        {
            while (true)
            {
                string s = port.ReadLine();
                if(s.Length > 0)
                {
                    if (readingScore)
                    {
                        readingScore = false;
                        this.Invoke((MethodInvoker)delegate {
                            label5.Text = s; // runs on UI thread
                        });
                    }
                    if (s.StartsWith("score:"))
                        readingScore = true;

                    if(s.StartsWith("game over"))
                    {
                        this.Invoke((MethodInvoker)delegate {
                            label5.ForeColor = Color.Red; // runs on UI thread
                            label6.ForeColor = Color.Red;
                        });
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate {
                            label5.ForeColor = Color.Black; // runs on UI thread
                            label6.ForeColor = Color.Black;
                        });
                    }
                    if (s.StartsWith("snakepos:")){
                        setSnake(s);
                    }else
                    {
                        this.Invoke((MethodInvoker)delegate {
                            label1.Text = s; // runs on UI thread
                        });
                    }
                }
            }
        }

        private void setSnake(string pos)
        {
            foreach(myButtonObject b in buttons)
            {
                b.BackColor = Color.LightGray;
            }
            //pos is: "snakepos: 0,1;0,0;" 
            pos = pos.Replace("snakepos:", String.Empty);
            string[] xypos = pos.Split(';');
            for(int i = 0; i < xypos.Length; i ++)
            {
                string[] position = xypos[i].Split(',');
                if(position.Length > 1)
                {
                    int x = Int32.Parse(position[0]);
                    int y = Int32.Parse(position[1]);
                    int buttonIndex = ((6-x) * 5) + (y);

                    if (buttonIndex >= buttons.Count || buttonIndex < 0)
                    {
                        //do nothing
                    }
                    else if(i == 0)
                        buttons[buttonIndex].BackColor = Color.Green;
                    else if(i == 1)
                        buttons[buttonIndex].BackColor = Color.Orange;
                    else
                        buttons[buttonIndex].BackColor = Color.Red;
                }
            }
        }

        private void SetSnakeDirection(int direction)
        {
            port.Write("" + direction);
        }

        //right
        private void button1_Click(object sender, EventArgs e)
        {
            SetSnakeDirection(1);
        }

        //down
        private void button2_Click(object sender, EventArgs e)
        {
            SetSnakeDirection(2);
        }

        //left
        private void button3_Click(object sender, EventArgs e)
        {
            SetSnakeDirection(3);
        }

        //up
        private void button4_Click(object sender, EventArgs e)
        {
            SetSnakeDirection(0);
        }

        //reset game
        private void button5_Click(object sender, EventArgs e)
        {
            port.Write("" + 4);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            port.Write("" + 7);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            port.Write("" + 6);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            port.Write("" + 5);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            port.Write("" + 8);
        }
    }

    public class myButtonObject : UserControl
    {
        // Draw the new button. 
        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath grPath = new GraphicsPath();
            grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new System.Drawing.Region(grPath);
            base.OnPaint(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "myButtonObject";
            this.Load += new System.EventHandler(this.myButtonObject_Load);
            this.ResumeLayout(false);
        }

        private void myButtonObject_Load(object sender, EventArgs e)
        {

        }
    }
}
