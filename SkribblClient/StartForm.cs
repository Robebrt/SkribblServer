using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkribblClient
{
    public partial class StartForm : Form
    {
        int roomId = 0;
        int countJoin = 0;
        public static Player player = null;
        public static string actionType = "";
        public StartForm()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            actionType = "<Create room>";
            string username = textBox1.Text;
            if (username == "")
            {
                //when username is left empty fill it random
                username = "username random";
            }
            player = new Player(username, roomId, "avatar");
            roomId++;

            Form1 frm = new Form1(player);
            
            try
            {
                frm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la afișarea formularului: " + ex.Message);
            };
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (countJoin == 0)
            {
                button1.Enabled = false;
                label3.Visible = true;
                textBox2.Visible = true;
                //
            }
            else if(countJoin == 1)
            {
                //join the room with the specified id
                actionType = "<Join room>";
            }
            countJoin++;

        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            label3.Visible = false;
            textBox2.Visible = false;
        }
    }
}
