using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DailyDepositReader
{
    public partial class MessageListForm : Form
    {
        public MessageListForm()
        {
            InitializeComponent();
        }

        public void Show(List<string> messages)
        {
            lstMessages.Items.Clear();
            foreach(string msg in messages)
            {
                lstMessages.Items.Add(msg);
            }
            ShowDialog();
        }
    }
}
