using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CIFX_50RE;

namespace _0428_1
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];
         bool autostop = false;
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
                button1.Text = "Communication OK";
                button1.BackColor = Color.Pink;
                button1.ForeColor = Color.Blue;
                timer1.Interval = 200;
                timer1.Start();
            }
            else
            {
                button1.Text = "Communication NG";
                button1.BackColor = Color.White;
                button1.ForeColor = Color.Red;
            }
        }

        private string ReadDataConv = "00000000";
        private string WriteDataConv = "00000000";

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "Communication OK")
            {
                Readdata = CIFX.xChannelRead();
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv;

                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label6.Text = WriteDataConv;

                label7.BackColor = (ReadDataConv[7] == '1') ? Color.GreenYellow : Color.White;
                label8.BackColor = (ReadDataConv[6] == '1') ? Color.GreenYellow : Color.White;

                label9.BackColor = (ReadDataConv[5] == '1') ? Color.GreenYellow : Color.White;
                label10.BackColor = (ReadDataConv[4] == '1') ? Color.GreenYellow : Color.White;

                label11.BackColor = (ReadDataConv[3] == '1') ? Color.GreenYellow : Color.White;
                label12.BackColor = (ReadDataConv[2] == '1') ? Color.GreenYellow : Color.White;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) // A 실린더 전진
        {
            if (ReadDataConv[7] == '1')
            {
                Writedata[0] |= 0x01;
                Writedata[0] &= unchecked((byte)~0x02); // 인터록 설정(보수의 값을 확인하지 않는다)
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전";
                button2.ForeColor = Color.White;
                button2.BackColor = Color.Blue;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) // A 실린더 후진
        {
            if (ReadDataConv[6] == '1')
            {
                Writedata[0] |= 0x02;
                Writedata[0] &= unchecked((byte)~0x01); // 인터록 설정
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전";
                button2.ForeColor = Color.White;
                button2.BackColor = Color.Blue;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e) // B 실린더 전진
        {
            if (ReadDataConv[5] == '1')
            {
                Writedata[0] |= 0x04;
                Writedata[0] &= unchecked((byte)~0x08); // 인터록 설정
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전";
                button2.ForeColor = Color.White;
                button2.BackColor = Color.Blue;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) // B 실린더 후진
        {
            if (ReadDataConv[4] == '1')
            {
                Writedata[0] |= 0x08;
                Writedata[0] &= unchecked((byte)~0x04); // 인터록 설정
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전";
                button2.ForeColor = Color.White;
                button2.BackColor = Color.Blue;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e) // C 실린더 전진
        {
            if (ReadDataConv[3] == '1')
            {
                Writedata[0] |= 0x10;
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전";
                button2.ForeColor = Color.White;
                button2.BackColor = Color.Blue;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e) // C 실린더 후진
        {
            if (ReadDataConv[2] == '1')
            {
                Writedata[0] &= unchecked((byte)~0x10);// 편솔이어서 전진 신호를 없애주는 신호를 준다.
                CIFX.xChannelWrite(Writedata);

                button2.Text = "수동운전";
                button2.ForeColor = Color.White;
                button2.BackColor = Color.Blue;
            }
        }

        private void button3_Click(object sender, EventArgs e) // 자동운전
        {
            timer2.Interval = 200;
            timer2.Start();

            mode = 1;

            Auto = 0;
            button2.Text = "자동운전";
            button2.ForeColor = Color.White;
            button2.BackColor = Color.Violet;
        }

        private void button4_Click(object sender, EventArgs e) // 자동운전(횟수지정)
        {
            timer2.Interval = 200;
            timer2.Start();

            mode = 2;
            count = 0;

            button2.Text = "자동운전(횟수지정)";
            
        }

        private void button5_Click(object sender, EventArgs e)  //운전정지
        {
            timer2.Stop();
            mode = 0;
            button2.Text = "자동운전 정지";
            button2.ForeColor = Color.White;
            button2.BackColor = Color.Violet;
        }

        private void button6_Click(object sender, EventArgs e) // 초기화
        {
            timer2.Stop();
            Auto = 0;
            mode = 0;
            count = 0;
            // 양솔 실린더가 전진되어 있을 경우 어떤 실린더가 전진되어 있는지 모름으로
            // 두개를 동시에 후진시켜야 함 0x02 + 0x08 => 0x0a
            Writedata[0] = (byte)0x0a; 
            CIFX.xChannelWrite(Writedata);

            button2.Text = "초기화";
            button2.ForeColor = Color.White;
            button2.BackColor = Color.MediumSpringGreen;
        }
        int mode = 0; // 자동운전 1, 자동운전(횟수지정) 2 -> 카운터 필요
        int count = 0; // 자동운전 횟수지정 카운터
        private void timer2_Tick(object sender, EventArgs e) // 자동운전 타이머
        {
            label15.Text = count.ToString();
            int counting = Convert.ToInt32(numericUpDown1.Value);

            // |, & -> 하나의 bit만 비교하는경우, ||, && -> 변수를 비교하는 경우
            if (mode == 1 || (mode == 2 && count < counting) || (mode == 3 && autostop == true))
            {
                switch (Auto)
                {
                    case 0:
                        if (ReadDataConv[7] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x11;
                            Writedata[0] &= unchecked((byte)~0x02);
                            CIFX.xChannelWrite(Writedata);
                            if(mode != 3) Auto++;
                            autostop = false;
                        }
                        break;
                    case 1:
                        if (ReadDataConv[6] == '1' && ReadDataConv[2] == '1'&& ReadDataConv[5] == '1')
                        {
                            
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 3) Auto++;
                            autostop = false;
                        }
                        break;
                    case 2:
                        if (ReadDataConv[6] == '1' && ReadDataConv[5] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x16;
                            Writedata[0] &= unchecked((byte)~0x09);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 3) Auto++;
                            autostop = false;
                        }
                        break;
                    case 3:
                        if (ReadDataConv[7] == '1' && ReadDataConv[4] == '1' && ReadDataConv[2] == '1')
                        {
                            
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 3) Auto++;
                            autostop = false;
                        }
                        break;
                    case 4:
                        if (ReadDataConv[7] == '1' && ReadDataConv[4] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x19;
                            Writedata[0] &= unchecked((byte)~0x06);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 3) Auto++;
                            autostop = false;
                        }
                        break;
                    case 5:
                        if (ReadDataConv[6] == '1' && ReadDataConv[5] == '1' && ReadDataConv[2] == '1')
                        {
                            Writedata[0] |= 0x02;
                            Writedata[0] &= unchecked((byte)~0x11);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 3) Auto++;
                            autostop = false;
                            stepcount = 0;
                        }
                        break;
                    case 6:
                        if (mode != 3)
                        {
                            Auto = 0;
                            count++; // 모드 2일때만 작동하고 싶으면 if문 활용
                        }
                        break;
                }

            }
        }

        private int stepcount = 0;
        private void button7_Click(object sender, EventArgs e) // STEP
        {
            timer2.Interval = 200;
            timer2.Start();
            label16.Text = stepcount.ToString();
            mode = 3;
            count = 0;
            button2.Text = "STEP";

            autostop = true;

            Auto = stepcount; // STEP버튼을 처음 눌렀을때 0부터 작동해야 하기때문에 순서 반드시 지킬것
            stepcount++;
        }
    }
}
