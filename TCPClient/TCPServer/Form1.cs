using SuperSimpleTcp;
using System.Text;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;
        System.Diagnostics.Stopwatch executionTime = new System.Diagnostics.Stopwatch();

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();

            txtInfo.Text = $"Starting...{Environment.NewLine}";
            
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer(txtIP.Text + ":" + txtPort.Text);
            
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

            btnSend.Enabled = false;
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Client [{e.IpPort}] connected.{Environment.NewLine}{Environment.NewLine}";
                listClientIP.Items.Add(e.IpPort);
            });   
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
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
                }
                catch
                {
                    MessageBox.Show("A client must be selected!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //txtInfo.Text += $"A client must be selected!{Environment.NewLine}{Environment.NewLine}";
                }
            } 
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
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