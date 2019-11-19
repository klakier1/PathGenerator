using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathGenerator
{
    class RobPoint
    {
        double x;
        double y;
        double z;
        double v;
        GlueFunc func = GlueFunc.Without;
        double apo_dist = 2;

        public RobPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public RobPoint(double x, double y, double z, double v)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.v = v;
        }

        public RobPoint(double x, double y, double z, double v, GlueFunc func)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.v = v;
            this.func = func;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }
        public double V { get => v; set => v = value; }
        public double Apo_dist { get => apo_dist; set => apo_dist = value; }
        internal GlueFunc Func { get => func; set => func = value; }

        public enum GlueFunc { Without , StartGlue, StopGlue}
    }
}
