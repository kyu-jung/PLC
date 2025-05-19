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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace _0519
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv = "00000000";//변수선언
        private string WriteDataConv = "00000000";//변수선언

        int Auto = 0;
        int mode = 0;
        int count = 0;
        private int stepcount = 0;
        bool autostop = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            button1_Click(null, null);

            if (connect != 0)
            {
                label3.Text = "OK";
                label3.ForeColor = Color.Blue;
                label3.BackColor = Color.Pink;

                timer1.Interval = 100;
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

        private void button2_Click(object sender, EventArgs e) // 자동운전
        {
            
            mode = 1;
            Auto = 0;
            count = 0;
            timer2.Interval = 1000;
            timer2.Start();
            
        }

        private void button3_Click(object sender, EventArgs e) // 스텝제어(수동운전)
        {
            timer2.Interval = 200;
            timer2.Start();

            mode = 2;
            label8.Text = stepcount.ToString();
            count = 0;
            autostop = true;
            Auto = stepcount; // STEP버튼을 처음 눌렀을때 0부터 작동해야 하기때문에 순서 반드시 지킬것
            stepcount++;
            
        }

        private void button1_Click(object sender, EventArgs e)// 정지
        {
            timer1.Stop();
            timer2.Stop();
            Writedata[0] &= unchecked((byte)~0x1a);
            CIFX.xChannelWrite(Writedata);
            mode = 0;
            Auto = 0;
            count = 0;
            stepcount = 0;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label5.Text = count.ToString();
            int counting = Convert.ToInt32(numericUpDown1.Value);
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
                        if (mode != 2) Auto++;
                        autostop = false;
                    }
                    break;

                    case 1: 
                        if (ReadDataConv[6] == '1')
                        {
                            Writedata[0] |= 0x42;
                            Writedata[0] &= unchecked((byte)~0x01);
                            CIFX.xChannelWrite(Writedata);
                            Thread.Sleep(2000); // 2초 
                            Writedata[0] &= unchecked((byte)~0x40); // 정지 신호 전송
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 2) Auto++;
                            autostop = false;
                        }
                    break;

                    case 2: 
                        if (ReadDataConv[5] == '1' && ReadDataConv[3] == '1')
                        {
                            Writedata[0] |= 0x14;
                            Writedata[0] &= unchecked((byte)~0x08);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 2) Auto++;
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
                            if (mode != 2) Auto++;
                            autostop = false;
                        }
                        break;
                    case 4: 
                        if (ReadDataConv[5] == '1' )
                        {
                            Writedata[0] &= unchecked((byte)~0x10);
                            CIFX.xChannelWrite(Writedata);
                            if (mode != 2) Auto++;
                            autostop = false;
                            stepcount = 0;
                        }
                        break;
                    case 5:
                        if (mode != 2)
                        {
                            Auto = 0;
                            count++; 

                        }
                        
                        break;
                }
                
            }
                    



            
        }
    }
}
