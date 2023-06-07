using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkribblClient
{
    public class DrawingData
    {
        public Point StartPoint { get; set; }
        public Color LineColor { get; set; }
        public float LineThickness { get; set; }
        public Boolean Fill { get; set; }
        public Boolean Clear { get; set; }
        public DrawingData(Point startPoint, Color lineColor, float lineThickness, Boolean fill, bool clear)
        {
            StartPoint = startPoint;
            LineColor = lineColor;
            LineThickness = lineThickness;
            Fill = fill;
            Clear = clear;
        }

        public byte[] ConvertDrawingData(DrawingData data, int id)
        {
            string json = JsonConvert.SerializeObject(data);
            string message = "<Draw>"+id+json+"<EOF>";
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            return bytesToSend;
        }


        
    }
}
