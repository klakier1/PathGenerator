using PathGenerator.Kuka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace PathGenerator
{
    class Generator
    {
        List<List<RobPoint>> program = new List<List<RobPoint>>();
        StringBuilder stringBuilderSRC = new StringBuilder();
        StringBuilder stringBuilderDAT = new StringBuilder();
        LDAT ldat = new LDAT("LL");

        internal List<List<RobPoint>> Program { get => program; set => program = value; }

        public Generator() { }

        public void LineFromXYVArray(double[] X, double[] Y, double Z, double[] speed, int startGlue, int stopGlue)
        {
            var line = new List<RobPoint>();
            for (int i = 0; i < X.Count(); i++)
            {

                var func = GetGlueFunc(i, startGlue, stopGlue);
                var p = new RobPoint(X[i], Y[i], Z, speed[i], func);
                line.Add(p);
            }
            program.Add(line);
        }

        public void LineFromXYZVArray(double[] X, double[] Y, double[] Z, double[] speed, int startGlue, int stopGlue)
        {
            var line = new List<RobPoint>();
            for (int i = 0; i < X.Count(); i++)
            {

                var func = GetGlueFunc(i, startGlue, stopGlue);
                var p = new RobPoint(X[i], Y[i], Z[i], speed[i], func);
                line.Add(p);
            }
            program.Add(line);
        }

        public void LineFromXVArray(double[] X, double Y, double Z, double[] speed, int startGlue, int stopGlue)
        {
            double[] Ys = Enumerable.Repeat<double>(Y, X.Count()).ToArray();
            LineFromXYVArray(X, Ys, Z, speed, startGlue, stopGlue);
        }

        public void LineFromXZVArray(double[] X, double Y, double[] Z, double[] speed, int startGlue, int stopGlue)
        {
            double[] Ys = Enumerable.Repeat<double>(Y, X.Count()).ToArray();
            LineFromXYZVArray(X, Ys, Z, speed, startGlue, stopGlue);
        }

        public void MatrixFromXYVArray(double[] X, double[] X_rev, double[] Y, double Z, double[] speed, double[] speed_rev, int startGlue, int stopGlue, bool forward)
        {
            for (int i = 0; i < Y.Count(); i++)
            {
                bool evenOrOdd = (i % 2 == 0);
                if (!forward) evenOrOdd = !evenOrOdd;

                double[] Xs = evenOrOdd ? X : X_rev;
                double[] Vs = evenOrOdd ? speed : speed_rev;

                LineFromXVArray(Xs, Y[i], Z, Vs, startGlue, stopGlue);
            }
        }

        public void MatrixFromXYZVArray(double[] X, double[] X_rev, double[] Y, double[] Z, double[] Z_rev, double[] speed, double[] speed_rev, int startGlue, int stopGlue, bool forward)
        {
            for (int i = 0; i < Y.Count(); i++)
            {
                bool evenOrOdd = (i % 2 == 0);
                if (!forward) evenOrOdd = !evenOrOdd;

                double[] Xs = evenOrOdd ? X : X_rev;
                double[] Vs = evenOrOdd ? speed : speed_rev;
                double[] Zs = evenOrOdd ? Z : Z_rev;

                LineFromXZVArray(Xs, Y[i], Zs, Vs, startGlue, stopGlue);
            }
        }

        public RobPoint.GlueFunc GetGlueFunc(int i, int startGlue, int stopGlue)
        {
            RobPoint.GlueFunc func;
            if (i == startGlue)
                func = RobPoint.GlueFunc.StartGlue;
            else if (i == stopGlue)
                func = RobPoint.GlueFunc.StopGlue;
            else
                func = RobPoint.GlueFunc.Without;

            return func;
        }

        public bool IsNextLineEven()
        {
            return Program.Count() % 2 == 0;
        }

        public void GenerateProgram(int tool_no, int base_no)
        {
            stringBuilderDAT.Clear();
            stringBuilderSRC.Clear();

            for (int i = 0; i < program.Count(); i++)
            {
                List<RobPoint> line = program[i];

                for (int j = 0; j < line.Count(); j++)
                {
                    string nameE6POS = String.Format("XKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                    string nameFDAT = String.Format("FKLB_{0}_{1}", (i + 1).ToString("D2"), (j + 1).ToString("D2"));
                    E6POS e6pos = new E6POS(program[i][j].X, program[i][j].Y, program[i][j].Z, nameE6POS);
                    FDAT fdat = new FDAT(tool_no, base_no, nameFDAT);
                    SLIN slin;
                    RobPoint.GlueFunc func = program[i][j].Func;
                    switch (func)
                    {
                        case RobPoint.GlueFunc.StartGlue:
                            {
                                slin = new SLIN_GLUE_ON(program[i][j].V, e6pos, fdat, ldat);
                                break;
                            }
                        case RobPoint.GlueFunc.StopGlue:
                            {
                                bool endMeas = (i == program.Count - 1) ? true : false; //na ostaniej linii wylacz pomiar
                                slin = new SLIN_GLUE_OFF(program[i][j].V, e6pos, fdat, ldat);
                                ((SLIN_GLUE_OFF)slin).EndMeasurement = endMeas;
                                break;
                            }
                        default:
                            {
                                slin = new SLIN(program[i][j].V, e6pos, fdat, ldat);
                                break;
                            }
                    }

                    stringBuilderSRC.AppendLine(slin.ToString());
                    stringBuilderDAT.AppendLine(e6pos.ToString());
                    stringBuilderDAT.AppendLine(fdat.ToString());
                }
                stringBuilderSRC.AppendLine();
                stringBuilderDAT.AppendLine();
            }
            stringBuilderDAT.AppendLine(ldat.ToString());
        }

        public string GetDAT()
        {
            return stringBuilderDAT.ToString();
        }

        public string GetSRC()
        {
            return stringBuilderSRC.ToString();
        }

        public void DrawChart(Chart chart)
        {
            Series glue = new Series("Glue");
            Series path = new Series("Path");

            chart.Series.Clear();

            chart.Series.Add(path);
            chart.Series.Add(glue);

            chart.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;

            bool glueOn = false;

            for (int i = 0; i < program.Count(); i++)
            {
                List<RobPoint> line = program[i];

                for (int j = 0; j < line.Count(); j++)
                {
                    chart.Series[0].Points.AddXY(program[i][j].X, program[i][j].Y);

                    RobPoint.GlueFunc func = program[i][j].Func;
                    switch (func)
                    {
                        case RobPoint.GlueFunc.StartGlue:
                            {
                                glueOn = true;
                                if (glueOn) chart.Series[1].Points.AddXY(program[i][j].X, program[i][j].Y);
                                break;
                            }
                        case RobPoint.GlueFunc.StopGlue:
                            {
                                if (glueOn) chart.Series[1].Points.AddXY(program[i][j].X, program[i][j].Y);
                                glueOn = false;
                                break;
                            }
                        default:
                            {
                                if (glueOn) chart.Series[1].Points.AddXY(program[i][j].X, program[i][j].Y);
                                break;
                            }
                    }
                }
            }

            chart.ChartAreas[0].AxisY.ScaleView.Zoom(-10, 350);
            chart.ChartAreas[0].AxisX.ScaleView.Zoom(-10, 140);
        }
    }
}
