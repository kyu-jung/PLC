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

namespace 편솔실린더_수동제어
{
    public partial class Form1 : Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        string ReadDataConv;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint connect = CIFX.DriveConnect();

            if (connect != 0)
            {
                label6.Text = "OK";

                label6.ForeColor = Color.Green;

                timer1.Interval = (int)500;

                timer1.Start();
            }

            else
            {
                label1.Text = "NG";
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label6.Text == "OK") 
            {
                Readdata = CIFX.xChannelRead();

                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');

                //실린더 1번 비트로 ON/OFF 판정

                label8.Text = ReadDataConv[7] == '1' ? "ON" : "OFF";  // 삼항연산자
                
                label13.Text = ReadDataConv[5] == '1' ? "ON" : "OFF";
               


                /*if (ReadDataConv[7] == 1)
                { label8.Text = "ON"; }
                else if (ReadDataConv[7] == '0')
                { label8.Text = "OFF"; }

                if (ReadDataConv[6] == 1)
                { label9.Text = "ON"; }
                else if (ReadDataConv[6] == '0')
                { label9.Text = "OFF"; }

                //실린더 2번 비트로 ON/OFF 판정
                if (ReadDataConv[5] == 1)
                { label13.Text = "ON"; }
                else if (ReadDataConv[5] == '0')
                { label13.Text = "OFF"; }

                if (ReadDataConv[4] == 1)
                { label11.Text = "ON"; }
                else if (ReadDataConv[4] == '0')
                { label11.Text = "OFF"; } */
            }
        }

        // 실린더 1
        private void button1_Click(object sender, EventArgs e)
        {
            if (ReadDataConv[7] == '1') 
            {
                Writedata[0] ^= 0x01;  // 전진

                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ReadDataConv[6] == '1')
            {
                Writedata[0] ^= 0x01;   // 후진

                CIFX.xChannelWrite(Writedata);
            }
        }

        // 실린더 2
        private void button3_Click(object sender, EventArgs e)
        {
            if (ReadDataConv[5] == '1')
            {
                Writedata[0] ^= 0x02;   // 전진

                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ReadDataConv[4] == '1')
            {
                Writedata[0] ^= 0x02;    // 후진

                CIFX.xChannelWrite(Writedata);
            }
        }

        private void button5_Click(object sender, EventArgs e)  // 초기화
        {
            Writedata[0] = (byte)0x00;

            CIFX.xChannelWrite(Writedata);
        }

        private void button6_Click(object sender, EventArgs e) // 수동제어상태일때 사용함
        {
        }

        private void button7_Click(object sender, EventArgs e) // 수동제어상태일때 사용함
        {
            label6.Text = "해제됨";
            label6.ForeColor = Color.Red;
            timer1.Stop();
            CIFX.close();
        }
    }
}
