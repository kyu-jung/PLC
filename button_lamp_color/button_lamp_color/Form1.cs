using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace button_lamp_color
{
    public partial class Form1: Form
    {
        int A = 0;
        int Auto = 0;
        int Count = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.BackColor = Color.White;

        }

        private void button2_Click(object sender, EventArgs e) //연속 점멸 (타이머 구동)
        {
            timer1.Start();
            timer1.Interval = 300;
            A = 1;
            Auto = 0;
            Count = 0;
        }

        private void button3_Click(object sender, EventArgs e) // 연속 횟수지정 (타이머 구동)
        {
            timer1.Start();
            timer1.Interval = 300;
            A = 2;
            Auto = 0;
            Count = 0;
        }

        private void button4_Click(object sender, EventArgs e) // 초기화
        {
            timer1.Stop();
            A = 0;
            Auto = 0;
            Count = 0;
        }

        private void timer1_Tick(object sender, EventArgs e) // 타이머 동작프로그램
        {
            label5.Text = Count.ToString(); // 현재 카운트 표시

            int Counting = Convert.ToInt32(numericUpDown1.Value); 

            if ((A == 1) || (A == 2 && Count < Counting)) 
            {
                switch (Auto)
                {
                    case 0:
                        if (button1.BackColor == Color.White)
                        {
                            button1.BackColor = Color.Aqua;
                            button1.ForeColor = Color.White;
                            button1.Text = "점등";
                            label2.BackColor = Color.DarkOrange;
                            label2.ForeColor = Color.Indigo;
                            label2.Text = "점등";

                            Auto++;
                        }
                        break;
                    case 1:
                        if (button1.BackColor == Color.Aqua)
                        {
                            button1.BackColor = Color.White;
                            button1.ForeColor = Color.Blue;
                            button1.Text = "소등";
                            label2.BackColor = Color.OrangeRed;
                            label2.ForeColor = Color.White;
                            label2.Text = "소등";

                            Auto++;
                            Count++;
                        }
                        break;
                    case 2:
                        Auto = 0;
                        break;

                }            
            }
        }
    }
}
