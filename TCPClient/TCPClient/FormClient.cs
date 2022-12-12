using SuperSimpleTcp;
using System.Globalization;
using System.Text;

namespace TCPClient
{
    public partial class FormClient : Form
    {     
        SimpleTcpClient client;

        public FormClient()
        {
            InitializeComponent();
        }
        
        private void FormClient_Load(object sender, EventArgs e)
        {
            txtBoxTransactionId.CharacterCasing = CharacterCasing.Upper;
            txtBoxProtocolId.CharacterCasing = CharacterCasing.Upper;
            txtBoxUnitId.CharacterCasing = CharacterCasing.Upper;
            txtBoxFunctionCode.CharacterCasing = CharacterCasing.Upper;
            txtBoxData.CharacterCasing = CharacterCasing.Upper;

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

                btnSend.Enabled = true;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                txtIP.Enabled = false;
                txtPort.Enabled = false;
                txtBoxFunctionCode.Enabled = true;
                txtBoxTransactionId.Enabled = true;
                txtBoxProtocolId.Enabled = true;
                txtBoxData.Enabled = true;
                txtBoxUnitId.Enabled = true;
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
                btnSend.Enabled = false;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;

                txtIP.Enabled = true;
                txtPort.Enabled = true;
                txtBoxFunctionCode.Enabled = false;
                txtBoxTransactionId.Enabled = false;
                txtBoxProtocolId.Enabled = false;
                txtBoxData.Enabled = false;
                txtBoxUnitId.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //def length
        public const byte unitIdLength = 0x01;
        public const byte functionCodeLength = 0x01;

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                if ((!string.IsNullOrEmpty(txtBoxFunctionCode.Text)) || (!string.IsNullOrEmpty(txtBoxData.Text)))
                {
                    try
                    {
                        short transactionId = byte.Parse(txtBoxTransactionId.Text, NumberStyles.HexNumber);
                        short protocolId = byte.Parse(txtBoxProtocolId.Text, NumberStyles.HexNumber);
                        byte unitId = byte.Parse(txtBoxUnitId.Text, NumberStyles.HexNumber);
                        byte functionCode = byte.Parse(txtBoxFunctionCode.Text, NumberStyles.HexNumber);
                        short[] dataFrame = txtBoxData.Text.Split(' ')
                            .Select(hex => Convert.ToInt16(hex))
                            .ToArray();
                        short lengthOfMessage = (short)(unitIdLength + functionCodeLength + 2 * dataFrame.Length); //vf asta pe site la FC 

                        short[] buffer = new short[5 + dataFrame.Length];
                        buffer[0] = transactionId;
                        buffer[1] = protocolId;
                        buffer[2] = lengthOfMessage;
                        buffer[3] = unitId; //byte -> short !!
                        buffer[4] = functionCode;

                        int elementNumber = 5;
                        foreach(short d in dataFrame)
                        {
                            buffer[elementNumber] = d;
                            elementNumber++;
                        }

                        txtInfo.Text += "request: ";
                        foreach(short elem in buffer)
                        {
                            txtInfo.Text += $" {elem.ToString("X4")}";
                        }

                        //client.Send( );  

                        //txtInfo.Text += $"{Environment.NewLine}request: {transactionId.ToString("X4")} {protocolId.ToString("X4")} " +
                        //                $"{lengthOfMessage.ToString("X4")} {unitId.ToString("X2")} {functionCode.ToString("X2")}";

                        //foreach (int element in dataFrame) //met mai ok ?
                        //    txtInfo.Text += $" {element.ToString("X4")}";

                        txtInfo.Text += Environment.NewLine;
                    }
                    catch
                    {
                    MessageBox.Show("Invalid format", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    txtBoxTransactionId.Text = string.Empty;
                    txtBoxProtocolId.Text = string.Empty;
                    txtBoxUnitId.Text = string.Empty;
                    txtBoxFunctionCode.Text = string.Empty;
                    txtBoxData.Text = string.Empty;
                }
                else
                {   //cred ca trebuie scoasa conditia asta si sa apara doar invalid format

                    MessageBox.Show("The text box is empty. A message must be entered.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                string response = Encoding.UTF8.GetString(e.Data);
                txtInfo.Text += $"response: {response}{Environment.NewLine}";
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
                txtInfo.Text += $"{Environment.NewLine}[{e.IpPort}] disconnected.{Environment.NewLine}";
            });
        }

        private void txtInfo_TextChanged(object sender, EventArgs e)
        {
            txtInfo.SelectionStart = txtInfo.TextLength;
            txtInfo.ScrollToCaret();
        }
    }
}