using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TUIO_TEST
{
    public partial class Form1 : Form
    {

        TuioDemo Demo;
        OpenFileDialog open;
        public Form1(TuioDemo demo)
        {

            Demo = demo;
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                pictureBox1.Image = new Bitmap(open.FileName);
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Demo.setMarkerInfo(textBox1.Text, new Bitmap(open.FileName), Int32.Parse(textBox2.Text));
            this.Close();
        }
    }
}
