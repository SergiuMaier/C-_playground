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
        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            
            txtInfo.Text = $"Starting...{Environment.NewLine}";
            
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
           
            server = new SimpleTcpServer(txtIP.Text);
            
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

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
                txtInfo.Text += $"{DateTime.UtcNow}{Environment.NewLine}";

                var startTime = DateTime.UtcNow;

                txtInfo.Text += $"Client [{e.IpPort}]: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                txtInfo.Text += $"(transfer time: {DateTime.UtcNow - startTime} ms){Environment.NewLine}{Environment.NewLine}";
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
                        txtInfo.Text += $"{DateTime.UtcNow}{Environment.NewLine}";

                        var startTime = DateTime.UtcNow;

                        server.Send(listClientIP.SelectedItem.ToString(), txtMessage.Text);

                        txtInfo.Text += $"Sent: {txtMessage.Text}{Environment.NewLine}";
                        txtInfo.Text += $"(transfer time: {DateTime.UtcNow - startTime}){Environment.NewLine}{Environment.NewLine}";
                        txtMessage.Text = String.Empty;
                    }
                }
                catch (Exception ex)
                {
                        MessageBox.Show("A client must be selected!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}