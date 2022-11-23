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
                    var startTime = DateTime.UtcNow;
                    client.Send(txtMessage.Text);
                    //client.Send(Convert.ToString(DateTime.UtcNow - startTime)); 

                    txtInfo.Text += $"Sent: {txtMessage.Text}{Environment.NewLine}";
                    txtInfo.Text += $"(transfer time: {DateTime.UtcNow - startTime}){Environment.NewLine}{Environment.NewLine}";
                    
                    txtMessage.Text = string.Empty;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new(txtIP.Text);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
            
            btnSend.Enabled = false;
        }
        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server [{e.IpPort}] connected.{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                var startTime = DateTime.UtcNow;
                txtInfo.Text += $"Server: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                txtInfo.Text += $"(transfer time: {DateTime.UtcNow - startTime}){Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server {e.IpPort} disconnected.{Environment.NewLine}{Environment.NewLine}";
            });
        }
    }
}