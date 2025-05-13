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


namespace _0512_1
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv = "00000000";
        private string WriteDataConv = "00000000";

        int Auto = 0;
        int A = 0;
        int Count = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            button2.Text = "수동운전";

            if (connect != 0)
            {
                button1.Text = "Communication OK";
                button1.ForeColor = Color.Green;
                button1.BackColor = Color.White;

                timer1.Start();
                timer1.Interval = 300;

                button6_Click(null, null);

            }
            else
            {
                button1.Text = "Communication NG";
                button1.ForeColor = Color.Red;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "Communication OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv;
                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label6.Text = WriteDataConv;

                label7.BackColor = (ReadDataConv[7] == '1')? Color.AliceBlue : Color.White;
                label8.BackColor = (ReadDataConv[6] == '1') ? Color.AliceBlue : Color.White;
                label10.BackColor = (ReadDataConv[5] == '1') ? Color.AliceBlue : Color.White;
                label9.BackColor = (ReadDataConv[4] == '1') ? Color.AliceBlue : Color.White;
                label12.BackColor = (ReadDataConv[3] == '1') ? Color.AliceBlue : Color.White;
                label11.BackColor = (ReadDataConv[2] == '1') ? Color.AliceBlue : Color.White;

            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[7] == '1')
            {
                Writedata[0] |= 0x01;
                Writedata[0] &= unchecked((byte)~0x02);
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전 상태";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[6] == '1')
            {
                Writedata[0] |= 0x02;
                Writedata[0] &= unchecked((byte)~0x01);
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전 상태";
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[5] == '1')
            {
                Writedata[0] |= 0x04;
                Writedata[0] &= unchecked((byte)~0x08);
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전 상태";
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[4] == '1')
            {
                Writedata[0] |= 0x08;
                Writedata[0] &= unchecked((byte)~0x04);
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전 상태";
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[3] == '1')
            {
                Writedata[0] |= 0x10;
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전 상태";
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[2] == '1')
            {
                Writedata[0] &= unchecked((byte)~0x10);
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전 상태";
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            label15.Text = Count.ToString();
            int Counting = Convert.ToInt32(numericUpDown1.Value);

            if (A == 1 || (A == 2 && Count < Counting))
            {
                switch(Auto)
                {
                    case 0:
                        if (ReadDataConv[6] == '1' && ReadDataConv[5] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] = 0x15;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                        break;
                    case 1:
                        if (ReadDataConv[4] == '1' && ReadDataConv[2] == '1')
                        {
                            Writedata[0] = 0x1a;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                        break;
                    case 2:
                        if (ReadDataConv[7] == '1' && ReadDataConv[5] == '1')
                        {
                            Writedata[0] = 0x09; //?
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                        break;
                    case 3:
                        if (ReadDataConv[6] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] = 0x15;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                        break;
                    case 4:
                        if (ReadDataConv[4] == '1' && ReadDataConv[2] == '1')
                        {
                            Writedata[0] = 0x1a;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                        break;
                    case 5:
                        if (ReadDataConv[7] == '1' && ReadDataConv[5] == '1')
                        {
                            Writedata[0] = 0x09;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                        break;
                    case 6:
                        if (ReadDataConv[6] == '1')
                        {
                            Auto = 0;
                            if (A == 2) Count++;
                        }
                        break;
                }
                    
            }
        }

        private void button6_Click(object sender, EventArgs e)// 초기화
        {
            Writedata[0] = (byte)0x09;
            CIFX.xChannelWrite(Writedata);

            button2.Text = "초기화 상태";

            Auto = 0;
            Count = 0;
            A = 0;
            label15.Text = "0";
        }

        private void button3_Click(object sender, EventArgs e)// 자동운전
        {
            timer2.Start();
            timer2.Interval = 500;

            timer3.Start();
            timer3.Interval = 500;
            button2.Text = "자동운전 상태";

            A = 1;
            Auto = 0;
        }

        private void button4_Click(object sender, EventArgs e)//자동운전(횟수지정)
        {
            timer2.Start();
            timer2.Interval = 500;

            timer3.Start();
            timer3.Interval = 500;
            button2.Text = "자동운전(횟수지정) 상태";

            A = 2;
            Auto = 0;
            Count = 0;
        }

        private void button5_Click(object sender, EventArgs e)//운전정지
        {
            timer2.Stop();

            timer3.Stop();
            button2.Text = "운전정지 상태";
            A = 0;
            Count = 0;
            A = 0;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (ReadDataConv[6] == '1' && ReadDataConv[4] == '1' && ReadDataConv[2] == '1')
            {
                button7.BackColor = Color.Red;
            }
            else
            {
                button7.BackColor = Color.White;
            }
        }
    }
}
