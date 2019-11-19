using PathGenerator.Kuka;
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
using System.Windows;

namespace PathGenerator
{
    public partial class Form1 : Form
    {
        public Size initSize;
        public Size initButton1;
        public Size initButton2;
        public Size initButton3;
        public Size initTextBox1;
        public Size initTextBox2;
        public Point initLocButton3;
        public Point initLocTextBox2;

        private const double Z_VALUE = -10;
        private const double Y_SIZE = 340;
        private const double X_SIZE = 133;
        private const double Y_OFFSET = 24; //po obu stronach, czarne zaczyna sie od 12mm 
        private const double Y_START = 0; //tylko na poczatku

        private const int TOOL = 1;
        private const int BASE = 10;

        //STRIPSE
        private const int LINES = 26;
        private readonly double[] X_POINTS = new double[] { 6, 31, 100, 127 };
        private readonly double[] SPEED = new double[] { 0.5, 0.3, 0.1, 0.3 };
        private readonly double[] X_POINTS_REV = new double[] { 6, 33, 102, 127 }.Reverse().ToArray();
        private readonly double[] SPEED_REV = new double[] { 0.5, 0.3, 0.1, 0.3 };
        private const int STOP_GLUE_POINT = 2;
        private const int START_GLUE_POINT = 1;

        //mustache
        private readonly double[] X_POINTS_MUSTACH = new double[] { 6, 28, 31, 46, 85, 100, 103, 127 };
        private readonly double[] Y_POINTS_MUSTACH = new double[] { 15, 10, 5, 0, 0, 5, 10, 15 };  //
        private readonly double[] SPEED_MUSTACH = new double[] { 0.5, 0.3, 0.1, 0.1, 0.1, 0.1, 0.1, 0.3 };
        private const int STOP_GLUE_POINT_MUSTACHE = 6;
        private const int START_GLUE_POINT_MUSTACHE = 1;

        //ZIGZAG
        private const double ZZ_Y_START = 24;
        private const double ZZ_Y_STOP = 316;
        private const double ZZ_X_START = 16;
        private const double ZZ_X_STOP = 117;
        private const int ZZ_VERTICES = 20;
        private const double ZZ_SPEED = 0.2;


        public Form1()
        {
            InitializeComponent();
            initSize = Size;
            MinimumSize = Size;
            initButton1 = button1.Size;
            initButton2 = button2.Size;
            initButton3 = button3.Size;
            initTextBox1 = textBox1.Size;
            initTextBox2 = textBox2.Size;
            initLocButton3 = button3.Location;
            initLocTextBox2 = textBox2.Location;

            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();

            StringBuilder stringBuilderSRC = new StringBuilder();
            StringBuilder stringBuilderDAT = new StringBuilder();

            //generowanie punktów w X            
            double[] y_points = new double[LINES];
            double y_spracing = (Y_SIZE - 2.0 * Y_OFFSET - Y_START) / (LINES - 1);
            for (int i = 0; i < LINES; i++)
            {
                y_points[i] = Y_OFFSET + Y_START + y_spracing * i;
            }

            List<E6POS> points = new List<E6POS>();

            LDAT ldat = new LDAT("LL");

            for (int i = 0; i < LINES; i++)
            {
                bool evenOrOdd = (i % 2 == 0);
                double[] x_points = evenOrOdd ? X_POINTS : X_POINTS_REV; //dla parzysty punkty normalne a nie parzystych odwrotne
                double[] speed = evenOrOdd ? SPEED : SPEED_REV;

                for (int j = 0; j < X_POINTS.Count(); j++)
                {
                    string nameE6POS = String.Format("XKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                    string nameFDAT = String.Format("FKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                    E6POS e6pos = new E6POS(x_points[j], y_points[i], Z_VALUE, nameE6POS);
                    FDAT fdat = new FDAT(TOOL, BASE, nameFDAT);
                    SLIN slin;
                    switch (j)
                    {
                        case START_GLUE_POINT:
                            {
                                slin = new SLIN_GLUE_ON(speed[j], e6pos, fdat, ldat);
                                break;
                            }
                        case STOP_GLUE_POINT:
                            {
                                bool endMeas = (i == LINES - 1) ? true : false; //na ostaniej linii wylacz pomiar
                                slin = new SLIN_GLUE_OFF(speed[j], e6pos, fdat, ldat);
                                ((SLIN_GLUE_OFF)slin).EndMeasurement = endMeas;
                                break;
                            }
                        default:
                            {
                                slin = new SLIN(speed[j], e6pos, fdat, ldat);
                                break;
                            }
                    }
                    points.Add(e6pos);

                    //Debug.WriteLine(e6pos.ToString());
                    stringBuilderSRC.AppendLine(slin.ToString());
                    stringBuilderDAT.AppendLine(e6pos.ToString());
                    stringBuilderDAT.AppendLine(fdat.ToString());
                }
                stringBuilderSRC.AppendLine();
                stringBuilderDAT.AppendLine();
            }

            stringBuilderDAT.AppendLine(ldat.ToString());

            textBox1.Text = stringBuilderSRC.ToString();
            textBox2.Text = stringBuilderDAT.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();

            StringBuilder stringBuilderSRC = new StringBuilder();
            StringBuilder stringBuilderDAT = new StringBuilder();

            //genereruj Y
            double[] y_points = new double[ZZ_VERTICES];
            double y_spracing = (ZZ_Y_STOP - ZZ_Y_START) / (ZZ_VERTICES - 1);
            for (int i = 0; i < ZZ_VERTICES; i++)
            {
                y_points[i] = ZZ_Y_START + y_spracing * i;
            }

            LDAT ldat = new LDAT("LL");

            //first point
            string nameE6POS = String.Format("XKLB_01");
            string nameFDAT = String.Format("FKLB_01");
            E6POS e6pos = new E6POS(ZZ_X_START, ZZ_Y_START, Z_VALUE, nameE6POS);
            FDAT fdat = new FDAT(TOOL, BASE, nameFDAT);
            SLIN slin = new SLIN_GLUE_ON(ZZ_SPEED, e6pos, fdat, ldat);

            stringBuilderSRC.AppendLine(slin.ToString());
            stringBuilderDAT.AppendLine(e6pos.ToString());
            stringBuilderDAT.AppendLine(fdat.ToString());

            //middle points
            for (int i = 0; i < ZZ_VERTICES; i++)
            {
                bool evenOrOdd = (i % 2 == 0);
                double X = evenOrOdd ? ZZ_X_STOP : ZZ_X_START;
                double Y = y_points[i];

                nameE6POS = String.Format("XKLB_{0}", (i + 2).ToString("D2"));
                nameFDAT = String.Format("FKLB_{0}", (i + 2).ToString("D2"));
                e6pos = new E6POS(X, Y, Z_VALUE, nameE6POS);
                fdat = new FDAT(TOOL, BASE, nameFDAT);
                slin = new SLIN(ZZ_SPEED, e6pos, fdat, ldat);

                stringBuilderSRC.AppendLine(slin.ToString());
                stringBuilderDAT.AppendLine(e6pos.ToString());
                stringBuilderDAT.AppendLine(fdat.ToString());
            }

            //last point
            nameE6POS = String.Format("XKLB_{0}", (ZZ_VERTICES + 2).ToString("D2"));
            nameFDAT = String.Format("FKLB_{0}", (ZZ_VERTICES + 2).ToString("D2"));
            e6pos = new E6POS((ZZ_VERTICES % 2 == 0) ? ZZ_X_STOP : ZZ_X_START, ZZ_Y_STOP, Z_VALUE, nameE6POS);
            fdat = new FDAT(TOOL, BASE, nameFDAT);
            slin = new SLIN_GLUE_OFF(ZZ_SPEED, e6pos, fdat, ldat);
            ((SLIN_GLUE_OFF)slin).EndMeasurement = true;

            stringBuilderSRC.AppendLine(slin.ToString());
            stringBuilderDAT.AppendLine(e6pos.ToString());
            stringBuilderDAT.AppendLine(fdat.ToString());

            textBox1.Text = stringBuilderSRC.ToString();
            textBox2.Text = stringBuilderDAT.ToString();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                Clipboard.SetText(textBox1.Text);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
                Clipboard.SetText(textBox2.Text);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Size diff = Size - initSize;
            int halfDiffY = diff.Height / 2;
            button1.Width = diff.Width + initButton1.Width;
            textBox1.Width = diff.Width + initTextBox1.Width;
            textBox2.Width = diff.Width + initTextBox2.Width;
            button2.Height = halfDiffY + initButton2.Height;
            button3.Height = halfDiffY + initButton3.Height;
            textBox1.Height = halfDiffY + initTextBox1.Height;
            textBox2.Height = halfDiffY + initTextBox2.Height;

            button3.Top = halfDiffY + initLocButton3.Y;
            textBox2.Top = halfDiffY + initLocTextBox2.Y;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();

            StringBuilder stringBuilderSRC = new StringBuilder();
            StringBuilder stringBuilderDAT = new StringBuilder();



            //generowanie punktów w X            
            double[] y_points = new double[LINES];
            double y_spracing = (Y_SIZE - 2.0 * Y_OFFSET - Y_START) / (LINES - 1);
            for (int i = 0; i < LINES; i++)
            {
                y_points[i] = Y_OFFSET + Y_START + y_spracing * i;
            }

            List<E6POS> points = new List<E6POS>();

            LDAT ldat = new LDAT("LL");

            for (int i = 0; i < LINES; i++)
            {
                bool evenOrOdd = (i % 2 == 0);
                double[] x_points = evenOrOdd ? X_POINTS : X_POINTS_REV; //dla parzysty punkty normalne a nie parzystych odwrotne
                double[] speed = evenOrOdd ? SPEED : SPEED_REV;

                //jesli pierwsza lub ostania linia zwiedz ilosc punktow w X 
                bool firstLine = i == 0;
                bool lastLine = i == LINES - 1;

                switch (i)
                {
                    case 0:             //pierwsza linia
                        {
                            double refY = y_points[i];
                            for (int j = 0; j < X_POINTS_MUSTACH.Count(); j++)
                            {
                                string nameE6POS = String.Format("XKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                                string nameFDAT = String.Format("FKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                                E6POS e6pos = new E6POS(X_POINTS_MUSTACH[j], refY - Y_POINTS_MUSTACH[j], Z_VALUE, nameE6POS);
                                FDAT fdat = new FDAT(TOOL, BASE, nameFDAT);
                                SLIN slin;
                                switch (j)
                                {
                                    case START_GLUE_POINT_MUSTACHE:
                                        {
                                            slin = new SLIN_GLUE_ON(SPEED_MUSTACH[j], e6pos, fdat, ldat);
                                            break;
                                        }
                                    case STOP_GLUE_POINT_MUSTACHE:
                                        {
                                            bool endMeas = (i == LINES - 1) ? true : false; //na ostaniej linii wylacz pomiar
                                            slin = new SLIN_GLUE_OFF(SPEED_MUSTACH[j], e6pos, fdat, ldat);
                                            ((SLIN_GLUE_OFF)slin).EndMeasurement = endMeas;
                                            break;
                                        }
                                    default:
                                        {
                                            slin = new SLIN(SPEED_MUSTACH[j], e6pos, fdat, ldat);
                                            break;
                                        }
                                }
                                points.Add(e6pos);

                                //Debug.WriteLine(e6pos.ToString());
                                stringBuilderSRC.AppendLine(slin.ToString());
                                stringBuilderDAT.AppendLine(e6pos.ToString());
                                stringBuilderDAT.AppendLine(fdat.ToString());

                            }
                            break;
                        }
                    case LINES - 1:     //ostania linia
                        {
                            double[] x_points_mustach = evenOrOdd ? X_POINTS_MUSTACH : X_POINTS_MUSTACH.Reverse().ToArray();
                            double refY = y_points[i];
                            for (int j = 0; j < X_POINTS_MUSTACH.Count(); j++)
                            {
                                string nameE6POS = String.Format("XKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                                string nameFDAT = String.Format("FKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                                E6POS e6pos = new E6POS(x_points_mustach[j], refY + Y_POINTS_MUSTACH[j], Z_VALUE, nameE6POS);
                                FDAT fdat = new FDAT(TOOL, BASE, nameFDAT);
                                SLIN slin;
                                switch (j)
                                {
                                    case START_GLUE_POINT_MUSTACHE:
                                        {
                                            slin = new SLIN_GLUE_ON(SPEED_MUSTACH[j], e6pos, fdat, ldat);
                                            break;
                                        }
                                    case STOP_GLUE_POINT_MUSTACHE:
                                        {
                                            bool endMeas = (i == LINES - 1) ? true : false; //na ostaniej linii wylacz pomiar
                                            slin = new SLIN_GLUE_OFF(SPEED_MUSTACH[j], e6pos, fdat, ldat);
                                            ((SLIN_GLUE_OFF)slin).EndMeasurement = endMeas;
                                            break;
                                        }
                                    default:
                                        {
                                            slin = new SLIN(SPEED_MUSTACH[j], e6pos, fdat, ldat);
                                            break;
                                        }
                                }
                                points.Add(e6pos);

                                //Debug.WriteLine(e6pos.ToString());
                                stringBuilderSRC.AppendLine(slin.ToString());
                                stringBuilderDAT.AppendLine(e6pos.ToString());
                                stringBuilderDAT.AppendLine(fdat.ToString());
                            }
                            break;
                        }
                    default:            //kazda inna linia
                        {
                            for (int j = 0; j < X_POINTS.Count(); j++)
                            {

                                string nameE6POS = String.Format("XKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                                string nameFDAT = String.Format("FKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                                E6POS e6pos = new E6POS(x_points[j], y_points[i], Z_VALUE, nameE6POS);
                                FDAT fdat = new FDAT(TOOL, BASE, nameFDAT);
                                SLIN slin;
                                switch (j)
                                {
                                    case START_GLUE_POINT:
                                        {
                                            slin = new SLIN_GLUE_ON(speed[j], e6pos, fdat, ldat);
                                            break;
                                        }
                                    case STOP_GLUE_POINT:
                                        {
                                            bool endMeas = (i == LINES - 1) ? true : false; //na ostaniej linii wylacz pomiar
                                            slin = new SLIN_GLUE_OFF(speed[j], e6pos, fdat, ldat);
                                            ((SLIN_GLUE_OFF)slin).EndMeasurement = endMeas;
                                            break;
                                        }
                                    default:
                                        {
                                            slin = new SLIN(speed[j], e6pos, fdat, ldat);
                                            break;
                                        }
                                }
                                points.Add(e6pos);

                                //Debug.WriteLine(e6pos.ToString());
                                stringBuilderSRC.AppendLine(slin.ToString());
                                stringBuilderDAT.AppendLine(e6pos.ToString());
                                stringBuilderDAT.AppendLine(fdat.ToString());
                            }
                            break;
                        }
                }

                stringBuilderSRC.AppendLine();
                stringBuilderDAT.AppendLine();
            }

            stringBuilderDAT.AppendLine(ldat.ToString());

            textBox1.Text = stringBuilderSRC.ToString();
            textBox2.Text = stringBuilderDAT.ToString();
        }
    }
}
