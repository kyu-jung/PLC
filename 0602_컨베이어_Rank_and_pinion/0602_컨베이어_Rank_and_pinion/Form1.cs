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

namespace _0602_컨베이어_Rank_and_pinion
{
    
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv18 = "00000000";
        private string ReadDataConv19 = "00000000";
        private string WriteDataConv0 = "00000000";
        private string WriteDataConv1 = "00000000";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            if (connect != 0)
            {
                button1.Text = "OK";
                button1.ForeColor = Color.Blue;
                button1.BackColor = Color.Pink;

                timer1.Interval = 100;
                timer1.Start();

                label12.Text = "수동 운전";
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
                ReadDataConv18 = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label3.Text = ReadDataConv18;

                Readdata = CIFX.xChannelRead();
                ReadDataConv19 = Convert.ToString(Readdata[19], 2).PadLeft(8, '0');
                label5.Text = ReadDataConv19;

                WriteDataConv0 = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label7.Text = WriteDataConv0;

                WriteDataConv1 = Convert.ToString(Writedata[1], 2).PadLeft(8, '0');
                label9.Text = WriteDataConv1;

                label13.BackColor = (ReadDataConv18[6] == '1') ? Color.GreenYellow : Color.White;
                label14.BackColor = (ReadDataConv18[7] == '1') ? Color.GreenYellow : Color.White;
                label15.BackColor = (ReadDataConv18[4] == '1') ? Color.GreenYellow : Color.White;
                label16.BackColor = (ReadDataConv18[5] == '1') ? Color.GreenYellow : Color.White;
                label17.BackColor = (ReadDataConv18[2] == '1') ? Color.GreenYellow : Color.White;
                label18.BackColor = (ReadDataConv18[3] == '1') ? Color.GreenYellow : Color.White;
                label19.BackColor = (ReadDataConv18[1] == '1') ? Color.GreenYellow : Color.White;

                label20.BackColor = (ReadDataConv18[0] == '1') ? Color.GreenYellow : Color.White;
                label21.BackColor = (ReadDataConv19[7] == '1') ? Color.GreenYellow : Color.White;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)// A실린더 전진
        {
            if (ReadDataConv18[7] == '1')
            {
                Writedata[0] |= 0x01;
                Writedata[0] &= unchecked((byte)~0x02);
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)// A실린더 후진
        {
            if (ReadDataConv18[6] == '1')
            {
                Writedata[0] |= 0x02;
                Writedata[0] &= unchecked((byte)~0x01);
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)// B실린더 전진
        {
            if (ReadDataConv18[5] == '1')
            {
                Writedata[0] |= 0x04;
                Writedata[0] &= unchecked((byte)~0x08);
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)// B실린더 후진
        {
            if (ReadDataConv18[4] == '1')
            {
                Writedata[0] |= 0x08;
                Writedata[0] &= unchecked((byte)~0x04);
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)// C실린더 전진
        {
            if (ReadDataConv18[3] == '1')
            {
                Writedata[0] ^= 0x10;
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)// C실린더 후진
        {
            if (ReadDataConv18[2] == '1')
            {
                Writedata[0] ^= 0x10;
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)//모터 정회전
        { 

            Writedata[0] ^= 0x40;
            
            CIFX.xChannelWrite(Writedata);

            label12.Text = "수동운전 상태";
            
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)//모터 역회전
        {
            Writedata[0] ^= 0x40;
            
            CIFX.xChannelWrite(Writedata);

            label12.Text = "수동운전 상태";
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)//모터 정지
        {
            Writedata[0] &= unchecked((byte)~0xc0);
            CIFX.xChannelWrite(Writedata);

            label12.Text = "수동운전 상태";
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)// 랙엔피니언 전진
        {
            if (ReadDataConv18[0] == '1')
            {
                Writedata[1] |= 0x01;
                Writedata[1] &= unchecked((byte)~0x02);
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)// 랙엔피니언 후진
        {
            if (ReadDataConv19[7] == '1')
            {
                Writedata[1] |= 0x02;
                Writedata[1] &= unchecked((byte)~0x01);
                CIFX.xChannelWrite(Writedata);

                label12.Text = "수동운전 상태";
            }
        }

        int Auto = 0;
        int Count = 0;
        bool AutoStop = false;
        int Mode = 0;
        private int ClickCount = 0;

        private void button2_Click(object sender, EventArgs e)// 자동운전(횟수지정)
        {
            timer3.Interval = 1000;
            timer3.Start();

            Mode = 1;
            Auto = 0;
            Count = 0;

            label12.Text = "자동운전 상태";
        }

        private void button3_Click(object sender, EventArgs e)// 수동스텝제어
        {
            timer3.Interval = 100;
            timer3.Start();

            Mode = 2;
            AutoStop = true;
            Auto = ClickCount;
            ClickCount++;
            Count = 0;

            label12.Text = "스텝제어 상태";
        }

        private void button4_Click(object sender, EventArgs e)// 정지 초기화
        {
            Writedata[0] = (byte)0x0a;
            Writedata[1] = (byte)0x02;
            CIFX.xChannelWrite(Writedata);

            Mode = 0;
            Auto = 0;
            Count = 0;
            ClickCount = 0;

            label12.Text = "정지 초기화";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label24.Text = Count.ToString();
            int Counting = Convert.ToInt32(numericUpDown1.Value);
            label26.Text = (Mode == 2) ? Auto.ToString() : "not step driving";

            if ((Mode == 1 && Count < Counting) || (Mode == 2 && AutoStop == true))
            {
                switch (Auto)
                {
                    case 0:
                        if (ReadDataConv18[7] == '1')
                        {
                            Writedata[0] |= 0x01;
                            Writedata[0] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);
                        }
                        Thread.Sleep(2000);
                        if (ReadDataConv18[6] == '1')
                        {
                            Writedata[0] |= 0x02;
                            Writedata[0] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 1:
                        if (ReadDataConv18[7] == '1')
                        {
                            Writedata[0] |= 0x40;
                            
                            CIFX.xChannelWrite(Writedata);
                            Thread.Sleep(2000);

                            Writedata[0] &= unchecked((byte)~0x40);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 2:
                        if (ReadDataConv18[5] == '1')
                        {
                            Writedata[0] |= 0x04;
                            Writedata[0] &= unchecked((byte)~0x08);
                            CIFX.xChannelWrite(Writedata);
                        }
                        
                        if (ReadDataConv18[4] == '1')
                        {
                            Writedata[0] |= 0x08;
                            Writedata[0] &= unchecked((byte)~0x04);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;
                    case 3:
                        if (ReadDataConv18[0] == '1')
                        {
                            Writedata[1] |= 0x01;
                            Writedata[1] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);

                        }

                        if (ReadDataConv19[7] == '1')
                        {
                            Writedata[1] |= 0x02;
                            Writedata[1] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;
                    case 4:
                        if (ReadDataConv18[0] == '1')
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
                        if (ReadDataConv18[3] == '1')
                        {
                            Writedata[0] |= 0x10;
                            CIFX.xChannelWrite(Writedata);
                        }

                        if (ReadDataConv18[2] == '1')
                        {
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        ClickCount = 0;
                        if (Mode == 2) MessageBox.Show("STEP 마지막 공정");
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
            label24.Text = Count.ToString();
            int Counting = Convert.ToInt32(numericUpDown1.Value);
            label26.Text = (Mode == 2) ? Auto.ToString() : "not step driving";

            if ((Mode == 1 && Count < Counting) || (Mode == 2 && AutoStop == true))
            {
                switch (Auto)
                {
                    case 0:
                        if (ReadDataConv18[7] == '1')
                        {
                            Writedata[0] |= 0x01;
                            Writedata[0] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 1:
                        if (ReadDataConv18[6] == '1')
                        {
                            Writedata[0] |= 0x40;

                            CIFX.xChannelWrite(Writedata);
                            Thread.Sleep(2000);

                            Writedata[0] &= unchecked((byte)~0x40);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;

                    case 2:
                        if (ReadDataConv18[6] == '1')
                        {
                            Writedata[0] |= 0x02;
                            Writedata[0] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;
                    case 3:
                        if (ReadDataConv18[5] == '1' && ReadDataConv18[3] == '1')
                        {
                            Writedata[0] |= 0x14;
                            Writedata[0] &= unchecked((byte)~0x08);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;
                    case 4:
                        if (ReadDataConv18[4] == '1' && ReadDataConv18[2] == '1')
                        {
                            Writedata[0] |= 0x08;
                            Writedata[0] &= unchecked((byte)~0x04);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;
                    case 5:
                        if (ReadDataConv18[0] == '1')
                        {
                            Writedata[1] |= 0x01;
                            Writedata[1] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);

                        }

                        if (ReadDataConv19[7] == '1')
                        {
                            Writedata[1] |= 0x02;
                            Writedata[1] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        break;
                    case 6:
                        if (ReadDataConv18[0] == '1')
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
                    case 7:
                        if (ReadDataConv18[2] == '1')
                        {
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (Mode == 1) Auto++;
                            AutoStop = false;
                        }
                        ClickCount = 0;
                        if (Mode == 2) MessageBox.Show("STEP 마지막 공정");
                        break;
                    case 8:
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
