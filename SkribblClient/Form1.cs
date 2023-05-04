using System.Text;

namespace SkribblClient
{
    public partial class Form1 : Form
    {
        Client client;
        GameForm gameForm;
        public Form1()
        {
            InitializeComponent();
            client = new Client(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            client.StartClient(richTextBox1);
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string message = textBox1.Text+"<EOF>";
            byte[] msg = Encoding.ASCII.GetBytes(message);
            client.sendMessage(msg, richTextBox1, textBox1);

        }
        public void RunOnUiThread(Action action)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(action);
                }
                catch(Exception ex)
                {
                    client = null;
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

        private void button4_Click(object sender, EventArgs e)
        {
            gameForm = new GameForm();
            gameForm.Show();
        }
    }
}