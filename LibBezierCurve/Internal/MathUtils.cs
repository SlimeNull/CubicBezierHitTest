using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TestConsole")]

namespace LibBezierCurve.Internal
{
    internal static class MathUtils
    {
        /// <summary>
        /// 解一元一次方程
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static IEnumerable<double> CoreResolveLinearEquation(double a, double b)
        {
            if (b == 0)
            {
                yield break;
            }

            yield return -b / a;
        }

        /// <summary>
        /// 解二元一次方程
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IEnumerable<double> CoreResolveQuadraticEquation(double a, double b, double c)
        {
            var deltaT = Math.Pow(b, 2) - 4 * a * c;

            if (deltaT < 0)
            {
                // no real number root
                yield break;
            }

            var sqrtDeltaT = Math.Sqrt(deltaT);
            yield return (-b + sqrtDeltaT) / (2 * a);

            if (deltaT != 0)
            {
                yield return (-b - sqrtDeltaT) / (2 * a);
            }
        }

        /// <summary>
        /// 解三元一次方程
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static IEnumerable<double> CoreResolveCubicEquation(double a, double b, double c, double d)
        {
            static double CubeRoot(double x)
            {
                if (x < 0)
                {
                    return -Math.Pow(-x, 1.0 / 3);
                }
                else
                {
                    return Math.Pow(x, 1.0 / 3);
                }
            }

            double s = b / (3 * a);
            double p = (3 * a * c - b * b) / (3 * a * a);
            double q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);
            double delta = (q / 2) * (q / 2) + (p / 3) * (p / 3) * (p / 3);

            if (delta > double.Epsilon)
            {
                // 一个实根和两个复数根，只返回实根
                double sqrtDelta = Math.Sqrt(delta);
                double u = CubeRoot(-q / 2 + sqrtDelta);
                double v = CubeRoot(-q / 2 - sqrtDelta);
                double y = u + v;
                yield return y - s;
            }
            else if (delta < -double.Epsilon)
            {
                // 三个不同的实根，使用三角函数形式
                double sqrt_p_3 = Math.Sqrt(-p / 3);
                double denominator = sqrt_p_3 * sqrt_p_3 * sqrt_p_3;
                double cosTheta = (-q / 2) / denominator;
                cosTheta = Math.Max(Math.Min(cosTheta, 1.0), -1.0); // 确保acos参数有效
                double theta = Math.Acos(cosTheta);
                double temp = 2 * sqrt_p_3;

                yield return temp * Math.Cos(theta / 3) - s;
                yield return temp * Math.Cos((theta + 2 * Math.PI) / 3) - s;
                yield return temp * Math.Cos((theta + 4 * Math.PI) / 3) - s;
            }
            else
            {
                // Delta近似为0的情况
                bool pIsZero = Math.Abs(p) < double.Epsilon;
                bool qIsZero = Math.Abs(q) < double.Epsilon;

                if (pIsZero && qIsZero)
                {
                    // 三重根
                    yield return -s;
                }
                else
                {
                    // 一个单根和一个双重根
                    double u = CubeRoot(-q / 2);
                    yield return 2 * u - s;
                    yield return -u - s;
                }
            }
        }

        public static IEnumerable<double> ResolveEquation(double a, double b)
        {
            return CoreResolveLinearEquation(a, b);
        }

        public static IEnumerable<double> ResolveEquation(double a, double b, double c)
        {
            if (a != 0)
            {
                return CoreResolveQuadraticEquation(a, b, c);
            }
            else
            {
                return ResolveEquation(b, c);
            }
        }

        public static IEnumerable<double> ResolveEquation(double a, double b, double c, double d)
        {
            if (a != 0)
            {
                return CoreResolveCubicEquation(a, b, c, d);
            }
            else
            {
                return ResolveEquation(b, c, d);
            }
        }

        public static double Lerp(double x, double y, double t)
        {
            return x * (1 - t) + y * t;
        }

        /// <summary>
        /// 从 <paramref name="sortedValues1"/> 中和 <paramref name="sortedValues2"/> 中分别取一个值, 并且保证这两个值之间的差是所有组合中的最小值. 最后返回它们的平均值
        /// </summary>
        /// <param name="sortedValues1"></param>
        /// <param name="sortedValues2"></param>
        /// <returns></returns>
        public static double GetAverage(IList<double> sortedValues1, List<double> sortedValues2)
        {
            int i = 0, j = 0;
            double minDiff = double.MaxValue;
            double result = 0;

            // 双指针法遍历
            while (i < sortedValues1.Count && j < sortedValues2.Count)
            {
                double val1 = sortedValues1[i];
                double val2 = sortedValues2[j];

                // 计算差值
                double diff = Math.Abs(val1 - val2);

                // 如果当前差值比之前记录的最小差值还小，更新最小差值和结果
                if (diff < minDiff)
                {
                    minDiff = diff;
                    result = (val1 + val2) / 2.0;
                }

                // 移动指针
                if (val1 < val2)
                    i++;  // val1 较小，移动 sortedValues1 的指针
                else
                    j++;  // val2 较小或相等，移动 sortedValues2 的指针
            }

            return result;
        }
    }
}
