﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ASCOM.GeminiTelescope
{
    public partial class frmNetworkSettings : Form
    {
        public frmNetworkSettings()
        {
            InitializeComponent();


            txtIP.Culture = GeminiHardware.Instance.m_GeminiCulture;
            System.Net.IPAddress ip;
            if (!System.Net.IPAddress.TryParse(GeminiHardware.Instance.EthernetIP, out ip))
                ip = System.Net.IPAddress.Parse("192.168.000.111");

            txtIP.Text = string.Format("{0:000}.{1:000}.{2:000}.{3:000}",
                ip.GetAddressBytes()[0],
                ip.GetAddressBytes()[1],
                ip.GetAddressBytes()[2],
                ip.GetAddressBytes()[3]);

                        
            txtUser.Text = GeminiHardware.Instance.EthernetUser;
            txtPassword.Text = GeminiHardware.Instance.EthernetPassword;
            chkNoProxy.Checked = GeminiHardware.Instance.BypassProxy;
            chkDHCP.Checked = GeminiHardware.Instance.UseDHCP;
            txtPort.Text = GeminiHardware.Instance.UDPPort.ToString();

            if (GeminiHardware.Instance.UDP)
                rbUDP.Checked = true;
            else
                rbHTTP.Checked = true;

        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string tIP = txtIP.Text;


            if (!chkDHCP.Checked) //IP address specified:
            {
                tIP = tIP.Replace(" ", "");
                tIP = tIP.Trim();

                //reparse this to remove leading zeroes -- leading zeroes are 
                // a signal that ip bytes are specified in octal, not decimal!
                Regex R = new Regex(@"(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})");
                Match m = R.Match(tIP);

                string sIP = string.Format("{0}.{1}.{2}.{3}",
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(m.Groups[3].Value),
                    int.Parse(m.Groups[4].Value));

                System.Net.IPAddress ip;
                if (!System.Net.IPAddress.TryParse(sIP, out ip))
                {
                    MessageBox.Show("Invalid IP address: " + sIP);
                    txtIP.Focus();
                    return;
                }
                GeminiHardware.Instance.EthernetIP = sIP;

                GeminiHardware.Instance.UseDHCP = false;
            }
            else
            {
                GeminiHardware.Instance.UseDHCP = true;
                GeminiHardware.Instance.GeminiDHCPName = tIP;
            }

            GeminiHardware.Instance.EthernetUser = txtUser.Text;
            GeminiHardware.Instance.EthernetPassword = txtPassword.Text;

            GeminiHardware.Instance.UDP = rbUDP.Checked;
            if (rbUDP.Checked)
            {
                int port = 11110;
                if (!int.TryParse(txtPort.Text, out port))
                {
                    MessageBox.Show("Invalid UDP Port: " + txtPort.Text);
                    txtPort.Focus();
                    return;
                }
                GeminiHardware.Instance.UDPPort = port;
            }

            GeminiHardware.Instance.BypassProxy = chkNoProxy.Checked;
          
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkDHCP_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDHCP.Checked)
            {
                txtIP.Mask = "";
                txtIP.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Default;
                txtIP.Text = GeminiHardware.Instance.GeminiDHCPName;

            }
            else
            {
                txtIP.Mask = "000.000.000.000";
                txtIP.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Default;

                System.Net.IPAddress ip;
                if (!System.Net.IPAddress.TryParse(GeminiHardware.Instance.EthernetIP, out ip))
                    ip = System.Net.IPAddress.Parse("192.168.000.100");

                txtIP.Text = string.Format("{0:000}.{1:000}.{2:000}.{3:000}",
                    ip.GetAddressBytes()[0],
                    ip.GetAddressBytes()[1],
                    ip.GetAddressBytes()[2],
                    ip.GetAddressBytes()[3]);
            }
        }

        private void rbHTTP_CheckedChanged(object sender, EventArgs e)
        {
            if (rbHTTP.Checked)
            {
                txtPort.Enabled = false;
                txtPort.BackColor = Color.Gray;
            }
            else
            {
                txtPort.Enabled = true;
                txtPort.BackColor = Color.White;
            }
        }

        private void frmNetworkSettings_Load(object sender, EventArgs e)
        {
            SharedResources.SetInstance(this);
        }
    }
}
