using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZeroPrinters;

namespace PrintersTest
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private PrinterTest test;
        private void btnInit_Click(object sender, EventArgs e)
        {
            this.log.Text = "";
            test = new PrinterTest((int.Parse(comboBox1.SelectedItem.ToString())),Log,(s)=> MessageBox.Show(s, "Pregunta",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes);
            commandList.Items.Clear();
            foreach (KeyValuePair<int, KeyValuePair<string, Action>> actionCommand in test.ActionCommands)
            {
                commandList.Items.Add(actionCommand.Value.Key);
            }
        }

        private void commandList_SelectedIndexChanged(object sender, EventArgs e)
        {
            test.TryExecute(commandList.SelectedIndex);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnInit.Visible = true;
        }

        private void Log(string log)
        {

            this.log.Text += string.Format("{0}{1}", log,Environment.NewLine);
            this.log.ScrollToCaret();
        }
    }
}
