using SuperSimpleTcp;
using System.Text;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        SimpleTcpServer server;
        System.Diagnostics.Stopwatch executionTime = new System.Diagnostics.Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer(txtIP.Text + ":" + txtPort.Text);

            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;

            txtMessage.Enabled = false;
            btnSend.Enabled = false;
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();

            txtIP.Enabled = false;
            txtPort.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnSend.Enabled = true;
            txtMessage.Enabled = true;

            txtInfo.Text = $"Starting...{Environment.NewLine}";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();

            foreach (var item in listClientIP.Items)
            {
                server.Send(item.ToString(), "stop");
            }

            listClientIP.Items.Clear();
            
            txtInfo.Text += $"Stopping...{Environment.NewLine}";

            txtIP.Enabled = true;
            txtPort.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            txtMessage.Enabled = false;
            btnSend.Enabled = false;
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Client [{e.IpPort}] connected.{Environment.NewLine}{Environment.NewLine}";
                listClientIP.Items.Add(e.IpPort);
            });   
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";

                //executionTime.Reset();
                executionTime.Start();
                txtInfo.Text += $"Client [{e.IpPort}]: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                executionTime.Stop();
                
                txtInfo.Text += $"[Time: {executionTime.ElapsedMilliseconds} ms]{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (server.IsListening)
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtMessage.Text) && listClientIP.SelectedItems != null)
                    {
                        //executionTime.Reset();
                        //executionTime.Start();
                        server.Send(listClientIP.SelectedItem.ToString(), txtMessage.Text);
                        //executionTime.Stop();

                        txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";
                        txtInfo.Text += $"Sent: {txtMessage.Text}{Environment.NewLine}{Environment.NewLine}";
                        //txtInfo.Text += $"[Execution time: {executionTime.ElapsedMilliseconds} ms]{Environment.NewLine}{Environment.NewLine}";
                        txtMessage.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("The text box is empty. A message must be entered.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Please select a client!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //txtInfo.Text += $"A client must be selected!{Environment.NewLine}{Environment.NewLine}";
                }
            } 
        }

        private void ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Client [{e.IpPort}] disconnected.{Environment.NewLine}{Environment.NewLine}";
                listClientIP.Items.Remove(e.IpPort);
            });
        }

        private void txtInfo_TextChanged(object sender, EventArgs e)
        {
            txtInfo.SelectionStart = txtInfo.TextLength;
            txtInfo.ScrollToCaret();
        }
    }
}