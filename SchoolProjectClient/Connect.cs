﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace SchoolProjectClient
{


    public partial class ConnectForm : Form
    {
        
        private TTSConnectionClient mConnection;
        private Mainform mMainform;

        public ConnectForm(TTSConnectionClient pConnection, Mainform pMainform)
        {
            InitializeComponent();
            mConnection = pConnection;
            mMainform = pMainform;
            IPAddress lIPAddress = mConnection.GetINetIPAddress();
            if (lIPAddress!=null)
            {
                IPTextBox.Text = lIPAddress.ToString();
            }
        }

        public void setConnectionFailure(string pFail)
        {
            StatusTextBox.Text = pFail;
            Application.DoEvents();
        }


        public void Connected()
        {
            ConnectButton.Enabled = false;
        }

        public void Disconnected()
        {
            ConnectButton.Enabled = false;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public async void NonBlockingWaitAwhile(int milliseconds)
        {
            Stopwatch sWatch = Stopwatch.StartNew();

            while (sWatch.Elapsed.Milliseconds <= milliseconds)
            {
                await Task.Delay(50);
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            Application.DoEvents();
            if (mConnection.isConnected())
            {
                mConnection.CloseConnection();
            }
            mConnection.SetDestination(IPTextBox.Text);
            mConnection.Connect();
            NonBlockingWaitAwhile(3000);
            ConnectButton.Enabled = true;
            if (mConnection.isConnected())
            {
                Hide();
                mMainform.Show();
                mConnection.RequestStyleList();
            }
        }

        public void SetStatus(string pStatus)
        {
            StatusTextBox.Text = pStatus;
        }

        private void ConnectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mConnection.isConnected())
            {
                if (MessageBox.Show("There is no active connection yet, so I can not work. Do you want to exit the application?", "Uh.. oh..", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.FormClosing -= ConnectForm_FormClosing;
                    Application.Exit();
                } else
                {
                    e.Cancel = true;
                }
            }
        }

        private void IPTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConnectButton_Click(this, new EventArgs());
            }
        }

    }
}
