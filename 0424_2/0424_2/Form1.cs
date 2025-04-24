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


namespace _0424_2
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv = "00000000";
        private string WriteDataConv = "00000000";

        int Auto = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            if (connect != 0)
            {
                label3.Text = "OK";
                label3.ForeColor = Color.Green;
                timer1.Interval = 300;
                timer1.Start();

            }
            else
            {
                label3.Text = "NG";
                label3.ForeColor = Color.Red;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label3.Text == "OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv;

                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label11.Text = WriteDataConv;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) // 실린더 A 전진
        {
            Writedata[0] |= 0x01;
            Writedata[0] &= unchecked((byte)~0x02);
            CIFX.xChannelWrite(Writedata);

            label6.Text = "전진";
            label6.ForeColor = Color.Blue;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) // 실린더 A 후진
        {
            Writedata[0] |= 0x02;
            Writedata[0] &= unchecked((byte)~0x01);
            CIFX.xChannelWrite(Writedata);

            label6.Text = "후진";
            label6.ForeColor = Color.Blue;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) // 실린더 B 전진
        {
            Writedata[0] |= 0x04;
            Writedata[0] &= unchecked((byte)~0x08);
            CIFX.xChannelWrite(Writedata);

            label8.Text = "전진";
            label8.ForeColor = Color.Green;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)  // 실린더 B 후진
        {
            Writedata[0] |= 0x08;
            Writedata[0] &= unchecked((byte)~0x04);
            CIFX.xChannelWrite(Writedata);

            label8.Text = "후진";
            label8.ForeColor = Color.Green;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e) // 실린더 C 전진
        {
            Writedata[0] |= 0x10;
            Writedata[0] &= unchecked((byte)~0x20);
            CIFX.xChannelWrite(Writedata);
            label10.Text = "전진";
            label10.ForeColor = Color.Green;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)// 실린더 C 후진
        {
            Writedata[0] |= 0x20;
            Writedata[0] &= unchecked((byte)~0x10);
            CIFX.xChannelWrite(Writedata);

            label10.Text = "후진";
            label10.ForeColor = Color.Green;
        }

        private void button1_Click(object sender, EventArgs e) // 자동운전
        {
            timer2.Interval = 300;
            timer2.Start();

            Auto = 0;
            label12.Text = "자동운전 시작";
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
                    if (ReadDataConv[4] == '1' && ReadDataConv[3] == '1')
                    {
                        Writedata[0] = 0x10;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 3:
                    if (ReadDataConv[2] == '1' && ReadDataConv[6] == '1')
                    {
                        Writedata[0] |= 0x02;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 4:
                    if (ReadDataConv[7] == '1' && ReadDataConv[4] == '1')
                    {
                        Writedata[0] |= 0x08;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 5:
                    if (ReadDataConv[5] == '1' && ReadDataConv[2] == '1')
                    {
                        Writedata[0] = 0x20;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 6:
                    if (ReadDataConv[7] == '1' && ReadDataConv[5] == '1' && ReadDataConv[3] == '1')
                    {
                        Auto = 0;
                    }
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e) // 자동운전 정지
        {
            timer2.Stop();
            Auto = 0;
            label12.Text = "자동운전 정지";
        }

        private void button3_Click(object sender, EventArgs e) // 초기화
        {
            timer2.Stop();
            Auto = 0;

            Writedata[0] = (byte)0x20;
            CIFX.xChannelWrite(Writedata);

            label12.Text = "실린더 초기화";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Writedata[0] |= 0x40;
            CIFX.xChannelWrite(Writedata);
        }
    }
}
