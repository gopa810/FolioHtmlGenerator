using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FolioHtmlGenerator
{
    public partial class Form1 : Form
    {
        HtmlContentGenerator html = new HtmlContentGenerator();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            html.Clear();
            foreach (string s in richTextBox1.Lines)
            {
                html.Add(s);
            }

            richTextBox2.Text = html.Generate();
        }
    }
}
