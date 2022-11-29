using SuperSimpleTcp;
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
                //client.Disconnected();

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
                    //alta varianta de calcul timp
                    //var startTime = DateTime.Now;
                    //...
                    //Sending time: {DateTime.Now - startTime}
                    
                    //executionTimeClient.Reset();
                    //executionTimeClient.Start();
                    client.Send(txtMessage.Text);
                    //executionTimeClient.Stop();

                    txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";
                    txtInfo.Text += $"Sent: {txtMessage.Text}{Environment.NewLine}{Environment.NewLine}";
                    //txtInfo.Text += $"[Execution time: {executionTimeClient.ElapsedMilliseconds} ms]{Environment.NewLine}{Environment.NewLine}";
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
                //executionTimeClient.Reset();
                txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";

                executionTimeClient.Start();
                txtInfo.Text += $"Server: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                executionTimeClient.Stop();
                
                txtInfo.Text += $"[Time: {executionTimeClient.ElapsedMilliseconds} ms]{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Connected to server [{e.IpPort}].{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server [{e.IpPort}] disconnected.{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void txtInfo_TextChanged(object sender, EventArgs e)
        {
            txtInfo.SelectionStart = txtInfo.TextLength;
            txtInfo.ScrollToCaret();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            //asdsdasdfasdfad
        }
    }
}