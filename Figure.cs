using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
namespace TUIO_TEST
{
   public class Figure
    {
        public Bitmap img;
        public int x, y, w, h;
        public int MarkerId;
        public int type;
        public string word;
        public int secondMarkerId = -1;
        public SolidBrush brush;
        public bool played = false;

       public Figure(string path, int id)
        {
            img = new Bitmap(path);
            w = 100;
            h = 100;
            img = new Bitmap(img, new Size(w, h));
            MarkerId = id;
            type = 0;

        }
        public Figure(int id,string label)
        {

            w = 100;

            h = 100;
            MarkerId = id;
            type = 1;
            word = label;
        }
        public Figure(Bitmap img, int id)
        {
            w = 100;
            h = 100;
            this.img = new Bitmap(img, new Size(w, h));
            MarkerId = id;
            type = 0;

        }

    


    }
}
