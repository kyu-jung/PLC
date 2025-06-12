using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CIFX_50RE;

namespace _0602_컨베이어_공작물_이송_시스템
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv = "00000000";
        private string WriteDataConv = "00000000";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            label15.Text = "수동 운전";


            if (connect != 0)
            {
                button1.Text = "OK";
                button1.ForeColor = Color.Blue;
                button1.BackColor = Color.Pink;

                timer1.Start();
                timer1.Interval = 100;
            }
            else
            {
                button1.Text = "NG";
                button1.ForeColor = Color.Black;
                button1.BackColor = Color.White;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv;

                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label6.Text = WriteDataConv;

                label7.BackColor = (ReadDataConv[1] == '1') ? Color.GreenYellow : Color.White;
                label8.BackColor = (ReadDataConv[7] == '1') ? Color.GreenYellow : Color.White;
                label9.BackColor = (ReadDataConv[6] == '1') ? Color.GreenYellow : Color.White;
                label10.BackColor = (ReadDataConv[5] == '1') ? Color.GreenYellow : Color.White;
                label11.BackColor = (ReadDataConv[4] == '1') ? Color.GreenYellow : Color.White;
                label12.BackColor = (ReadDataConv[3] == '1') ? Color.GreenYellow : Color.White;
                label13.BackColor = (ReadDataConv[2] == '1') ? Color.GreenYellow : Color.White;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)// 모터 정회전
        {
            Writedata[0] |= 0x40;
            Writedata[0] &= unchecked((byte)~0x80);
            CIFX.xChannelWrite(Writedata);

            label15.Text = "수동운전 상태";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)// 모터 역회전
        {
            Writedata[0] |= 0x80;
            Writedata[0] &= unchecked((byte)~0x40);
            CIFX.xChannelWrite(Writedata);

            label15.Text = "수동운전 상태";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)// 스토퍼 실린더 전진
        {
            if (ReadDataConv[7] == '1')
            {
                Writedata[0] |= 0x01;
                Writedata[0] &= unchecked((byte)~0x02);
                CIFX.xChannelWrite(Writedata);

                label15.Text = "수동운전 상태";
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)// 스토퍼 실린더 후진
        {
            if (ReadDataConv[6] == '1')
            {
                Writedata[0] |= 0x02;
                Writedata[0] &= unchecked((byte)~0x01);
                CIFX.xChannelWrite(Writedata);

                label15.Text = "수동운전 상태";
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)// 전후진 실린더 전진
        {
            if (ReadDataConv[5] == '1')
            {
                Writedata[0] |= 0x04;
                Writedata[0] &= unchecked((byte)~0x08);
                CIFX.xChannelWrite(Writedata);

                label15.Text = "수동운전 상태";
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)// 전후진 실린더 후진
        {
            if (ReadDataConv[4] == '1')
            {
                Writedata[0] |= 0x08;
                Writedata[0] &= unchecked((byte)~0x04);
                CIFX.xChannelWrite(Writedata);

                label15.Text = "수동운전 상태";
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)// 상하강 실린더 전진
        {
            if (ReadDataConv[3] == '1')
            {
                Writedata[0] ^= 0x10;
                CIFX.xChannelWrite(Writedata);

                label15.Text = "수동운전 상태";
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)// 상하상 실린더 후진
        {
            if (ReadDataConv[2] == '1')
            {
                Writedata[0] ^= 0x10;
                CIFX.xChannelWrite(Writedata);

                label15.Text = "수동운전 상태";
            }
        }

        int Auto = 0;
        int Count = 0;
        bool AutoStop = false;
        int Mode = 0;

        private void button2_Click(object sender, EventArgs e)// 자동운전(횟수지정)
        {
            timer3.Interval = 1000;
            timer3.Start();

            Mode = 1;
            Auto = 0;
            Count = 0;

            label15.Text = "자동운전 상태";
        }

        private int ClickCount = 0;
        private void button3_Click(object sender, EventArgs e)// 수동스텝제어
        {
            timer3.Interval = 1000;
            timer3.Start();

            Mode = 2;
            AutoStop = true;
            Auto = ClickCount;
            ClickCount++;
            Count = 0;

            label15.Text = "수동스텝제어 상태";
        }

        private void button4_Click(object sender, EventArgs e)// 정지 초기화
        {
            Writedata[0] = (byte)0x0a;
            CIFX.xChannelWrite(Writedata);

            Mode = 0;
            Auto = 0;
            Count = 0;
            ClickCount = 0;

            label15.Text = "정지 초기화";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label18.Text = Count.ToString();
            int Counting = Convert.ToInt32(numericUpDown1.Value);
            label20.Text = (Mode == 2) ? Auto.ToString() : "not step driving";

            if ((Mode == 1 && Count < Counting) || (Mode == 2 && AutoStop == true))
            {
                switch (Auto)
                {
                    case 0:
                        Writedata[0] |= 0x40;
                        CIFX.xChannelWrite(Writedata);

                        Thread.Sleep(2000);

                        Writedata[0] &= unchecked((byte)~0x40);
                        CIFX.xChannelWrite(Writedata);

                        if (ReadDataConv[7] == '1')
                        {
                            Writedata[0] |= 0x01;
                            Writedata[0] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 1:
                        if (ReadDataConv[6] == '1' && ReadDataConv[5] == '1')
                        {
                            Writedata[0] |= 0x06;
                            Writedata[0] &= unchecked((byte)~0x09);
                            CIFX.xChannelWrite(Writedata);

                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 2:
                        if (ReadDataConv[4] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x10;
                            CIFX.xChannelWrite(Writedata);
                        }

                        Thread.Sleep(1000);

                        if (ReadDataConv[2] == '1')
                        {
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 3:
                        if (ReadDataConv[4] == '1')
                        {
                            Writedata[0] |= 0x08;
                            Writedata[0] &= unchecked((byte)~0x04);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 4:
                        if (ReadDataConv[5] == '1')
                        {
                            Writedata[0] |= 0x80;
                            CIFX.xChannelWrite(Writedata);

                            Thread.Sleep(2000);

                            Writedata[0] &= unchecked((byte)~0x80);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 5:
                        if (ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x10;
                            CIFX.xChannelWrite(Writedata);
                        }
                        Thread.Sleep(500);
                        if (ReadDataConv[2] == '1')
                        {
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        ClickCount = 0;
                        break;
                    case 6:
                        if (Mode == 1)
                        {
                            Auto = 0;
                            Count++;
                        }
                        break;
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            label18.Text = Count.ToString();
            int Counting = Convert.ToInt32(numericUpDown1.Value);
            label20.Text = (Mode == 2) ? Auto.ToString() : "not step driving";

            if ((Mode == 1 && Count < Counting) || (Mode == 2 && AutoStop == true))
            {
                switch (Auto)
                {
                    case 0:
                        if (ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x10;
                            
                            CIFX.xChannelWrite(Writedata);
                        }

                        Thread.Sleep(1000);

                        if (ReadDataConv[2] == '1')
                        {
                            
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);

                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 1:
                        if (ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x80;
                            CIFX.xChannelWrite(Writedata);

                            Thread.Sleep(3000);

                            Writedata[0] &= unchecked((byte)~0x80);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 2:
                        if (ReadDataConv[5] == '1')
                        {
                            Writedata[0] |= 0x04;
                            Writedata[0] &= unchecked((byte)~0x08);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 3:
                        if (ReadDataConv[4] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x10;
                            
                            CIFX.xChannelWrite(Writedata);
                        }
                        
                        if (ReadDataConv[2] == '1')
                        {
                            
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);

                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 4:
                        if (ReadDataConv[4] == '1')
                        {
                            Writedata[0] |= 0x08;
                            Writedata[0] &= unchecked((byte)~0x04);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 5:
                        if (ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x40;
                            CIFX.xChannelWrite(Writedata);

                            Thread.Sleep(3000);

                            Writedata[0] &= unchecked((byte)~0x40);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 6:
                        if (ReadDataConv[7] == '1')
                        {
                            Writedata[0] |= 0x01;
                            Writedata[0] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);
                            
                        }
                        
                        if (ReadDataConv[6] == '1')
                        {
                            Writedata[0] |= 0x02;
                            Writedata[0] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        ClickCount = 0;
                        break;

                    case 7:
                        if (Mode == 1)
                        {
                            Auto = 0;
                            Count++;
                        }
                        break;
                }
            }
        }
    }
}
