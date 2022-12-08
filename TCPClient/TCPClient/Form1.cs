using SuperSimpleTcp;
using System.Globalization;
using System.Text;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        SimpleTcpClient client;
        System.Diagnostics.Stopwatch executionTimeClient = new System.Diagnostics.Stopwatch(); //fixing ambiguous reference between namespaces  

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            txtMessage.Enabled = false;
            btnSend.Enabled = false;
            btnDisconnect.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new SimpleTcpClient(txtIP.Text + ":" + txtPort.Text);

                client.Events.Connected += Connected;
                client.Events.DataReceived += DataReceived;
                client.Events.Disconnected += Disconnected;

                client.Connect();

                txtIP.Enabled = false;
                txtPort.Enabled = false;
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
            catch
            {
                if((txtIP.Text == "") && (txtPort.Text == ""))
                    MessageBox.Show("Please enter an IP address and a port number.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Please enter a correct IP address and port number.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                //client.Disconnect(); //crash 

                txtIP.Enabled = true;
                txtPort.Enabled = true;
                btnSend.Enabled = false;
                txtMessage.Enabled = false;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text))
                {
                    try
                    {
                        var bytes = txtMessage.Text.Split(' ').Select(hx => byte.Parse(hx, NumberStyles.AllowHexSpecifier)).ToArray();
                       
                        //trebuie tinut cont de header (vezi bookmark)
                        //fiecare dintre cele 3 casete inainte de user id (slave address) sunt create separat  
                        //+ bytes
                        //iar apoi sunt 'adunate' toate in Send(); 
                        
                        client.Send(bytes);
                        
                        txtInfo.Text += $"Sent: {txtMessage.Text}{Environment.NewLine}{Environment.NewLine}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Invalid format", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    //txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("The text box is empty. A message must be entered.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";
                txtInfo.Text += $"{Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Connected to [{e.IpPort}].{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"[{e.IpPort}] disconnected.{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void txtInfo_TextChanged(object sender, EventArgs e)
        {
            txtInfo.SelectionStart = txtInfo.TextLength;
            txtInfo.ScrollToCaret();
        }

        //private void txtMessage_Validating(object sender, CancelEventArgs e)
        //{
        //    char[] allowedChars = new char[] { ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        //    foreach (char character in txtMessage.Text.ToUpper().ToArray())
        //    {
        //        if (!allowedChars.Contains(character))
        //        {
        //            MessageBox.Show($"[{character}] is not a hexadecimal character", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        //cum sa sterg asta fara sa afectez?
        private void groupBox1_Enter(object sender, EventArgs e)
        {
            //asdsdasdfasdfad
        }
    }
}