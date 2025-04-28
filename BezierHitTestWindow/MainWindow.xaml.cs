using LibBezierCurve;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BezierHitTestWindow;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Point[]? _controlPoints;
    private int _changingControlPointIndex;

    public MainWindow()
    {
        InitializeComponent();

        bezierCurve.Curve = new CubicBezierCurve(10, 10, 200, 100, 10, 200, 200, 300);
    }

    private IBezierCurve? BuildBezierCurve(Point[] controlPoints)
    {
        if (controlPoints.Length == 3)
        {
            return new QuadraticBezierCurve(
                controlPoints[0].X, controlPoints[0].Y,
                controlPoints[1].X, controlPoints[1].Y,
                controlPoints[2].X, controlPoints[2].Y);
        }
        else if (controlPoints.Length == 4)
        {
            return new CubicBezierCurve(
                controlPoints[0].X, controlPoints[0].Y,
                controlPoints[1].X, controlPoints[1].Y,
                controlPoints[2].X, controlPoints[2].Y,
                controlPoints[3].X, controlPoints[3].Y);
        }

        return null;
    }

    private void bezierCurve_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var mousePosition = e.GetPosition(bezierCurve);

        if (bezierCurve.Curve is QuadraticBezierCurve quadraticBezierCurve)
        {
            _controlPoints = new Point[]
            {
                new Point(quadraticBezierCurve.ControlPoint1X, quadraticBezierCurve.ControlPoint1Y),
                new Point(quadraticBezierCurve.ControlPoint2X, quadraticBezierCurve.ControlPoint2Y),
                new Point(quadraticBezierCurve.ControlPoint3X, quadraticBezierCurve.ControlPoint3Y),
            };
        }
        else if (bezierCurve.Curve is CubicBezierCurve cubicBezierCurve)
        {
            _controlPoints = new Point[]
            {
                new Point(cubicBezierCurve.ControlPoint1X, cubicBezierCurve.ControlPoint1Y),
                new Point(cubicBezierCurve.ControlPoint2X, cubicBezierCurve.ControlPoint2Y),
                new Point(cubicBezierCurve.ControlPoint3X, cubicBezierCurve.ControlPoint3Y),
                new Point(cubicBezierCurve.ControlPoint4X, cubicBezierCurve.ControlPoint4Y),
            };
        }
        else
        {
            return;
        }

        _changingControlPointIndex = _controlPoints
            .Select((controlPoint, index) => (controlPoint, index))
            .OrderBy(controlPointAndIndex => new Vector(mousePosition.X - controlPointAndIndex.controlPoint.X, mousePosition.Y - controlPointAndIndex.controlPoint.Y).LengthSquared)
            .Select(controlPointAndIndex => controlPointAndIndex.index)
            .First();
    }

    private void bezierCurve_MouseMove(object sender, MouseEventArgs e)
    {
        if (_controlPoints is null)
        {
            return;
        }

        var mousePosition = e.GetPosition(bezierCurve);
        _controlPoints[_changingControlPointIndex] = mousePosition;

        var newBezierCurve = BuildBezierCurve(_controlPoints);
        if (newBezierCurve is not null)
        {
            bezierCurve.Curve = newBezierCurve;
        }
    }

    private void bezierCurve_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _controlPoints = null;
    }

    private void bezierCurve_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var newSampleCount = bezierCurve.SampleCount;
        if (e.Delta > 0)
        {
            newSampleCount *= 2;
        }
        else
        {
            newSampleCount /= 2;
        }

        newSampleCount = Math.Max(2, newSampleCount);
        bezierCurve.SampleCount = newSampleCount;
    }
}