using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibBezierCurve;

namespace BezierHitTestWindow
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BezierHitTestWindow"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BezierHitTestWindow;assembly=BezierHitTestWindow"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:BezierCurve/>
    ///
    /// </summary>
    public class BezierCurve : Control
    {
        static BezierCurve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BezierCurve), new FrameworkPropertyMetadata(typeof(BezierCurve)));
        }

        public IBezierCurve Curve
        {
            get { return (IBezierCurve)GetValue(CurveProperty); }
            set { SetValue(CurveProperty, value); }
        }

        public int SampleCount
        {
            get { return (int)GetValue(SampleCountProperty); }
            set { SetValue(SampleCountProperty, value); }
        }
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }


        public static readonly DependencyProperty CurveProperty =
            DependencyProperty.Register("Curve", typeof(IBezierCurve), typeof(BezierCurve), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SampleCountProperty =
            DependencyProperty.Register("SampleCount", typeof(int), typeof(BezierCurve), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender), ValidateSampleCount);

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(BezierCurve), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));



        private static bool ValidateSampleCount(object value)
        {
            return value is int number && number >= 2;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var curve = Curve;
            var sampleCount = SampleCount;

            if (curve is null ||
                sampleCount < 2)
            {
                return;
            }

            var pen = new Pen(Foreground, StrokeThickness);

            curve.Sample(0, out var lastX, out var lastY);
            for (int i = 1; i < sampleCount; i++)
            {
                var t = (double)i / (sampleCount - 1);
                curve.Sample(t, out var x, out var y);

                drawingContext.DrawLine(pen, new Point(lastX, lastY), new Point(x, y));
                lastX = x;
                lastY = y;
            }

            foreach (var controlPoint in curve.EnumerateControlPoints())
            {
                drawingContext.DrawEllipse(Foreground, null, new Point(controlPoint.Item1, controlPoint.Item2), StrokeThickness, StrokeThickness);
            }

            var mouseRelatedPosition = Mouse.GetPosition(this);
            if (curve.HitTest(mouseRelatedPosition.X, mouseRelatedPosition.Y, 5, out var mouseT))
            {
                curve.Sample(mouseT, out var hitX, out var hitY);
                drawingContext.DrawLine(pen, new Point(hitX - 5, hitY), new Point(hitX + 5, hitY));
                drawingContext.DrawLine(pen, new Point(hitX, hitY - 5), new Point(hitX, hitY + 5));
            }
        }
    }
}
