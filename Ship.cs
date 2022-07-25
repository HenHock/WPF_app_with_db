using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBD
{
    public class Ship
    {
        private int idShip { get; set; }
        private string nameShip { get; set; }

        private Image imageShip;

        public Ship()
        {

        }

        public System.Drawing.Image ImageShip
        {
            get
            {
                return imageShip;
            }

            set
            {
                imageShip = value;
            }
        }
    }
}
