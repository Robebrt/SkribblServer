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
    public partial class GameForm : Form
    {
        Bitmap bitmap;
        Graphics g;
        bool paint = false;
        Point px, py;
        Color color;
        bool colorSet = false;
        bool sizeSet = false;
        int size;
        Pen pen = new Pen(Color.Black, 1);
        Pen eraser = new Pen(Color.White, 1);
        int index = 1;

        public GameForm()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            px = e.Location;
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
                    py = e.Location;
                    g.DrawRectangle(eraser, e.X, e.Y, eraser.Width, eraser.Width);
                    px = py;
                    pictureBox1.Image = bitmap;

                }

            }

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
            paint=false;
        }
        public void validadte(Bitmap bitmap, Stack<Point>sp,int x, int y,Color old_color, Color new_color)
        {
            Color cx = bitmap.GetPixel(x, y);
            if(cx==old_color)
            {
                sp.Push(new Point(x,y));
                bitmap.SetPixel(x, y, new_color);
            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(index == 3)
            {
                Point point = set_point(pictureBox1, e.Location);
                Fill(bitmap, point.X, point.Y, pen.Color);
            }
        }

        public void Fill(Bitmap bitmap, int x, int y, Color new_clr)
        {
            Color old_color = bitmap.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x,y));
            bitmap.SetPixel(x, y, new_clr);
            if (old_color == new_clr) return;
            while(pixel.Count>0)
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
        static Point set_point(PictureBox pictureBox, Point pt)
        {
            float pX = 1f * pictureBox.Image.Width / pictureBox.Width;
            float pY = 1f * pictureBox.Image.Height / pictureBox.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }
    }
}
