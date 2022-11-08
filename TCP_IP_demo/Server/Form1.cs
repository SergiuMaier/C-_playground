using SimpleTCP;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;
        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;
            server.DataReceived += Server_DataReceived;
        }
        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text += e.MessageString;
                e.ReplyLine(string.Format("You said: {0}", e.MessageString));
            });
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            txtLabelStatus.Text = "Starting...";
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtHost.Text);
            server.Start(ip, Convert.ToInt32(txtPort.Text));
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
            { 
                txtLabelStatus.Text = "-";
                server.Stop();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
        }
    }
}