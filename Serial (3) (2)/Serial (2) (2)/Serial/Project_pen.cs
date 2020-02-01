using System;
using System.Drawing;
using System.Windows.Forms;



namespace Serial
{
    
    public partial class Form1 : Form
    {

        int cap_flag = 0;
        int pen_flag;
        int bold=1;
        public Point p;
        Graphics G;
        private IMyInterface myInterface= null;

        public Form1(IMyInterface myInterface)
        {
            InitializeComponent();
            this.myInterface = myInterface;
        }


        private void button1_Click(object sender, EventArgs e) //빨강
        {
            cap_flag = 1;
            pen_flag = 1;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new Point(0, 0);
            this.BackColor = Color.Teal;            
            panel2.BackColor = Color.Red;

        }

        private void button2_Click(object sender, EventArgs e) //파랑
        {
            cap_flag = 1;
            pen_flag = 2;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new Point(0, 0);
            this.BackColor = Color.Teal;
            panel2.BackColor = Color.Blue;
        }

        private void button3_Click(object sender, EventArgs e) //검정
        {
            cap_flag = 1;
            pen_flag = 3;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new Point(0, 0);
            this.BackColor = Color.Teal;
            panel2.BackColor = Color.Black;
        }

        private void button4_Click(object sender, EventArgs e) //흰색
        {
            cap_flag = 1;
            pen_flag = 4;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new Point(0, 0);
            this.BackColor = Color.Teal;
            panel2.BackColor = Color.White;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pen_flag == 1 || pen_flag == 2 || pen_flag == 3 || pen_flag == 4)
            {
                pen_flag = 0;
                G = CreateGraphics();
                G.Clear(Color.Teal);
                G.Dispose();
            }

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            G = CreateGraphics();
            if (cap_flag == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (pen_flag == 1)
                    {
                        Pen P = new Pen(Color.Red , bold);
                        G.DrawLine(P, p.X, p.Y, e.X, e.Y);
                    }

                    else if (pen_flag == 2)
                    {
                        Pen P = new Pen(Color.Blue, bold);
                        G.DrawLine(P, p.X, p.Y, e.X, e.Y);
                    }

                    else if (pen_flag == 3)
                    {
                        Pen P = new Pen(Color.Black, bold);
                        G.DrawLine(P, p.X, p.Y, e.X, e.Y);
                    }

                    else if (pen_flag == 4)
                    {
                        Pen P = new Pen(Color.White, bold);
                        G.DrawLine(P, p.X, p.Y, e.X, e.Y);
                    }


                }
            }
            p.X = e.X;
            p.Y = e.Y;
            G.Dispose();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                p.X = e.X;
                p.Y = e.Y;
            }
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            G = CreateGraphics();
            if (e.Button == MouseButtons.Right && cap_flag == 1)
            {
                cap_flag = 0;
                this.Size = new Size(181, 165);
                G.Clear(Color.FromArgb(50, 50, 50));
                this.BackColor = Color.FromArgb(50, 50, 50);
                G.Dispose();
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bold = 3;
            label2.Text = "3";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            bold = 6;
            label2.Text = "6";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            bold = 10;
            label2.Text = "10";
        }
        
    }
 }
