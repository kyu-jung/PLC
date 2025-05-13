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

namespace _0512_2_dc_motor
{
    public partial class Form1: Form
    {
        byte[] Writedata = new byte[8];
        byte[] Readdata = new byte[34];

        private string ReadDataConv = "00000000";
        private string WriteDataConv = "00000000";

        int mode = 0;
        int count = 0;
        int counting = 0;
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

                timer1.Start();
                timer1.Interval = 20;

                timer2.Start();
                timer2.Interval = 20;
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
                ReadDataConv = Convert.ToString(Readdata[18], 2).PadLeft(8, '0');
                label4.Text = ReadDataConv;

                WriteDataConv = Convert.ToString(Writedata[0], 2).PadLeft(8, '0');
                label6.Text = WriteDataConv;
            }
        }

        private void button2_Click(object sender, EventArgs e) // 정회전
        {
            if (mode == 0) mode = 1;
        }

        private void button3_Click(object sender, EventArgs e) // 역회전
        {
            if (mode == 0) mode = 2;
        }

        private void button4_Click(object sender, EventArgs e) // 정지
        {
            mode = 0;
        }

        private void button5_Click(object sender, EventArgs e) // 모터 카운터 클리어
        {
            if (mode == 0)
            {
                count = 0;
                label8.Text = count.ToString();
            }
            else
            {
                MessageBox.Show("모터가 회전중입니다. 정지 후 클리어 진행하세요.");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label8.Text = count.ToString();

            counting = Convert.ToInt32(numericUpDown1.Value);

            if (mode == 1)
            {
                Writedata[0] |= 0x40;
                Writedata[0] &= unchecked((byte)~0x80);
                CIFX.xChannelWrite(Writedata);

                if ((Writedata[0] == 0x40 || Writedata[0] == 0x80)
                    && ReadDataConv[1] == '1') count++;
                if (count > counting) mode = 0;
            }
            else if (mode == 2)
            {
                Writedata[0] |= 0x80;
                Writedata[0] &= unchecked((byte)~0x40);
                CIFX.xChannelWrite(Writedata);

                if (ReadDataConv[1] == '1') count--;

                if (counting <= 0) mode = 0;
            }
            else
            {
                mode = 0;

                Writedata[0] = 0x00;
                CIFX.xChannelWrite(Writedata);
            }
        }
    }
}
