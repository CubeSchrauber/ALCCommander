using alclib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace alcwin
{
    public class ChartControl : FrameworkElement
    {
        private VisualCollection Children;
        private DrawingVisual Visual;

        public ChartControl() => Children = new VisualCollection(this);

        protected override int VisualChildrenCount => Children.Count;

        protected override Visual GetVisualChild(int index) => Children[index];

        public void SetData(LogData log)
        {
            var g = new Plot(this, log);

            Visual = new DrawingVisual();
            using (var c = Visual.RenderOpen())
            {
                g.Render(c);
            }
            Children.Add(Visual);
        }
    }

}
