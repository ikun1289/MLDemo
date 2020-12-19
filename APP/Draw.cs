using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MLAPPML.ConsoleApp
{
    public class DrawNote
    {
        public int X { set; get; }
        public int Y { set; get; }
        public Color color { set; get; }
        public Pen pen { set; get; }
        public bool isDraw { get; set; }

        public DrawNote()
        {
            color = Color.Black;
            pen = new Pen(color, 4);
            isDraw = false;
        }
    }
}
