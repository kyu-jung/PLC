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

namespace _0331
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        string ReadDataConv = "00000000"; // ReadDataConv byte 모두 0 정의
        private string WriteDataConv;
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

                timer1.Interval = 500;
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
            Readdata = CIFX.xChannelRead();
            ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
            WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
            
            label9.Text = ReadDataConv;
            label11.Text = WriteDataConv;

            label4.Text = ReadDataConv[7] == '1' ? "ON" : "OFF";
            label5.Text = ReadDataConv[6] == '1' ? "ON" : "OFF";
            label7.Text = ReadDataConv[5] == '1' ? "ON" : "OFF";
            label6.Text = ReadDataConv[4] == '1' ? "ON" : "OFF";
        }

        private void button1_Click(object sender, EventArgs e) // 실린더1 전진
        {
            if (ReadDataConv[7] == '1')
            {
                Writedata[0] ^= 0x01; // ex-or x,y 0,0 ,,, 1, 1->1
                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button2_Click(object sender, EventArgs e)  // 실린더1 후진
        {
            if (ReadDataConv[6] == '1')
            {
                Writedata[0] ^= 0x01;
                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button4_Click(object sender, EventArgs e) // 실린더2 전진
        {
            if (ReadDataConv[5] == '1')
            {
                Writedata[0] ^= 0x02;
                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button3_Click(object sender, EventArgs e) // 실린더2 후진
        {
            if (ReadDataConv[4] == '1')
            {
                Writedata[0] ^= 0x02;
                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button5_Click(object sender, EventArgs e) // 자동운전시작
        {
            timer2.Interval = 500;
            timer2.Start();
            Auto = 0;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            switch (Auto)
            {
                case 0:
                    
                    if (ReadDataConv[7] == '1') // 1 전진 센서 0x01
                        {
                            Writedata[0] ^= 0x01;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                    break;
                    
                case 1:
                    
                    if (ReadDataConv[6] == '1') // 2 전진 센서 0x02
                        {
                            Writedata[0] ^= 0x01;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                    break;
                    
                case 2:
                    
                    if (ReadDataConv[7] == '1') // 1 후진 센서 0x01
                        {
                            Writedata[0] ^= 0x02;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                    break;
                    
                case 3:
                    
                    if (ReadDataConv[4] == '1') // 2 후진 센서 0x02
                        {
                            Writedata[0] ^= 0x02;
                            CIFX.xChannelWrite(Writedata);
                            Auto++;
                        }
                    break;
                    
                case 4:
                    
                    if (ReadDataConv[5] == '1') // 1 후진
                        {
                            Auto = 0;
                        }
                    break;
                    

            }       
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Auto = 0;
            timer2.Stop();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Writedata[0] = 0x00;
            CIFX.xChannelWrite(Writedata);
        }
    }
}
