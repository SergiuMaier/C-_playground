using SuperSimpleTcp;
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
                    txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";

                    var startTime = DateTime.Now;
                    
                    client.Send(txtMessage.Text);

                    txtInfo.Text += $"Message: {txtMessage.Text}{Environment.NewLine}";
                    txtInfo.Text += $"[Sending time: {DateTime.Now - startTime} seconds]{Environment.NewLine}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
            }
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"[{DateTime.Now}]{Environment.NewLine}";

                var startTime = DateTime.Now;
                txtInfo.Text += $"Server: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                txtInfo.Text += $"[Receiving time: {DateTime.Now - startTime} seconds]{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server [{e.IpPort}] connected.{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server [{e.IpPort}] disconnected.{Environment.NewLine}{Environment.NewLine}";
            });
        }
    }
}