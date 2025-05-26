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

namespace _0526_2
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

            if (connect != 0)
            {
                label3.Text = "OK";
                label3.ForeColor = Color.Blue;
                label3.BackColor = Color.Pink;

                timer1.Interval = 500;
                timer1.Start();
            }
            else
            {
                label3.Text = "NG";
                label3.ForeColor = Color.Black;
                label3.BackColor = Color.White;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label3.Text == "OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
            }
        }

        private int stepcount = 0; // 스텝카운트
        bool autostop = false; // 스텝 플래그 신호를 주기위해
        int mode = 0; // 운전모드
        int count = 0; // 카운터
        int Auto = 0; // 자동운전
        private void button3_Click(object sender, EventArgs e) // 스텝제어
        {
            timer2.Interval = 200;
            timer2.Start();
            label8.Text = stepcount.ToString();
            mode = 2;
            count = 0;
            autostop = true; 

            Auto = stepcount;
            stepcount++;
        }

        private void button1_Click(object sender, EventArgs e) // 정지
        {
            
            timer2.Stop();
            mode = 0;
            Auto = 0;
            count = 0;
            stepcount = 0;
            Writedata[0] = 0x0a;
            CIFX.xChannelWrite(Writedata);
        }

        private void button2_Click(object sender, EventArgs e) // 자동운전
        {
            mode = 1;
            Auto = 0;
            count = 0;
            timer2.Interval = 500;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label5.Text = count.ToString(); // 동작횟수
            int counting = Convert.ToInt32(numericUpDown1.Value); // 카운터 설정값
            label8.Text = Auto.ToString(); // 스텝카운트

            if ((mode == 1 && count < counting) || (mode == 2 && autostop == true))
            {
                switch (Auto)
                {
                    case 0:
                        if (ReadDataConv[7] == '1')
                        {
                            Writedata[0] |= 0x01;
                            Writedata[0] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);
                            if (mode == 1) Auto++;
                            autostop = false; // 한 스탭씩 작동하기 위한 플래그
                        }
                        break;
                    case 1:
                        if (ReadDataConv[6] == '1')
                        {
                            Writedata[0] |= 0x42;
                            Writedata[0] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);

                            Thread.Sleep(2000);

                            Writedata[0] &= unchecked((byte)~0x40);
                            CIFX.xChannelWrite(Writedata);

                            if (mode == 1) Auto++;
                            autostop = false;
                        }
                        break;
                    case 2:
                        if (ReadDataConv[5] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x14;
                            Writedata[0] &= unchecked((byte)~0x08);
                            CIFX.xChannelWrite(Writedata);
                            if (mode == 1) Auto++;
                            autostop = false;
                        }
                        break;
                    case 3:
                        if (ReadDataConv[4] == '1' && ReadDataConv[2] == '1')
                        {
                            Writedata[0] |= 0x88;
                            Writedata[0] &= unchecked((byte)~0x04);
                            CIFX.xChannelWrite(Writedata);

                            Thread.Sleep(3000);

                            Writedata[0] &= unchecked((byte)~0x80);
                            CIFX.xChannelWrite(Writedata);

                            if (mode == 1) Auto++;
                            autostop = false;
                        }
                        break;
                    case 4:
                        if (ReadDataConv[5] == '1')
                        {
                            
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (mode == 1) Auto++;
                            autostop = false;
                        }
                        stepcount = 0; // 스텝제어를 초기화시킨다
                        break;
                    case 5:
                        if (mode == 1) // 자동운전일때만 작동되도록 설정
                        {
                            Auto = 0; // case0으로 되돌아가야한다.
                            count++; // 동작횟수를 증가시킨다.
                        }
                        break;
                }

            }
        }
    }
}
