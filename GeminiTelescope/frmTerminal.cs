using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASCOM.GeminiTelescope
{
    public partial class frmTerminal : Form
    {
        public frmTerminal()
        {
            InitializeComponent();
        }

        private void frmTerminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmMain frm = Application.OpenForms[0] as frmMain;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return; //exiting
            e.Cancel = true;
            this.Hide();
        }

        private void txtTerm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                try
                {
                    string line;
                    int start = txtTerm.GetFirstCharIndexOfCurrentLine();
                    int idx = start - 2;
                    while (idx >= 0 && txtTerm.Text[idx] != '\n') idx--;
                    idx++;
                    int end = txtTerm.Text.IndexOf('\n', idx);
                    if (end > idx)
                        line = txtTerm.Text.Substring(idx, end - idx - 1);
                    else
                        line = txtTerm.Text.Substring(idx);

                    line = line.TrimStart();
                    if (line == "" || line.StartsWith("#"))
                        return;
                    string start_chars = ":<>\07@";
                    string result = "bad command";
                    if (!GeminiHardware.Instance.Connected)
                        result = "GEMINI NOT CONNECTED!!!";

                    if (GeminiHardware.Instance.Connected && line.Length > 0 && start_chars.IndexOf(line[0]) >= 0)
                    {
                        var cmds = line.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < cmds.Length; ++i)
                        {
                            string ln = cmds[i];
                            bool bRaw = false;
                            if (ln.StartsWith("@"))
                            {
                                bRaw = true;
                                ln = ln.Substring(1);
                            }
                            if (i == 0) result = ""; else result += "   ";

                            GeminiCommand.ResultType gemini_result = GeminiCommand.ResultType.HashChar;
                            int char_count = 0;
                            GeminiCommand gmc = GeminiHardware.Instance.FindGeminiCommand(ln);

                            if (gmc != null)
                            {
                                gemini_result = gmc.Type;
                                char_count = gmc.Chars;
                            }

                            string s = null;
                            if (gemini_result == GeminiCommand.ResultType.NoResult)
                                GeminiHardware.Instance.DoCommand(ln, bRaw);
                            else
                                s = GeminiHardware.Instance.DoCommandResult(ln, GeminiHardware.Instance.MAX_TIMEOUT,        bRaw);
                            if (gemini_result == GeminiCommand.ResultType.NoResult)
                                result += "(none)";
                            else
                            if (s == null)
                                result += "TIMEOUT!!!";
                            else
                                result += s;
                        }
                    }

                    result = Regex.Replace(result,
                          @"\p{Cc}",
                          a => string.Format("[{0:X2}]", (byte)a.Value[0])
                        );

                    txtTerm.Text = txtTerm.Text.Substring(0, start) + $"{DateTime.Now.ToLongTimeString()} [{line}] Response: {result}\r\n" + txtTerm.Text.Substring(start, txtTerm.Text.Length - start);
                    txtTerm.SelectionStart = txtTerm.Text.Length;
                    txtTerm.SelectionLength = 0;
                    txtTerm.ScrollToCaret();

                }
                catch (Exception ex)
                {
                    int start = txtTerm.GetFirstCharIndexOfCurrentLine();

                    txtTerm.Text = txtTerm.Text.Substring(0, start) + $"{DateTime.Now.ToLongTimeString()} [Error: {ex.Message}]\r\n" + txtTerm.Text.Substring(start, txtTerm.Text.Length - start);
                    txtTerm.SelectionStart = txtTerm.Text.Length;
                    txtTerm.SelectionLength = 0;
                }
        }

        private void txtTerm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {        
                var idx = txtTerm.GetLineFromCharIndex(txtTerm.SelectionStart);
                if (idx < txtTerm.Lines.Length)
                {
                    var line = txtTerm.Lines[idx];
                    txtTerm.SelectionStart = txtTerm.GetFirstCharIndexOfCurrentLine() + line.Length;
                }
            }
        }
    }
}
