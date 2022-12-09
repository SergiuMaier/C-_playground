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
            txtBoxTransactionIdentifier.CharacterCasing = CharacterCasing.Upper;
            txtBoxProtocolIdentifier.CharacterCasing = CharacterCasing.Upper;
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
                txtBoxTransactionIdentifier.Enabled = true;
                txtBoxProtocolIdentifier.Enabled = true;
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
                txtBoxTransactionIdentifier.Enabled = false;
                txtBoxProtocolIdentifier.Enabled = false;
                txtBoxData.Enabled = false;
                txtBoxUnitId.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Length
        public const byte UnitIdentifierLength = 0x01;
        public const byte FunctionCodeLength = 0x01;

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                if ((!string.IsNullOrEmpty(txtBoxFunctionCode.Text)) || (!string.IsNullOrEmpty(txtBoxData.Text)))
                {
                    try
                    {
                        var transactionIdentifier = Convert.ToInt32(txtBoxTransactionIdentifier.Text, 16);
                        var protocolIdentifier = Convert.ToInt32(txtBoxProtocolIdentifier.Text, 16);

                        byte unitId = byte.Parse(txtBoxUnitId.Text, NumberStyles.HexNumber);
                        byte functionCode = byte.Parse(txtBoxFunctionCode.Text, NumberStyles.HexNumber);

                        byte[] dataFrame = txtBoxData.Text.Split(' ').Select(hex => byte.Parse(hex, NumberStyles.AllowHexSpecifier)).ToArray();
                        //byte[] dataFrame = txtBoxData.Text.Split(' ').Select(hex => Convert.ToByte(hex, 16)).ToArray();

                        //var lungime_in_hex = unitId + fc + data

                        //client.Send( );  //trebuie toate concatenate
                                           //nu pot trimite byte cu Send
                                           //
                        //txtInfo.Text += $"{dataFrameLength}";

                        txtInfo.Text += $"{Environment.NewLine}request: 0x{txtBoxTransactionIdentifier.Text} 0x{txtBoxProtocolIdentifier.Text} " +
                        //$"{lungime} " +
                        $" 0x{txtBoxUnitId.Text} 0x{txtBoxFunctionCode.Text}";
                        
                        foreach(byte element in dataFrame)
                        {
                            txtInfo.Text += $" 0x{element}";
                        }

                        txtInfo.Text += $"{Environment.NewLine}"; 
                    }
                    catch
                    {
                        MessageBox.Show("Invalid format", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    txtBoxTransactionIdentifier.Text = string.Empty;
                    txtBoxProtocolIdentifier.Text = string.Empty;
                    txtBoxUnitId.Text = string.Empty;
                    txtBoxFunctionCode.Text = string.Empty;
                    txtBoxData.Text = string.Empty;
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
                string response = Encoding.UTF8.GetString(e.Data);
                txtInfo.Text += $"response: {response}{Environment.NewLine}";
            });
        }

        private void Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Connected to [{e.IpPort}].{Environment.NewLine}";
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