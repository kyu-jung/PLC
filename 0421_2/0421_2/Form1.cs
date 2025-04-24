using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CIFX_50RE;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace _0421_2
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv = "00000000";
        private string WriteDataConv = "00000000";

        int Auto = 0;
        int Count = 0;
        int Countmax = 3;
        bool RepeatMode = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            if (connect != 0)
            {
                label2.Text = "OK";
                label2.BackColor = Color.Pink;
                label2.ForeColor = Color.Blue;
                timer1.Interval = 300;
                timer1.Start();

            }
            else
            {
                label2.Text = "NG";
                label2.ForeColor = Color.White;
                label2.BackColor = Color.Red;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label2.Text == "OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv;

                WriteDataConv = Convert.ToString(Writedata[0],2).PadLeft(8, '0');
                label6.Text = WriteDataConv;

            }
        }

        private void button2_Click(object sender, EventArgs e) // 실린더 A 전진
        {
            Writedata[0] |= 0x01;
            Writedata[0] &= unchecked((byte)~0x02);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "실린더 A 전진";
            label8.BackColor = Color.LightGreen;
        }

        private void button3_Click(object sender, EventArgs e) // 실린더 A 후진
        {
            Writedata[0] |= 0x02;
            Writedata[0] &= unchecked((byte)~0x01);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "실린더 A 후진";
            label7.BackColor = Color.LightGreen;
        }

        private void button5_Click(object sender, EventArgs e) // 실린더 B 전진
        {
            Writedata[0] |= 0x04;
            Writedata[0] &= unchecked((byte)~0x08);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "실린더 B 전진";
            label10.BackColor = Color.LimeGreen;
        }

        private void button4_Click(object sender, EventArgs e) // 실린더 B 후진
        {
            Writedata[0] |= 0x08;
            Writedata[0] &= unchecked((byte)~0x04);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "실린더 B 후진";
            label9.BackColor = Color.LimeGreen;
        }

        private void button6_Click(object sender, EventArgs e) // 자동모드
        {
            timer2.Interval = 300;
            timer2.Start();

            Auto = 0;
            button1.Text = "자동모드";
            button1.BackColor = Color.LightGreen;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            switch (Auto)
            {
                case 0:
                    if (ReadDataConv[7] == '1')
                    {
                        Writedata[0] = 0x01;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 1:
                    if (ReadDataConv[6] == '1' && ReadDataConv[5] == '1')
                    {
                        Writedata[0] = 0x04;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 2:
                    if (ReadDataConv[4] == '1')
                    {
                        Writedata[0] = 0x02;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 3:
                    if (ReadDataConv[4] == '1' && ReadDataConv[7] == '1')
                    {
                        Writedata[0] = 0x08;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 4:
                    if (ReadDataConv[7] == '1' && ReadDataConv[5] == '1')
                    {
                        Auto = 0;

                        if (RepeatMode)
                        {
                            Count++;
                            if (Count >= Countmax)
                            {
                                timer2.Stop();
                                RepeatMode = false;
                                button1.Text = "자동운전 정지";
                                button1.BackColor = Color.Red;
                            }
                        }
                    }
                    break;
            }
        }

        private void button7_Click(object sender, EventArgs e) // 3회 반복
        {
            Count = 0;
            Countmax = 3;
            RepeatMode = true;
            Auto = 0;

            button1.Text = "자동모드 3회 반복";
            button1.BackColor = Color.SpringGreen;
            timer2.Interval = 300;
            timer2.Start();
        }

        private void button8_Click(object sender, EventArgs e) // 자동운전 정지
        {
            timer2.Stop();
            Auto = 0;
            button1.Text = "자동운전 정지";
            button1.BackColor = Color.Red;
        }

        private void button9_Click(object sender, EventArgs e) // 초기화
        {
            timer2.Stop();
            Auto = 0;

            Writedata[0] = (byte)0x0a;
            CIFX.xChannelWrite(Writedata);

            button1.Text = "초기화";
            button1.BackColor = Color.Magenta;
        }
    }
}
