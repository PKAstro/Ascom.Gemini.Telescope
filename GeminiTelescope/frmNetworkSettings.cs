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


            txtIP.Culture = GeminiHardware.m_GeminiCulture;
            System.Net.IPAddress ip;
            if (!System.Net.IPAddress.TryParse(GeminiHardware.EthernetIP, out ip))
                ip = System.Net.IPAddress.Parse("192.168.000.100");

            txtIP.Text = string.Format("{0:000}.{1:000}.{2:000}.{3:000}",
                ip.GetAddressBytes()[0],
                ip.GetAddressBytes()[1],
                ip.GetAddressBytes()[2],
                ip.GetAddressBytes()[3]);

                        
            txtUser.Text = GeminiHardware.EthernetUser;
            txtPassword.Text = GeminiHardware.EthernetPassword;
            chkNoProxy.Checked = GeminiHardware.BypassProxy;
            chkDHCP.Checked = GeminiHardware.UseDHCP;

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

                System.Net.IPAddress ip;
                if (!System.Net.IPAddress.TryParse(tIP, out ip))
                {
                    MessageBox.Show("Invalid IP address: " + txtIP.Text);
                    txtIP.Focus();
                    return;
                }

                //reparse this to remove leading zeroes -- leading zeroes are 
                // a signal that ip bytes are specified in octal, not decimal!
                Regex R = new Regex(@"(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})");
                Match m = R.Match(tIP);

                GeminiHardware.EthernetIP = string.Format("{0}.{1}.{2}.{3}",
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(m.Groups[3].Value),
                    int.Parse(m.Groups[4].Value));

                GeminiHardware.UseDHCP = false;
            }
            else
            {
                GeminiHardware.UseDHCP = true;
                GeminiHardware.GeminiDHCPName = tIP;
            }

            GeminiHardware.EthernetUser = txtUser.Text;
            GeminiHardware.EthernetPassword = txtPassword.Text;
            GeminiHardware.BypassProxy = chkNoProxy.Checked;
          
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkDHCP_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDHCP.Checked)
            {
                txtIP.Mask = "";
                txtIP.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Default;
                txtIP.Text = GeminiHardware.GeminiDHCPName;

            }
            else
            {
                txtIP.Mask = "000.000.000.000";
                txtIP.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Default;

                System.Net.IPAddress ip;
                if (!System.Net.IPAddress.TryParse(GeminiHardware.EthernetIP, out ip))
                    ip = System.Net.IPAddress.Parse("192.168.000.100");

                txtIP.Text = string.Format("{0:000}.{1:000}.{2:000}.{3:000}",
                    ip.GetAddressBytes()[0],
                    ip.GetAddressBytes()[1],
                    ip.GetAddressBytes()[2],
                    ip.GetAddressBytes()[3]);
            }
        }

    }
}
