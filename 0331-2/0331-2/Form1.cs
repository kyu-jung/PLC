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

namespace _0331_2
{
    public partial class Form1 : Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        int Auto = 0;

        string ReadDataConv = "00000000";
        private string WriteDataConv;

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
                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');

                // 실린더 1, 2 전후진 상태확인
                label9.Text = ReadDataConv[6] == '1' ? "전진" : "후진";
                label10.Text = ReadDataConv[4] == '1' ? "전진" : "후진";

                // ECC-203 입출력 데이터 확인
                label5.Text = ReadDataConv;
                label7.Text = WriteDataConv;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Writedata[0] |= 0x01;
            CIFX.xChannelWrite(Writedata);

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // 후진 시 0x01 비트 OFF
            Writedata[0] &= unchecked((byte)~0x01);
            CIFX.xChannelWrite(Writedata);

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[5] == '1')
            {
                // B 실린더 수동전진
                Writedata[0] |= 0x02;
                Writedata[0] &= unchecked((byte)~0x04);
                CIFX.xChannelWrite(Writedata);
            }

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadDataConv[4] == '1')
            {
                // B 실린더 수동후진
                Writedata[0] |= 0x04;
                Writedata[0] &= unchecked((byte)~0x02);
                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button1_Click(object sender, EventArgs e) // 자동운전 시작
        {
            timer2.Interval = 1000;
            timer2.Start();
            Auto = 0;
        }

        private void timer2_Tick(object sender, EventArgs e) // 자동운전 동작 타이머
        {
            switch(Auto)
            {
                case 0:
                    if (ReadDataConv[7] == '1'& ReadDataConv[5] == '1')
                    {
                        Writedata[0] = 0x03;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 1:
                    if (ReadDataConv[6] == '1' & ReadDataConv[4] == '1')
                    {
                        Writedata[0] = unchecked((byte)~0x01);
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                case 2:
                    if (ReadDataConv[7] == '1')
                    {
                        Writedata[0] = 0x04;
                        CIFX.xChannelWrite(Writedata);
                        Auto++;
                    }
                    break;
                    
                case 3:
                    if (ReadDataConv[5] == '1')
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
        }

        private void button3_Click(object sender, EventArgs e) // 초기화
        {
            Writedata[0] = 0x04;
            CIFX.xChannelWrite(Writedata);
        }
    }
}