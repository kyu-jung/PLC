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

namespace _0421
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
                label5.Text = ReadDataConv;

                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label7.Text = WriteDataConv;

                label9.Text = ReadDataConv[6] == '1' ? "전진" : "후진";
                label11.Text = ReadDataConv[4] == '1' ? "전진" : "후진";

            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Writedata[0] |= 0x01;
            Writedata[0] &= unchecked((byte)~0x02);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "수동운전";
            button1.BackColor = Color.Red;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Writedata[0] |= 0x02;
            Writedata[0] &= unchecked((byte)~0x01);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "수동운전";
            button1.BackColor = Color.Red;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Writedata[0] |= 0x04;
            Writedata[0] &= unchecked((byte)~0x08);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "수동운전";
            button1.BackColor = Color.Red;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Writedata[0] |= 0x08;
            Writedata[0] &= unchecked((byte)~0x04);
            CIFX.xChannelWrite(Writedata);

            button1.Text = "수동운전";
            button1.BackColor = Color.Red;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer2.Interval = 300;
            timer2.Start();

            Auto = 0;
            button1.Text = "자동운전 중";
            button1.BackColor = Color.Green;
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
                        Writedata[0] = 0x0a;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 3:
                    if (ReadDataConv[7] == '1' && ReadDataConv[5] == '1')
                    {
                        Auto = 0;
                    }
                    break;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            Auto = 0;
            button1.Text = "자동 운전 정지";
            button1.BackColor = Color.Red;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            Auto = 0;
            Writedata[0] = (byte)0x0a;
            CIFX.xChannelWrite(Writedata);
            button1.Text = "실린더 초기화";
            button1.BackColor = Color.Pink;
        }
    }
}
