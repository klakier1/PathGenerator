using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathGenerator
{
    public partial class Form1 : Form
    {
        private const double Z_VALUE = -5;
        private const double X_SIZE = 330;
        private const double Y_SIZE = 130;
        private const double X_OFFSET = 20; //po obu stronach
        private const double X_START = 0; //tylko na poczatku
        private const int LINES = 10;
        private readonly double[] Y_POINTS = new double[] { 17.1, 22, 52, 67, 82, 112, 116.9 };
        private readonly double[] Y_POINTS_REV = new double[] { 17.1, 22, 52, 67, 82, 112, 115.9 }.Reverse().ToArray();
        private const int START_GLUE = 1;
        private const int STOP_GLUE = 5;

        private const int TOOL = 1;
        private const int BASE = 10;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //generowanie punktów w X            
            double[] x_points = new double[LINES];
            double x_spracing = (X_SIZE - 2.0 * X_OFFSET - X_START) / (LINES - 1);
            for (int i = 0; i < LINES; i++)
            {
                x_points[i] = X_OFFSET + X_START + x_spracing * i;
            }

            List<E6POS> points = new List<E6POS>();

            LDAT ldat = new LDAT("LL");
            
            for (int i = 0; i < LINES; i++)
            {
                double[] y_points = (i % 2 == 0) ? Y_POINTS : Y_POINTS_REV; //dla parzysty punkty normalne a nie parzystych odwrotne
                for (int j = 0; j < Y_POINTS.Count(); j++)
                {
                    string nameE6POS = String.Format("XKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                    string nameFDAT = String.Format("FKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                    E6POS e6pos = new E6POS(x_points[i], y_points[j], Z_VALUE, nameE6POS);
                    FDAT fdat = new FDAT(TOOL, BASE, nameFDAT);
                    SLIN slin = new SLIN(0.1, e6pos, fdat, ldat);
                    points.Add(e6pos);

                    Debug.WriteLine(slin.ToString());
                }
            }
        }
    }
}
