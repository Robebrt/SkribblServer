using System.Text;
using System.Collections;
namespace SkribblClient
{
    public partial class Form1 : Form
    {
        Client client;
        Player player;
        public string actionType;
        public Form1(Player player)
        {
            InitializeComponent();
            client = new Client(this);
            this.player = player;
            actionType = StartForm.actionType;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //client.StartClient(richTextBox1);
            //button1.Enabled = false;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string message = textBox1.Text+"<EOF>";
            //byte[] msg = Encoding.ASCII.GetBytes(message);
            //client.SendMessage(msg, richTextBox1, textBox1);

        }
        private void Form1_Load(object sender, EventArgs e)
        {
           client.StartClient(player);
        }
        public void RunOnUiThread(Action action)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(action);
                }
                catch (Exception ex)
                {
                    //client = null;
                }
            }
            else
            {
                action();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.stopClient();
            client = null;
        }
    }
}