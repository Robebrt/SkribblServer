using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkribblClient
{
    public partial class GameForm : Form
    {
        Bitmap bitmap;
        Graphics g;
        bool paint = false;
        Point px, py;
        Color color = Color.Black;
        bool colorSet = false;
        bool sizeSet = false;
        int size;
        Pen pen = new Pen(Color.Black, 1);
        Pen eraser = new Pen(Color.White, 1);
        int index = 1;
        public int roomToJoin;
        int countJoin = 0;
        public Player player = null;
        public string actionType = "";
        Client client;
        int idImg = 1;
        DrawingData drawingData;
        string word;
        string[] words = { "Casa", "Copac", "Soare", "Caine", "Masa", "Padure", "Fluture", "Ciocolata", "Minge", "Telefon", "Munte", "Bicicleta", "Cantec", "Fereastra", "Apa", "Trandafir", "Carte", "Stea", "Paine", "Avion", "Portocala", "Buzunar", "Lac", "Clopot", "Vultur", "Inghetata", "Valiza", "Hartie", "Balon", "Calatorie" };
        public GameForm()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            client = new Client(this);
            pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar1;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown; // Așează etichetele pe coloană
            flowLayoutPanel1.BackColor = Color.Transparent;

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                paint = true;
                px = e.Location;
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if (index == 1)
                {
                    if (colorSet)
                    {
                        pen.Color = color;
                        colorSet = false;

                    }
                    if (sizeSet)
                    {
                        pen.Width = size;
                        sizeSet = false;
                    }
                    py = e.Location;

                    g.DrawRectangle(pen, e.X, e.Y, pen.Width, pen.Width);


                    px = py;
                    pictureBox1.Image = bitmap;

                }
                else if (index == 2)
                {
                    if (sizeSet)
                    {
                        eraser.Width = size;
                        sizeSet = false;
                    }
                    pen.Color = Color.White;
                    py = e.Location;
                    g.DrawRectangle(eraser, e.X, e.Y, eraser.Width, eraser.Width);
                    px = py;
                    pictureBox1.Image = bitmap;

                }
                drawingData = new DrawingData(px, pen.Color, pen.Width, false);
                byte[] bytesToSend = drawingData.ConvertDrawingData(drawingData, player.roomId);
                client.SendMessage(bytesToSend);
            }

        }
        public void SetTimer(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            string formattedTime = timeSpan.ToString("mm\\:ss");
            label2.Text = formattedTime;
           
        }
        public void OnDataReceived(DrawingData data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<DrawingData>(OnDataReceived), data);
                return;
            }

            if (data.Fill == true)
            {
                Fill(bitmap, data.StartPoint.X, data.StartPoint.Y, data.LineColor);
            }
            else
            {
                if (data.LineColor == Color.White)
                {
                    g.DrawRectangle(eraser, data.StartPoint.X, data.StartPoint.Y, data.LineThickness, data.LineThickness);
                }
                else
                {
                    pen.Color = data.LineColor;
                    pen.Width = data.LineThickness;
                    g.DrawRectangle(pen, data.StartPoint.X, data.StartPoint.Y, data.LineThickness, data.LineThickness);
                }
            }
            pictureBox1.Image = bitmap;

        }
        private void button13_Click(object sender, EventArgs e)
        {
            index = 1;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            color = Color.Black;
            colorSet = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            color = Color.White;
            colorSet = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            color = Color.Red;
            colorSet = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            color = Color.Blue;
            colorSet = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            color = Color.Yellow;
            colorSet = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            color = Color.Green;
            colorSet = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            color = Color.Purple;
            colorSet = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            color = Color.Orange;
            colorSet = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            size = 1;
            sizeSet = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            size = 3;
            sizeSet = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            size = 5;
            sizeSet = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            size = 10;
            sizeSet = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
        }
        public void validadte(Bitmap bitmap, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bitmap.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bitmap.SetPixel(x, y, new_color);
            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 3)
            {
                Point point = set_point(pictureBox1, e.Location);
                Fill(bitmap, point.X, point.Y, color);

                DrawingData dd = new DrawingData(point, color, 0, true);
                byte[] bytesToSend = dd.ConvertDrawingData(dd, player.roomId);
                client.SendMessage(bytesToSend);

            }
            pictureBox1.Image = bitmap;
        }

        public void Fill(Bitmap bitmap, int x, int y, Color new_clr)
        {
            Color old_color = bitmap.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bitmap.SetPixel(x, y, new_clr);
            if (old_color == new_clr) { return; }
            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bitmap.Width - 1 && pt.Y < bitmap.Height - 1)
                {
                    validadte(bitmap, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validadte(bitmap, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validadte(bitmap, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validadte(bitmap, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

        }

        static Point set_point(PictureBox pictureBox, Point pt)
        {
            float pX = 1f * pictureBox.Width / pictureBox.Width;
            float pY = 1f * pictureBox.Height / pictureBox.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }



        private void createRoomButton_Click(object sender, EventArgs e)
        {
            actionType = "<Create room>";
            string username = usernameTextBox.Text;
            Random rnd = new Random();
            int num = rnd.Next(1, 1000);
            if (username == "")
            {
                //when username is left empty fill it random
                username = "username" + num;
            }
            Random random = new Random();
            int Newnum = random.Next(1, 30);
            player = new Player(username, "Avatar" + idImg, false, words[Newnum], 0);

            client.StartClient(player);

        }

        private void joinRoomButton_Click(object sender, EventArgs e)
        {

            if (countJoin % 2 == 0)
            {
                //createRooomButton.Enabled = false;
                roomIdLabel.Visible = true;
                RoomIdTextBox.Visible = true;
                connectButton.Visible = true;

            }
            else if (countJoin % 2 == 1)
            {

                roomIdLabel.Visible = false;
                RoomIdTextBox.Visible = false;
                connectButton.Visible = false;

            }

            countJoin++;

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                actionType = "<Chat>";
                string message = actionType + player.roomId + textBox1.Text;
                //richTextBox1.AppendText(message);
                byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");
                client.SendMessage(msg);
                //richTextBox1.AppendText(textBox1.Text);
                textBox1.Text = "";
            }
        }

        public void AddMessage(string message)
        {

            richTextBox1.AppendText(message);

        }
        public void joinRoom(int room, List<Player> list)
        {
            if (room != -1)
            {
                player.roomId = room;
                roomShowId.Text = "ROOM ID: " + player.roomId;
                actionType = "<Chat>";
                panel6.Visible = false;
                panel5.Visible = true;
                showPlayers(list);

            }

        }

        public void showPlayers(List<Player> list)
        {
            flowLayoutPanel1.Controls.Clear();
            foreach(Player player in list)
            {
                if(player.word!="")
                {
                    word = player.word;
                }
            }
            foreach (Player player in list)
            {
                FlowLayoutPanel fpanel = new FlowLayoutPanel();
                PictureBox avatarImg = new PictureBox();
                switch (player.avatar)
                {
                    case "Avatar1":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar1;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case "Avatar2":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar2;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case "Avatar3":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar3;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case "Avatar4":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar4;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case "Avatar5":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar5;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case "Avatar6":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar6;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case "Avatar7":
                        avatarImg.Image = global::SkribblClient.Properties.Resources.Avatar7;
                        avatarImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                }
                Label label = new Label();
                label.AutoSize = true;
                label.Font = new Font("Times New Roman", 15);
                label.ForeColor = Color.White;
                label.Text = player.username;
                Label label2 = new Label();
                label2.AutoSize = true;
                label2.Font = new Font("Times New Roman", 15);
                label2.ForeColor = Color.White;
                label2.Text = "Score:" + player.score.ToString();
                avatarImg.Width = 50;
                avatarImg.Height = 50;
                fpanel.FlowDirection = FlowDirection.TopDown;
                fpanel.BackColor = Color.Transparent;
                fpanel.Controls.Add(label);
                fpanel.Controls.Add(avatarImg);
                fpanel.Controls.Add(label2);
                fpanel.BorderStyle = BorderStyle.FixedSingle;
                fpanel.Width = 130;
                fpanel.Height = 110;
                flowLayoutPanel1.Controls.Add(fpanel);
                
                
                if (player.username == this.player.username)
                {
                    if (player.isDrawing == false)
                    {
                        pictureBox1.Enabled = false;
                        textBox1.Enabled = true;
                        label1.Text = "";
                        for(int i=0;i<word.Length;i++)
                        {
                            label1.Text += "_ ";
                        }
                    }
                    else
                    {
                        pictureBox1.Enabled = true;
                        textBox1.Enabled = false;
                        label1.Text = word;

                    }
                }

            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            //join the room with the specified id
            actionType = "<Join room>";
            string username = usernameTextBox.Text;
            Random rnd = new Random();
            int num = rnd.Next(1, 1000);
            if (username == "")
            {
                //when username is left empty fill it random
                username = "username" + num;
            }
            player = new Player(username, "Avatar" + idImg, false,"",0);
            roomToJoin = Int32.Parse(RoomIdTextBox.Text);
            client.StartClient(player);
        }

        private void prevImgButton_Click(object sender, EventArgs e)
        {
            if (idImg > 1)
            {
                idImg--;
            }
            switch (idImg)
            {
                case 1:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar1;
                    break;
                case 2:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar2;
                    break;
                case 3:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar3;
                    break;
                case 4:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar4;
                    break;
                case 5:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar5;
                    break;
                case 6:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar6;
                    break;
                case 7:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar7;
                    break;
            }
        }

        private void nextImgButton_Click(object sender, EventArgs e)
        {
            if (idImg < 7)
            {
                idImg++;
            }
            switch (idImg)
            {
                case 1:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar1;
                    break;
                case 2:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar2;
                    break;
                case 3:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar3;
                    break;
                case 4:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar4;
                    break;
                case 5:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar5;
                    break;
                case 6:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar6;
                    break;
                case 7:
                    pictureBox3.Image = global::SkribblClient.Properties.Resources.Avatar7;
                    break;
            }
        }

        private void createRadioButton_Click(object sender, EventArgs e)
        {
            roomIdLabel.Visible = false;
            RoomIdTextBox.Visible = false;
            connectButton.Visible = false;
            createRooomButton.Visible = true;
        }

        private void joinRadioButton_Click(object sender, EventArgs e)
        {
            connectButton.Visible = true;
            roomIdLabel.Visible = true;
            RoomIdTextBox.Visible = true;
            createRooomButton.Visible = false;
        }

        public void RunOnUiThread(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
