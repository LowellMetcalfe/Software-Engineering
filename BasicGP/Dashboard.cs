﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicGP
{
    public partial class Results : Form
    {
        public Results()
        {
            InitializeComponent();
        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            //Is this bad coding practice to repeat everytime???
            this.Visible = false;
            new RegisterForm().Visible = true;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //and here repeated
            this.Visible = false;
            new ResultsForm().Visible = true;

        }
    }
}