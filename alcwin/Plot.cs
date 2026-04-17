using alclib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.FormattableString;

namespace alcwin
{
    class Plot
    {
        double bx = 0.1;
        double by = 0.1;
        double width;
        double height;
        Visual parent;
        double ppd;

        Pen p1, p2;
        LogData log;

        CultureInfo Culture = CultureInfo.InvariantCulture;
        Typeface Typeface = new Typeface("Arial");

        Point[][] Voltages;
        Point[][] Currents;

        public Plot(FrameworkElement parent, LogData log)
        {
            this.log = log;
            this.width = parent.ActualWidth;
            this.height = parent.ActualHeight;
            this.parent = parent;
            this.ppd = VisualTreeHelper.GetDpi(parent).PixelsPerDip;

            p1 = new Pen(Brushes.Blue, 1);
            p2 = new Pen(Brushes.Red, 1);
            p1.Freeze();
            p2.Freeze();

            int l = log.Sections.Length;
            Voltages = new Point[l][];
            Currents = new Point[l][];

            for (int s = 0; s < l; s++)
            {
                Voltages[s] = Polygon(log.Sections[s].Count, i => px(log.nt(s, i)), i => py(log.nv(s, i)));
                Currents[s] = Polygon(log.Sections[s].Count, i => px(log.nt(s, i)), i => py(log.nc(s, i)));
            }
        }

        public double px(double x) => (x* (1.0 - 2.0 * bx) + bx) * (width - 1);
        public double py(double y) => ((1.0 - y) * (1.0 - 2.0 * by) + by) * (height - 1);

        private (Point[], Point[]) Split(Point[] p, int i)
        {
            var p1 = new Point[i + 1];
            var p2 = new Point[p.Length - i];
            Array.Copy(p, 0, p1, 0, i + 1);
            Array.Copy(p, i, p2, 0, p.Length - i);
            return (p1, p2);
        }

        private Point[] Concat(Point[] p1, Point[] p2)
        {
            var p = new Point[p1.Length + p2.Length - 1];
            Array.Copy(p1, 0, p, 0, p1.Length - 1);
            Array.Copy(p2, 0, p, p1.Length - 1, p2.Length);
            return p;
        }

        private Point[] Simplify(Point[] p, double epsilon)
        {
            int l = p.Length;
            if (l > 2)
            {
                double x0 = p[0].X;
                double y0 = p[0].Y;
                double x1 = p[l - 1].X;
                double y1 = p[l - 1].Y;
                double m = (y1 - y0) / (x1 - x0);
                double dmax = double.MinValue;
                int didx = -1;

                for (int i = 1; i < l - 1; i++)
                {
                    double x = p[i].X;
                    double y = (x - x0) * m + y0;
                    double d = Math.Abs(y - p[i].Y);
                    if (d > dmax)
                    {
                        dmax = d;
                        didx = i;
                    }
                }

                if (dmax > epsilon)
                {
                    var (p1, p2) = Split(p, didx);
                    p1 = Simplify(p1, epsilon);
                    p2 = Simplify(p2, epsilon);
                    p = Concat(p1, p2);
                }
                else
                {
                    var q = new Point[2];
                    q[0] = p[0];
                    q[1] = p[p.Length - 1];
                    p = q;
                }
            }
            return p;
        }

        private Point[] Polygon(int count, Func<int, double> x, Func<int, double> y)
        {
            var p = new Point[count];
            for (int i = 0; i < count; i++)
                p[i] = new Point(x(i), y(i));
            p = Simplify(p, 0.5);
            return p;
        }

        private void Draw(DrawingContext c, Pen p, Point[] poly)
        {
            for (int i = 0; i < poly.Length - 1; i++)
                c.DrawLine(p, poly[i], poly[i + 1]);
        }

        public void TextRight(DrawingContext c, double x, double y, FormattableString text)
        {
            var ft = new FormattedText(Invariant(text), Culture, FlowDirection.LeftToRight, Typeface, 12, Brushes.Blue, ppd);
            ft.TextAlignment = TextAlignment.Right;
            c.DrawText(ft, new Point(x-5, y - ft.Height / 2));
        }

        public void TextLeft(DrawingContext c, double x, double y, FormattableString text)
        {
            var ft = new FormattedText(Invariant(text), Culture, FlowDirection.LeftToRight, Typeface, 12, Brushes.Red, ppd);
            c.DrawText(ft, new Point(x + 5, y - ft.Height / 2));
        }

        public void Render(DrawingContext c)
        {
            for (int s = 0; s < log.Sections.Length; s++)
            {
                Draw(c, p1, Voltages[s]);
                Draw(c, p2, Currents[s]);
            }

            TextRight(c, px(0), py(1), $"{LogConvert.Voltage(log.VoltageMax):f2}V");
            TextRight(c, px(0), py(0), $"{LogConvert.Voltage(log.VoltageMin):f2}V");

            TextLeft(c, px(1), py(1), $"{LogConvert.Current(log.CurrentMax):f3}A");
            TextLeft(c, px(1), py(0), $"{LogConvert.Current(log.CurrentMin):f3}A");
        }
    }
}
