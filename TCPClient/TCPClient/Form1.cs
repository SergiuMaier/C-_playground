using SuperSimpleTcp;
//using System.Diagnostics;
//using DataReceiveEventArgs = tool.System.Diagnostics;
using System.Text;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpClient client;
        System.Diagnostics.Stopwatch executionTimeClient = new System.Diagnostics.Stopwatch(); //fixing ambiguous reference between namespaces  

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new(txtIP.Text);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;

            btnSend.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect();
                
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
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
                    //executionTimeClient.Reset();
                    txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";

                    //alta varianta de calcul timp
                    //var startTime = DateTime.Now;
                    //...
                    //Sending time: {DateTime.Now - startTime}

                    //executionTimeClient.Start();
                    client.Send(txtMessage.Text);
                    //executionTimeClient.Stop();

                    txtInfo.Text += $"Sent: {txtMessage.Text}{Environment.NewLine}{Environment.NewLine}";
                    //txtInfo.Text += $"[Execution time: {executionTimeClient.ElapsedMilliseconds} ms]{Environment.NewLine}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;     
                }
            }
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
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

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Connected to server [{e.IpPort}].{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
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
    }
}