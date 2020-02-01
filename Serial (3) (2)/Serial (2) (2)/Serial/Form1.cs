using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;


namespace Serial
{
    public interface IMyInterface
    {
        void SetData(String Data);
    }
    public partial class Connect : Form, IMyInterface
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        private Point m_PrevPoint;

        public delegate void OnMoveEvent(Point pt);
        public OnMoveEvent MoveEventHandler;

        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;

        private bool mouse_drag = false;

        public SensorValue sens = new SensorValue();

       

        public Connect()
        {
            InitializeComponent();

            MoveEventHandler = new OnMoveEvent(MoveEventHandler_Body);

            m_PrevPoint = new Point();

            //텍스스 박스 초기화;
            //rbText.ScrollBars = RichTextBoxScrollBars.Vertical;

            //Port
            cmbPort.Items.Clear();   // 리스트 박스 초기화
            cmbPort.BeginUpdate();
            foreach (string comport in SerialPort.GetPortNames())
            {
                if (comport.Length == 5)
                {
                    if (Char.IsDigit(Char.Parse(comport.Substring(3, 1))))
                        cmbPort.Items.Add(comport.Substring(0, 4));
                }
                else if (comport.Length == 6)
                {
                    if (Char.IsDigit(Char.Parse(comport.Substring(4, 1))))
                        cmbPort.Items.Add(comport.Substring(0, 5));
                }
                else cmbPort.Items.Add(comport);
            }
            cmbPort.EndUpdate();

            //SerialPort 초기 설정
            SP.PortName = "COM1";
            SP.BaudRate = (int)9600;
            SP.DataBits = (int)8;
            SP.Parity = Parity.None;
            SP.StopBits = StopBits.One;
            //SP.ReadTimeout = (int)100;
            //SP.WriteTimeout = (int)100;

            
        }

        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //마우스 무브 다운 업하기
            

            //향후 진행 방향 - 로봇, 드론
            // 애견..자동 장남감 (강아지)
            if (SP.IsOpen) // 포트 오픈
            {
                sens.str = SP.ReadLine(); // 입력 버퍼에서 NewLine(줄끝을 표현)값까지 읽음

                sens.str = sens.str.Trim().Replace("\r\n", "");

                sens.spstring = sens.str.Split(sens.sp);

                sens.preButton1 = sens.spstring[0]; //첫번째 손가락 1/0/0 (왼클릭)
                sens.preButton2 = sens.spstring[1]; //두번째 손가락 0/1/0 (오른클릭)
                sens.preButton3 = sens.spstring[2]; //세번째 손가락 0/0/1 (드레그 및 레이져)
                sens.sX = sens.spstring[3];
                sens.sY = sens.spstring[4];

                sens.preButton1_int = Convert.ToInt32(sens.preButton1);
                sens.preButton2_int = Convert.ToInt32(sens.preButton2);
                sens.preButton3_int = Convert.ToInt32(sens.preButton3);
                sens.GyX = Convert.ToInt32(sens.sX); // 문자를 숫자로 변환
                sens.GyY = Convert.ToInt32(sens.sY);

                //lbResult.Text = str;
                // rbText.Text = string.Format("{0}{1}{2}", rbText.Text, "[Received]", sens.str+"\r\n");
                // rbText.SelectionStart = rbText.Text.Length;
                //rbText.ScrollToCaret();
                //rbText.Text += "[전송된 Data] " + str;

                SetCursorPos(sens.GyX, sens.GyY);

                if (sens.preButton1_int == 1  )
                {
                    mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }

                if (sens.preButton2_int == 1 )
                {
                    mouse_event(MOUSEEVENTF_RIGHTUP | MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                }

                if (sens.preButton3_int == 1 )
                {        
                    mouse_drag = !mouse_drag;
                }

                if (sens.preButton1_int == 2)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }

                if(mouse_drag == true)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }
                else if (mouse_drag == false)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }

            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
                SP.Open();
                if (SP.IsOpen)
                {
                    //rbText.Text = string.Format("{0}{1}", rbText.Text, "\r\n[Succed] Port Open!!");
                   // rbText.Text = "["+SP.PortName.ToString() +"] Port Open Connect!!";
                    lbStatus.Text = "Connect!!";
                    btnOpen.Visible = false;
                    btnPortClose.Visible = true;
                
            }
                else
                {
                    //rbText.Text = string.Format("{0}{1}", rbText.Text, "\r\n[Fail] Port Open!!");
                    //rbText.Text = "[" + SP.PortName.ToString() + "] Port Open Failed!";
                    lbStatus.Text = "[Fail] Port Open!";
                    lbStatus.ForeColor = Color.Red;
                }
        }

        private void cmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SP.PortName = cmbPort.SelectedItem.ToString();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btnPortClose_Click(object sender, EventArgs e)
        {
            SP.Close();
            //rbText.Text += "\r\n" + "[" + SP.PortName.ToString() + "] Port Close!!";
            lbStatus.Text = "Not Connect!!";
            btnOpen.Visible = true;
            btnPortClose.Visible = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SP.IsOpen)
            {
                SP.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Connect_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.MoveEventHandler(
                    JPointCalculator.Add(
                        this.Location,
                        JPointCalculator.Subtract(Cursor.Position, m_PrevPoint)));
                JPointCalculator.Set(Cursor.Position, ref m_PrevPoint);
            }
        }

        private void MoveEventHandler_Body(Point pt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(MoveEventHandler, new object[] { pt });
            }
            else
            {
                this.Location = pt;
            }

        }

        public void SetData(string Data)
        {
            throw new NotImplementedException();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 fo1 = new Form1(this as IMyInterface);
            fo1.Show();
        }


    }

    public class SensorValue
    {
        // 자이로센서 x,y
        public int GyX, GyY = 0;
        public string sX, sY = "";
        // Button
        public string preButton1, preButton2, preButton3 = "";
        public string[] spstring = { };
        public char sp = '/';
        public int preButton1_int, preButton2_int, preButton3_int;
        public string str = "";
    }

    public class JPointCalculator
    {
        public static Point Add(Point x, Point y)
        {
            return new Point(x.X + y.X, x.Y + y.Y);
        }

        public static void Add(Point x, Point y, ref Point dst)
        {
            dst.Offset(x.X + y.X, x.Y + y.Y);
        }

        public static Point Subtract(Point x, Point y)
        {
            return new Point(x.X - y.X , x.Y - y.Y);
        }

        public static void Subtract(Point x, Point y, ref Point dst)
        {
            dst.Offset(x.X - y.X, x.Y - y.Y);
        }

        public static void Set(Point x, ref Point dst)
        {
            dst.X = x.X;
            dst.Y = x.Y;
        }
    }
}
