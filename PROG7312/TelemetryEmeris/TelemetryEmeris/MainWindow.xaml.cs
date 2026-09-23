using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TelemetryEmeris
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private readonly DispatcherTimer _timer;
        private readonly List<double> _temperatures = new List<double>();
        private const int MaxPoints = 55;

        private int _counter;
        private int _alertCount;
        private double _temperature = 55;
        private double _minTemp = double.MaxValue;
        private double _maxTemp = double.MinValue;
        private readonly double Threshold = 60;

        private static readonly Brush OkBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xF5, 0xFF));
        private static readonly Brush AlertBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x2D, 0x95));
        private static readonly Brush TileBg = new SolidColorBrush(Color.FromRgb(0x12, 0x12, 0x2A));
        private static readonly Brush TileAlertBg = new SolidColorBrush(Color.FromRgb(0x2A, 0x0A, 0x1C));

        public MainWindow()
        {
            InitializeComponent();
            OkBrush.Freeze();
            AlertBrush.Freeze();
            TileBg.Freeze();
            TileAlertBg.Freeze();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _timer.Tick += Timer_click;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            StatusBanner.Text = "LIVE DROP";
            StatusBanner.Foreground = OkBrush;
        }

        private void Timer_click(object? sender, EventArgs e)
        {
            _temperature += (_random.NextDouble() * 2 - 1) * 3;
            _counter++;

            bool isAlert = _temperature >= Threshold;

            if (_temperature < _minTemp) _minTemp = _temperature;
            if (_temperature > _maxTemp) _maxTemp = _temperature;
            if (isAlert) _alertCount++;

            // Beep on every alert reading
            if (isAlert)
            {
                SystemSounds.Beep.Play();
                StatusBanner.Text = "⚠ ALERT ACTIVE";
                StatusBanner.Foreground = AlertBrush;
            }
            else
            {
                StatusBanner.Text = "SIGNAL OK";
                StatusBanner.Foreground = OkBrush;
            }

            LiveTempText.Text = $"{_temperature:0.0}°";
            LiveTempText.Foreground = isAlert ? AlertBrush : (Brush)FindResource("NeonLime");
            CountText.Text = _counter.ToString();
            AlertCountText.Text = _alertCount.ToString();
            MinTempText.Text = $"{_minTemp:0.0}°";
            MaxTempText.Text = $"{_maxTemp:0.0}°";

            // Dashboard tile for this reading
            var tile = new Border
            {
                Background = isAlert ? TileAlertBg : TileBg,
                BorderBrush = isAlert ? AlertBrush : OkBrush,
                BorderThickness = new Thickness(1.5),
                Padding = new Thickness(10, 8, 10, 8),
                Margin = new Thickness(0, 0, 8, 8),
                MinWidth = 88,
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = DateTime.Now.ToString("HH:mm:ss"),
                            Foreground = (Brush)FindResource("TextMuted"),
                            FontSize = 9,
                            FontFamily = new FontFamily("Consolas"),
                            FontWeight = FontWeights.Bold
                        },
                        new TextBlock
                        {
                            Text = $"{_temperature:0.0}",
                            Foreground = isAlert ? AlertBrush : OkBrush,
                            FontSize = 20,
                            FontWeight = FontWeights.Black
                        },
                        new TextBlock
                        {
                            Text = isAlert ? "ALERT" : "OK",
                            Foreground = isAlert ? AlertBrush : (Brush)FindResource("NeonLime"),
                            FontSize = 10,
                            FontWeight = FontWeights.Bold
                        }
                    }
                }
            };

            if (isAlert)
            {
                tile.Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(0xFF, 0x2D, 0x95),
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Opacity = 0.8
                };
            }

            ReadingsList.Items.Insert(0, tile);
            if (ReadingsList.Items.Count > MaxPoints)
            {
                ReadingsList.Items.RemoveAt(ReadingsList.Items.Count - 1);
            }

            _temperatures.Add(_temperature);
            if (_temperatures.Count > MaxPoints)
            {
                _temperatures.RemoveAt(0);
            }
            RedrawChart();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            StatusBanner.Text = "EXTRACTED";
            StatusBanner.Foreground = (Brush)FindResource("NeonYellow");
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawChart();
        }

        private void RedrawChart()
        {
            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
                return;

            const double padLeft = 8;
            const double padRight = 8;
            const double padTop = 14;
            const double padBottom = 8;
            double plotWidth = Math.Max(1, width - padLeft - padRight);
            double plotHeight = Math.Max(1, height - padTop - padBottom);

            double minTemp = Threshold - 10;
            double maxTemp = Threshold + 10;
            if (_temperatures.Count > 0)
            {
                foreach (double t in _temperatures)
                {
                    if (t < minTemp) minTemp = t;
                    if (t > maxTemp) maxTemp = t;
                }
            }

            double range = Math.Max(1, maxTemp - minTemp);
            minTemp -= range * 0.08;
            maxTemp += range * 0.08;
            range = maxTemp - minTemp;

            double MapY(double temp) =>
                padTop + plotHeight - ((temp - minTemp) / range) * plotHeight;

            double thresholdY = MapY(Threshold);
            ThresholdLine.X1 = padLeft;
            ThresholdLine.Y1 = thresholdY;
            ThresholdLine.X2 = width - padRight;
            ThresholdLine.Y2 = thresholdY;
            Canvas.SetLeft(ThresholdLabel, padLeft);
            Canvas.SetTop(ThresholdLabel, Math.Max(0, thresholdY - 16));

            var points = new PointCollection();
            if (_temperatures.Count == 1)
            {
                double y = MapY(_temperatures[0]);
                points.Add(new Point(padLeft, y));
                points.Add(new Point(padLeft + plotWidth, y));
            }
            else if (_temperatures.Count > 1)
            {
                for (int i = 0; i < _temperatures.Count; i++)
                {
                    double x = padLeft + (i / (double)(MaxPoints - 1)) * plotWidth;
                    double y = MapY(_temperatures[i]);
                    points.Add(new Point(x, y));
                }
            }
            TempLine.Points = points;
        }
    }
}
