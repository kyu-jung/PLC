using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CIFX_50RE;

namespace _0519_2
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv18 = "00000000";
        private string ReadDataConv19 = "00000000";
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
                button1.Text = "Communication OK";
                button1.BackColor = Color.Pink;

                timer1.Interval = 20;
                timer1.Start();

                timer2.Interval = 20;
                timer2.Start();
            }
            else
            {
                button1.Text = "Communication NG";
                button1.BackColor = Color.White;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "Communication OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv18 = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv18;
                ReadDataConv19 = Convert.ToString(Readdata[19], 2).PadLeft(8, '0');
                label6.Text = ReadDataConv19;
                WriteDataConv = Convert.ToString(Writedata[1], 2).PadLeft(8, '0');
                label8.Text = WriteDataConv;
            }
        }

        int mode = 0; // 0 = 초기화, 1 = 수동, 2 = 자동
        bool motccw = false; // 수동 역회전 플래그
        bool motcw = false; // 수동 정회전 플래그
        bool Autocount = false; // 자동운전 정역회전 플래그
        int count = 0; // 동작카운트 변수

        private void button2_Click(object sender, EventArgs e) // 수동 정회전
        {
            mode = 1;
            motcw = true;
            motccw = false;
        }

        private void button3_Click(object sender, EventArgs e) // 수동 역회전
        {
            mode = 1;
            motcw = false;
            motccw = true;
        }

        private void button4_Click(object sender, EventArgs e) // 카운터 클리어
        {
            count = 0;
            label10.Text = count.ToString();
        }

        private void button5_Click(object sender, EventArgs e) // 자동 정역회전
        {
            mode = 2;
            Autocount = true;
            count = 0;
            countflag = 0;
        }

        int countflag = 0; // 자동운전 정역회전 전환 및 카운트 처리를 위한 플래그
        private void timer2_Tick(object sender, EventArgs e) // 자동운전 타이머
        {
            label10.Text = count.ToString();

            if (mode == 0) // 초기화
            {
                // 스위치1,2가 모두 감지되지 않을때 모터 역회전을 시켜 초기화시킨다.
                if (ReadDataConv18[0] != '1' || ReadDataConv19[7] != '1')  
                {
                    Writedata[1] |= 0x02;
                    Writedata[1] &= unchecked((byte)~0x01);
                    CIFX.xChannelWrite(Writedata);
                }
            }
            if (mode == 1) // 수동운전
            {
                if (motcw == true && ReadDataConv18[0] == '1')
                {
                    Writedata[1] |= 0x01;
                    Writedata[1] &= unchecked((byte)~0x02);
                    CIFX.xChannelWrite(Writedata);
                }
                else if (motccw == true && ReadDataConv19[7] == '1')
                {
                    Writedata[1] |= 0x02;
                    Writedata[1] &= unchecked((byte)~0x01);
                    CIFX.xChannelWrite(Writedata);
                }
            }

            int counting = Convert.ToInt32(numericUpDown1.Value);
            if (mode == 2 && count < counting) // 자동운전
            {
                if (ReadDataConv18[0] == '1' && Autocount == true)
                {
                    Writedata[1] |= 0x01;
                    Writedata[1] &= unchecked((byte)~0x02);
                    CIFX.xChannelWrite(Writedata);

                    if (countflag > 0) count++;

                    countflag++;

                    Autocount = false;
                }

                else if (ReadDataConv19[7] == '1' && Autocount == false)
                {
                    Writedata[1] |= 0x02;
                    Writedata[1] &= unchecked((byte)~0x01);
                    CIFX.xChannelWrite(Writedata);

                    Autocount = true;
                }

                if (count >= counting)
                {
                    mode = 0;
                    Writedata[1] &= unchecked((byte)~0x03);
                    CIFX.xChannelWrite(Writedata);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e) // 정지(초기화)
        {
            mode = 0;
            count = 0;
            countflag = 0;
        }
    }
}
