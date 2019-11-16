using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathGenerator
{
    class E6POS
    {
        double x, y, z;
        string name;

        public E6POS(double x, double y, double z, string name)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Name = name;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }
        public string Name { get => name; set => name = value; }

        override public String ToString()
        {
            //DECL E6POS XKLB_12_05={X 112.710,Y 267.240,Z -5.01000,A -90.0000,B 0.0,C 180.000,S 6,T 50} 
            return String.Format("DECL E6POS {0}={{X {1},Y {2},Z {3},A -90.0000,B 0.0,C 180.000,S 6,T 50}}",
                Name,
                X.ToString("F5", CultureInfo.InvariantCulture),
                Y.ToString("F5", CultureInfo.InvariantCulture),
                Z.ToString("F5", CultureInfo.InvariantCulture));
        }
    }
}
